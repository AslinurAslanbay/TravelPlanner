using TravelPlanner.Application.Abstractions;
using TravelPlanner.Application.Models;
using TravelPlanner.Infrastructure.Service.Data;

namespace TravelPlanner.Infrastructure.Service.Services;

public sealed class InMemoryTravelDataService : ITravelDataService
{
    public IReadOnlyList<CityCardModel> GetCities()
    {
        return TravelSeedData.Cities
            .Select(city => new CityCardModel
            {
                Slug = city.Slug,
                Name = city.Name,
                Region = city.Region,
                Summary = city.Summary,
                PlaceCount = city.Places.Count
            })
            .ToList();
    }

    public CityDetailModel? GetCityBySlug(string slug)
    {
        var city = TravelSeedData.Cities.FirstOrDefault(x => x.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
        if (city is null)
        {
            return null;
        }

        return new CityDetailModel
        {
            Slug = city.Slug,
            Name = city.Name,
            Region = city.Region,
            Summary = city.Summary,
            HeroTitle = city.HeroTitle,
            HeroDescription = city.HeroDescription,
            Places = city.Places
                .Select(place => new PlaceCardModel
                {
                    Id = place.Id,
                    Name = place.Name,
                    Description = place.Description,
                    Category = place.Category,
                    EstimatedVisitDuration = place.EstimatedVisitDuration
                })
                .ToList()
        };
    }
}
