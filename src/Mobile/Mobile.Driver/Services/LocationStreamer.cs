namespace Mobile.Driver.Services;

// TODO: foreground service for Android background tracking — for MVP app must stay foregrounded.
public sealed class LocationStreamer
{
    private readonly DriverHubClient _hub;
    private CancellationTokenSource? _cts;

    public LocationStreamer(DriverHubClient hub)
    {
        _hub = hub;
    }

    public bool IsRunning => _cts is { IsCancellationRequested: false };

    public void Start()
    {
        if (IsRunning) return;
        _cts = new CancellationTokenSource();
        _ = LoopAsync(_cts.Token);
    }

    public void Stop()
    {
        _cts?.Cancel();
        _cts = null;
    }

    private async Task LoopAsync(CancellationToken ct)
    {
        var period = TimeSpan.FromSeconds(3);
        var timer = new PeriodicTimer(period);
        try
        {
            while (await timer.WaitForNextTickAsync(ct))
            {
                try
                {
                    var loc = await Geolocation.Default.GetLocationAsync(
                        new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(2)));
                    if (loc != null && _hub.IsConnected)
                        await _hub.SendLocationAsync(loc.Latitude, loc.Longitude);
                }
                catch
                {
                    // swallow per-tick errors — keep streaming
                }
            }
        }
        catch (OperationCanceledException) { /* expected */ }
        finally
        {
            timer.Dispose();
        }
    }
}
