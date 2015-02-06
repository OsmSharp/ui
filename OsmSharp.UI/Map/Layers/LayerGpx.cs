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

using System.IO;
using OsmSharp.Geo.Geometries;
using OsmSharp.Geo.Streams.Gpx;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene;
using System.Collections.Generic;
using OsmSharp.UI.Renderer.Primitives;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A layer drawing GPX data.
    /// </summary>
    public class LayerGpx : Layer
    {
        /// <summary>
        /// Holds the projection.
        /// </summary>
        private readonly IProjection _projection;

        /// <summary>
        /// Creates a new OSM data layer.
        /// </summary>
        /// <param name="projection"></param>
        public LayerGpx(IProjection projection)
        {			
            _projection = projection;

            _scene = new Scene2D(projection, 16);
        }

        /// <summary>
        /// Called when the view on the map containing this layer has changed.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        /// <param name="extraView"></param>
        protected internal override void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view, View2D extraView)
        {
            // all data is pre-loaded for now.

            // when displaying huge amounts of GPX-data use another approach.
        }

        /// <summary>
        /// Returns all object in this layer that are visible for the given parameters.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        protected internal override IEnumerable<Primitive2D> Get(float zoomFactor, View2D view)
        {
            return _scene.Get(view, zoomFactor);
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
        /// Adds a new GPX.
        /// </summary>
        /// <param name="stream">Stream.</param>
        public GeoCoordinateBox AddGpx(Stream stream)
        {
            GeoCoordinateBox bounds = null;
            var gpxStream = new GpxFeatureStreamSource(stream);
            foreach (var feature in gpxStream)
            {
                if (feature.Geometry is Point)
                { // add the point.
                    var point = (feature.Geometry as Point);

                    // get x/y.
                    var x = _projection.LongitudeToX(point.Coordinate.Longitude);
                    var y = _projection.LatitudeToY(point.Coordinate.Latitude);

                    // set the default color if none is given.
                    var blue = SimpleColor.FromKnownColor(KnownColor.Blue);
                    var transparantBlue = SimpleColor.FromArgb(128,
                                                      blue.R, blue.G, blue.B);

                    uint pointId = _scene.AddPoint(x, y);
                    _scene.AddStylePoint(pointId, 0, float.MinValue, float.MaxValue, transparantBlue.Value, 8);

                    if (bounds == null)
                    { // create box.
                        bounds = point.Box;
                    }
                    else
                    { // add to the current box.
                        bounds = bounds + point.Box;
                    }
                }
                else if (feature.Geometry is LineString)
                { // add the lineString.
                    var lineString = (feature.Geometry as LineString);

                    // get x/y.
                    var x = new double[lineString.Coordinates.Count];
                    var y = new double[lineString.Coordinates.Count];
                    for (int idx = 0; idx < lineString.Coordinates.Count; idx++)
                    {
                        x[idx] = _projection.LongitudeToX(
                            lineString.Coordinates[idx].Longitude);
                        y[idx] = _projection.LatitudeToY(
                            lineString.Coordinates[idx].Latitude);
                    }

                    // set the default color if none is given.
                    var blue = SimpleColor.FromKnownColor(KnownColor.Blue);
                    var transparantBlue = SimpleColor.FromArgb(128,
                                                      blue.R, blue.G, blue.B);

                    uint? pointsId = _scene.AddPoints(x, y);
                    if (pointsId.HasValue)
                    {
                        _scene.AddStyleLine(pointsId.Value, 0, float.MinValue, float.MaxValue, transparantBlue.Value, 8, Renderer.Primitives.LineJoin.Round, null);

                        if (bounds == null)
                        { // create box.
                            bounds = lineString.Box;
                        }
                        else
                        { // add to the current box.
                            bounds = bounds + lineString.Box;
                        }
                    }
                }
            }

            // update envelope.
            if (_envelope == null)
            { // create initial envelope.
                _envelope = bounds;
            }
            if (bounds != null)
            {  // also include the current bounds.
                _envelope = _envelope + bounds;
            }

            return bounds;
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
        /// Closes this layer.
        /// </summary>
        public override void Close()
        {
            base.Close();

            // nothing to stop, even better!
        }
    }
}