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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.Units.Angle;

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
		/// Render the specified target, projection, layers, zoomFactor and coordinate.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="projection">Projection.</param>
		/// <param name="layers">Layers.</param>
		/// <param name="view">View</param>
		public bool Render(TTarget target, List<ILayer> layers, View2D view)
		{	
			// create the view for this control.
			Target2DWrapper<TTarget> target2DWrapper = _renderer.CreateTarget2DWrapper(target);
			
			// draw all layers seperatly but in the correct order.
			var scenes = new List<Scene2D>();
			for (int layerIdx = 0; layerIdx < layers.Count; layerIdx++)
			{
				// get the layer.
				scenes.Add(layers[layerIdx].Scene);
			}
			
			// render the scenes.
			return _renderer.Render(target, scenes, view);
		}

        /// <summary>
        /// Renders the given map on the target using the given view.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="map"></param>
        /// <param name="view"></param>
        public bool Render(TTarget target, Map map, View2D view)
        {
            // draw all layers seperatly but in the correct order.
            var scenes = new List<Scene2D>();
            for (int layerIdx = 0; layerIdx < map.LayerCount; layerIdx++)
            {
                // get the layer.
                scenes.Add(map[layerIdx].Scene);
            }

            // render the scenes.
            return _renderer.Render(target, scenes, view);
        }

        /// <summary>
        /// Renders the given map using the given zoom factor and center.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="map"></param>
        /// <param name="view"></param>
        public void RenderCache(TTarget target, Map map, View2D view)
        {
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
        /// <param name="xInverted">True when the x-axis on the target is inverted (right->left).</param>
        /// <param name="yInverted">True when the y-axis on the target is inverted (top->bottom).</param>
        /// <returns></returns>
        public View2D Create(float width, float height, Map map, float zoomFactor, GeoCoordinate center, bool xInverted, bool yInverted)
        {
            return this.Create(width, height, map, zoomFactor, center, xInverted, yInverted);
        }

        /// <summary>
        /// Creates a view.
        /// </summary>
        /// <param name="height"></param>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="angle"></param>
        /// <param name="xInverted">True when the x-axis on the target is inverted (right->left).</param>
        /// <param name="yInverted">True when the y-axis on the target is inverted (top->bottom).</param>
        /// <returns></returns>
        public View2D Create(float width, float height, Map map, float zoomFactor, GeoCoordinate center, bool xInverted, bool yInverted, Degree angle)
        {
            // get the projection.
            IProjection projection = map.Projection;

            // calculate the center/zoom in scene coordinates.
            double[] sceneCenter = projection.ToPixel(center.Latitude, center.Longitude);
            float sceneZoomFactor = zoomFactor;

            // inversion flags for view: only invert when different.
            bool invertX = xInverted && (xInverted != !projection.DirectionX);
            bool invertY = yInverted && (yInverted != !projection.DirectionY);

            // create the view for this control.
            return View2D.CreateFrom((float)sceneCenter[0], (float)sceneCenter[1],
                                             width, height, sceneZoomFactor,
                                             invertX, invertY, angle);
        }

		/// <summary>
		/// Holds the scene renderer.
		/// </summary>
		/// <value>The scene renderer.</value>
		public Renderer2D<TTarget> SceneRenderer
		{
			get{
				return _renderer;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is running.
		/// </summary>
		/// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
		public bool IsRunning
		{
			get{
				return _renderer.IsRunning;
			}
		}
	
		/// <summary>
		/// Cancels the current run.
		/// </summary>
		/// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
		public void Cancel()
		{
			_renderer.Cancel ();
		}

		/// <summary>
		/// Cancels the current run and waits.
		/// </summary>
		/// <returns><c>true</c> if this instance cancel and wait; otherwise, <c>false</c>.</returns>
		public void CancelAndWait()
		{
			_renderer.CancelAndWait ();
		}
    }
}