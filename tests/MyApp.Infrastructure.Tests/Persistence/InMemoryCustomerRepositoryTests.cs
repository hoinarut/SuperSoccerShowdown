using MyApp.Infrastructure.Persistence;

namespace MyApp.Infrastructure.Tests.Persistence;

public class InMemoryPlayerRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_ReturnsSampleCustomer_WhenIdMatches()
    {
        var repository = new InMemoryPlayerRepository();
        var sampleId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var customer = await repository.GetByIdAsync(sampleId);

        Assert.NotNull(customer);
        Assert.Equal("Jane Doe", customer.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenIdDoesNotMatch()
    {
        var repository = new InMemoryPlayerRepository();

        var customer = await repository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(customer);
    }
}
