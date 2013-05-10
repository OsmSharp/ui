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
        /// <param name="target"></param>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        public void Render(TTarget target, Map map, float zoomFactor, GeoCoordinate center)
        {
            // get the projection.
            IProjection projection = map.Projection;

            // calculate the center/zoom in scene coordinates.
            double[] sceneCenter = projection.ToPixel(center.Latitude, center.Longitude);
            float sceneZoomFactor = zoomFactor; // TODO: find out the conversion rate and see if this is related to the projection?

            // create the view for this control.
            Target2DWrapper<TTarget> target2DWrapper = _renderer.CreateTarget2DWrapper(target);
            View2D view = View2D.CreateFrom((float)sceneCenter[0], (float)sceneCenter[1],
                                            target2DWrapper.Width, target2DWrapper.Height, sceneZoomFactor);

            // draw all layers seperatly but in the correct order.
            for (int layerIdx = 0; layerIdx < map.LayerCount; layerIdx++)
            {
                // get the layer.
                ILayer layer = map[layerIdx];

                // render the scene for this layer.
                // TODO: only render the layers with the correct min/max zooms.
                _renderer.Render(target, layer.Scene, view);
            }
        }

        /// <summary>
        /// Renders the given map using the given zoom factor and center.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        public void RenderCache(TTarget target, Map map, float zoomFactor, GeoCoordinate center)
        {
            // get the projection.
            IProjection projection = map.Projection;

            // calculate the center/zoom in scene coordinates.
            double[] sceneCenter = projection.ToPixel(center.Latitude, center.Longitude);
            float sceneZoomFactor = zoomFactor; // TODO: find out the conversion rate and see if this is related to the projection?

            // create the view for this control.
            Target2DWrapper<TTarget> target2DWrapper = _renderer.CreateTarget2DWrapper(target);
            View2D view = View2D.CreateFrom((float)sceneCenter[0], (float)sceneCenter[1],
                                             target2DWrapper.Width, target2DWrapper.Height, sceneZoomFactor);

            // draw all layers seperatly but in the correct order.
            for (int layerIdx = 0; layerIdx < map.LayerCount; layerIdx++)
            {
                // get the layer.
                ILayer layer = map[layerIdx];

                // render the scene for this layer.
                _renderer.RenderCache(target, view);
            }
        }

        /// <summary>
        /// Creates a view.
        /// </summary>
        /// <param name="height"></param>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public View2D Create(float width, float height, Map map, float zoomFactor, GeoCoordinate center)
        {
            // get the projection.
            IProjection projection = map.Projection;

            // calculate the center/zoom in scene coordinates.
            double[] sceneCenter = projection.ToPixel(center.Latitude, center.Longitude);
            float sceneZoomFactor = zoomFactor; // TODO: find out the conversion rate and see if this is related to the projection?

            // create the view for this control.
            return View2D.CreateFrom((float)sceneCenter[0], (float)sceneCenter[1],
                                             width, height, sceneZoomFactor);
        }
    }
}