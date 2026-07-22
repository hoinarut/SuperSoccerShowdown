using System.ComponentModel.DataAnnotations;
using MyApp.Domain.Enums;

namespace MyApp.Domain.Entities;

public class Player(string name, Weight weight, Height height, int externalResourceId)
    : BaseEntity
{
    [MaxLength(50)]
    public string Name { get; private set; } = name;

    public List<PlayerStat> Stats { get; private set; } = [weight, height];

    public Weight Weight => GetStat<Weight>();

    public Height Height => GetStat<Height>();

    public PlayerType Type { get; private set; }
    public int ExternalResourceId { get; private set; } = externalResourceId;
    public virtual Team Team { get; set; }
    
    public void SetPlayerType(PlayerType playerType)
    {
        Type = playerType;
    }

    public void AddStat(PlayerStat stat)
    {
        if (Stats.Any(existing => existing.GetType() == stat.GetType()))
        {
            throw new InvalidOperationException($"Player already has a {stat.GetType().Name} stat.");
        }

        Stats.Add(stat);
    }
    
    public bool HasValidMeasurements => Stats.All(stat => stat.IsValid);

    private T GetStat<T>() where T : PlayerStat =>
        Stats.OfType<T>().Single();
}
