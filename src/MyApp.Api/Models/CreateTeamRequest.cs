namespace MyApp.Api.Models;

public record CreateTeamRequest(int UniverseId, string Name, int Attackers, int Defenders);