using System.Net.Http.Json;
using CAMEATrafficLightMapper.Models;

namespace CAMEATrafficLightMapper;

public sealed class FeedClient
{
    private readonly HttpClient _http;
    private readonly ILogger<FeedClient> _log;

    public FeedClient(HttpClient http, ILogger<FeedClient> log)
    {
        _http = http;
        _http.Timeout = TimeSpan.FromSeconds(60);
        _log = log;
    }

    public void UpdateBaseAddress(string url)
    {
        var uri = new Uri(url.TrimEnd('/') + "/");
        if (_http.BaseAddress != uri)
            _http.BaseAddress = uri;
    }

    public async Task<int> SendBatchAsync(List<TrafficRecord> records, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync("feed/records", records, ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException(
                $"Feed endpoint returned {(int)response.StatusCode}: {body}");
        }

        return records.Count;
    }
}
