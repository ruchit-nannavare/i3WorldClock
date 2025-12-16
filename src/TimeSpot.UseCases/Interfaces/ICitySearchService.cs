using TimeSpot.UseCases.DTOs;

namespace TimeSpot.UseCases.Interfaces;

public interface ICitySearchService
{
    Task<IEnumerable<CityDto>> SearchCitiesAsync(string query, CancellationToken cancellationToken = default);
}
