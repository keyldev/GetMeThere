using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Rider.Services;

namespace Mobile.Rider.ViewModels;

public partial class DestinationSearchViewModel : ObservableObject, IQueryAttributable
{
    private readonly NominatimClient _nominatim;
    private CancellationTokenSource? _cts;

    [ObservableProperty] private double _fromLat;
    [ObservableProperty] private double _fromLng;
    [ObservableProperty] private string _query = "";
    [ObservableProperty] private bool _isBusy;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("fromLat", out var v) && TryGetDouble(v, out var d)) FromLat = d;
        if (query.TryGetValue("fromLng", out v) && TryGetDouble(v, out d)) FromLng = d;
    }

    internal static bool TryGetDouble(object? v, out double d)
    {
        if (v is double dv) { d = dv; return true; }
        return double.TryParse(v?.ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d);
    }


    public ObservableCollection<NominatimResult> Results { get; } = new();

    public DestinationSearchViewModel(NominatimClient nominatim)
    {
        _nominatim = nominatim;
    }

    partial void OnQueryChanged(string value) => _ = SearchDebouncedAsync(value);

    private async Task SearchDebouncedAsync(string text)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;
        try
        {
            await Task.Delay(500, ct);
            if (string.IsNullOrWhiteSpace(text)) { Results.Clear(); return; }
            IsBusy = true;
            var hits = await _nominatim.SearchAsync(text, ct);
            if (ct.IsCancellationRequested) return;
            Results.Clear();
            foreach (var h in hits) Results.Add(h);
        }
        catch (TaskCanceledException) { /* expected */ }
        catch (Exception) { /* swallow for UX */ }
        finally
        {
            if (!ct.IsCancellationRequested) IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task PickAsync(NominatimResult result)
    {
        await Shell.Current.GoToAsync("confirm", new Dictionary<string, object>
        {
            ["fromLat"] = FromLat,
            ["fromLng"] = FromLng,
            ["toLat"] = result.Latitude,
            ["toLng"] = result.Longitude,
            ["toAddress"] = result.DisplayName
        });
    }
}
