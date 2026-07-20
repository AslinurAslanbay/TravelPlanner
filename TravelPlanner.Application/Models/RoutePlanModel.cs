namespace TravelPlanner.Application.Models;

public sealed class RoutePlanModel
{
    public string CityName { get; init; } = string.Empty;
    public string RouteType { get; init; } = "city";
    public string? StartLocationName { get; init; }
    public double? StartLatitude { get; init; }
    public double? StartLongitude { get; init; }
    public double? DestinationLatitude { get; init; }
    public double? DestinationLongitude { get; init; }
    public double ReturnToStartDistanceKm { get; init; }
    public int ReturnToStartTravelMinutes { get; init; }
    public string ReturnToStartTravelDuration { get; init; } = string.Empty;
    public int TravelDays { get; init; }
    public int SelectedCount { get; init; }
    public double TotalDistanceKm { get; init; }
    public int TotalTravelMinutes { get; init; }
    public int TotalVisitMinutes { get; init; }
    public string EstimatedTravelDuration { get; init; } = string.Empty;
    public IReadOnlyList<RoutePlanStopModel> Stops { get; init; } = Array.Empty<RoutePlanStopModel>();
    public IReadOnlyList<RoutePlanDayModel> Days { get; init; } = Array.Empty<RoutePlanDayModel>();
}
