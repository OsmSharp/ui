using OsmSharp.Math.Geo;
using OsmSharp.Math;

namespace OsmSharp.UI.Map
{	
	/// <summary>
	/// Represents a view on a map.
	/// </summary>
	public sealed class View
	{
		/// <summary>
		/// Holds the pixels per zoom.
		/// </summary>
		private const int PixelsPerZoom = 256;

		/// <summary>
		/// Holds the box for this view.
		/// </summary>
		private readonly GeoCoordinateBox _box;

		/// <summary>
		/// Initializes a new instance of the <see cref="View"/> class.
		/// </summary>
		/// <param name="box">The boundaries.</param>
		private View(GeoCoordinateBox box)
		{
			_box = box;
		}

		/// <summary>
		/// Gets the boundaries of this view.
		/// </summary>
		/// <value>The _box.</value>
		public GeoCoordinateBox Box {
			get {
				return _box;
			}
		}

		/// <summary>
		/// Converts from target.
		/// </summary>
		/// <returns>The from target.</returns>
		/// <param name="zoom">Zoom.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public GeoCoordinate ConvertFromTarget(float zoom, int width, int height,
		                                       int x, int y)
		{
			GeoCoordinate topLeft = this.Box.TopLeft;
			
			float resolutionPerXDegree = ((float)width) / (float)this.Box.DeltaLon;
			float resolutionPerYDegree = ((float)height / (float)this.Box.DeltaLat);
			
			float angleDivX = -x / resolutionPerXDegree;
			float angleDivY = (y / resolutionPerYDegree); // x-axis and latitude-axis differ in direction.
			
			float longitude = (float)(topLeft.Longitude - angleDivX);
			float latitude = (float)(topLeft.Latitude - angleDivY);

			return new GeoCoordinate(latitude, longitude);
		}
		
		/// <summary>
		/// Converts a geo coordinate to projected two dimension versions on the target.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="point"></param>
		/// <returns></returns>
		public PointF2D ConvertToTarget(
			float zoom, int width, int height,
			GeoCoordinate point)
		{
			double resolutionPerXDegree = ((double)width) / this.Box.DeltaLon;
			
			// TODO: explain the arbirtrairy parameter and the actual projection used.
			double resolutionPerYDegree = ((double)height / this.Box.DeltaLat);
			
			double angleDivX = (point.Longitude - _box.MinLon);
			double angleDivY = (point.Latitude - _box.MaxLat);
			double x = angleDivX * resolutionPerXDegree;
			double y = -(angleDivY * resolutionPerYDegree); // x-axis and latitude-axis differ in direction.
			
			return new PointF2D((float)x,(float)y);
		}

		/// <summary>
		/// Converts the given geo coordinates to projected two dimension versions on the target.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="points"></param>
		/// <returns></returns>
		public PointF2D[] ConvertToTargets(
			float zoom, int width, int height,
			GeoCoordinate[] points)
		{
			PointF2D[] target_points = new PointF2D[points.Length];
			
			for (int idx = 0; idx < points.Length; idx++)
			{
				target_points[idx] = this.ConvertToTarget(zoom, width, height, points[idx]);
			}
			
			return target_points;
		}

		#region Create From

		/// <summary>
		/// Creates a view for a rectangular viewing window from a central location, zoom and size of the viewing window.
		/// </summary>
		/// <returns>The from.</returns>
		/// <param name="center">The central location.</param>
		/// <param name="zoom">Zoom.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public static View CreateFrom(GeoCoordinate center, float zoom,
		                              int width, int height)
		{
			float degreesPerZoom = (360f / (float)System.Math.Pow(2, zoom));
			float degreesPerPixel = degreesPerZoom / degreesPerZoom;

			float degreesX = (width * degreesPerPixel);
			float degreesY = (height * degreesPerPixel / 1.5f); // TODO: figure this stuff out! 

			return new View(new GeoCoordinateBox(
				new GeoCoordinate(
				center.Latitude - degreesY / 2.0f,
				center.Longitude - degreesX / 2.0f),
				new GeoCoordinate(
				center.Latitude + degreesY / 2.0f,
				center.Longitude + degreesX / 2.0f)));
		}

		#endregion
	}
}