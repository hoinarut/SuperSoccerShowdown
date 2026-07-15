using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Services;
using MyApp.Domain.Ports;

namespace MyApp.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        
        services.AddScoped<ITeamManager, TeamManager>();
        return services;
    }
}
