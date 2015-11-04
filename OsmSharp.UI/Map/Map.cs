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
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Map.Styles;
using OsmSharp.UI.Renderer;
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
        /// Holds the explicitly set backcolor.
        /// </summary>
        private int? _backcolor;

        /// <summary>
        /// Returns the backcolor of this map.
        /// </summary>
        public int? BackColor
        {
            get
            {
                if (_backcolor.HasValue)
                { // backcolor overridden.
                    return _backcolor;
                }
                else
                { // get backcolor from first layer.
                    if (_layers != null && _layers.Count > 0 && _layers[0] != null)
                    {
                        return _layers[0].BackColor;
                    }
                }
                return null;
            }
            set
            {
                _backcolor = value;
            }
        }

        /// <summary>
        /// The view on this map has changed. Maybe the layers need to load new data, they need to be notified.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        /// <param name="extraView"></param>
        public void ViewChanged(float zoomFactor, GeoCoordinate center, View2D view, View2D extraView)
        {
            var zoomLevel = (float)this.Projection.ToZoomLevel(zoomFactor);
            lock (_layers)
            {
                foreach (var layer in _layers)
                {
                    if (layer.IsLayerVisibleFor(zoomLevel))
                    {
                        layer.ViewChanged(this, zoomFactor, center, view, extraView);
                    }
                }
            }
        }

        /// <summary>
        /// Utility method for ensuring a view stays within a bounding box of geo coordinated.
        /// </summary>
        /// <param name="center">The map center we want to move to.</param>
        /// <param name="boundingBox">A GeoCoordinateBox defining the bounding box.</param>
        /// <param name="view" The current view.</param>
        /// <returns>Returns a center geo coordinate that is corrected so the view stays within the bounding box.</returns>
        public GeoCoordinate EnsureViewWithinBoundingBox(GeoCoordinate center, GeoCoordinateBox boundingBox, View2D view)
        {
            double[] mapCenterSceneCoords = this.Projection.ToPixel(center);

            var toViewPort = view.CreateToViewPort(view.Width, view.Height);
            double mapCenterPixelsX, mapCenterPixelsY;
            toViewPort.Apply(mapCenterSceneCoords[0], mapCenterSceneCoords[1], out mapCenterPixelsX, out mapCenterPixelsY);

            //double[] mapCenterPixels = view.ToViewPort(view.Width, view.Height, mapCenterSceneCoords[0], mapCenterSceneCoords[1]);

            var fromViewPort = view.CreateFromViewPort(view.Width, view.Height);
            double leftScene, topScene, rightScene, bottomScene;
            fromViewPort.Apply(mapCenterPixelsX - (view.Width) / 2.0, mapCenterPixelsY - (view.Height) / 2.0, out leftScene, out topScene);

            //double[] topLeftSceneCoordinates = view.FromViewPort(view.Width,
            //                                                    view.Height,
            //                                                    mapCenterPixels[0] - (view.Width) / 2.0,
            //                                                    mapCenterPixels[1] - (view.Height) / 2.0);
            GeoCoordinate topLeft = this.Projection.ToGeoCoordinates(leftScene, topScene);
            //GeoCoordinate topLeft = this.Projection.ToGeoCoordinates(topLeftSceneCoordinates[0], topLeftSceneCoordinates[1]);

            fromViewPort.Apply(mapCenterPixelsX + (view.Width) / 2.0, mapCenterPixelsY + (view.Height) / 2.0, out rightScene, out bottomScene);
            //double[] bottomRightSceneCoordinates = view.FromViewPort(view.Width,
            //                                                    view.Height,
            //                                                    mapCenterPixels[0] + (view.Width) / 2.0,
            //                                                    mapCenterPixels[1] + (view.Height) / 2.0);
            GeoCoordinate bottomRight = this.Projection.ToGeoCoordinates(rightScene, bottomScene);

            // Early exit when the view is inside the box.
            if (boundingBox.Contains(topLeft) && boundingBox.Contains(bottomRight))
                return center;

            double viewNorth = topLeft.Latitude;
            double viewEast = bottomRight.Longitude;
            double viewSouth = bottomRight.Latitude;
            double viewWest = topLeft.Longitude;

            double boxNorth = boundingBox.MaxLat;
            double boxEast = boundingBox.MaxLon;
            double boxSouth = boundingBox.MinLat;
            double boxWest = boundingBox.MinLon;

            //TODO: Check if the view acrually fits the bounding box, if not resize the view.

            // Correct all view bounds if neccecary.
            if (viewNorth > boxNorth)
            {
                viewSouth -= viewNorth - boxNorth;
                viewNorth = boxNorth;
            }
            if (viewEast > boxEast)
            {
                viewWest -= viewEast - boxEast;
                viewEast = boxEast;
            }
            if (viewSouth < boxSouth)
            {
                viewNorth += boxSouth - viewSouth;
                viewSouth = boxSouth;
            }
            if (viewWest < boxWest)
            {
                viewEast += boxWest - viewWest;
                viewWest = boxWest;
            }

            // Compute and return corrected map center
            return new GeoCoordinate(viewSouth + (viewNorth - viewSouth) / 2.0f, viewWest + (viewEast - viewWest) / 2.0f);
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
			if (this.MapChanged != null) 
			{ 
				OsmSharp.Logging.Log.TraceEvent("Map.layer_LayerChanged (Before)", Logging.TraceEventType.Information, 
					"RaiseLayerChanged");
				this.MapChanged(); 
				OsmSharp.Logging.Log.TraceEvent("Map.layer_LayerChanged (After)", Logging.TraceEventType.Information, 
					"RaiseLayerChanged");
			}
        }

        #region Layer Helpers

        /// <summary>
        /// Adds a new tile layer.
        /// </summary>
        /// <param name="tileUrl">The tile URL.</param>
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

        /// <summary>
        /// Pauses all activity in this map and it's layers.
        /// </summary>
        public void Pause()
        {
            // remove and close all layers.
            lock (_layers)
            {
                var layers = new List<Layer>(_layers);
                foreach (var layer in layers)
                {
                    layer.Pause();
                }
            }
        }

        /// <summary>
        /// Pauses all activity in this map and it's layers.
        /// </summary>
        public void Resume()
        {
            // remove and close all layers.
            lock (_layers)
            {
                var layers = new List<Layer>(_layers);
                foreach (var layer in layers)
                {
                    layer.Resume();
                }
            }
        }

        /// <summary>
        /// Closes this map and all it's layers.
        /// </summary>
        /// <remarks>If you want to reuse a layer, please remove it before closing the map.</remarks>
        public void Close()
        {
            // make sure not more changes are triggered.
            this.MapChanged = null;

            // remove and close all layers.
            lock (_layers)
            {
                var layers = new List<Layer>(_layers);
                foreach(var layer in layers)
                {
                    layer.Close();
                    this.RemoveLayer(layer);
                }
            }
        }
    }
}