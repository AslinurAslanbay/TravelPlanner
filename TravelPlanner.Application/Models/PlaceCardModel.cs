namespace TravelPlanner.Application.Models;

public sealed class PlaceCardModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string EstimatedVisitDuration { get; init; } = string.Empty;
}
