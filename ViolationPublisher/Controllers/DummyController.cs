using Microsoft.AspNetCore.Mvc;

namespace ViolationPublisher.Controllers;

[ApiController]
[Route("[controller]")]
public class DummyController : ControllerBase
{
    private readonly ILogger<DummyController> _logger;

    public DummyController(ILogger<DummyController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "Dummy")]
    public IActionResult Get()
    {
        _logger.LogInformation("Called dummy controller");
        return Ok();
    }
}