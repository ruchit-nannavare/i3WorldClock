using TimeSpot.Core.Enums;

namespace TimeSpot.UseCases.DTOs;

public record WeatherDto(
    double Temperature,
    WeatherCondition Condition,
    DayNightStatus DayNight,
    string Sunrise,
    string Sunset
);
