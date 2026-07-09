using Mediator;
using MyApp.Application.DTOs;

namespace MyApp.Application.Handlers.Player;

public sealed record GetPlayerQuery(int PlayerId) : IQuery<PlayerDto?>;
