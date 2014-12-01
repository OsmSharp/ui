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
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm.Data;
using OsmSharp.UI.Map.Styles;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Primitives;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A layer drawing OSM data.
    /// </summary>
    public class LayerOsm : Layer
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
        /// <param name="projection"></param>
        public LayerOsm(IDataSourceReadOnly dataSource, StyleInterpreter styleInterpreter, IProjection projection)
        {
            // build the zoom-level cutoffs.
            List<float> zoomFactors = new List<float>();
            zoomFactors.Add(16);
            zoomFactors.Add(14);
            zoomFactors.Add(12);
            zoomFactors.Add(10);

            _dataSource = dataSource;
            _styleSceneManager = new StyleSceneManager(styleInterpreter, projection, zoomFactors);
        }

        /// <summary>
        /// Called when the view on the map containing this layer has changed.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        /// <param name="extraView"></param>
        protected internal override void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view, View2D extraView)
        {
            this.BuildScene(map, zoomFactor, center, extraView);
        }

        /// <summary>
        /// Returns all objects from this layer visible for the given parameters.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        protected internal override IEnumerable<Primitive2D> Get(float zoomFactor, View2D view)
        {
            return _styleSceneManager.Scene.Get(view, zoomFactor);
        }

        /// <summary>
        /// Returns the backcolor of this layer.
        /// </summary>
        public override int? BackColor
        {
            get
            {
                return _styleSceneManager.Scene.BackColor;
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
            var box = new GeoCoordinateBox(map.Projection.ToGeoCoordinates(viewBox.Min[0], viewBox.Min[1]),
                 map.Projection.ToGeoCoordinates(viewBox.Max[0], viewBox.Max[1]));

            _styleSceneManager.FillScene(_dataSource, box, map.Projection);
        }

        #endregion

        /// <summary>
        /// Returns the bounding rectangle of this layer (if available).
        /// </summary>
        /// <remarks>Not all layers, formats support getting an envolope. Property can return null even on some types of bounded data.</remarks>
        public override GeoCoordinateBox Envelope
        {
            get
            {
                return _dataSource.BoundingBox;
            }
        }

        /// <summary>
        /// Closes this layer.
        /// </summary>
        public override void Close()
        {
            base.Close();

            // nothing to stop, even better!
        }
    }
}