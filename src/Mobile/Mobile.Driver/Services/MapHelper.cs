using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using MsuiColor = Mapsui.Styles.Color;
using MsuiMap = Mapsui.Map;
using SymbolStyle = Mapsui.Styles.SymbolStyle;
using SymbolType = Mapsui.Styles.SymbolType;
using Brush = Mapsui.Styles.Brush;
using Pen = Mapsui.Styles.Pen;

namespace Mobile.Driver.Services;

public static class MapHelper
{
    public static MsuiMap CreateOsmMap()
    {
        var map = new MsuiMap { CRS = "EPSG:3857" };
        map.Layers.Add(OpenStreetMap.CreateTileLayer("GetMeThere-Driver/1.0"));
        return map;
    }

    public static MemoryLayer CreateMarkerLayer(string name, MsuiColor color)
    {
        var style = new SymbolStyle
        {
            SymbolType = SymbolType.Ellipse,
            SymbolScale = 0.8,
            Fill = new Brush(color),
            Outline = new Pen(MsuiColor.White, 2)
        };
        return new MemoryLayer
        {
            Name = name,
            Style = style,
            Features = new List<IFeature>()
        };
    }

    public static void SetMarkerPosition(MemoryLayer layer, double lat, double lng)
    {
        var (x, y) = SphericalMercator.FromLonLat(lng, lat);
        layer.Features = new IFeature[] { new PointFeature(new MPoint(x, y)) };
        layer.DataHasChanged();
    }

    public static void CenterOn(MapControl control, double lat, double lng, double resolution = 4)
    {
        var (x, y) = SphericalMercator.FromLonLat(lng, lat);
        control.Map.Navigator.CenterOnAndZoomTo(new MPoint(x, y), resolution);
    }
}
