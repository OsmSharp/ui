// OsmSharp - OpenStreetMap tools & library.
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
using OsmSharp.Routing.Route;
using OsmSharp.UI.Renderer;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Renderer.Scene;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A layer drawing OsmSharpRoute layer data.
    /// </summary>
    public class LayerOsmSharpRoute : ILayer
    {
        /// <summary>
        /// Holds the projection.
        /// </summary>
        private readonly IProjection _projection;

        /// <summary>
        /// Creates a new OsmSharpRoute layer.
        /// </summary>
        /// <param name="projection"></param>
        public LayerOsmSharpRoute(IProjection projection)
        {
            _projection = projection;

            this.Scene = new Scene2DSimple();
            this.Scene.BackColor = 
                SimpleColor.FromKnownColor(KnownColor.Transparent).Value;
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
            // all data is pre-loaded for now.

            // when displaying huge amounts of GPX-data use another approach.
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
        /// Adds a new OsmSharpRoute.
        /// </summary>
        /// <param name="route">Stream.</param>
        public void AddRoute(OsmSharpRoute route)
		{
			// set the default color if none is given.
			SimpleColor blue = SimpleColor.FromKnownColor (KnownColor.Blue);
			SimpleColor transparantBlue = SimpleColor.FromArgb (128, blue.R, blue.G, blue.B);
			this.AddRoute (route, transparantBlue.Value);
		}

		/// <summary>
		/// Adds a new OsmSharpRoute.
		/// </summary>
		/// <param name="route">Stream.</param>
		/// <param name="argb">Stream.</param>
		public void AddRoute(OsmSharpRoute route, int argb)
		{
			if (route.Entries != null && route.Entries.Length > 0)
			{ // there are entries.
				// get x/y.
				var x = new double[route.Entries.Length];
				var y = new double[route.Entries.Length];
				for (int idx = 0; idx < route.Entries.Length; idx++)
				{
					x[idx] = _projection.LongitudeToX(
						route.Entries[idx].Longitude);
					y[idx] = _projection.LatitudeToY(
						route.Entries[idx].Latitude);
				}

				// set the default color if none is given.
				SimpleColor blue = new SimpleColor () {
					Value = argb
				};
				SimpleColor color = SimpleColor.FromArgb (argb);
				this.Scene.AddLine(float.MinValue, float.MaxValue, x, y,
				                   color.Value, 4);
			}
		}

        #endregion

        /// <summary>
        /// Removes all objects from this layer.
        /// </summary>
        public void Clear()
        {
			this.Scene.Clear();
        }
    }
}