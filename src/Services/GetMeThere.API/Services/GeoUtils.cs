namespace GetMeThere.API.Services
{
    public static class GeoUtils
    {
        private const double EarthRadiusKm = 6371.0088;

        public static double HaversineKm(double lat1, double lng1, double lat2, double lng2)
        {
            var dLat = ToRad(lat2 - lat1);
            var dLng = ToRad(lng2 - lng1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                  + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
                  * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        private static double ToRad(double degrees) => degrees * Math.PI / 180.0;
    }
}
