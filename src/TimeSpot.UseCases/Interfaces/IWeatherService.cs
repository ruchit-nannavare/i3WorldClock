using TimeSpot.UseCases.DTOs;

namespace TimeSpot.UseCases.Interfaces;

public interface IWeatherService
{
    Task<WeatherDto> GetWeatherAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
}
