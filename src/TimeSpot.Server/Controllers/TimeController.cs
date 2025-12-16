using Microsoft.AspNetCore.Mvc;
using TimeSpot.UseCases.Services;

namespace TimeSpot.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeController : ControllerBase
{
    private readonly TimeService _timeService;

    public TimeController(TimeService timeService)
    {
        _timeService = timeService;
    }

    [HttpGet("utc")]
    public IActionResult GetUtcTime()
    {
        var utcTime = _timeService.GetUtcTime();
        return Ok(utcTime);
    }
}
