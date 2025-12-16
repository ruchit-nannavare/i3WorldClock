using System.Net.Http.Json;
using System.Text.Json.Serialization;
using TimeSpot.UseCases.DTOs;
using TimeSpot.UseCases.Interfaces;

namespace TimeSpot.Infrastructure.ExternalServices;

public class GeocodingService : ICitySearchService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://geocoding-api.open-meteo.com/v1/search";

    public GeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<CityDto>> SearchCitiesAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            return Enumerable.Empty<CityDto>();
        }

        var url = $"{BaseUrl}?name={Uri.EscapeDataString(query)}&count=10&language=en&format=json";

        var response = await _httpClient.GetFromJsonAsync<GeocodingResponse>(url, cancellationToken);

        if (response?.Results == null)
        {
            return Enumerable.Empty<CityDto>();
        }

        return response.Results.Select(r => new CityDto(
            Id: GenerateCityId(r.Name, r.CountryCode),
            Name: r.Name,
            Country: r.Country ?? "",
            CountryCode: r.CountryCode ?? "",
            Latitude: r.Latitude,
            Longitude: r.Longitude,
            UtcOffsetSeconds: r.Timezone != null ? GetUtcOffsetSeconds(r.Timezone) : 0,
            UtcOffsetDisplay: FormatUtcOffset(r.Timezone != null ? GetUtcOffsetSeconds(r.Timezone) : 0)
        ));
    }

    private static string GenerateCityId(string name, string? countryCode)
    {
        var normalized = name.ToLowerInvariant().Replace(" ", "-");
        var code = (countryCode ?? "xx").ToLowerInvariant();
        return $"{normalized}-{code}";
    }

    private static int GetUtcOffsetSeconds(string timezone)
    {
        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            return (int)tz.BaseUtcOffset.TotalSeconds;
        }
        catch
        {
            // Fallback: try to get offset from current time
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
                return (int)tz.GetUtcOffset(DateTime.UtcNow).TotalSeconds;
            }
            catch
            {
                return 0;
            }
        }
    }

    private static string FormatUtcOffset(int offsetSeconds)
    {
        var hours = offsetSeconds / 3600;
        var minutes = Math.Abs((offsetSeconds % 3600) / 60);

        if (minutes == 0)
        {
            return hours >= 0 ? $"UTC+{hours}" : $"UTC{hours}";
        }

        var sign = hours >= 0 ? "+" : "-";
        return $"UTC{sign}{Math.Abs(hours)}:{minutes:D2}";
    }
}

// Geocoding API response models
public class GeocodingResponse
{
    [JsonPropertyName("results")]
    public GeocodingResult[]? Results { get; set; }
}

public class GeocodingResult
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }
}
