namespace TravelPlanner.Application.Models;

public sealed record GeoPointModel
{
    public string Label { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}
