//using System;
//using OsmSharp.Tools.Math.Geo;

//namespace OsmSharp.Math.Geo.Projections
//{
//    /// <summary>
//    /// Represents a spherical mercator projection.
//    /// </summary>
//    /// <remarks>Based on the code from: http://wiki.openstreetmap.org/wiki/Mercator#C.23</remarks>
//    public class SphericalMercator : IProjection
//    {
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
//            return longitude;
//        }

//        /// <summary>
//        /// Converts the given y-pixel coordinate into logitude.
//        /// </summary>
//        /// <returns>The y.</returns>
//        /// <param name="latitude">Latitude.</param>
//        public double LatitudeToY (double latitude)
//        {
//            return 180.0/System.Math.PI * 
//                System.Math.Log(
//                    System.Math.Tan(
//                        System.Math.PI/4.0+latitude*(System.Math.PI/180.0)/2));
//        }

//        /// <summary>
//        /// Converts the given y-coordinate to latitude.
//        /// </summary>
//        /// <returns>The latitude.</returns>
//        /// <param name="y">The y coordinate.</param>
//        public double YToLatitude(double y)
//        {
//            return 180.0/System.Math.PI * 
//                (2 * 
//                 System.Math.Atan(
//                    System.Math.Exp(y*System.Math.PI/180)) - System.Math.PI/2);
//        }

//        /// <summary>
//        /// Converts the given x-coordinate to longitude.
//        /// </summary>
//        /// <returns>The longitude.</returns>
//        /// <param name="x">The x coordinate.</param>
//        public double XToLongitude(double x)
//        {
//            return x;
//        }

//        /// <summary>
//        /// Converts the given zoom level to a zoomfactor for this projection.
//        /// </summary>
//        /// <param name="zoomLevel"></param>
//        /// <returns></returns>
//        public double ToZoomFactor(double zoomLevel)
//        {
//            return (360.0/512.0)*System.Math.Pow(2, zoomLevel);
//        }

//        #endregion
//    }
//}