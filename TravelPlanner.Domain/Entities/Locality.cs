namespace TravelPlanner.Domain.Entities;

public sealed class Locality
{
    public int Id { get; set; }
    public int DistrictId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public District? District { get; set; }
    public ICollection<Neighborhood> Neighborhoods { get; set; } = new List<Neighborhood>();
}
