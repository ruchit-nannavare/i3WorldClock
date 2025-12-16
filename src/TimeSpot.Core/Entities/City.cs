namespace TimeSpot.Core.Entities;

public class City
{
    public string Id { get; set; } = string.Empty;           // "london-gb"
    public string Name { get; set; } = string.Empty;         // "London"
    public string Country { get; set; } = string.Empty;      // "United Kingdom"
    public string CountryCode { get; set; } = string.Empty;  // "GB"
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int UtcOffsetSeconds { get; set; }
    public string UtcOffsetDisplay { get; set; } = string.Empty; // "UTC+0"
}
