namespace TravelPlanner.Application.Models;

public sealed class CityCardModel
{
    public string Slug { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public int PlaceCount { get; init; }
}
