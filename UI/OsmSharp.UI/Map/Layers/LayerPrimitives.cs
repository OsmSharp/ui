using System;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Map;
using OsmSharp.Math.Geo;
using OsmSharp.UI.Renderer;
using OsmSharp.Math.Geo.Projections;

namespace OsmSharp.UI.Map.Layers
{
	/// <summary>
	/// A layer containing several simple primitives.
	/// </summary>
	public class LayerPrimitives : ILayer
	{
		/// <summary>
		/// Holds the projection for this layer.
		/// </summary>
		private IProjection _projection;

		/// <summary>
		/// Holds the scene.
		/// </summary>
		private Scene2D _scene;

		/// <summary>
		/// Creates a new primitives layer.
		/// </summary>
		public LayerPrimitives (IProjection projection)
		{
			_projection = projection;

			_scene = new Scene2D();
		}

//		/// <summary>
//		/// Adds a maker.
//		/// </summary>
//		/// <param name="coordinate">Coordinate.</param>
//		/// <param name="image">Image.</param>
//		private uint AddMarker(GeoCoordinate coordinate, byte[] image)
//		{
//			throw new 
//		}
//
		/// <summary>
		/// Adds a point.
		/// </summary>
		/// <returns>The point.</returns>
		/// <param name="coordinate">Coordinate.</param>
		/// <param name="sizePixels">Size pixels.</param>
		public uint AddPoint(GeoCoordinate coordinate, float sizePixels, int color)
		{
			double[] projectedCoordinates = _projection.ToPixel(
				coordinate);
			uint id = _scene.AddPoint(float.MinValue, float.MaxValue, 
			                       projectedCoordinates[0], projectedCoordinates[1],
			                       color, sizePixels);
			this.RaiseLayerChanged();
			return id;
		}

		/// <summary>
		/// Remove the object with the specified id.
		/// </summary>
		/// <param name="id">Identifier.</param>
		private void Remove(uint id)
		{
			if(_scene.Remove(id))
			{
				this.RaiseLayerChanged();
			}
		}

		#region ILayer implementation

		/// <summary>
		/// Raised when the content of this layer has changed.
		/// </summary>
		public event OsmSharp.UI.Map.Map.LayerChanged LayerChanged;

		/// <summary>
		/// Raises the layer changed event.
		/// </summary>
		private void RaiseLayerChanged()
		{
			if(this.LayerChanged != null)
			{
				this.LayerChanged(this);
			}
		}

		/// <summary>
		/// Called when the view on the map has changed.
		/// </summary>
		/// <param name="map"></param>
		/// <param name="zoomFactor"></param>
		/// <param name="center"></param>
		/// <param name="view"></param>
		public void ViewChanged (Map map, float zoomFactor, GeoCoordinate center, View2D view)
		{

		}

		/// <summary>
		/// The minimum zoom.
		/// </summary>
		/// <remarks>The minimum zoom is the 'highest'.</remarks>
		/// <value>The minimum zoom.</value>
		public float? MinZoom {
			get {
				throw new NotImplementedException ();
			}
		}

		/// <summary>
		/// The maximum zoom.
		/// </summary>
		/// <remarks>The maximum zoom is the 'lowest' or most detailed view.</remarks>
		/// <value>The max zoom.</value>
		public float? MaxZoom {
			get {
				throw new NotImplementedException ();
			}
		}

		/// <summary>
		/// Gets the scene.
		/// </summary>
		/// <value>The scene.</value>
		public OsmSharp.UI.Renderer.Scene2D Scene {
			get {
				return _scene;
			}
		}

		#endregion
	}
}