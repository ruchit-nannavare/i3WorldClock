using Microsoft.AspNetCore.Mvc;
using TimeSpot.UseCases.Interfaces;

namespace TimeSpot.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWeather([FromQuery] double lat, [FromQuery] double lon, CancellationToken cancellationToken)
    {
        var weather = await _weatherService.GetWeatherAsync(lat, lon, cancellationToken);
        return Ok(weather);
    }
}
