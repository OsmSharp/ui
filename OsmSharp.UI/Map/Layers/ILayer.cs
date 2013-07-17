using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.UI.Renderer;
using OsmSharp.Math.Geo;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// An abstract representation of a layer.
    /// </summary>
    public interface ILayer
    {
        /// <summary>
        /// The minimum zoom.
        /// </summary>
        /// <remarks>
        /// The minimum zoom is the 'highest'.
        /// </remarks>
        float? MinZoom { get; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        /// <remarks>
        /// The maximum zoom is the 'lowest' or most detailed view.
        /// </remarks>
        float? MaxZoom { get; }

        /// <summary>
        /// Gets the scene.
        /// </summary>
        Scene2D Scene { get; }

        /// <summary>
        /// Called when the view on the map has changed.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view);

        /// <summary>
        /// An event raised when the content of this layer has changed.
        /// </summary>
        event OsmSharp.UI.Map.Map.LayerChanged LayerChanged;
    }
}