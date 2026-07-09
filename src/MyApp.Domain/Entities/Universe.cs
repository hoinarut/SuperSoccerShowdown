using System.ComponentModel.DataAnnotations;

namespace MyApp.Domain.Entities;

public class Universe : BaseEntity
{
    [MaxLength(50)]
    public required string Name { get; init; }
    public bool IsEnabled { get; init; }
    [MaxLength(100)]
    public required string ApiUrl { get; init; }
}