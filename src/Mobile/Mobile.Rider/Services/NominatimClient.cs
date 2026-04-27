using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Mobile.Shared.Configuration;

namespace Mobile.Rider.Services;

public sealed class NominatimClient
{
    private readonly HttpClient _http;
    private readonly AppConfig _cfg;

    private static readonly NominatimResult[] MockResults = new[]
    {
        new NominatimResult("Красная площадь, Москва", 55.7539, 37.6208),
        new NominatimResult("Тверская улица, 10, Москва", 55.7616, 37.6080),
        new NominatimResult("Парк Горького, Москва", 55.7298, 37.6019),
        new NominatimResult("Шереметьево (SVO), Москва", 55.9726, 37.4146),
    };

    public NominatimClient(AppConfig cfg)
    {
        _cfg = cfg;
        _http = new HttpClient
        {
            BaseAddress = new Uri("https://nominatim.openstreetmap.org/")
        };
        _http.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("GetMeThere-Rider", "1.0"));
        _http.DefaultRequestHeaders.AcceptLanguage.Add(
            new StringWithQualityHeaderValue("ru"));
    }

    public async Task<IReadOnlyList<NominatimResult>> SearchAsync(string query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query)) return Array.Empty<NominatimResult>();

        if (_cfg.UseMocks)
        {
            await Task.Delay(200, ct);
            var q = query.Trim().ToLowerInvariant();
            return MockResults
                .Where(r => r.DisplayName.ToLowerInvariant().Contains(q) || q.Length < 3)
                .ToList();
        }

        var url = $"search?q={Uri.EscapeDataString(query)}&format=json&limit=5";
        var raw = await _http.GetFromJsonAsync<NominatimRaw[]>(url, ct);
        return raw == null
            ? Array.Empty<NominatimResult>()
            : raw.Select(r => new NominatimResult(
                r.DisplayName ?? "",
                double.Parse(r.Lat ?? "0", System.Globalization.CultureInfo.InvariantCulture),
                double.Parse(r.Lon ?? "0", System.Globalization.CultureInfo.InvariantCulture))).ToList();
    }

    private sealed class NominatimRaw
    {
        [JsonPropertyName("display_name")] public string? DisplayName { get; set; }
        [JsonPropertyName("lat")] public string? Lat { get; set; }
        [JsonPropertyName("lon")] public string? Lon { get; set; }
    }
}

public record NominatimResult(string DisplayName, double Latitude, double Longitude);
