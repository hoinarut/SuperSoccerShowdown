using MyApp.Domain;
using MyApp.Domain.Entities;
using MyApp.Domain.Ports;

namespace MyApp.Application.Services;

public class TeamManager(
    IEnumerable<IUniverseDataService> universeDataServices,
    IUniverseRepository universeRepository,
    ITeamRepository teamRepository) : ITeamManager
{
    public async Task<Team> CreateTeam(int universeId, string name, int attackersCount, int defendersCount)
    {
        var universe = await universeRepository.GetByIdAsync(universeId);
        
        if (universe == null)
        {
            throw new ArgumentException("Universe not found");
        }

        var universeDataService = universeDataServices.FirstOrDefault(x => x.GetType().Name.StartsWith(universe.Name));
        
        if (universeDataService == null)
        {
            throw new ArgumentException($"Universe {universe.Name} not supported");
        }

        var team = new Team(name, universeId, attackersCount, defendersCount);
        
        var availableResourceIds = await universeDataService.GetAvailableResourceIds();
        var selected = new List<int>();
        var rand = new Random();
        while (team.Players?.Count < Constants.NumberOfPlayers)
        {
            var resourceId = GetRandomNumber(rand, 1, availableResourceIds.Count, selected);
            selected.Add(resourceId);
            team.AddPlayer(await universeDataService.GeneratePlayer(resourceId));
        }
        
        team.SetPlayerTypes();
        team.ValidateSetup();
        await teamRepository.CreateAsync(team);
        
        return team;
    }
    
    private static int GetRandomNumber(Random rand, int min, int max, List<int> alreadySelected)
    {
        var number = rand.Next(min, max);
        while (alreadySelected.Contains(number))
        {
            number = rand.Next(min, max);
        }
        return number;
    }
}