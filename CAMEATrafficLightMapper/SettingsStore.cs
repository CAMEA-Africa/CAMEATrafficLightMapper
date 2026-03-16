using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;

namespace CAMEATrafficLightMapper;

public sealed class LightMapperSettings
{
    public MySqlSettings MySql { get; set; } = new();
    public FeedSettings Feed { get; set; } = new();
}

public sealed class MySqlSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 3306;
    public string Database { get; set; } = "unicam_sensors";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";

    public string ToConnectionString() =>
        $"Server={Host};Port={Port};Database={Database};User ID={Username};Password={Password};SslMode=None;Allow User Variables=true;";
}

public sealed class FeedSettings
{
    public string TargetUrl { get; set; } = "https://camea.app/demo/api";
    public int BatchSize { get; set; } = 5000;
    public int PollIntervalSeconds { get; set; } = 10;
}

public sealed class SettingsStore
{
    private readonly IDataProtector _protector;
    private readonly string _filePath;
    private readonly ILogger<SettingsStore> _log;
    private readonly IConfiguration _config;

    public SettingsStore(
        IDataProtectionProvider dataProtectionProvider,
        IHostEnvironment env,
        IConfiguration config,
        ILogger<SettingsStore> log)
    {
        _protector = dataProtectionProvider.CreateProtector("CAMEATrafficLightMapper.Credentials");
        _filePath = Path.Combine(AppContext.BaseDirectory, "credentials.dat");
        _config = config;
        _log = log;
    }

    public LightMapperSettings? Load()
    {
        if (!File.Exists(_filePath))
            return null;

        try
        {
            var ciphertext = File.ReadAllText(_filePath);
            var json = _protector.Unprotect(ciphertext);
            return JsonSerializer.Deserialize<LightMapperSettings>(json);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to load credentials from {Path}", _filePath);
            return null;
        }
    }

    public void Save(LightMapperSettings settings)
    {
        var json = JsonSerializer.Serialize(settings);
        var ciphertext = _protector.Protect(json);
        File.WriteAllText(_filePath, ciphertext);
    }

    /// <summary>
    /// Load saved settings, or fall back to appsettings.json defaults.
    /// </summary>
    public LightMapperSettings LoadOrDefault()
    {
        var saved = Load();
        if (saved is not null)
            return saved;

        // Fall back to appsettings.json
        var s = new LightMapperSettings();

        var connStr = _config.GetConnectionString("MySql");
        if (!string.IsNullOrEmpty(connStr))
        {
            // Parse connection string into fields
            var parts = connStr.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var kv = part.Split('=', 2);
                if (kv.Length != 2) continue;
                var key = kv[0].Trim();
                var val = kv[1].Trim();
                switch (key.ToLowerInvariant())
                {
                    case "server": s.MySql.Host = val; break;
                    case "port": if (int.TryParse(val, out var p)) s.MySql.Port = p; break;
                    case "database": s.MySql.Database = val; break;
                    case "user id": s.MySql.Username = val; break;
                    case "password": s.MySql.Password = val; break;
                }
            }
        }

        s.Feed.TargetUrl = _config["Feed:TargetUrl"] ?? s.Feed.TargetUrl;
        s.Feed.BatchSize = _config.GetValue("Feed:BatchSize", s.Feed.BatchSize);
        s.Feed.PollIntervalSeconds = _config.GetValue("Feed:PollIntervalSeconds", s.Feed.PollIntervalSeconds);

        return s;
    }
}
