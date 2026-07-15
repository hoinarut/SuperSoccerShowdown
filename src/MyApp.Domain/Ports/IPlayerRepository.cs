using MyApp.Domain.Entities;

namespace MyApp.Domain.Ports;

public interface IPlayerRepository : IRepository<Player>
{
    Task<bool> Exists(string name);
}