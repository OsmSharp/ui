using System;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Tools
{
	/// <summary>
	/// An abstract of a projection.
	/// </summary>
	public interface IProjection
	{
		/// <summary>
		/// Converts the given lat lon to pixels.
		/// </summary>
		/// <returns>The pixel.</returns>
		/// <param name="lat">Lat.</param>
		/// <param name="lon">Lon.</param>
		double[] ToPixel(double lat, double lon);

		/// <summary>
		/// Converts the given coordinate to pixels.
		/// </summary>
		/// <returns>The pixel.</returns>
		/// <param name="coordinate">Coordinate.</param>
		double[] ToPixel(GeoCoordinate coordinate);

		/// <summary>
		/// Converts the given x-y pixel coordinates into geocoordinates.
		/// </summary>
		/// <returns>The geo coordinates.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		GeoCoordinate ToGeoCoordinates(double x, double y);

		/// <summary>
		/// Converts the given x-pixel coordinate into logitude.
		/// </summary>
		/// <returns>The x.</returns>
		/// <param name="longitude">Longitude.</param>
		double LongitudeToX(double longitude);
		
		/// <summary>
		/// Converts the given y-pixel coordinate into logitude.
		/// </summary>
		/// <returns>The y.</returns>
		/// <param name="latitude">Latitude.</param>
		double LatitudeToY(double latitude);

		/// <summary>
		/// Converts the given y-coordinate to latitude.
		/// </summary>
		/// <returns>The latitude.</returns>
		/// <param name="y">The y coordinate.</param>
		double YToLatitude(double y);

		/// <summary>
		/// Converts the given x-coordinate to longitude.
		/// </summary>
		/// <returns>The longitude.</returns>
		/// <param name="x">The x coordinate.</param>
		double XToLongitude(double x);
	}
}