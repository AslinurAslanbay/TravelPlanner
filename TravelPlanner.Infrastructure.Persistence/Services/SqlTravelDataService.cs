using Microsoft.EntityFrameworkCore;
using TravelPlanner.Application.Abstractions;
using TravelPlanner.Application.Models;

namespace TravelPlanner.Infrastructure.Persistence.Services;

public sealed class SqlTravelDataService : ITravelDataService
{
    private readonly TravelPlannerDbContext _dbContext;

    public SqlTravelDataService(TravelPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IReadOnlyList<CityCardModel> GetCities()
    {
        return _dbContext.Cities
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CityCardModel
            {
                Slug = x.Slug,
                Name = x.Name,
                Region = x.Region,
                Summary = x.Summary,
                PlaceCount = x.Places.Count
            })
            .ToList();
    }

    public CityDetailModel? GetCityBySlug(string slug)
    {
        return _dbContext.Cities
            .AsNoTracking()
            .Where(x => x.Slug == slug)
            .Select(x => new CityDetailModel
            {
                Slug = x.Slug,
                Name = x.Name,
                Region = x.Region,
                Summary = x.Summary,
                HeroTitle = x.HeroTitle,
                HeroDescription = x.HeroDescription,
                Places = x.Places
                    .OrderBy(place => place.Name)
                    .Select(place => new PlaceCardModel
                    {
                        Id = place.Id,
                        Name = place.Name,
                        Description = place.Description,
                        Category = place.Category,
                        EstimatedVisitDuration = place.EstimatedVisitDuration
                    })
                    .ToList()
            })
            .FirstOrDefault();
    }
}
