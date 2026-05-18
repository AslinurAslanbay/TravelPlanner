namespace TravelPlanner.Application.Models;

public sealed class RoutePlanModel
{
    public string CityName { get; init; } = string.Empty;
    public string? StartLocationName { get; init; }
    public int TravelDays { get; init; }
    public int SelectedCount { get; init; }
    public double TotalDistanceKm { get; init; }
    public int TotalTravelMinutes { get; init; }
    public int TotalVisitMinutes { get; init; }
    public string EstimatedTravelDuration { get; init; } = string.Empty;
    public IReadOnlyList<RoutePlanStopModel> Stops { get; init; } = Array.Empty<RoutePlanStopModel>();
    public IReadOnlyList<RoutePlanDayModel> Days { get; init; } = Array.Empty<RoutePlanDayModel>();
}
