namespace TravelPlanner.Domain.Entities;

public sealed class Neighborhood
{
    public int Id { get; set; }
    public int LocalityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Locality? Locality { get; set; }
}
