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

using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm.Data;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Map.Styles;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Images;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.UI.Map
{
    /// <summary>
    /// Represents a renderable map.
    /// </summary>
    public class Map
    {
        /// <summary>
        /// Holds the layers list.
        /// </summary>
        private readonly IList<Layer> _layers;

        /// <summary>
        /// Creates a new map.
        /// </summary>
        public Map()
        {
            _layers = new List<Layer>();
            this.Projection = new WebMercator();
        }

        /// <summary>
        /// Creates a new map.
        /// </summary>
        public Map(IProjection projection)
        {
            _layers = new List<Layer>();
            this.Projection = projection;
        }

        /// <summary>
        /// Delegate for map changes.
        /// </summary>
        /// <param name="?"></param>
        public delegate void MapChangedDelegate();

        /// <summary>
        /// Event raised when the map content changed.
        /// </summary>
        public event MapChangedDelegate MapChanged; 

        /// <summary>
        /// A delegate for layer changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public delegate void LayerChanged(Layer sender);

        /// <summary>
        /// Gets the default projection this map is using.
        /// </summary>
        public IProjection Projection { get; private set; }

        /// <summary>
        /// The view on this map has changed. Maybe the layers need to load new data, they need to be notified.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        /// <param name="extraView"></param>
        public void ViewChanged(float zoomFactor, GeoCoordinate center, View2D view, View2D extraView)
        {
            lock (_layers)
            {
                foreach (var layer in _layers)
                {
                    layer.ViewChanged(this, zoomFactor, center, view, extraView);
                }
            }
        }

        /// <summary>
        /// Cancels any ongoing map change notification.
        /// </summary>
        public void ViewChangedCancel()
        {
            lock (_layers)
            {
                foreach (var layer in _layers)
                {
                    layer.ViewChangedCancel();
                }
            }
        }

        #region Layers

        /// <summary>
        /// Adds a layer on top of the existing layers.
        /// </summary>
        /// <param name="layer"></param>
        public void AddLayer(Layer layer)
        {
            lock (_layers)
            {
                layer.LayerChanged += new LayerChanged(layer_LayerChanged);
                _layers.Add(layer);
            }

            // map has obviously changed here!
            if (this.MapChanged != null) { this.MapChanged(); }
        }

        /// <summary>
        /// Inserts a layer.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="layer"></param>
        public void InsertLayer(int idx, Layer layer)
        {
            lock (_layers)
            {
                layer.LayerChanged += new LayerChanged(layer_LayerChanged);
                _layers.Insert(idx, layer);
            }

            // map has obviously changed here!
            if (this.MapChanged != null) { this.MapChanged(); }
        }

        /// <summary>
        /// Removes the given layer.
        /// </summary>
        /// <param name="layer"></param>
        public bool RemoveLayer(Layer layer)
        {
            lock (_layers)
            {
                if (!_layers.Remove(layer))
                {
                    return false;
                }
            }
            layer.LayerChanged -= new LayerChanged(layer_LayerChanged); // remove event handler.

            // map has obviously changed here!
            if (this.MapChanged != null) { this.MapChanged(); }

            return true;
        }

        /// <summary>
        /// Removes a layer at the given index.
        /// </summary>
        /// <param name="idx"></param>
        public void RemoveLayerAt(int idx)
        {
            lock (_layers)
            {
                var layer = _layers[idx];
                layer.LayerChanged -= new LayerChanged(layer_LayerChanged); // remove event handler.
                _layers.RemoveAt(idx);
            }

            // map has obviously changed here!
            if (this.MapChanged != null) { this.MapChanged(); }
        }

        /// <summary>
        /// Gets the layer at the given idx.
        /// </summary>
        /// <param name="layerIdx"></param>
        /// <returns></returns>
        public Layer this[int layerIdx]
        {
            get
            {
                lock (_layers)
                {
                    return _layers[layerIdx];
                }
            }
        }

        /// <summary>
        /// Gets the layer count.
        /// </summary>
        public int LayerCount
        {
            get
            {
                lock (_layers)
                { 
                    return _layers.Count;
                }
            }
        }

        /// <summary>
        /// Called when a layer changed.
        /// </summary>
        /// <param name="sender"></param>
        void layer_LayerChanged(Layer sender)
        {
            if (this.MapChanged != null) { this.MapChanged(); }
        }

        #region Layer Helpers

        /// <summary>
        /// Adds a new tile layer.
        /// </summary>
        /// <param name="tileUrl">The tile URL.</param>
        /// <param name="nativeImageCache">The native image cache.</param>
        public LayerTile AddLayerTile(string tileUrl)
        {
            var layerTile = new LayerTile(tileUrl);
            this.AddLayer(layerTile);
            return layerTile;
        }

        /// <summary>
        /// Adds a new gpx layer for the given gpx-stream.
        /// </summary>
        /// <param name="gpxStream"></param>
        public LayerGpx AddLayerGpx(Stream gpxStream)
        {
            var layerGpx = new LayerGpx(this.Projection);
            layerGpx.AddGpx(gpxStream);
            this.AddLayer(layerGpx);
            return layerGpx;
        }

        /// <summary>
        /// Adds a new route layer with the given route loaded.
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public LayerRoute AddLayerRoute(Route route)
        {
            var layerRoute = new LayerRoute(this.Projection);
            layerRoute.AddRoute(route);
            this.AddLayer(layerRoute);
            return layerRoute;
        }

        ///// <summary>
        ///// Adds a new scene layer with the given primitives source as it's source.
        ///// </summary>
        ///// <param name="scene"></param>
        ///// <returns></returns>
        //public LayerScene AddLayerScene(IScene2DPrimitivesSource scene)
        //{
        //    LayerScene layerScene = new LayerScene(scene);
        //    this.AddLayer(layerScene);
        //    return layerScene;
        //}

        /// <summary>
        /// Adds a graph layer with the given data and style.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="styleInterpreter"></param>
        /// <returns></returns>
        public LayerDynamicGraphLiveEdge AddLayerGraph(IBasicRouterDataSource<LiveEdge> dataSource,
            StyleInterpreter styleInterpreter)
        {
            LayerDynamicGraphLiveEdge layerGraph = new LayerDynamicGraphLiveEdge(dataSource, styleInterpreter);
            this.AddLayer(layerGraph);
            return layerGraph;
        }

        /// <summary>
        /// Adds a layer displaying osm data from a given data source using the given style.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="styleInterpreter"></param>
        /// <returns></returns>
        public LayerOsm AddLayerOsm(IDataSourceReadOnly dataSource, StyleInterpreter styleInterpreter)
        {
            LayerOsm layerOsm = new LayerOsm(dataSource, styleInterpreter, this.Projection);
            this.AddLayer(layerOsm);
            return layerOsm;
        }

        #endregion

        #endregion
    }
}