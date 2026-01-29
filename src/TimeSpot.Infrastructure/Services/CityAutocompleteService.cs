using Microsoft.EntityFrameworkCore;
using TimeSpot.Infrastructure.Data;
using TimeSpot.UseCases.DTOs;
using TimeSpot.UseCases.Interfaces;

namespace TimeSpot.Infrastructure.Services;

public class CityAutocompleteService : ICitySearchService
{
    private readonly WorldTimeDbContext _context;
    private const int DefaultLimit = 10;

    public CityAutocompleteService(WorldTimeDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CityDto>> SearchCitiesAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            return Enumerable.Empty<CityDto>();
        }

        var normalizedQuery = query.Trim().ToLower();

        var cities = await _context.Cities
            .Where(c => EF.Functions.ILike(c.Name, $"{normalizedQuery}%") ||
                        EF.Functions.ILike(c.Name, $"% {normalizedQuery}%"))
            .OrderBy(c => EF.Functions.ILike(c.Name, $"{normalizedQuery}%") ? 0 : 1)
            .ThenBy(c => c.Name)
            .Take(DefaultLimit)
            .Select(c => new CityDto(
                (c.Name.ToLower().Replace(" ", "-") + "-" + c.Country.ToLower().Replace(" ", "-")),
                c.Name,
                c.Country,
                c.Country.Length >= 2 ? c.Country.Substring(0, 2).ToUpper() : c.Country.ToUpper(),
                0, // latitude - not in our data
                0, // longitude - not in our data
                (int)(c.UtcOffset * 3600),
                c.UtcOffset >= 0
                    ? "UTC+" + ((int)c.UtcOffset).ToString()
                    : "UTC" + ((int)c.UtcOffset).ToString()
            ))
            .ToListAsync(cancellationToken);

        return cities;
    }

    private static string FormatUtcOffset(double hours)
    {
        var sign = hours >= 0 ? "+" : "";
        if (hours == Math.Floor(hours))
        {
            return $"UTC{sign}{(int)hours}";
        }
        var wholeHours = (int)hours;
        var minutes = (int)((Math.Abs(hours) - Math.Abs(wholeHours)) * 60);
        return $"UTC{sign}{wholeHours}:{minutes:D2}";
    }
}
