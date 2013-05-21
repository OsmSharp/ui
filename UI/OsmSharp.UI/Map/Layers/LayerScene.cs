using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Data;
using OsmSharp.Math.Geo;
using OsmSharp.UI.Map.Styles;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene2DPrimitives;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A layer drawing scene objects.
    /// </summary>
    public class LayerScene : ILayer
    {
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
            this.Scene = new Scene2D();
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
        public Scene2D Scene { get; private set; }

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
            // reset the scene.
            this.Scene = new Scene2D();

            // get from the index.
            this.Scene.BackColor = SimpleColor.FromKnownColor(KnownColor.White).Value;
            _index.Get(this.Scene, view, zoomFactor);
        }

        #endregion
    }
}