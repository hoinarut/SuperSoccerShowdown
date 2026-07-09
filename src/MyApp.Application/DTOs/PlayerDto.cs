using MyApp.Domain.Enums;

namespace MyApp.Application.DTOs;

public sealed record PlayerDto(int Id, string Name, decimal Weight, decimal Height, PlayerType PlayerType);
