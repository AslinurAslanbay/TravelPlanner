using TravelPlanner.Application.Models;

namespace TravelPlanner.Application.Abstractions;

public interface ITripPlannerService
{
    RoutePlanModel? CreatePlan(
        string citySlug,
        IReadOnlyCollection<int> selectedPlaceIds,
        int travelDays,
        GeoPointModel? startLocation = null);
}
