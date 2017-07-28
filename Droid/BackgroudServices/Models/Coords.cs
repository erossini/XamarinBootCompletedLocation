using System;

namespace LocationTest.Droid.BackgroudServices.Models
{
    public class Coords
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Coords(double lat, double lng)
        {
            this.Latitude = lat;
            this.Longitude = lng;
        }
    }
}