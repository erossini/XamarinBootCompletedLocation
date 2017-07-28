using System;
using LocationTest.Droid.BackgroudServices.Models;

namespace LocationTest.Droid
{
    public static class Utils
    {
        public enum DistanceUnit { Miles, Kilometers };

        /// <summary>
        /// Tos the radian.
        /// </summary>
        /// <returns>The radian.</returns>
        /// <param name="value">Value.</param>
        public static double ToRadian(this double value)
        {
            return (Math.PI / 180) * value;
        }

        /// <summary>
        /// Haversines the distance.
        /// </summary>
        /// <returns>The distance.</returns>
        /// <param name="coord1">Coord1.</param>
        /// <param name="coord2">Coord2.</param>
        /// <param name="unit">Unit.</param>
        public static double HaversineDistance(Coords coord1, Coords coord2, DistanceUnit unit)
        {
            double R = (unit == DistanceUnit.Miles) ? 3960 : 6371;
            var lat = (coord2.Latitude - coord1.Latitude).ToRadian();
            var lng = (coord2.Longitude - coord1.Longitude).ToRadian();

            var h1 = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                     Math.Cos(coord1.Latitude.ToRadian()) * Math.Cos(coord2.Latitude.ToRadian()) *
                     Math.Sin(lng / 2) * Math.Sin(lng / 2);

            var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));

            return R * h2;
        }
    }
}