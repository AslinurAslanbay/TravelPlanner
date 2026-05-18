namespace TravelPlanner.Domain.Entities;

public sealed class Place
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string EstimatedVisitDuration { get; set; } = string.Empty;
    public int EstimatedVisitMinutes { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsOnRouteCandidate { get; set; }
    public City? City { get; set; }
}
