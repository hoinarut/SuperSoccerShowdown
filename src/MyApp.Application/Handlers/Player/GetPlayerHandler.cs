using Mediator;
using MyApp.Application.DTOs;
using MyApp.Domain.Ports;

namespace MyApp.Application.Handlers.Player;

public sealed class GetPlayerHandler(IPlayerRepository playerRepository)
    : IQueryHandler<GetPlayerQuery, PlayerDto?>
{
    public async ValueTask<PlayerDto?> Handle(GetPlayerQuery query, CancellationToken cancellationToken)
    {
        var player = await playerRepository.GetByIdAsync(query.PlayerId, cancellationToken);

        return player is null
            ? null
            : new PlayerDto(player.Id, player.Name, player.Weight, player.Height, player.Type);
    }
}
