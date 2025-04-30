using Microsoft.AspNetCore.Mvc;

namespace BikeRendalApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok("Hello World!");
}
