namespace Mobile.Shared.Configuration;

public sealed class AppConfig
{
    public string BaseUrl { get; init; } = "http://10.0.2.2:5000";
    public string HubUrl => $"{BaseUrl.TrimEnd('/')}/taxiHub";

    /// <summary>When true, IApi/Nominatim/OSRM/Hub are replaced with in-process mocks for UI demo without backend.</summary>
    public bool UseMocks { get; init; } = false;
}
