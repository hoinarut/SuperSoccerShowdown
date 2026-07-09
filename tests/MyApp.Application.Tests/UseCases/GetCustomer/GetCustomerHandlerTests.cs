using MyApp.Application.DTOs;
using MyApp.Application.Handlers.Player;
using MyApp.Domain.Entities;
using MyApp.Domain.Ports;

namespace MyApp.Application.Tests.UseCases.GetCustomer;

public class GetPlayerHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsCustomer_WhenCustomerExists()
    {
        var customerId = Guid.NewGuid();
        var repository = new FakePlayerRepository(new Player(
            customerId,
            "Jane Doe",
            Email.Create("jane.doe@example.com")));

        var handler = new GetPlayerHandler(repository);

        var result = await handler.HandleAsync(new GetPlayerQuery(customerId));

        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
        Assert.Equal("Jane Doe", result.Name);
        Assert.Equal("jane.doe@example.com", result.Email);
    }

    [Fact]
    public async Task HandleAsync_ReturnsNull_WhenCustomerDoesNotExist()
    {
        var handler = new GetPlayerHandler(new FakePlayerRepository(null));

        var result = await handler.HandleAsync(new GetPlayerQuery(Guid.NewGuid()));

        Assert.Null(result);
    }

    private sealed class FakePlayerRepository(Player? customer) : IPlayerRepository
    {
        public Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(customer is not null && customer.Id == id ? customer : null);
    }
}
