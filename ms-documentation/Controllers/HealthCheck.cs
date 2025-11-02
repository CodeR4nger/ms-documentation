using Microsoft.AspNetCore.Mvc;

namespace ms_documentation.Controllers;

[ApiController]
[Route("/")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("API OK");
}