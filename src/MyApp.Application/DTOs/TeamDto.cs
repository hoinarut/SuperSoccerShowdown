namespace MyApp.Application.DTOs;

public sealed record TeamDto(int Id, string Name, int Attackers, int Defenders, List<PlayerDto> Players);