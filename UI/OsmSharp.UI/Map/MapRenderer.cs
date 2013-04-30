using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Renderer;

namespace OsmSharp.UI.Map
{
    /// <summary>
    /// A renderer for the map object. Uses a Renderer2D to render the map.
    /// </summary>
    public class MapRenderer<TTarget>
    {
        /// <summary>
        /// The 2D renderer.
        /// </summary>
        private readonly Renderer2D<TTarget> _renderer;

		/// <summary>
        /// Initializes a new instance of the <see cref="OsmSharp.UI.Map.MapRenderer"/> class.
		/// </summary>
		/// <param name="renderer">The renderer to use.</param>
        public MapRenderer(Renderer2D<TTarget> renderer)
		{
		    _renderer = renderer;
		}

        /// <summary>
        /// Renders the given map using the given zoom factor and center.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        public void Render(Map map, float zoomFactor, GeoCoordinate center)
        {
            // get the projection.
            IProjection projection = map.Projection;

            // calculate the center/zoom in scene coordinates.
            double[] sceneCenter = projection.ToPixel(center.Latitude, center.Longitude);
            float sceneZoomFactor = zoomFactor; // TODO: find out the conversion rate and see if this is related to the projection?

            // create the view for this control.
            View2D view = View2D.CreateFrom((float)sceneCenter[0], (float)sceneCenter[1],
                                            _renderer.Width, _renderer.Height, sceneZoomFactor);

            // draw all layers seperatly but in the correct order.
            for (int layerIdx = 0; layerIdx < map.LayerCount; layerIdx++)
            {
                // get the layer.
                ILayer layer = map[layerIdx];

                // render the scene for this layer.
                _renderer.Render(layer.Scene, view);
            }
        }

        /// <summary>
        /// Creates a view.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public View2D Create(Map map, float zoomFactor, GeoCoordinate center)
        {
            // get the projection.
            IProjection projection = map.Projection;

            // calculate the center/zoom in scene coordinates.
            double[] sceneCenter = projection.ToPixel(center.Latitude, center.Longitude);
            float sceneZoomFactor = zoomFactor; // TODO: find out the conversion rate and see if this is related to the projection?

            // create the view for this control.
            return View2D.CreateFrom((float)sceneCenter[0], (float)sceneCenter[1],
                                            _renderer.Width, _renderer.Height, sceneZoomFactor);
        }
    }
}