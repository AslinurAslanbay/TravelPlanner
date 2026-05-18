using Microsoft.EntityFrameworkCore;
using TravelPlanner.Application.Abstractions;
using TravelPlanner.Application.Models;
using TravelPlanner.Domain.Entities;

namespace TravelPlanner.Infrastructure.Persistence.Services;

public sealed class SqlTripPlannerService : ITripPlannerService
{
    private readonly TravelPlannerDbContext _dbContext;

    public SqlTripPlannerService(TravelPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public RoutePlanModel? CreatePlan(
        string citySlug,
        IReadOnlyCollection<int> selectedPlaceIds,
        int travelDays,
        GeoPointModel? startLocation = null)
    {
        if (selectedPlaceIds.Count == 0)
        {
            return null;
        }

        var city = _dbContext.Cities
            .AsNoTracking()
            .Include(x => x.Places)
            .FirstOrDefault(x => x.Slug == citySlug);

        if (city is null)
        {
            return null;
        }

        var selectedPlaces = city.Places
            .Where(place => selectedPlaceIds.Contains(place.Id))
            .ToList();

        if (selectedPlaces.Count == 0)
        {
            return null;
        }

        travelDays = Math.Clamp(travelDays, 1, 14);
        var orderedPlaces = BuildNearestNeighborRoute(selectedPlaces, startLocation);
        var stops = new List<RoutePlanStopModel>();
        double totalDistance = 0;
        var totalTravelMinutes = 0;
        var totalVisitMinutes = orderedPlaces.Sum(place => place.EstimatedVisitMinutes);
        var estimatedLegs = BuildEstimatedLegs(orderedPlaces, startLocation);
        var targetDayMinutes = Math.Max(1, (int)Math.Ceiling((estimatedLegs.Sum(x => x.TravelMinutes) + totalVisitMinutes) / (double)travelDays));
        var currentDay = 1;
        var currentDayMinutes = 0;

        for (var index = 0; index < orderedPlaces.Count; index++)
        {
            var isFromStartLocation = index == 0 && startLocation is not null;
            var distance = estimatedLegs[index].DistanceKm;
            var travelMinutes = estimatedLegs[index].TravelMinutes;
            var stopActivityMinutes = travelMinutes + orderedPlaces[index].EstimatedVisitMinutes;

            if (currentDay < travelDays &&
                currentDayMinutes > 0 &&
                currentDayMinutes + stopActivityMinutes > targetDayMinutes)
            {
                currentDay++;
                currentDayMinutes = 0;
            }

            totalDistance += distance;
            totalTravelMinutes += travelMinutes;
            currentDayMinutes += stopActivityMinutes;

            stops.Add(new RoutePlanStopModel
            {
                Order = index + 1,
                DayNumber = currentDay,
                PlaceName = orderedPlaces[index].Name,
                Category = orderedPlaces[index].Category,
                EstimatedVisitDuration = orderedPlaces[index].EstimatedVisitDuration,
                EstimatedVisitMinutes = orderedPlaces[index].EstimatedVisitMinutes,
                DistanceFromPreviousKm = Math.Round(distance, 1),
                TravelMinutesFromPrevious = travelMinutes,
                TravelDurationFromPrevious = FormatTravelDuration(travelMinutes),
                IsFromStartLocation = isFromStartLocation
            });
        }

        var days = stops
            .GroupBy(stop => stop.DayNumber)
            .Select(group => new RoutePlanDayModel
            {
                DayNumber = group.Key,
                DistanceKm = Math.Round(group.Sum(stop => stop.DistanceFromPreviousKm), 1),
                TravelMinutes = group.Sum(stop => stop.TravelMinutesFromPrevious),
                VisitMinutes = group.Sum(stop => stop.EstimatedVisitMinutes),
                Stops = group.ToList()
            })
            .ToList();

        return new RoutePlanModel
        {
            CityName = city.Name,
            StartLocationName = startLocation?.Label,
            TravelDays = travelDays,
            SelectedCount = orderedPlaces.Count,
            TotalDistanceKm = Math.Round(totalDistance, 1),
            TotalTravelMinutes = totalTravelMinutes,
            TotalVisitMinutes = totalVisitMinutes,
            EstimatedTravelDuration = FormatTravelDuration(totalTravelMinutes),
            Stops = stops,
            Days = days
        };
    }

    private static List<(double DistanceKm, int TravelMinutes)> BuildEstimatedLegs(List<Place> orderedPlaces, GeoPointModel? startLocation)
    {
        var legs = new List<(double DistanceKm, int TravelMinutes)>();

        for (var index = 0; index < orderedPlaces.Count; index++)
        {
            var airDistance = index == 0 && startLocation is not null
                ? CalculateDistanceKm(startLocation, orderedPlaces[index])
                : index == 0
                    ? 0
                    : CalculateDistanceKm(orderedPlaces[index - 1], orderedPlaces[index]);
            var distance = EstimateRoadDistanceKm(airDistance);

            legs.Add((distance, CalculateTravelMinutes(distance)));
        }

        return legs;
    }

    private static List<Place> BuildNearestNeighborRoute(List<Place> places, GeoPointModel? startLocation)
    {
        var remaining = new List<Place>(places);
        var ordered = new List<Place>();
        var current = startLocation is null
            ? remaining
                .OrderBy(place => place.Latitude)
                .ThenBy(place => place.Longitude)
                .First()
            : remaining
                .OrderBy(place => CalculateDistanceKm(startLocation, place))
                .First();

        ordered.Add(current);
        remaining.Remove(current);

        while (remaining.Count > 0)
        {
            var next = remaining
                .OrderBy(place => CalculateDistanceKm(current, place))
                .First();

            ordered.Add(next);
            remaining.Remove(next);
            current = next;
        }

        return ordered;
    }

    private static double CalculateDistanceKm(GeoPointModel from, Place to)
    {
        return CalculateDistanceKm(from.Latitude, from.Longitude, to.Latitude, to.Longitude);
    }

    private static double CalculateDistanceKm(Place from, Place to)
    {
        return CalculateDistanceKm(from.Latitude, from.Longitude, to.Latitude, to.Longitude);
    }

    private static double CalculateDistanceKm(double fromLatitudeValue, double fromLongitudeValue, double toLatitudeValue, double toLongitudeValue)
    {
        const double earthRadiusKm = 6371;

        var latitudeDelta = DegreesToRadians(toLatitudeValue - fromLatitudeValue);
        var longitudeDelta = DegreesToRadians(toLongitudeValue - fromLongitudeValue);
        var fromLatitude = DegreesToRadians(fromLatitudeValue);
        var toLatitude = DegreesToRadians(toLatitudeValue);

        var a =
            Math.Sin(latitudeDelta / 2) * Math.Sin(latitudeDelta / 2) +
            Math.Cos(fromLatitude) * Math.Cos(toLatitude) *
            Math.Sin(longitudeDelta / 2) * Math.Sin(longitudeDelta / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    private static int CalculateTravelMinutes(double distanceKm)
    {
        if (distanceKm <= 0)
        {
            return 0;
        }

        var averageRoadSpeedKmPerHour = distanceKm switch
        {
            <= 5 => 28,
            <= 30 => 45,
            _ => 70
        };

        return Math.Max(1, (int)Math.Ceiling(distanceKm / averageRoadSpeedKmPerHour * 60));
    }

    private static double EstimateRoadDistanceKm(double airDistanceKm)
    {
        if (airDistanceKm <= 0)
        {
            return 0;
        }

        var roadFactor = airDistanceKm switch
        {
            <= 5 => 1.25,
            <= 30 => 1.2,
            _ => 1.3
        };

        return airDistanceKm * roadFactor;
    }

    private static string FormatTravelDuration(int totalMinutes)
    {
        var hours = totalMinutes / 60;
        var minutes = totalMinutes % 60;

        if (hours == 0)
        {
            return $"{minutes} dk";
        }

        return minutes == 0
            ? $"{hours} saat"
            : $"{hours} saat {minutes} dk";
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
