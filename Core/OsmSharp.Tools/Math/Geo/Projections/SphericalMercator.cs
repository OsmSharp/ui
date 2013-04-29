using System;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Tools
{
	/// <summary>
	/// Represents a spherical mercator projection.
	/// </summary>
	/// <remarks>Based on the code from: http://wiki.openstreetmap.org/wiki/Mercator#C.23</remarks>
	public class SphericalMercator : IProjection
	{
		/// <summary>
		/// The radius of earth (largest).
		/// </summary>
		private static readonly double R_MAJOR = 6378137.0;
		/// <summary>
		/// The radius of earth (smallest).
		/// </summary>
		private static readonly double R_MINOR = 6356752.3142;
		/// <summary>
		/// The ratio between smallest/largets radius of earth.
		/// </summary>
		private static readonly double RATIO = R_MINOR / R_MAJOR;
		/// <summary>
		/// The eccent.
		/// </summary>
		private static readonly double ECCENT = System.Math.Sqrt(1.0 - (RATIO * RATIO));
		/// <summary>
		/// The CO.
		/// </summary>
		private static readonly double COM = 0.5 * ECCENT;
		/// <summary>
		/// The DE g2 RA.
		/// </summary>
		private static readonly double DEG2RAD = System.Math.PI / 180.0;
		/// <summary>
		/// The RA d2 deg.
		/// </summary>
		private static readonly double RAD2Deg = 180.0 / System.Math.PI;
		//private static readonly double PI_2 = System.Math.PI / 2.0;

		#region IProjection implementation

		/// <summary>
		/// Converts the given lat lon to pixels.
		/// </summary>
		/// <returns>The pixel.</returns>
		/// <param name="lat">Lat.</param>
		/// <param name="lon">Lon.</param>
		public double[] ToPixel (double lat, double lon)
		{
			return new double[] { this.LongitudeToX(lon), this.LatitudeToY(lat) };
		}

		/// <summary>
		/// Converts the given coordinate to pixels.
		/// </summary>
		/// <returns>The pixel.</returns>
		/// <param name="coordinate">Coordinate.</param>
		public double[] ToPixel (GeoCoordinate coordinate)
		{
			return this.ToPixel(coordinate.Latitude, coordinate.Longitude);
		}

		/// <summary>
		/// Converts the given x-y pixel coordinates into geocoordinates.
		/// </summary>
		/// <returns>The geo coordinates.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public GeoCoordinate ToGeoCoordinates (double x, double y)
		{
			return new GeoCoordinate(this.YToLatitude(y), this.XToLongitude(x));
		}

		/// <summary>
		/// Converts the given x-pixel coordinate into logitude.
		/// </summary>
		/// <returns>The x.</returns>
		/// <param name="longitude">Longitude.</param>
		public double LongitudeToX (double longitude)
		{
			return R_MAJOR * DegToRad(longitude);
		}

		/// <summary>
		/// Converts the given y-pixel coordinate into logitude.
		/// </summary>
		/// <returns>The y.</returns>
		/// <param name="latitude">Latitude.</param>
		public double LatitudeToY (double latitude)
		{
			return 180.0/System.Math.PI * 
				System.Math.Log(
					System.Math.Tan(
						System.Math.PI/4.0+latitude*(System.Math.PI/180.0)/2));
		}

		/// <summary>
		/// Converts the given y-coordinate to latitude.
		/// </summary>
		/// <returns>The latitude.</returns>
		/// <param name="y">The y coordinate.</param>
		public double YToLatitude(double y)
		{
			return 180.0/System.Math.PI * 
				(2 * 
				 System.Math.Atan(
					System.Math.Exp(y*System.Math.PI/180)) - System.Math.PI/2);
		}

		/// <summary>
		/// Converts the given x-coordinate to longitude.
		/// </summary>
		/// <returns>The longitude.</returns>
		/// <param name="x">The x coordinate.</param>
		public double XToLongitude(double x)
		{
			return SphericalMercator.RadToDeg(x) / R_MAJOR;
		}

		#endregion

		private static double RadToDeg(double rad)
		{
			return rad * RAD2Deg;
		}
		
		private static double DegToRad(double deg)
		{
			return deg * DEG2RAD;
		}
	}
}