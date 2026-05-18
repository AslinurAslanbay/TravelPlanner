namespace TravelPlanner.Application.Models;

public sealed class RoutePlanDayModel
{
    public int DayNumber { get; init; }
    public double DistanceKm { get; init; }
    public int TravelMinutes { get; init; }
    public int VisitMinutes { get; init; }
    public IReadOnlyList<RoutePlanStopModel> Stops { get; init; } = Array.Empty<RoutePlanStopModel>();
}
