using Microsoft.Extensions.DependencyInjection;
using TravelPlanner.Application.Abstractions;
using TravelPlanner.Infrastructure.Service.Services;

namespace TravelPlanner.Infrastructure.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddTravelPlannerServices(this IServiceCollection services)
    {
        services.AddSingleton<ITravelDataService, InMemoryTravelDataService>();
        services.AddSingleton<ITripPlannerService, GreedyTripPlannerService>();
        return services;
    }
}
