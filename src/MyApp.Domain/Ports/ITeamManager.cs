using MyApp.Domain.Entities;

namespace MyApp.Domain.Ports;

public interface ITeamManager
{
    Task<Team> CreateTeam(int universeId, string name, int attackersCount, int defendersCount);
}