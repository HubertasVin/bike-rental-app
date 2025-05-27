using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/logging")]
public class LoggingController : ControllerBase
{
    readonly ILoggingToggleService _toggles;

    public LoggingController(ILoggingToggleService toggles) => _toggles = toggles;

    [HttpPost]
    public IActionResult Set([FromQuery] bool enabled)
    {
        _toggles.SetEnabled(enabled);
        return Ok(new { LoggingEnabled = _toggles.Enabled });
    }
}
