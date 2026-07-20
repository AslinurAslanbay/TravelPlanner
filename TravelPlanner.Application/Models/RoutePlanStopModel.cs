namespace TravelPlanner.Application.Models;

public sealed class RoutePlanStopModel
{
    public int Order { get; init; }
    public int DayNumber { get; init; }
    public string PlaceName { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string EstimatedVisitDuration { get; init; } = string.Empty;
    public int EstimatedVisitMinutes { get; init; }
    public double DistanceFromPreviousKm { get; init; }
    public int TravelMinutesFromPrevious { get; init; }
    public string TravelDurationFromPrevious { get; init; } = string.Empty;
    public bool IsFromStartLocation { get; init; }
}
