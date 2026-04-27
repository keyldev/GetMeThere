using System.ComponentModel;
using Mapsui.Layers;
using Mobile.Rider.Services;
using Mobile.Rider.ViewModels;

namespace Mobile.Rider.Pages;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _vm;
    private MemoryLayer? _userLayer;

    public MapPage(MapViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;

        var map = MapHelper.CreateOsmMap();
        _userLayer = MapHelper.CreateMarkerLayer("user", Mapsui.Styles.Color.FromString("#1976D2"));
        map.Layers.Add(_userLayer);
        MapView.Map = map;

        vm.PropertyChanged += OnVmPropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCurrentLocationAsync();
    }

    private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MapViewModel.CurrentLatitude) or nameof(MapViewModel.CurrentLongitude))
        {
            if (_vm.CurrentLatitude is double lat && _vm.CurrentLongitude is double lng && _userLayer != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MapHelper.SetMarkerPosition(_userLayer, lat, lng);
                    MapHelper.CenterOn(MapView, lat, lng);
                });
            }
        }
    }
}
