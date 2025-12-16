using System.Net.Http.Json;
using System.Text.Json.Serialization;
using TimeSpot.Core.Enums;
using TimeSpot.UseCases.DTOs;
using TimeSpot.UseCases.Interfaces;

namespace TimeSpot.Infrastructure.ExternalServices;

public class OpenMeteoService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.open-meteo.com/v1/forecast";

    public OpenMeteoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherDto> GetWeatherAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}?latitude={latitude}&longitude={longitude}&current=temperature_2m,weather_code,is_day&daily=sunrise,sunset&timezone=auto";

        var response = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(url, cancellationToken);

        if (response?.Current == null || response.Daily == null)
        {
            throw new InvalidOperationException("Invalid response from Open-Meteo API");
        }

        var condition = MapWeatherCode(response.Current.WeatherCode);
        var dayNight = response.Current.IsDay == 1 ? DayNightStatus.Day : DayNightStatus.Night;
        var sunrise = response.Daily.Sunrise?.FirstOrDefault() ?? "";
        var sunset = response.Daily.Sunset?.FirstOrDefault() ?? "";

        // Extract just the time portion from the datetime string
        sunrise = ExtractTime(sunrise);
        sunset = ExtractTime(sunset);

        return new WeatherDto(
            response.Current.Temperature2m,
            condition,
            dayNight,
            sunrise,
            sunset
        );
    }

    private static string ExtractTime(string dateTimeString)
    {
        if (string.IsNullOrEmpty(dateTimeString)) return "";

        // Format is "2024-01-15T07:12" - extract time after 'T'
        var tIndex = dateTimeString.IndexOf('T');
        return tIndex >= 0 ? dateTimeString[(tIndex + 1)..] : dateTimeString;
    }

    private static WeatherCondition MapWeatherCode(int code)
    {
        return code switch
        {
            0 => WeatherCondition.Clear,
            1 or 2 => WeatherCondition.PartlyCloudy,
            3 => WeatherCondition.Cloudy,
            45 or 48 => WeatherCondition.Fog,
            51 or 53 or 55 or 56 or 57 or 61 or 63 or 65 or 66 or 67 or 80 or 81 or 82 => WeatherCondition.Rain,
            71 or 73 or 75 or 77 or 85 or 86 => WeatherCondition.Snow,
            95 or 96 or 99 => WeatherCondition.Thunderstorm,
            _ => WeatherCondition.Clear
        };
    }
}

// Open-Meteo API response models
public class OpenMeteoResponse
{
    [JsonPropertyName("current")]
    public CurrentWeather? Current { get; set; }

    [JsonPropertyName("daily")]
    public DailyWeather? Daily { get; set; }
}

public class CurrentWeather
{
    [JsonPropertyName("temperature_2m")]
    public double Temperature2m { get; set; }

    [JsonPropertyName("weather_code")]
    public int WeatherCode { get; set; }

    [JsonPropertyName("is_day")]
    public int IsDay { get; set; }
}

public class DailyWeather
{
    [JsonPropertyName("sunrise")]
    public string[]? Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    public string[]? Sunset { get; set; }
}
