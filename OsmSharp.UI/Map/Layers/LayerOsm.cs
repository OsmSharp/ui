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

using System.Collections.Generic;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Data;
using OsmSharp.UI.Map.Styles;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.Math.Geo.Projections;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A layer drawing OSM data.
    /// </summary>
    public class OsmLayer : ILayer
    {
        /// <summary>
        /// Holds the source of the OSM raw data.
        /// </summary>
        private readonly IDataSourceReadOnly _dataSource;

        /// <summary>
        /// Holds the style scene manager.
        /// </summary>
        private readonly StyleSceneManager _styleSceneManager;

        /// <summary>
        /// Creates a new OSM data layer.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="styleInterpreter"></param>
        public OsmLayer(IDataSourceReadOnly dataSource, StyleInterpreter styleInterpreter, IProjection projection)
        {
            // build the zoom-level cutoffs.
            List<float> zoomLevelCutoffs = new List<float>();
            zoomLevelCutoffs.Add((float)projection.ToZoomFactor(18));
            //zoomLevelCutoffs.Add((float)projection.ToZoomFactor(16));
            //zoomLevelCutoffs.Add((float)projection.ToZoomFactor(14));
            //zoomLevelCutoffs.Add((float)projection.ToZoomFactor(12));
            //zoomLevelCutoffs.Add((float)projection.ToZoomFactor(10));
            //zoomLevelCutoffs.Add((float)projection.ToZoomFactor(8));
            zoomLevelCutoffs.Add((float)projection.ToZoomFactor(0));

            _dataSource = dataSource;
            _styleSceneManager = new StyleSceneManager(styleInterpreter, zoomLevelCutoffs);
            //_styleSceneManager = new StyleSceneManager(styleInterpreter);
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
        public Scene2D Scene { get { return _styleSceneManager.Scene; } }

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
        /// Builds the scene.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        private void BuildScene(Map map, float zoomFactor, GeoCoordinate center, View2D view)
		{
			// build the boundingbox.
			var viewBox = view.OuterBox;
			var box = new GeoCoordinateBox (map.Projection.ToGeoCoordinates (viewBox.Min [0], viewBox.Min [1]),
			                                map.Projection.ToGeoCoordinates (viewBox.Max [0], viewBox.Max [1]));

            _styleSceneManager.FillScene(_dataSource, box, map.Projection);
        }

        #endregion
    }
}