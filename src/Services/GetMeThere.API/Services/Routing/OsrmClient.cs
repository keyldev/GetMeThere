using System.Globalization;
using System.Text.Json;

namespace GetMeThere.API.Services.Routing
{
    public class OsrmClient : IOsrmClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<OsrmClient> _logger;

        public OsrmClient(HttpClient http, ILogger<OsrmClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<RouteResult?> GetRouteAsync(GeoPoint from, GeoPoint to, CancellationToken ct = default)
        {
            // OSRM expects coords as lon,lat
            var url = $"route/v1/driving/{Fmt(from.Longitude)},{Fmt(from.Latitude)};{Fmt(to.Longitude)},{Fmt(to.Latitude)}?overview=full&geometries=polyline";
            try
            {
                using var resp = await _http.GetAsync(url, ct);
                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogWarning("OSRM /route non-2xx: {Status}", resp.StatusCode);
                    return null;
                }
                using var stream = await resp.Content.ReadAsStreamAsync(ct);
                using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
                var routes = doc.RootElement.GetProperty("routes");
                if (routes.GetArrayLength() == 0) return null;
                var route = routes[0];
                var distance = route.GetProperty("distance").GetDouble();
                var duration = route.GetProperty("duration").GetDouble();
                var geometry = route.TryGetProperty("geometry", out var g) ? g.GetString() : null;
                return new RouteResult(distance, duration, geometry);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "OSRM route call failed");
                return null;
            }
        }

        public async Task<double[]?> GetDurationsAsync(GeoPoint origin, IReadOnlyList<GeoPoint> destinations, CancellationToken ct = default)
        {
            if (destinations.Count == 0) return Array.Empty<double>();

            // /table/v1/driving/{coordinates}?sources=0&annotations=duration
            var coords = string.Join(";",
                new[] { origin }.Concat(destinations)
                    .Select(p => $"{Fmt(p.Longitude)},{Fmt(p.Latitude)}"));
            var url = $"table/v1/driving/{coords}?sources=0&annotations=duration";
            try
            {
                using var resp = await _http.GetAsync(url, ct);
                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogWarning("OSRM /table non-2xx: {Status}", resp.StatusCode);
                    return null;
                }
                using var stream = await resp.Content.ReadAsStreamAsync(ct);
                using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
                var durations = doc.RootElement.GetProperty("durations")[0];
                // durations[0][0] is origin→origin (0). Skip it.
                var len = durations.GetArrayLength() - 1;
                var result = new double[len];
                for (var i = 0; i < len; i++)
                {
                    var v = durations[i + 1];
                    result[i] = v.ValueKind == JsonValueKind.Null ? double.PositiveInfinity : v.GetDouble();
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "OSRM table call failed");
                return null;
            }
        }

        private static string Fmt(double d) => d.ToString("0.######", CultureInfo.InvariantCulture);
    }
}
