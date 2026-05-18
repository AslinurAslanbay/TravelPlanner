using TravelPlanner.Application.Models;

namespace TravelPlanner.Application.Abstractions;

public interface ITravelDataService
{
    IReadOnlyList<CityCardModel> GetCities();
    CityDetailModel? GetCityBySlug(string slug);
}
