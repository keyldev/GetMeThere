using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Shared.Models;
using Mobile.Shared.Net;

namespace Mobile.Driver.ViewModels;

public partial class IncomingOrderViewModel : ObservableObject, IDisposable
{
    private readonly IApi _api;
    private CancellationTokenSource? _countdownCts;

    [ObservableProperty] private OrderOfferDto? _offer;
    [ObservableProperty] private double _progress;       // 0..1
    [ObservableProperty] private int _secondsLeft;
    [ObservableProperty] private string? _statusText;
    [ObservableProperty] private bool _isBusy;

    public IncomingOrderViewModel(IApi api)
    {
        _api = api;
    }

    public void LoadOffer(OrderOfferDto offer)
    {
        Offer = offer;
        SecondsLeft = offer.TimeoutSeconds;
        Progress = 1.0;
        StatusText = null;
        StartCountdown();
    }

    private void StartCountdown()
    {
        _countdownCts?.Cancel();
        _countdownCts = new CancellationTokenSource();
        _ = TickAsync(_countdownCts.Token);
    }

    private async Task TickAsync(CancellationToken ct)
    {
        var total = Math.Max(1, SecondsLeft);
        for (var i = total; i > 0 && !ct.IsCancellationRequested; i--)
        {
            SecondsLeft = i;
            Progress = (double)i / total;
            await Task.Delay(1000, ct).ContinueWith(_ => { }, TaskContinuationOptions.NotOnCanceled);
            if (ct.IsCancellationRequested) return;
        }
        if (!ct.IsCancellationRequested)
        {
            // Timeout — close modal
            await CloseAsync();
        }
    }

    [RelayCommand]
    private async Task AcceptAsync()
    {
        if (Offer == null) return;
        IsBusy = true;
        try
        {
            var resp = await _api.AcceptOrder(Offer.OrderId);
            if (resp.IsSuccessStatusCode)
            {
                _countdownCts?.Cancel();
                await Shell.Current.Navigation.PopModalAsync();
                await Shell.Current.GoToAsync("//en-route", new Dictionary<string, object>
                {
                    ["orderId"] = Offer.OrderId,
                    ["pickupLat"] = Offer.PickupLat,
                    ["pickupLng"] = Offer.PickupLng
                });
            }
            else if (resp.StatusCode == HttpStatusCode.Conflict)
            {
                StatusText = "Заказ уже занят";
                await Task.Delay(1200);
                await CloseAsync();
            }
            else
            {
                StatusText = $"Ошибка {(int)resp.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Сбой: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeclineAsync()
    {
        if (Offer == null) return;
        try { await _api.DeclineOrder(Offer.OrderId); } catch { }
        await CloseAsync();
    }

    private async Task CloseAsync()
    {
        _countdownCts?.Cancel();
        await Shell.Current.Navigation.PopModalAsync();
    }

    public void Dispose() => _countdownCts?.Cancel();
}
