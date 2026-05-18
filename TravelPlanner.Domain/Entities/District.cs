namespace TravelPlanner.Domain.Entities;

public sealed class District
{
    public int Id { get; set; }
    public int ProvinceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Province? Province { get; set; }
    public ICollection<Locality> Localities { get; set; } = new List<Locality>();
}
