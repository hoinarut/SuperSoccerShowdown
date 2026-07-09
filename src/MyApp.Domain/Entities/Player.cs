using System.ComponentModel.DataAnnotations;
using MyApp.Domain.Enums;

namespace MyApp.Domain.Entities;

public class Player : BaseEntity
{
    [MaxLength(50)]
    public string Name { get; private set; }
    [Range(50, 120)]
    public decimal Weight { get; private set; }
    [Range(150, 220)]
    public decimal Height { get; private set; }
    public PlayerType Type { get; private set; }

    public Player(int id, string name, decimal weight, decimal height)
    {
        Id = id;
        Name = name;
        Weight = weight;
        Height = height;
    }
    public Player(string name, decimal weight, decimal height)
    {
        Name = name;
        Weight = weight;
        Height = height;
    }
    
    public void SetPlayerType(PlayerType playerType)
    {
        Type = playerType;
    }
}