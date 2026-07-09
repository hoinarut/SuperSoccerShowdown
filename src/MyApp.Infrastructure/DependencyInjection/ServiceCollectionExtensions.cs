using Microsoft.Extensions.DependencyInjection;
using MyApp.Domain.Ports;
using MyApp.Infrastructure.Persistence;

namespace MyApp.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPlayerRepository, InMemoryPlayerRepository>();
        return services;
    }
}
