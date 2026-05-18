namespace TravelPlanner.Domain.Entities;

public sealed class City
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string HeroTitle { get; set; } = string.Empty;
    public string HeroDescription { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public ICollection<Place> Places { get; set; } = new List<Place>();
}
