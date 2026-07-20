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
        GeoPointModel? startLocation = null,
        string routeType = "city")
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

        routeType = NormalizeRouteType(routeType);
        travelDays = routeType == "roundTrip" ? 1 : Math.Clamp(travelDays, 1, 14);
        var destination = new GeoPointModel
        {
            Label = city.Name,
            Latitude = city.Latitude,
            Longitude = city.Longitude
        };
        var orderedPlaces = BuildRoute(selectedPlaces, startLocation, destination, routeType);
        var stops = new List<RoutePlanStopModel>();
        double totalDistance = 0;
        var totalTravelMinutes = 0;
        var totalVisitMinutes = orderedPlaces.Sum(place => place.EstimatedVisitMinutes);
        var estimatedLegs = BuildEstimatedLegs(orderedPlaces, startLocation);
        var returnLeg = routeType == "roundTrip" && startLocation is not null
            ? BuildReturnLeg(orderedPlaces.Last(), startLocation)
            : (DistanceKm: 0d, TravelMinutes: 0);
        var targetDayMinutes = Math.Max(1, (int)Math.Ceiling((estimatedLegs.Sum(x => x.TravelMinutes) + returnLeg.TravelMinutes + totalVisitMinutes) / (double)travelDays));
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
                Latitude = orderedPlaces[index].Latitude,
                Longitude = orderedPlaces[index].Longitude,
                EstimatedVisitDuration = orderedPlaces[index].EstimatedVisitDuration,
                EstimatedVisitMinutes = orderedPlaces[index].EstimatedVisitMinutes,
                DistanceFromPreviousKm = Math.Round(distance, 1),
                TravelMinutesFromPrevious = travelMinutes,
                TravelDurationFromPrevious = FormatTravelDuration(travelMinutes),
                IsFromStartLocation = isFromStartLocation
            });
        }

        totalDistance += returnLeg.DistanceKm;
        totalTravelMinutes += returnLeg.TravelMinutes;

        var days = stops
            .GroupBy(stop => stop.DayNumber)
            .Select(group => new RoutePlanDayModel
            {
                DayNumber = group.Key,
                DistanceKm = Math.Round(group.Sum(stop => stop.DistanceFromPreviousKm) + (group.Key == currentDay ? Math.Round(returnLeg.DistanceKm, 1) : 0), 1),
                TravelMinutes = group.Sum(stop => stop.TravelMinutesFromPrevious) + (group.Key == currentDay ? returnLeg.TravelMinutes : 0),
                VisitMinutes = group.Sum(stop => stop.EstimatedVisitMinutes),
                Stops = group.ToList()
            })
            .ToList();

        return new RoutePlanModel
        {
            CityName = city.Name,
            RouteType = routeType,
            StartLocationName = startLocation?.Label,
            StartLatitude = startLocation?.Latitude,
            StartLongitude = startLocation?.Longitude,
            DestinationLatitude = destination.Latitude,
            DestinationLongitude = destination.Longitude,
            ReturnToStartDistanceKm = Math.Round(returnLeg.DistanceKm, 1),
            ReturnToStartTravelMinutes = returnLeg.TravelMinutes,
            ReturnToStartTravelDuration = FormatTravelDuration(returnLeg.TravelMinutes),
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

    private static (double DistanceKm, int TravelMinutes) BuildReturnLeg(Place lastPlace, GeoPointModel startLocation)
    {
        var distance = EstimateRoadDistanceKm(CalculateDistanceKm(
            lastPlace.Latitude,
            lastPlace.Longitude,
            startLocation.Latitude,
            startLocation.Longitude));

        return (distance, CalculateTravelMinutes(distance));
    }

    private static List<Place> BuildRoute(List<Place> places, GeoPointModel? startLocation, GeoPointModel destination, string routeType)
    {
        return routeType switch
        {
            "oneWay" => BuildOneWayRoute(places, startLocation, destination),
            "roundTrip" => BuildRoundTripRoute(places, startLocation),
            _ => BuildNearestNeighborRoute(places, startLocation)
        };
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

    private static List<Place> BuildOneWayRoute(List<Place> places, GeoPointModel? startLocation, GeoPointModel destination)
    {
        if (startLocation is null)
        {
            return BuildNearestNeighborRoute(places, startLocation);
        }

        var remaining = new List<Place>(places);
        var ordered = new List<Place>();
        double currentLatitude = startLocation.Latitude;
        double currentLongitude = startLocation.Longitude;
        var currentDestinationDistance = CalculateDistanceKm(currentLatitude, currentLongitude, destination.Latitude, destination.Longitude);

        while (remaining.Count > 0)
        {
            var next = remaining
                .OrderBy(place =>
                {
                    var legDistance = CalculateDistanceKm(currentLatitude, currentLongitude, place.Latitude, place.Longitude);
                    var destinationDistance = CalculateDistanceKm(place.Latitude, place.Longitude, destination.Latitude, destination.Longitude);
                    var backtrackPenalty = Math.Max(0, destinationDistance - currentDestinationDistance) * 2;
                    return legDistance + destinationDistance * 0.35 + backtrackPenalty;
                })
                .First();

            ordered.Add(next);
            remaining.Remove(next);
            currentLatitude = next.Latitude;
            currentLongitude = next.Longitude;
            currentDestinationDistance = CalculateDistanceKm(currentLatitude, currentLongitude, destination.Latitude, destination.Longitude);
        }

        return ordered;
    }

    private static List<Place> BuildRoundTripRoute(List<Place> places, GeoPointModel? startLocation)
    {
        var ordered = BuildNearestNeighborRoute(places, startLocation);
        if (startLocation is null || ordered.Count < 3)
        {
            return ordered;
        }

        var improved = true;
        while (improved)
        {
            improved = false;
            for (var i = 0; i < ordered.Count - 1; i++)
            {
                for (var j = i + 1; j < ordered.Count; j++)
                {
                    var candidate = new List<Place>(ordered);
                    candidate.Reverse(i, j - i + 1);
                    if (CalculateRoundTripDistance(candidate, startLocation) < CalculateRoundTripDistance(ordered, startLocation))
                    {
                        ordered = candidate;
                        improved = true;
                    }
                }
            }
        }

        return ordered;
    }

    private static double CalculateRoundTripDistance(List<Place> places, GeoPointModel startLocation)
    {
        double total = 0;
        for (var index = 0; index < places.Count; index++)
        {
            total += index == 0
                ? CalculateDistanceKm(startLocation, places[index])
                : CalculateDistanceKm(places[index - 1], places[index]);
        }

        total += CalculateDistanceKm(places[^1].Latitude, places[^1].Longitude, startLocation.Latitude, startLocation.Longitude);
        return total;
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

    private static string NormalizeRouteType(string routeType)
    {
        return routeType is "oneWay" or "roundTrip"
            ? routeType
            : "city";
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
