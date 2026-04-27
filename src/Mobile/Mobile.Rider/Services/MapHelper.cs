using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using NetTopologySuite.Geometries;
using MsuiColor = Mapsui.Styles.Color;
using MsuiMap = Mapsui.Map;
using SymbolStyle = Mapsui.Styles.SymbolStyle;
using SymbolType = Mapsui.Styles.SymbolType;
using Brush = Mapsui.Styles.Brush;
using Pen = Mapsui.Styles.Pen;
using PenStyle = Mapsui.Styles.PenStyle;
using VectorStyle = Mapsui.Styles.VectorStyle;

namespace Mobile.Rider.Services;

public static class MapHelper
{
    public static MsuiMap CreateOsmMap()
    {
        var map = new MsuiMap { CRS = "EPSG:3857" };
        map.Layers.Add(OpenStreetMap.CreateTileLayer("GetMeThere-Rider/1.0"));
        return map;
    }

    public static MemoryLayer CreateMarkerLayer(string name, MsuiColor color, double scale = 0.8)
    {
        var style = new SymbolStyle
        {
            SymbolType = SymbolType.Ellipse,
            SymbolScale = scale,
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

    public static MemoryLayer CreateRouteLayer(string name, MsuiColor color, double width = 4)
    {
        var style = new VectorStyle
        {
            Line = new Pen(color, width) { PenStyle = PenStyle.Solid },
            Fill = null
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

    public static void SetRoutePolyline(MemoryLayer layer, IReadOnlyList<(double Lat, double Lng)> points)
    {
        if (points.Count < 2)
        {
            layer.Features = Array.Empty<IFeature>();
            layer.DataHasChanged();
            return;
        }
        var coords = points
            .Select(p => { var (x, y) = SphericalMercator.FromLonLat(p.Lng, p.Lat); return new Coordinate(x, y); })
            .ToArray();
        var line = new LineString(coords);
        layer.Features = new IFeature[] { new GeometryFeature(line) };
        layer.DataHasChanged();
    }

    public static void CenterOn(MapControl control, double lat, double lng, double resolution = 4)
    {
        var (x, y) = SphericalMercator.FromLonLat(lng, lat);
        control.Map.Navigator.CenterOnAndZoomTo(new MPoint(x, y), resolution);
    }

    public static void FitToBounds(MapControl control, IReadOnlyList<(double Lat, double Lng)> points, double padding = 1.4)
    {
        if (points.Count == 0) return;
        double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;
        foreach (var p in points)
        {
            var (x, y) = SphericalMercator.FromLonLat(p.Lng, p.Lat);
            if (x < minX) minX = x;
            if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }
        var w = Math.Max(maxX - minX, 1);
        var h = Math.Max(maxY - minY, 1);
        var cx = (minX + maxX) / 2;
        var cy = (minY + maxY) / 2;
        var ctrlW = control.Width > 0 ? control.Width : 360;
        var r = Math.Max(w, h) / ctrlW * padding;
        if (double.IsNaN(r) || r <= 0) r = 4;
        control.Map.Navigator.CenterOnAndZoomTo(new MPoint(cx, cy), r);
    }
}
