using CAMEATrafficLightMapper.MySql;
using Microsoft.AspNetCore.Mvc;

namespace CAMEATrafficLightMapper.Controllers;

[ApiController]
[Route("api/settings")]
public sealed class SettingsController : ControllerBase
{
    private readonly SettingsStore _store;

    public SettingsController(SettingsStore store) => _store = store;

    [HttpGet]
    public IActionResult Get()
    {
        var s = _store.LoadOrDefault();

        return Ok(new
        {
            mySql = new
            {
                host = s.MySql.Host,
                port = s.MySql.Port,
                database = s.MySql.Database,
                username = s.MySql.Username,
                hasPassword = !string.IsNullOrEmpty(s.MySql.Password)
            },
            feed = new
            {
                targetUrl = s.Feed.TargetUrl,
                batchSize = s.Feed.BatchSize,
                pollIntervalSeconds = s.Feed.PollIntervalSeconds
            }
        });
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] LightMapperSettings settings, CancellationToken ct)
    {
        if (settings is null)
            return BadRequest("Missing body");

        // If password is blank, keep existing
        var existing = _store.LoadOrDefault();
        if (string.IsNullOrEmpty(settings.MySql.Password))
            settings.MySql.Password = existing.MySql.Password;

        // Test MySQL connection before saving
        try
        {
            var reader = new WimMysqlReader(settings.MySql.ToConnectionString());
            await reader.PingAsync(ct);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"MySQL connection failed: {ex.Message}" });
        }

        _store.Save(settings);

        return Ok(new { saved = true });
    }
}
