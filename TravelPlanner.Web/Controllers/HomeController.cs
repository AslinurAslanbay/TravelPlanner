using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Application.Abstractions;
using TravelPlanner.Application.Models;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.Web.Models;

namespace TravelPlanner.Web.Controllers;

public sealed class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITravelDataService _travelDataService;
    private readonly ITripPlannerService _tripPlannerService;
    private readonly TravelPlannerDbContext _dbContext;

    public HomeController(
        ILogger<HomeController> logger,
        ITravelDataService travelDataService,
        ITripPlannerService tripPlannerService,
        TravelPlannerDbContext dbContext)
    {
        _logger = logger;
        _travelDataService = travelDataService;
        _tripPlannerService = tripPlannerService;
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
        var cities = _travelDataService.GetCities();
        return View(cities);
    }

    public IActionResult City(string slug, string? startLocation)
    {
        var city = _travelDataService.GetCityBySlug(slug);
        if (city is null)
        {
            return NotFound();
        }

        ViewData["Title"] = city.Name;
        ViewBag.StartLocation = startLocation;
        return View(city);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreatePlan(string slug)
    {
        var city = _travelDataService.GetCityBySlug(slug);
        if (city is null)
        {
            return NotFound();
        }

        var selectedPlaceIds = Request.Form["selectedPlaceIds"]
            .Select(id => int.TryParse(id, out var parsedId) ? parsedId : 0)
            .Where(id => id > 0)
            .ToList();
        var startCity = Request.Form["startCity"].FirstOrDefault();
        var startDistrict = Request.Form["startDistrict"].FirstOrDefault();
        var startNeighborhood = Request.Form["startNeighborhood"].FirstOrDefault();
        var startAddress = Request.Form["startAddress"].FirstOrDefault();
        var startLocation = Request.Form["startLocation"].FirstOrDefault();
        var routeType = NormalizeRouteType(Request.Form["routeType"].FirstOrDefault());
        var travelDays = int.TryParse(Request.Form["travelDays"].FirstOrDefault(), out var parsedTravelDays)
            ? Math.Clamp(parsedTravelDays, 1, 14)
            : 1;
        if (routeType == "roundTrip")
        {
            travelDays = 1;
        }

        if (string.IsNullOrWhiteSpace(startLocation))
        {
            startLocation = string.Join(" ", new[] { startCity, startDistrict, startNeighborhood, startAddress }
                .Where(part => !string.IsNullOrWhiteSpace(part)));
        }

        var selectedSet = new HashSet<int>(selectedPlaceIds);
        var startPoint = ResolveStartPoint(startCity, startDistrict, startNeighborhood, startLocation);
        var routePlan = _tripPlannerService.CreatePlan(slug, selectedPlaceIds, travelDays, startPoint, routeType);

        ViewData["Title"] = city.Name;
        ViewBag.RoutePlan = routePlan;
        ViewBag.SelectedPlaceIds = selectedSet;
        ViewBag.StartLocation = startLocation;
        ViewBag.StartCity = startCity;
        ViewBag.StartDistrict = startDistrict;
        ViewBag.StartNeighborhood = startNeighborhood;
        ViewBag.StartAddress = startAddress;
        ViewBag.RouteType = routeType;
        ViewBag.TravelDays = travelDays;

        return View("City", city);
    }

    private GeoPointModel? ResolveStartPoint(
        string? province,
        string? district,
        string? neighborhood,
        string? label)
    {
        var resolvedLabel = string.IsNullOrWhiteSpace(label)
            ? string.Join(" ", new[] { province, district, neighborhood }.Where(part => !string.IsNullOrWhiteSpace(part)))
            : label;

        if (string.IsNullOrWhiteSpace(resolvedLabel))
        {
            return null;
        }

        var location = _dbContext.Neighborhoods
            .AsNoTracking()
            .Where(x =>
                x.Name == neighborhood &&
                x.Locality != null &&
                x.Locality.District != null &&
                x.Locality.District.Province != null &&
                x.Locality.District.Name == district &&
                x.Locality.District.Province.Name == province)
            .Select(x => new
            {
                NeighborhoodLatitude = x.Latitude,
                NeighborhoodLongitude = x.Longitude,
                LocalityLatitude = x.Locality!.Latitude,
                LocalityLongitude = x.Locality!.Longitude,
                DistrictLatitude = x.Locality.District!.Latitude,
                DistrictLongitude = x.Locality.District!.Longitude,
                ProvinceLatitude = x.Locality.District.Province!.Latitude,
                ProvinceLongitude = x.Locality.District.Province!.Longitude
            })
            .FirstOrDefault();

        if (location is not null &&
            TryCreatePoint(
                resolvedLabel,
                location.NeighborhoodLatitude ?? location.LocalityLatitude ?? location.DistrictLatitude ?? location.ProvinceLatitude,
                location.NeighborhoodLongitude ?? location.LocalityLongitude ?? location.DistrictLongitude ?? location.ProvinceLongitude,
                out var point))
        {
            return point;
        }

        var districtLocation = _dbContext.Districts
            .AsNoTracking()
            .Where(x => x.Name == district && x.Province != null && x.Province.Name == province)
            .Select(x => new
            {
                DistrictLatitude = x.Latitude,
                DistrictLongitude = x.Longitude,
                ProvinceLatitude = x.Province!.Latitude,
                ProvinceLongitude = x.Province!.Longitude
            })
            .FirstOrDefault();

        if (districtLocation is not null &&
            TryCreatePoint(
                resolvedLabel,
                districtLocation.DistrictLatitude ?? districtLocation.ProvinceLatitude,
                districtLocation.DistrictLongitude ?? districtLocation.ProvinceLongitude,
                out point))
        {
            return point;
        }

        var provinceLocation = _dbContext.Provinces
            .AsNoTracking()
            .Where(x => x.Name == province)
            .Select(x => new { x.Latitude, x.Longitude })
            .FirstOrDefault();

        if (provinceLocation is not null &&
            TryCreatePoint(resolvedLabel, provinceLocation.Latitude, provinceLocation.Longitude, out point))
        {
            return point;
        }

        return null;
    }

    private static bool TryCreatePoint(string label, double? latitude, double? longitude, out GeoPointModel point)
    {
        if (latitude.HasValue && longitude.HasValue)
        {
            point = new GeoPointModel
            {
                Label = label,
                Latitude = latitude.Value,
                Longitude = longitude.Value
            };
            return true;
        }

        point = new GeoPointModel();
        return false;
    }

    private static string NormalizeRouteType(string? routeType)
    {
        return routeType is "oneWay" or "roundTrip"
            ? routeType
            : "city";
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Provinces()
    {
        var provinces = _dbContext.Provinces
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => x.Name)
            .ToList();

        return Json(provinces);
    }

    [HttpGet]
    public IActionResult Districts(string province)
    {
        var districts = _dbContext.Districts
            .AsNoTracking()
            .Where(x => x.Province != null && x.Province.Name == province)
            .OrderBy(x => x.Name)
            .Select(x => x.Name)
            .ToList();

        return Json(districts);
    }

    [HttpGet]
    public IActionResult Neighborhoods(string province, string district)
    {
        var neighborhoods = _dbContext.Neighborhoods
            .AsNoTracking()
            .Where(x =>
                x.Locality != null &&
                x.Locality.District != null &&
                x.Locality.District.Province != null &&
                x.Locality.District.Province.Name == province &&
                x.Locality.District.Name == district)
            .Select(x => x.Name)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        return Json(neighborhoods);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
