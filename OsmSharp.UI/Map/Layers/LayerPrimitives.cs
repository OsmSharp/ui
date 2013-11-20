// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Map;
using OsmSharp.Math.Geo;
using OsmSharp.UI.Renderer;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Renderer.Scene;

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

            _scene = new Scene2DSimple(
                (float)(new OsmSharp.Math.Geo.Projections.WebMercator().ToZoomFactor(16)));
		}

		/// <summary>
		/// Adds a point.
		/// </summary>
		/// <returns>The point.</returns>
		/// <param name="coordinate">Coordinate.</param>
		/// <param name="sizePixels">Size pixels.</param>
		public void AddPoint(GeoCoordinate coordinate, float sizePixels, int color)
		{
			double[] projectedCoordinates = _projection.ToPixel(
				coordinate);
            uint pointId = _scene.AddPoint(projectedCoordinates[0], projectedCoordinates[1]);
            _scene.AddStylePoint(pointId, float.MinValue, float.MaxValue, color, sizePixels);
			this.RaiseLayerChanged();
		}

        /// <summary>
        /// Adds a line.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="sizePixels"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public void AddLine(GeoCoordinate point1, GeoCoordinate point2, float sizePixels, int color)
        {
            double[] projected1 = _projection.ToPixel(point1);
            double[] projected2 = _projection.ToPixel(point2);

            double[] x = new double[] { projected1[0], projected2[0] };
            double[] y = new double[] { projected1[1], projected2[1] };

            uint? pointsId = _scene.AddPoints(x, y);
            if (pointsId.HasValue)
            {
                _scene.AddStyleLine(pointsId.Value, float.MinValue, float.MaxValue, color, sizePixels);
                this.RaiseLayerChanged();
            }
        }

        /// <summary>
        /// Clears all data from this layer.
        /// </summary>
        public void Clear()
        {
            _scene.Clear();
        }

		#region ILayer implementation

		/// <summary>
		/// Raised when the content of this layer has changed.
		/// </summary>
		public event OsmSharp.UI.Map.Map.LayerChanged LayerChanged;
		
		/// <summary>
		/// Invalidates this layer.
		/// </summary>
		public void Invalidate()
		{
			if (this.LayerChanged != null) {
				this.LayerChanged (this);
			}
		}

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
		public Scene2D Scene {
			get {
				return _scene;
			}
		}

		#endregion
	}
}