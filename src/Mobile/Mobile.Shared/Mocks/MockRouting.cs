using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace Mobile.Shared.Mocks;

/// <summary>
/// Calls public OSRM demo to get a real driving route, decodes the polyline, and
/// downsamples to a fixed number of waypoints. Falls back to linear interpolation
/// if the network call fails.
/// </summary>
public static class MockRouting
{
    private static readonly HttpClient Http = new() { BaseAddress = new Uri("https://router.project-osrm.org/") };

    public static async Task<List<(double Lat, double Lng)>> GetRoutePointsAsync(
        double fromLat, double fromLng, double toLat, double toLng, int desiredCount = 12)
    {
        try
        {
            var inv = CultureInfo.InvariantCulture;
            var url =
                $"route/v1/driving/{fromLng.ToString(inv)},{fromLat.ToString(inv)};" +
                $"{toLng.ToString(inv)},{toLat.ToString(inv)}?overview=full&geometries=polyline";
            using var resp = await Http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return Linear(fromLat, fromLng, toLat, toLng, desiredCount);

            using var stream = await resp.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            if (!doc.RootElement.TryGetProperty("routes", out var routes) || routes.GetArrayLength() == 0)
                return Linear(fromLat, fromLng, toLat, toLng, desiredCount);

            var polyline = routes[0].GetProperty("geometry").GetString();
            if (string.IsNullOrEmpty(polyline)) return Linear(fromLat, fromLng, toLat, toLng, desiredCount);

            var pts = DecodePolyline(polyline);
            return pts.Count <= desiredCount ? pts : Downsample(pts, desiredCount);
        }
        catch
        {
            return Linear(fromLat, fromLng, toLat, toLng, desiredCount);
        }
    }

    private static List<(double Lat, double Lng)> Linear(double fl, double fln, double tl, double tln, int n)
    {
        var result = new List<(double, double)>(n);
        for (int i = 1; i <= n; i++)
        {
            var t = (double)i / n;
            result.Add((fl + (tl - fl) * t, fln + (tln - fln) * t));
        }
        return result;
    }

    private static List<(double Lat, double Lng)> Downsample(List<(double Lat, double Lng)> pts, int n)
    {
        var result = new List<(double, double)>(n);
        for (int i = 0; i < n; i++)
        {
            var idx = (int)Math.Round(i * (pts.Count - 1.0) / (n - 1));
            result.Add(pts[idx]);
        }
        return result;
    }

    /// <summary>
    /// Decodes Google's encoded polyline format (precision 5) used by OSRM by default.
    /// </summary>
    public static List<(double Lat, double Lng)> DecodePolyline(string polyline)
    {
        var coords = new List<(double, double)>();
        int index = 0, lat = 0, lng = 0;
        while (index < polyline.Length)
        {
            int b, shift = 0, result = 0;
            do
            {
                b = polyline[index++] - 63;
                result |= (b & 0x1f) << shift;
                shift += 5;
            } while (b >= 0x20 && index < polyline.Length);
            int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
            lat += dlat;

            shift = 0; result = 0;
            do
            {
                b = polyline[index++] - 63;
                result |= (b & 0x1f) << shift;
                shift += 5;
            } while (b >= 0x20 && index < polyline.Length);
            int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
            lng += dlng;

            coords.Add((lat / 1E5, lng / 1E5));
        }
        return coords;
    }
}
