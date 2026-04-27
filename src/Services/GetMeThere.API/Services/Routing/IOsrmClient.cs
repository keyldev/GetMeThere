namespace GetMeThere.API.Services.Routing
{
    public record GeoPoint(double Latitude, double Longitude);

    public record RouteResult(double DistanceMeters, double DurationSeconds, string? Geometry);

    public interface IOsrmClient
    {
        Task<RouteResult?> GetRouteAsync(GeoPoint from, GeoPoint to, CancellationToken ct = default);
        Task<double[]?> GetDurationsAsync(GeoPoint origin, IReadOnlyList<GeoPoint> destinations, CancellationToken ct = default);
    }
}
