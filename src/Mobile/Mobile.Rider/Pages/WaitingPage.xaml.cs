using System.ComponentModel;
using Mapsui.Layers;
using Mobile.Rider.Services;
using Mobile.Rider.ViewModels;
using MsuiColor = Mapsui.Styles.Color;

namespace Mobile.Rider.Pages;

public partial class WaitingPage : ContentPage
{
    private readonly WaitingViewModel _vm;
    private MemoryLayer? _routeLayer;
    private MemoryLayer? _pickupLayer;
    private MemoryLayer? _destLayer;
    private MemoryLayer? _driverLayer;

    public WaitingPage(WaitingViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;

        var map = MapHelper.CreateOsmMap();
        _routeLayer = MapHelper.CreateRouteLayer("route", MsuiColor.FromString("#1565C0"), 5);
        _pickupLayer = MapHelper.CreateMarkerLayer("pickup", MsuiColor.FromString("#2E7D32"), 1.0);
        _destLayer = MapHelper.CreateMarkerLayer("dest", MsuiColor.FromString("#E53935"), 1.0);
        _driverLayer = MapHelper.CreateMarkerLayer("driver", MsuiColor.FromString("#FFB300"), 1.1);
        map.Layers.Add(_routeLayer);
        map.Layers.Add(_pickupLayer);
        map.Layers.Add(_destLayer);
        map.Layers.Add(_driverLayer);
        MapView.Map = map;

        vm.PropertyChanged += OnVmPropertyChanged;
        vm.RouteReady += OnRouteReady;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try { await _vm.ConnectHubAsync(); } catch { }

        if (_vm.PickupLat != 0 && _pickupLayer != null)
            MapHelper.SetMarkerPosition(_pickupLayer, _vm.PickupLat, _vm.PickupLng);
        if (_vm.DestLat != 0 && _destLayer != null)
            MapHelper.SetMarkerPosition(_destLayer, _vm.DestLat, _vm.DestLng);
        if (_vm.PickupLat != 0)
            MapHelper.CenterOn(MapView, _vm.PickupLat, _vm.PickupLng, 6);

        await _vm.LoadRouteAsync();
    }

    private void OnRouteReady()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (_vm.RoutePoints != null && _routeLayer != null)
            {
                MapHelper.SetRoutePolyline(_routeLayer, _vm.RoutePoints);
                MapHelper.FitToBounds(MapView, _vm.RoutePoints);
            }
        });
    }

    private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(WaitingViewModel.DriverLatitude) or nameof(WaitingViewModel.DriverLongitude))
        {
            if (_vm.DriverLatitude is double lat && _vm.DriverLongitude is double lng && _driverLayer != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MapHelper.SetMarkerPosition(_driverLayer, lat, lng);
                });
            }
        }
    }
}
