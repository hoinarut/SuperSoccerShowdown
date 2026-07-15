using Microsoft.Extensions.Logging;
using MyApp.Domain;
using MyApp.Domain.Entities;
using MyApp.Domain.Ports;

namespace MyApp.Application.Services;

public class TeamManager(
    IEnumerable<IUniverseDataService> universeDataServices,
    IUniverseRepository universeRepository,
    ITeamRepository teamRepository,
    IPlayerRepository playerRepository,
    ILogger<TeamManager> logger) : ITeamManager
{
    public async Task<Team?> CreateTeam(int universeId, string name, int attackersCount, int defendersCount)
    {
        var universe = await universeRepository.GetByIdAsync(universeId);
        
        if (universe == null)
        {
            throw new ArgumentException("Universe not found");
        }

        var universeDataService = universeDataServices.FirstOrDefault(x => x.CanHandle(universe));
        
        if (universeDataService == null)
        {
            throw new ArgumentException($"Universe {universe.Name} not supported");
        }

        var team = new Team(name, universeId, attackersCount, defendersCount);
        
        var availableResourceIds = await universeDataService.GetAvailableResourceIds(universe);
        var alreadySelected = await universeRepository.GetUsedExternalResourceIds(universeId);
        
        var rand = new Random();
        while (team.Players?.Count < Constants.NumberOfPlayers)
        {
            if(alreadySelected.Count == availableResourceIds.Count)
            {
                logger.LogWarning("No more available resources for team {TeamName} in Universe {Universe}", team.Name, universe.Name);
                return null;
            }
            var resourceId = GetRandomNumber(rand, 1, availableResourceIds.Count, alreadySelected);
            alreadySelected.Add(resourceId);
            var player = await universeDataService.GeneratePlayer(universe, resourceId);
            if( player is { HasValidMeasurements: true } && !await playerRepository.Exists(player.Name))
            {
                team.AddPlayer(player);
            }
        }
        
        team.SetPlayerTypes();
        team.ValidateSetup();
        await teamRepository.AddAsync(team);
        
        return team;
    }
    
    private static int GetRandomNumber(Random rand, int min, int max, List<int> alreadySelected)
    {
        var number = rand.Next(min, max + 1);
        while (alreadySelected.Contains(number))
        {
            number = rand.Next(min, max + 1);
        }
        return number;
    }
}