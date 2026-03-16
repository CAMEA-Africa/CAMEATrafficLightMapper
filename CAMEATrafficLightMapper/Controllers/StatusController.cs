using Microsoft.AspNetCore.Mvc;

namespace CAMEATrafficLightMapper.Controllers;

[ApiController]
[Route("api/status")]
public sealed class StatusController : ControllerBase
{
    private readonly SyncStateStore _state;

    public StatusController(SyncStateStore state) => _state = state;

    [HttpGet]
    public IActionResult Get() => Ok(_state.Load());
}
