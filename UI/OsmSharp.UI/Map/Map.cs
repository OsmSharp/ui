using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Renderer;

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
        private readonly IList<ILayer> _layers;

        /// <summary>
        /// Creates a new map.
        /// </summary>
        public Map()
        {
            _layers = new List<ILayer>();
            this.Projection = new WebMercator();
        }

        /// <summary>
        /// Creates a new map.
        /// </summary>
        public Map(IProjection projection)
        {
            _layers = new List<ILayer>();
            this.Projection = projection;
        }

        /// <summary>
        /// A delegate for layer changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public delegate void LayerChanged(ILayer sender);

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
        public void ViewChanged(float zoomFactor, GeoCoordinate center, View2D view)
        {
            foreach (var layer in _layers)
            {
                layer.ViewChanged(this, zoomFactor, center, view);
            }
        }

        #region Layers

        /// <summary>
        /// Adds a layer on top of the existing layers.
        /// </summary>
        /// <param name="layer"></param>
        public void AddLayer(ILayer layer)
        {
            layer.LayerChanged += new LayerChanged(layer_LayerChanged);
            _layers.Add(layer);
        }

        /// <summary>
        /// Inserts a layer.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="layer"></param>
        public void InsertLayer(int idx, ILayer layer)
        {
            layer.LayerChanged += new LayerChanged(layer_LayerChanged);
            _layers.Insert(idx, layer);
        }

        /// <summary>
        /// Removes the given layer.
        /// </summary>
        /// <param name="layer"></param>
        public bool RemoveLayer(ILayer layer)
        {
            if (_layers.Remove(layer))
            {
                layer.LayerChanged -= new LayerChanged(layer_LayerChanged); // remove event handler.
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes a layer at the given index.
        /// </summary>
        /// <param name="idx"></param>
        public void RemoveLayerAt(int idx)
        {
            ILayer layer = _layers[idx];
            layer.LayerChanged -= new LayerChanged(layer_LayerChanged); // remove event handler.
            _layers.RemoveAt(idx);
        }

        /// <summary>
        /// Gets the layer at the given idx.
        /// </summary>
        /// <param name="layerIdx"></param>
        /// <returns></returns>
        public ILayer this[int layerIdx]
        {
            get { return _layers[layerIdx]; }
        }

        /// <summary>
        /// Gets the layer count.
        /// </summary>
        public int LayerCount
        {
            get { return _layers.Count; }
        }

        /// <summary>
        /// Called when a layer changed.
        /// </summary>
        /// <param name="sender"></param>
        void layer_LayerChanged(ILayer sender)
        {

        }

        #endregion
    }
}