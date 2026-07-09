using System.ComponentModel.DataAnnotations;
using MyApp.Domain.Enums;

namespace MyApp.Domain.Entities;

public class Team(string name, int universeId, int attackersCount, int defendersCount) : BaseEntity
{
    [MaxLength(50)]
    public string Name { get; private set; } = name;
    
    [Range(0, 4)]
    public int AttackersCount { get; private set; } = attackersCount;
    
    [Range(0, 4)]
    public int DefendersCount { get; private set; } = defendersCount;
    
    [MaxLength(Constants.NumberOfPlayers)]
    public List<Player>? Players { get; private set; }
    
    public int UniverseId { get; private set; } = universeId;

    public void AddPlayer(Player player)
    {
        Players ??= [];
        if (Players.Count == Constants.NumberOfPlayers)
        {
            throw new InvalidOperationException(Constants.ErrorMessages.MaxPlayersReached);
        }
        Players.Add(player);
    }

    public void SetPlayerTypes()
    {
        if(Players is null)
        {
            throw new InvalidOperationException(Constants.ErrorMessages.PlayersNotSet);
        }
        var tallest = Players.MaxBy(p => p.Height);
        tallest!.SetPlayerType(PlayerType.Goalie);

        var defenders = Players.OrderByDescending(p => p.Weight).Take(DefendersCount);
        foreach (var player in defenders)
        {
            player.SetPlayerType(PlayerType.Defence);
        }
        
        var attackers = Players.OrderBy(p =>p.Height).Take(AttackersCount);
        foreach (var player in attackers)
        {
            player.SetPlayerType(PlayerType.Offence);
        }
    }

    public void ValidateSetup()
    {
        if(Players!.Count != Constants.NumberOfPlayers)
        {
            throw new InvalidOperationException(Constants.ErrorMessages.PlayersNotSet);
        }
        if (Players.Count(p => p.Type == PlayerType.Goalie) != 1)
        {
            throw new InvalidOperationException(Constants.ErrorMessages.GoalieNotSet);
        }
        if(Players!.Count(p => p.Type == PlayerType.Defence) != DefendersCount)
        {
            throw new InvalidOperationException(Constants.ErrorMessages.DefendersNotSet);
        }
        if(Players!.Count(p => p.Type == PlayerType.Offence) != AttackersCount)
        {
            throw new InvalidOperationException(Constants.ErrorMessages.AttackersNotSet);
        }
    }
}