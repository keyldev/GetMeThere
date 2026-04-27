using System.ComponentModel;
using Mapsui.Layers;
using Mobile.Driver.Services;
using Mobile.Driver.ViewModels;

namespace Mobile.Driver.Pages;

public partial class OnlinePage : ContentPage
{
    private readonly OnlineViewModel _vm;
    private MemoryLayer? _driverLayer;

    public OnlinePage(OnlineViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;

        var map = MapHelper.CreateOsmMap();
        _driverLayer = MapHelper.CreateMarkerLayer("driver", Mapsui.Styles.Color.FromString("#1976D2"));
        map.Layers.Add(_driverLayer);
        MapView.Map = map;

        vm.PropertyChanged += OnVmPropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.RefreshLocationAsync();
    }

    private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(OnlineViewModel.CurrentLatitude) or nameof(OnlineViewModel.CurrentLongitude))
        {
            if (_vm.CurrentLatitude is double lat && _vm.CurrentLongitude is double lng && _driverLayer != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MapHelper.SetMarkerPosition(_driverLayer, lat, lng);
                    MapHelper.CenterOn(MapView, lat, lng);
                });
            }
        }
    }
}
