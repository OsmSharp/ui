//using System;
//using OsmSharp.Math.Geo;

//namespace OsmSharp.Math.Geo.Projections
//{
//    /// <summary>
//    /// Represents an elliptical mercator projection.
//    /// </summary>
//    public class EllipticalMercator : IProjection
//    {
//        /// <summary>
//        /// The radius of earth (largest).
//        /// </summary>
//        private static readonly double R_MAJOR = 6378137.0;
//        /// <summary>
//        /// The radius of earth (smallest).
//        /// </summary>
//        private static readonly double R_MINOR = 6356752.3142;
//        /// <summary>
//        /// The ratio between smallest/largets radius of earth.
//        /// </summary>
//        private static readonly double RATIO = R_MINOR / R_MAJOR;
//        /// <summary>
//        /// The eccent.
//        /// </summary>
//        private static readonly double ECCENT = System.Math.Sqrt(1.0 - (RATIO * RATIO));
//        /// <summary>
//        /// The CO.
//        /// </summary>
//        private static readonly double COM = 0.5 * ECCENT;
//        /// <summary>
//        /// The DE g2 RA.
//        /// </summary>
//        private static readonly double DEG2RAD = System.Math.PI / 180.0;
//        /// <summary>
//        /// The RA d2 deg.
//        /// </summary>
//        private static readonly double RAD2Deg = 180.0 / System.Math.PI;
//        /// <summary>
//        /// The P i_2.
//        /// </summary>
//        private static readonly double PI_2 = System.Math.PI / 2.0;
		
//        #region IProjection implementation
		
//        /// <summary>
//        /// Converts the given lat lon to pixels.
//        /// </summary>
//        /// <returns>The pixel.</returns>
//        /// <param name="lat">Lat.</param>
//        /// <param name="lon">Lon.</param>
//        public double[] ToPixel (double lat, double lon)
//        {
//            return new double[] { this.LongitudeToX(lon), this.LatitudeToY(lat) };
//        }
		
//        /// <summary>
//        /// Converts the given coordinate to pixels.
//        /// </summary>
//        /// <returns>The pixel.</returns>
//        /// <param name="coordinate">Coordinate.</param>
//        public double[] ToPixel (GeoCoordinate coordinate)
//        {
//            return this.ToPixel(coordinate.Latitude, coordinate.Longitude);
//        }
		
//        /// <summary>
//        /// Converts the given x-y pixel coordinates into geocoordinates.
//        /// </summary>
//        /// <returns>The geo coordinates.</returns>
//        /// <param name="x">The x coordinate.</param>
//        /// <param name="y">The y coordinate.</param>
//        public GeoCoordinate ToGeoCoordinates (double x, double y)
//        {
//            return new GeoCoordinate(this.YToLatitude(y), this.XToLongitude(x));
//        }
		
//        /// <summary>
//        /// Converts the given x-pixel coordinate into logitude.
//        /// </summary>
//        /// <returns>The x.</returns>
//        /// <param name="longitude">Longitude.</param>
//        public double LongitudeToX (double longitude)
//        {
//            return R_MAJOR * DegToRad(longitude);
//        }
		
//        /// <summary>
//        /// Converts the given y-pixel coordinate into logitude.
//        /// </summary>
//        /// <returns>The y.</returns>
//        /// <param name="latitude">Latitude.</param>
//        public double LatitudeToY (double latitude)
//        {
//            latitude = System.Math.Min(89.5, System.Math.Max(latitude, -89.5));
//            double phi = EllipticalMercator.DegToRad(latitude);
//            double sinphi = System.Math.Sin(phi);
//            double con = ECCENT * sinphi;
//            con = System.Math.Pow(((1.0 - con) / (1.0 + con)), COM);
//            double ts = System.Math.Tan(0.5 * ((System.Math.PI * 0.5) - phi)) / con;
//            return 0 - R_MAJOR * System.Math.Log(ts);
//        }
		
//        /// <summary>
//        /// Converts the given y-coordinate to latitude.
//        /// </summary>
//        /// <returns>The latitude.</returns>
//        /// <param name="y">The y coordinate.</param>
//        public double YToLatitude(double y)
//        {
//            double ts = System.Math.Exp(-y / R_MAJOR);
//            double phi = PI_2 - 2 * System.Math.Atan(ts);
//            double dphi = 1.0;
//            int i = 0;
//            while ((System.Math.Abs(dphi) > 0.000000001) && (i < 15))
//            {
//                double con = ECCENT * System.Math.Sin(phi);
//                dphi = PI_2 - 2 * System.Math.Atan(ts * System.Math.Pow((1.0 - con) / (1.0 + con), COM)) - phi;
//                phi += dphi;
//                i++;
//            }
//            return EllipticalMercator.RadToDeg(phi);
//        }
		
//        /// <summary>
//        /// Converts the given x-coordinate to longitude.
//        /// </summary>
//        /// <returns>The longitude.</returns>
//        /// <param name="x">The x coordinate.</param>
//        public double XToLongitude(double x)
//        {
//            return EllipticalMercator.RadToDeg(x) / R_MAJOR;
//        }

//        /// <summary>
//        /// Converts the given zoom level to a zoomfactor for this projection.
//        /// </summary>
//        /// <param name="zoomLevel"></param>
//        /// <returns></returns>
//        public double ToZoomFactor(double zoomLevel)
//        {
//            return 78271.516/System.Math.Pow(2, zoomLevel);
//        }

//        #endregion
		
//        private static double RadToDeg(double rad)
//        {
//            return rad * RAD2Deg;
//        }
		
//        private static double DegToRad(double deg)
//        {
//            return deg * DEG2RAD;
//        }
//    }
//}