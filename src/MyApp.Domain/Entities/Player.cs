using System.ComponentModel.DataAnnotations;
using MyApp.Domain.Enums;

namespace MyApp.Domain.Entities;

public class Player(string name, double weight, double height, int externalResourceId)
    : BaseEntity
{
    [MaxLength(50)]
    public string Name { get; private set; } = name;

    [Range(50, 120)]
    public double Weight { get; private set; } = weight;

    [Range(150, 220)]
    public double Height { get; private set; } = height;

    public PlayerType Type { get; private set; }
    public int ExternalResourceId { get; private set; } = externalResourceId;
    public virtual Team Team { get; set; }
    
    public void SetPlayerType(PlayerType playerType)
    {
        Type = playerType;
    }
    
    public bool HasValidMeasurements => Weight != 0 && Height != 0;
}