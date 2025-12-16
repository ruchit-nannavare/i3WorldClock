using Microsoft.AspNetCore.Mvc;
using TimeSpot.UseCases.Interfaces;

namespace TimeSpot.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly ICitySearchService _citySearchService;


    public CitiesController(ICitySearchService citySearchService)
    {
        _citySearchService = citySearchService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Query parameter is required");
        }

        var cities = await _citySearchService.SearchCitiesAsync(query, cancellationToken);
        return Ok(cities);
    }
}
