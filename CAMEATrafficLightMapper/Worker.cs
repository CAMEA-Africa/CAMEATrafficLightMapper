using CAMEATrafficLightMapper.Models;
using CAMEATrafficLightMapper.MySql;

namespace CAMEATrafficLightMapper;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _log;
    private readonly SettingsStore _settings;
    private readonly FeedClient _feed;
    private readonly SyncStateStore _state;

    public Worker(ILogger<Worker> log, SettingsStore settings, FeedClient feed, SyncStateStore state)
    {
        _log = log;
        _settings = settings;
        _feed = feed;
        _state = state;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // Wait for settings to be configured
        LightMapperSettings config;
        while (!ct.IsCancellationRequested)
        {
            config = _settings.LoadOrDefault();
            if (!string.IsNullOrEmpty(config.MySql.Host) && !string.IsNullOrEmpty(config.MySql.Database))
                break;

            _log.LogWarning("No MySQL settings configured — open the web UI to configure");
            await Task.Delay(TimeSpan.FromSeconds(10), ct);
        }

        while (!ct.IsCancellationRequested)
        {
            config = _settings.LoadOrDefault();
            var connStr = config.MySql.ToConnectionString();
            var batchSize = config.Feed.BatchSize;
            var pollSeconds = config.Feed.PollIntervalSeconds;

            _feed.UpdateBaseAddress(config.Feed.TargetUrl);

            var reader = new WimMysqlReader(connStr);

            _log.LogInformation("LightMapper started — polling every {Poll}s, batch size {Batch}, target {Url}",
                pollSeconds, batchSize, config.Feed.TargetUrl);

            // Wait for MySQL to be reachable
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await reader.PingAsync(ct);
                    _log.LogInformation("MySQL connection OK");
                    break;
                }
                catch (Exception ex)
                {
                    _log.LogWarning("Waiting for MySQL: {Msg}", ex.Message);
                    _state.SetError($"MySQL: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(5), ct);
                }
            }

            // Poll loop — runs until settings change or error forces reconnect
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await PollOnce(reader, batchSize, ct);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Poll error — retrying in {Poll}s", pollSeconds);
                    _state.SetError(ex.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(pollSeconds), ct);
            }
        }
    }

    private async Task PollOnce(WimMysqlReader reader, int batchSize, CancellationToken ct)
    {
        var state = _state.Load();
        var fromId = (long)state.LastProcessedMysqlId;

        var batch = new List<TrafficRecord>(batchSize);
        ulong lastId = state.LastProcessedMysqlId;

        await foreach (var mysqlRec in reader.ReadAsync(fromId, batchSize, ct))
        {
            var record = CAMEAWimMysqlRecordToTrafficRecordMapper.ToTrafficRecord(mysqlRec);
            batch.Add(record);
            lastId = mysqlRec.Id;
        }

        if (batch.Count == 0)
            return;

        _log.LogInformation("Read {Count} records from MySQL (last ID: {LastId})", batch.Count, lastId);

        var inserted = await _feed.SendBatchAsync(batch, ct);

        _state.Update(lastId, inserted);
        _log.LogInformation("Sent {Count} records to feed endpoint (total: {Total})",
            inserted, _state.Load().TotalSent);
    }
}
