using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Mobile.Shared.Configuration;

namespace Mobile.Rider.Services;

public sealed class OsrmRouteClient
{
    private readonly HttpClient _http;
    private readonly AppConfig _cfg;

    public OsrmRouteClient(AppConfig cfg)
    {
        _cfg = cfg;
        _http = new HttpClient
        {
            BaseAddress = new Uri("https://router.project-osrm.org/")
        };
    }

    public async Task<RouteResult?> GetRouteAsync(double fromLat, double fromLng, double toLat, double toLng, CancellationToken ct = default)
    {
        if (_cfg.UseMocks)
        {
            await Task.Delay(150, ct);
            // Crude great-circle distance × 1.3 to fake real road distance.
            var meters = HaversineMeters(fromLat, fromLng, toLat, toLng) * 1.3;
            return new RouteResult(meters, meters / 11.0, ""); // ~40 km/h average
        }

        var inv = CultureInfo.InvariantCulture;
        var url =
            $"route/v1/driving/{fromLng.ToString(inv)},{fromLat.ToString(inv)};{toLng.ToString(inv)},{toLat.ToString(inv)}" +
            "?overview=full&geometries=polyline";
        var resp = await _http.GetFromJsonAsync<OsrmRoot>(url, ct);
        var route = resp?.Routes?.FirstOrDefault();
        if (route == null) return null;
        return new RouteResult(route.Distance, route.Duration, route.Geometry ?? "");
    }

    private static double HaversineMeters(double lat1, double lng1, double lat2, double lng2)
    {
        const double R = 6371000;
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLng = (lng2 - lng1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180)
              * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        return 2 * R * Math.Asin(Math.Sqrt(a));
    }

    private sealed class OsrmRoot
    {
        [JsonPropertyName("routes")] public List<OsrmRoute>? Routes { get; set; }
    }
    private sealed class OsrmRoute
    {
        [JsonPropertyName("distance")] public double Distance { get; set; }
        [JsonPropertyName("duration")] public double Duration { get; set; }
        [JsonPropertyName("geometry")] public string? Geometry { get; set; }
    }
}

public record RouteResult(double DistanceMeters, double DurationSeconds, string Polyline);
