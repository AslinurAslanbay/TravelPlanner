using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TravelPlanner.Application.Abstractions;
using TravelPlanner.Infrastructure.Persistence.Services;

namespace TravelPlanner.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddTravelPlannerPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TravelPlannerDb")
            ?? throw new InvalidOperationException("Connection string 'TravelPlannerDb' was not found.");

        services.AddDbContext<TravelPlannerDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ITravelDataService, SqlTravelDataService>();
        services.AddScoped<ITripPlannerService, SqlTripPlannerService>();

        return services;
    }
}
