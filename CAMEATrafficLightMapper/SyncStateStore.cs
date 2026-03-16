using System.Text.Json;

namespace CAMEATrafficLightMapper;

public sealed class SyncState
{
    public ulong LastProcessedMysqlId { get; set; }
    public long TotalSent { get; set; }
    public DateTime? LastSyncUtc { get; set; }
    public string? LastError { get; set; }
}

public sealed class SyncStateStore
{
    private static readonly string DefaultPath =
        Path.Combine(AppContext.BaseDirectory, "sync-state.json");

    private readonly string _path;
    private readonly object _lock = new();
    private SyncState _state;

    public SyncStateStore(IConfiguration config)
    {
        _path = config["Feed:StateFile"] ?? DefaultPath;
        _state = LoadFromDisk();
    }

    public SyncState Load()
    {
        lock (_lock) return _state;
    }

    public void Update(ulong lastMysqlId, int sentCount)
    {
        lock (_lock)
        {
            _state.LastProcessedMysqlId = lastMysqlId;
            _state.TotalSent += sentCount;
            _state.LastSyncUtc = DateTime.UtcNow;
            _state.LastError = null;
            SaveToDisk();
        }
    }

    public void SetError(string error)
    {
        lock (_lock)
        {
            _state.LastError = error;
            _state.LastSyncUtc = DateTime.UtcNow;
            SaveToDisk();
        }
    }

    private SyncState LoadFromDisk()
    {
        try
        {
            if (File.Exists(_path))
            {
                var json = File.ReadAllText(_path);
                return JsonSerializer.Deserialize<SyncState>(json) ?? new SyncState();
            }
        }
        catch { /* start fresh */ }
        return new SyncState();
    }

    private void SaveToDisk()
    {
        try
        {
            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(_state, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_path, json);
        }
        catch { /* best effort */ }
    }
}
