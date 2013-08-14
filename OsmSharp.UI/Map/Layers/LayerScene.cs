// OsmSharp - OpenStreetMap tools & library.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Data;
using OsmSharp.Math.Geo;
using OsmSharp.UI.Map.Styles;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene.Scene2DPrimitives;
using OsmSharp.UI.Renderer.Scene;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A layer drawing scene objects.
    /// </summary>
    public class LayerScene : ILayer
    {
		/// <summary>
		/// Holds the scene.
		/// </summary>
		private Scene2DSimple _scene2DSimple;

        /// <summary>
        /// Holds the scene primitives source.
        /// </summary>
        private IScene2DPrimitivesSource _index;

        /// <summary>
        /// Creates a new scene layer.
        /// </summary>
        /// <param name="index"></param>
        public LayerScene(IScene2DPrimitivesSource index)
        {
            _index = index;
			_scene2DSimple = new Scene2DSimple();
        }

        /// <summary>
        /// Gets the minimum zoom.
        /// </summary>
        public float? MinZoom { get; private set; }

        /// <summary>
        /// Gets the maximum zoom.
        /// </summary>
        public float? MaxZoom { get; private set; }

        /// <summary>
        /// Gets the scene of this layer containing the objects already projected.
        /// </summary>
		public Scene2D Scene { 
			get {
				return _scene2DSimple;
			}
		}

        /// <summary>
        /// Called when the view on the map containing this layer has changed.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        public void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view)
        {
            this.BuildScene(map, zoomFactor, center, view);
        }

        /// <summary>
        /// Event raised when this layer's content has changed.
        /// </summary>
        public event Map.LayerChanged LayerChanged;
		
		/// <summary>
		/// Invalidates this layer.
		/// </summary>
		public void Invalidate()
		{
			if (this.LayerChanged != null) {
				this.LayerChanged (this);
			}
		}

        #region Scene Building

		/// <summary>
		/// Holds the last box.
		/// </summary>
		private GeoCoordinateBox _lastBox;

		/// <summary>
		/// Holds the last zoom level.
		/// </summary>
		private int _lastZoom;
        
        /// <summary>
        /// Builds the scene.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        private void BuildScene(Map map, float zoomFactor, GeoCoordinate center, View2D view)
        {
			// build the boundingbox.
			var box = new GeoCoordinateBox(map.Projection.ToGeoCoordinates(view.Left, view.Top),
			                               map.Projection.ToGeoCoordinates(view.Right, view.Bottom));
			var zoomLevel = (int)map.Projection.ToZoomLevel (zoomFactor);
			if (_lastBox != null && _lastBox.IsInside (box) &&
			    zoomLevel == _lastZoom) {
				return;
			}
			_lastBox = box;
			_lastZoom = zoomLevel;

            // reset the scene.
			_scene2DSimple = new Scene2DSimple();

            // get from the index.
            this.Scene.BackColor = SimpleColor.FromKnownColor(KnownColor.White).Value;
			_index.Get(_scene2DSimple, view, zoomFactor);
        }

        #endregion
    }
}