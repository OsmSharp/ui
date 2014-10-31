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
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Routing;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.Math.Geo;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A layer drawing OsmSharpRoute layer data.
    /// </summary>
    public class LayerRoute : Layer
    {
        /// <summary>
        /// Holds the projection.
        /// </summary>
        private readonly IProjection _projection;

        /// <summary>
        /// Creates a new OsmSharpRoute layer.
        /// </summary>
        /// <param name="projection"></param>
        public LayerRoute(IProjection projection)
        {
            _projection = projection;

            _scene = new Scene2D(projection, 16);
        }

        #region Scene Building

        /// <summary>
        /// Holds the envelope of the current data.
        /// </summary>
        private GeoCoordinateBox _envelope;

        /// <summary>
        /// Holds the scene.
        /// </summary>
        private Scene2D _scene;

        /// <summary>
        /// Adds a new OsmSharpRoute.
        /// </summary>
        /// <param name="route">Stream.</param>
        public void AddRoute(Route route)
        {
            // set the default color if none is given.
            var blue = SimpleColor.FromKnownColor(KnownColor.Blue);
            var transparantBlue = SimpleColor.FromArgb(128, blue.R, blue.G, blue.B);
            this.AddRoute(route, transparantBlue.Value);
        }

        /// <summary>
        /// Adds a new OsmSharpRoute.
        /// </summary>
        /// <param name="route">Route.</param>
        /// <param name="argb">ARGB.</param>
        public void AddRoute(Route route, int argb)
        {
            this.AddRoute(route, argb, 8);
        }

        /// <summary>
        /// Adds a new OsmSharpRoute.
        /// </summary>
        /// <param name="route">Stream.</param>
        /// <param name="argb">Stream.</param>
        /// <param name="width"></param>
        public void AddRoute(Route route, int argb, double width)
        {
            if (route != null &&
                route.Segments != null &&
                route.Segments.Length > 0)
            { // there are entries.
                // get x/y.
                var x = new double[route.Segments.Length];
                var y = new double[route.Segments.Length];
                for (int idx = 0; idx < route.Segments.Length; idx++)
                {
                    x[idx] = _projection.LongitudeToX(
                        route.Segments[idx].Longitude);
                    y[idx] = _projection.LatitudeToY(
                        route.Segments[idx].Latitude);

                    // update envelope.
                    if(_envelope == null)
                    { // create initial envelope.
                        _envelope = new GeoCoordinateBox(
                            new GeoCoordinate(route.Segments[idx].Latitude, route.Segments[idx].Longitude),
                            new GeoCoordinate(route.Segments[idx].Latitude, route.Segments[idx].Longitude));
                    }
                    // also include the current point.
                    _envelope.ExpandWith(new GeoCoordinate(route.Segments[idx].Latitude, route.Segments[idx].Longitude));
                }

                // set the default color if none is given.
                var color = SimpleColor.FromArgb(argb);
                var pointsId = _scene.AddPoints(x, y);
                if (pointsId.HasValue)
                {
                    _scene.AddStyleLine(pointsId.Value, 0, float.MinValue, float.MaxValue, 
                        color.Value, (float)width, Renderer.Primitives.LineJoin.Round, null);
                }
            }

            this.RaiseLayerChanged();
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
                return _envelope;
            }
        }

        /// <summary>
        /// Returns all objects in this layer visible for the given parameters.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        protected internal override IEnumerable<Primitive2D> Get(float zoomFactor, View2D view)
        {
            return _scene.Get(view, zoomFactor);
        }

        /// <summary>
        /// Removes all objects from this layer.
        /// </summary>
        public void Clear()
        {
            _scene.Clear();

            this.RaiseLayerChanged();
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