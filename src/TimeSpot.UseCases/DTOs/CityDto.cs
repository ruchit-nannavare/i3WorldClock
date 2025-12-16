namespace TimeSpot.UseCases.DTOs;

public record CityDto(
    string Id,
    string Name,
    string Country,
    string CountryCode,
    double Latitude,
    double Longitude,
    int UtcOffsetSeconds,
    string UtcOffsetDisplay
);
