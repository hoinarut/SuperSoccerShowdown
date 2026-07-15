using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Application.Services;
using MyApp.Domain;
using MyApp.Domain.Entities;
using MyApp.Domain.Enums;
using MyApp.Domain.Ports;
using MyApp.Testing.Common;

namespace MyApp.Application.Tests.Services;

public class TeamManagerTests : BaseUnitTests
{
    private readonly ITeamRepository _teamRepository = GetStrictMock<ITeamRepository>();
    private readonly IUniverseRepository _universeRepository = GetStrictMock<IUniverseRepository>();
    private readonly IPlayerRepository _playerRepository = GetStrictMock<IPlayerRepository>();
    private readonly IUniverseDataService _universeDataService = GetStrictMock<IUniverseDataService>();
    private readonly ILogger<TeamManager> _logger = GetStrictMock<ILogger<TeamManager>>();

    private readonly ITeamManager _teamManager;

    public TeamManagerTests()
    {
        _teamManager = new TeamManager(
            [_universeDataService],
            _universeRepository,
            _teamRepository,
            _playerRepository,
            _logger);
    }

    [Fact]
    public async Task CreateTeam_InvalidUniverse_ThrowsException()
    {
        SetUpUniverse(null);

        var action = () => _teamManager.CreateTeam(1, "Test", 1, 1);

        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Universe not found");
        VerifyAllMocks();
    }

    [Fact]
    public async Task CreateTeam_UnsupportedUniverse_ThrowsException()
    {
        SetUpUniverse(new Universe
        {
            Name = "Test",
            ApiUrl = "api"
        });
        UniverseDataServiceMock.Setup(x => x.CanHandle(It.IsAny<Universe>())).Returns(false);
        
        var action = () => _teamManager.CreateTeam(1, "Test", 1, 1);

        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Universe Test not supported");
        VerifyAllMocks();
    }

    [Fact]
    public async Task CreateTeam_WhenAllResourcesUsed_LogsAndReturnsNull()
    {
        const int universeId = 1;
        var universe = CreateResolvableUniverse(universeId);

        SetUpUniverse(universe);
        UniverseDataServiceMock.Setup(x => x.CanHandle(It.IsAny<Universe>())).Returns(true);
        UniverseDataServiceMock.Setup(x => x.GetAvailableResourceIds(universe))
            .ReturnsAsync([1, 2, 3]);
        UniverseRepository.Setup(x => x.GetUsedExternalResourceIds(universeId))
            .ReturnsAsync([1, 2, 3]);
        SetupLoggerMock(LoggerMock, LogLevel.Warning);

        var team = await _teamManager.CreateTeam(universeId, "Team", 1, 1);

        team.Should().BeNull();
        VerifyLog(LoggerMock, LogLevel.Warning, Times.Once(),
            "No more available resources for team Team in Universe IUniverse");
        VerifyAllMocks();
    }

    [Fact]
    public async Task CreateTeam_WhenNoAvailableResources_LogsAndReturnsNull()
    {
        const int universeId = 1;
        var universe = CreateResolvableUniverse(universeId);

        SetUpUniverse(universe);
        UniverseDataServiceMock.Setup(x => x.CanHandle(It.IsAny<Universe>())).Returns(true);
        UniverseDataServiceMock.Setup(x => x.GetAvailableResourceIds(universe))
            .ReturnsAsync([]);
        UniverseRepository.Setup(x => x.GetUsedExternalResourceIds(universeId))
            .ReturnsAsync([]);
        SetupLoggerMock(LoggerMock, LogLevel.Warning);

        var team = await _teamManager.CreateTeam(universeId, "Team", 1, 1);

        team.Should().BeNull();
        VerifyLog(LoggerMock, LogLevel.Warning, Times.Once(),
            "No more available resources for team Team in Universe IUniverse");
        TeamRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Never);
        VerifyAllMocks();
    }

    [Fact]
    public async Task CreateTeam_WhenValidPlayersAvailable_ReturnsTeamAndPersistsIt()
    {
        const int universeId = 1;
        const string teamName = "Dream Team";
        var universe = CreateResolvableUniverse(universeId);

        SetupTeamCreation(universe, resourceCount: 20);
        SetupPlayerExists(exists: false);
        UniverseDataServiceMock.Setup(x => x.GeneratePlayer(universe, It.IsAny<int>()))
            .ReturnsAsync((Universe _, int resourceId) => CreateValidPlayer($"Player-{resourceId}", resourceId));

        var team = await _teamManager.CreateTeam(universeId, teamName, attackersCount: 1, defendersCount: 1);

        team.Should().NotBeNull();
        team!.Name.Should().Be(teamName);
        team.UniverseId.Should().Be(universeId);
        team.AttackersCount.Should().Be(1);
        team.DefendersCount.Should().Be(1);
        team.Players.Should().HaveCount(Constants.NumberOfPlayers);
        team.Players!.Should().ContainSingle(p => p.Type == PlayerType.Goalie);
        team.Players.Should().ContainSingle(p => p.Type == PlayerType.Defence);
        team.Players.Should().ContainSingle(p => p.Type == PlayerType.Offence);

        TeamRepositoryMock.Verify(x => x.AddAsync(team, It.IsAny<CancellationToken>()), Times.Once);
        UniverseDataServiceMock.Verify(x => x.GeneratePlayer(universe, It.IsAny<int>()), Times.AtLeast(5));
        VerifyAllMocks();
    }

    [Fact]
    public async Task CreateTeam_WhenPlayerHasInvalidMeasurements_SkipsPlayerAndContinues()
    {
        const int universeId = 1;
        var universe = CreateResolvableUniverse(universeId);
        var generatePlayerCalls = 0;

        SetupTeamCreation(universe, resourceCount: 20);
        SetupPlayerExists(exists: false);
        UniverseDataServiceMock.Setup(x => x.GeneratePlayer(universe, It.IsAny<int>()))
            .ReturnsAsync((Universe _, int resourceId) =>
            {
                generatePlayerCalls++;
                return generatePlayerCalls <= 2
                    ? new Player("Invalid", 0, 180, resourceId)
                    : CreateValidPlayer($"Player-{resourceId}", resourceId);
            });

        var team = await _teamManager.CreateTeam(universeId, "Team", attackersCount: 1, defendersCount: 1);

        team.Should().NotBeNull();
        team!.Players.Should().HaveCount(Constants.NumberOfPlayers);
        team.Players!.Should().NotContain(p => p.Name == "Invalid");
        generatePlayerCalls.Should().BeGreaterThan(Constants.NumberOfPlayers);
        VerifyAllMocks();
    }

    [Fact]
    public async Task CreateTeam_WhenPlayerNameAlreadyExists_SkipsPlayerAndContinues()
    {
        const int universeId = 1;
        var universe = CreateResolvableUniverse(universeId);
        var generatePlayerCalls = 0;

        SetupTeamCreation(universe, resourceCount: 20);
        PlayerRepositoryMock.Setup(x => x.Exists("Duplicate")).ReturnsAsync(true);
        PlayerRepositoryMock.Setup(x => x.Exists(It.Is<string>(name => name != "Duplicate"))).ReturnsAsync(false);
        UniverseDataServiceMock.Setup(x => x.GeneratePlayer(universe, It.IsAny<int>()))
            .ReturnsAsync((Universe _, int resourceId) =>
            {
                generatePlayerCalls++;
                return generatePlayerCalls <= 2
                    ? CreateValidPlayer("Duplicate", resourceId)
                    : CreateValidPlayer($"Player-{resourceId}", resourceId);
            });

        var team = await _teamManager.CreateTeam(universeId, "Team", attackersCount: 1, defendersCount: 1);

        team.Should().NotBeNull();
        team!.Players.Should().HaveCount(Constants.NumberOfPlayers);
        team.Players!.Should().NotContain(p => p.Name == "Duplicate");
        generatePlayerCalls.Should().BeGreaterThan(Constants.NumberOfPlayers);
        PlayerRepositoryMock.Verify(x => x.Exists("Duplicate"), Times.AtLeastOnce);
        VerifyAllMocks();
    }

    [Fact]
    public async Task CreateTeam_WhenGeneratePlayerReturnsNull_SkipsAndContinues()
    {
        const int universeId = 1;
        var universe = CreateResolvableUniverse(universeId);
        var generatePlayerCalls = 0;

        SetupTeamCreation(universe, resourceCount: 20);
        SetupPlayerExists(exists: false);
        UniverseDataServiceMock.Setup(x => x.GeneratePlayer(universe, It.IsAny<int>()))
            .ReturnsAsync((Universe _, int resourceId) =>
            {
                generatePlayerCalls++;
                return generatePlayerCalls <= 2
                    ? null
                    : CreateValidPlayer($"Player-{resourceId}", resourceId);
            });

        var team = await _teamManager.CreateTeam(universeId, "Team", attackersCount: 1, defendersCount: 1);

        team.Should().NotBeNull();
        team!.Players.Should().HaveCount(Constants.NumberOfPlayers);
        generatePlayerCalls.Should().BeGreaterThan(Constants.NumberOfPlayers);
        VerifyAllMocks();
    }

    [Fact]
    public async Task CreateTeam_WhenResourcesExhaustedDuringLoop_LogsAndReturnsNull()
    {
        const int universeId = 1;
        var universe = CreateResolvableUniverse(universeId);

        SetUpUniverse(universe);
        UniverseDataServiceMock.Setup(x => x.CanHandle(It.IsAny<Universe>())).Returns(true);
        UniverseDataServiceMock.Setup(x => x.GetAvailableResourceIds(universe))
            .ReturnsAsync([1, 2, 3, 4, 5]);
        UniverseRepository.Setup(x => x.GetUsedExternalResourceIds(universeId))
            .ReturnsAsync([]);
        UniverseDataServiceMock.Setup(x => x.GeneratePlayer(universe, It.IsAny<int>()))
            .ReturnsAsync((Universe _, int resourceId) => new Player("Invalid", 0, 0, resourceId));
        SetupLoggerMock(LoggerMock, LogLevel.Warning);

        var team = await _teamManager.CreateTeam(universeId, "Team", 1, 1);

        team.Should().BeNull();
        VerifyLog(LoggerMock, LogLevel.Warning, Times.Once(),
            "No more available resources for team Team in Universe IUniverse");
        TeamRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Never);
        UniverseDataServiceMock.Verify(x => x.GeneratePlayer(universe, It.IsAny<int>()), Times.Exactly(5));
        VerifyAllMocks();
    }

    [Fact]
    public async Task CreateTeam_UsesMatchingUniverseDataService()
    {
        const int universeId = 1;
        var universe = new Universe
        {
            Id = universeId,
            Name = "StarWars",
            ApiUrl = "https://swapi.dev/api/"
        };
        var starWarsService = new StarWarsTestUniverseDataService();
        var pokemonService = new PokemonTestUniverseDataService();

        SetUpUniverse(universe);
        UniverseRepository.Setup(x => x.GetUsedExternalResourceIds(universeId))
            .ReturnsAsync([]);
        PlayerRepositoryMock.Setup(x => x.Exists(It.IsAny<string>())).ReturnsAsync(false);
        TeamRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Team team, CancellationToken _) => team);

        var teamManager = new TeamManager(
            [starWarsService, pokemonService],
            _universeRepository,
            _teamRepository,
            _playerRepository,
            _logger);

        var team = await teamManager.CreateTeam(universeId, "Team", attackersCount: 1, defendersCount: 1);

        team.Should().NotBeNull();
        starWarsService.GeneratePlayerCallCount.Should().BeGreaterThan(0);
        pokemonService.WasCalled.Should().BeFalse();
        VerifyAllMocks();
    }

    private Mock<ITeamRepository> TeamRepositoryMock => Mock.Get(_teamRepository);
    private Mock<IUniverseRepository> UniverseRepository => Mock.Get(_universeRepository);
    private Mock<IPlayerRepository> PlayerRepositoryMock => Mock.Get(_playerRepository);
    private Mock<IUniverseDataService> UniverseDataServiceMock => Mock.Get(_universeDataService);
    private Mock<ILogger<TeamManager>> LoggerMock => Mock.Get(_logger);

    private static Universe CreateResolvableUniverse(int universeId) =>
        new()
        {
            Id = universeId,
            Name = "IUniverse",
            ApiUrl = "api"
        };

    private static Player CreateValidPlayer(string name, int resourceId) =>
        new(name, weight: 80, height: 180, resourceId);

    private void SetUpUniverse(Universe? value)
    {
        UniverseRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(value);
    }

    private void SetupTeamCreation(Universe universe, int resourceCount)
    {
        SetUpUniverse(universe);
        UniverseDataServiceMock.Setup(x => x.CanHandle(It.IsAny<Universe>())).Returns(true);
        UniverseDataServiceMock.Setup(x => x.GetAvailableResourceIds(universe))
            .ReturnsAsync(Enumerable.Range(1, resourceCount).ToList());
        UniverseRepository.Setup(x => x.GetUsedExternalResourceIds(universe.Id))
            .ReturnsAsync([]);
        TeamRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Team team, CancellationToken _) => team);
    }

    private void SetupPlayerExists(bool exists)
    {
        PlayerRepositoryMock.Setup(x => x.Exists(It.IsAny<string>())).ReturnsAsync(exists);
    }

    protected override void VerifyAllMocks()
    {
        TeamRepositoryMock.VerifyAll();
        UniverseRepository.VerifyAll();
        PlayerRepositoryMock.VerifyAll();
        UniverseDataServiceMock.VerifyAll();
        LoggerMock.VerifyAll();
    }

    private sealed class StarWarsTestUniverseDataService : IUniverseDataService
    {
        public int GeneratePlayerCallCount { get; private set; }

        public bool CanHandle(Universe universe)
        {
            return universe.Name.Equals("StarWars", StringComparison.OrdinalIgnoreCase);
        }

        public Task<List<int>> GetAvailableResourceIds(Universe universe) =>
            Task.FromResult(Enumerable.Range(1, 20).ToList());

        public Task<Player?> GeneratePlayer(Universe universe, int resourceId)
        {
            GeneratePlayerCallCount++;
            return Task.FromResult<Player?>(new Player($"SW-{resourceId}", 80, 180, resourceId));
        }
    }

    private sealed class PokemonTestUniverseDataService : IUniverseDataService
    {
        public bool WasCalled { get; private set; }

        public bool CanHandle(Universe universe)
        {
            return universe.Name.Equals("Pokemon", StringComparison.OrdinalIgnoreCase);
        }

        public Task<List<int>> GetAvailableResourceIds(Universe universe)
        {
            WasCalled = true;
            throw new InvalidOperationException("Pokemon service should not be called");
        }

        public Task<Player?> GeneratePlayer(Universe universe, int resourceId)
        {
            WasCalled = true;
            throw new InvalidOperationException("Pokemon service should not be called");
        }
    }
}
