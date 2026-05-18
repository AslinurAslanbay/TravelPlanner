namespace TravelPlanner.Application.Models;

public sealed class CityDetailModel
{
    public string Slug { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string HeroTitle { get; init; } = string.Empty;
    public string HeroDescription { get; init; } = string.Empty;
    public IReadOnlyList<PlaceCardModel> Places { get; init; } = Array.Empty<PlaceCardModel>();
}
