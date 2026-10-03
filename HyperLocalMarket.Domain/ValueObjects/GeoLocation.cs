using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace HyperLocalMarket.Domain.ValueObjects
{
    public sealed class GeoLocation
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public Point? Point { get; }

        private GeoLocation() { }

        public GeoLocation(double latitude, double longitude)
        {
            if(latitude < -90 || latitude > 90)
            {
                throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");
            }

            if (longitude < -180 || longitude > 180)
            {
                throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");
            }

            Latitude = latitude;
            Longitude = longitude;

            // PostGIS uses (X=Longitude, Y=Latitude)
            Point = new Point(longitude, latitude) { SRID = 4326 };
        }


    }
}
