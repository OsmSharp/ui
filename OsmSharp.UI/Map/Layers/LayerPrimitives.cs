// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.UI.Renderer.Scene;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A layer containing several simple primitives.
    /// </summary>
    public class LayerPrimitives : Layer
    {
        private readonly IProjection _projection; // Holds the projection for this layer.
        private readonly Scene2D _scene; // Holds the scene.

        /// <summary>
        /// Creates a new primitives layer.
        /// </summary>
        public LayerPrimitives(IProjection projection)
        {
            _projection = projection;

            _scene = new Scene2D(projection, 16);
        }

        /// <summary>
        /// Holds the envelope of the current data.
        /// </summary>
        private GeoCoordinateBox _envelope;

        /// <summary>
        /// Adds a point.
        /// </summary>
        public void AddPoint(GeoCoordinate coordinate, float sizePixels, int color)
        {
            if (coordinate == null) { throw new ArgumentNullException(); }

            // update envelope.
            if (_envelope == null)
            { // create initial envelope.
                _envelope = new GeoCoordinateBox(coordinate, coordinate);
            }
            // also include the current point.
            _envelope.ExpandWith(coordinate);

            double[] projectedCoordinates = _projection.ToPixel(coordinate);
            uint pointId = _scene.AddPoint(projectedCoordinates[0], projectedCoordinates[1]);
            _scene.AddStylePoint(pointId, 0, float.MinValue, float.MaxValue, color, sizePixels);
            this.RaiseLayerChanged();
        }

        /// <summary>
        /// Adds a line.
        /// </summary>
        public void AddLine(GeoCoordinate point1, GeoCoordinate point2, float sizePixels, int color)
        {
            if (point1 == null) { throw new ArgumentNullException(); }
            if (point2 == null) { throw new ArgumentNullException(); }

            // update envelope.
            if (_envelope == null)
            { // create initial envelope.
                _envelope = new GeoCoordinateBox(point1, point2);
            }
            // also include the current point.
            _envelope.ExpandWith(point1);
            _envelope.ExpandWith(point2);

            var projected1 = _projection.ToPixel(point1);
            var projected2 = _projection.ToPixel(point2);

            var x = new double[] { projected1[0], projected2[0] };
            var y = new double[] { projected1[1], projected2[1] };

            uint? pointsId = _scene.AddPoints(x, y);
            if (pointsId.HasValue)
            {
                _scene.AddStyleLine(pointsId.Value, 0, float.MinValue, float.MaxValue, color, sizePixels, Renderer.Primitives.LineJoin.Round, null);
                this.RaiseLayerChanged();
            }
        }

        /// <summary>
        /// Adds a polyline.
        /// </summary>
        public void AddPolyline(GeoCoordinate[] points, float sizePixels, int color)
        {
            if (points == null) { throw new ArgumentNullException(); }

            var x = new double[points.Length];
            var y = new double[points.Length];
            for(int idx = 0; idx < points.Length; idx++)
            {
                // update envelope.
                if (_envelope == null)
                { // create initial envelope.
                    _envelope = new GeoCoordinateBox(points[idx], points[idx]);
                }
                // also include the current point.
                _envelope.ExpandWith(points[idx]);

                var projected =_projection.ToPixel(points[idx]);
                x[idx] = projected[0];
                y[idx] = projected[1];
            }

            uint? pointsId = _scene.AddPoints(x, y);
            if (pointsId.HasValue)
            {
                _scene.AddStyleLine(pointsId.Value, 0, float.MinValue, float.MaxValue, color, sizePixels, Renderer.Primitives.LineJoin.Round, null);
                this.RaiseLayerChanged();
            }
        }

        /// <summary>
        /// Adds a polygon.
        /// </summary>
        public void AddPolygon(GeoCoordinate[] points, int color, float width, bool fill)
        {
            if (points == null) { throw new ArgumentNullException(); }

            var x = new double[points.Length];
            var y = new double[points.Length];
            for (int idx = 0; idx < points.Length; idx++)
            {
                // update envelope.
                if (_envelope == null)
                { // create initial envelope.
                    _envelope = new GeoCoordinateBox(points[idx], points[idx]);
                }
                // also include the current point.
                _envelope.ExpandWith(points[idx]);

                var projected = _projection.ToPixel(points[idx]);
                x[idx] = projected[0];
                y[idx] = projected[1];
            }

            var pointsId = _scene.AddPoints(x, y);
            if (pointsId.HasValue)
            {
                _scene.AddStylePolygon(pointsId.Value, 0, float.MinValue, float.MaxValue, color, width, fill);
                this.RaiseLayerChanged();
            }
        }

        /// <summary>
        /// Adds all features in the given feature collection.
        /// </summary>
        public void AddFeatures(FeatureCollection features, int color, float width, bool fill)
        {
            if (features == null) { throw new ArgumentNullException(); }

            foreach (var feature in features)
            {
                this.AddFeature(feature, color, width, fill);
            }
        }

        /// <summary>
        /// Adds the given feature.
        /// </summary>
        public void AddFeature(Feature feature, int color, float width, bool fill)
        {
            if (feature == null) { throw new ArgumentNullException(); }

            this.AddGeometry(feature.Geometry, color, width, fill);
        }

        /// <summary>
        /// Adds the given geometry.
        /// </summary>
        private void AddGeometry(Geometry geometry, int color, float width, bool fill)
        {
            if (geometry == null) { throw new ArgumentNullException(); }

            if (geometry is Point)
            {
                this.AddGeometry(geometry as Point, color, width);
            }
            else if (geometry is Polygon)
            {
                this.AddGeometry(geometry as Polygon, color, width, fill);
            }
            else if (geometry is LineString)
            {
                this.AddGeometry(geometry as LineString, color, width);
            }
            else if (geometry is LineairRing)
            {
                this.AddGeometry(geometry as LineairRing, color, width, fill);
            }
        }

        /// <summary>
        /// Adds a point.
        /// </summary>
        private void AddGeometry(Point geometry, int color, float size)
        {
            if (geometry == null) { throw new ArgumentNullException(); }

            this.AddPoint(geometry.Coordinate, size, color);
        }

        /// <summary>
        /// Adds a polygon.
        /// </summary>
        private void AddGeometry(Polygon geometry, int color, float width, bool fill)
        {
            if (geometry == null) { throw new ArgumentNullException(); }

            if(geometry.Holes != null &&
               geometry.Holes.Count() > 0)
            {
                OsmSharp.Logging.Log.TraceEvent("LayerPrimitive.AddGeometry", Logging.TraceEventType.Warning,
                    "Polygons with holes are not supported, only showing outerring.");
            }
            this.AddPolygon(geometry.Ring.Coordinates.ToArray(), color, width, fill);
        }

        /// <summary>
        /// Adds a linestring.
        /// </summary>
        private void AddGeometry(LineString geometry, int color, float width)
        {
            if (geometry == null) { throw new ArgumentNullException(); }

            this.AddPolyline(geometry.Coordinates.ToArray(), width, color);
        }

        /// <summary>
        /// Adds a ring.
        /// </summary>
        private void AddGeometry(LineairRing geometry, int color, float width, bool fill)
        {
            if (geometry == null) { throw new ArgumentNullException(); }

            this.AddPolygon(geometry.Coordinates.ToArray(), color, width, fill);
        }

        /// <summary>
        /// Clears all data from this layer.
        /// </summary>
        public void Clear()
        {
            _scene.Clear();
        }

        /// <summary>
        /// Called when the view on the map has changed.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        /// <param name="extraView"></param>
        protected internal override void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view, View2D extraView)
        {
            // all data is preloaded for now.

            // when displaying huge amounts of GPX-data use another approach.
        }

        /// <summary>
        /// Returns all the object from this layer visible for the given parameters.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        protected internal override IEnumerable<Primitive2D> Get(float zoomFactor, View2D view)
        {
            return _scene.Get(view, zoomFactor);
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