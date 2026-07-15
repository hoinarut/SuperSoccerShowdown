using MyApp.Domain.Enums;

namespace MyApp.Application.DTOs;

public sealed record PlayerDto(int Id, string Name, double Weight, double Height, PlayerType PlayerType, int ExternalResourceId);
