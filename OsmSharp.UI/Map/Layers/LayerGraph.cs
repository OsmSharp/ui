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
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.UI.Map.Styles;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A layer drawing OSM data.
    /// </summary>
    public class LayerGraph : Layer
    {
        /// <summary>
        /// Holds the source of the OSM raw data.
        /// </summary>
        private readonly IRoutingAlgorithmData<Edge> _dataSource;
        /// <summary>
        /// Holds the style interpreter.
        /// </summary>
        private readonly StyleInterpreter _styleInterpreter;
        /// <summary>
        /// Holds the scene.
        /// </summary>
        private readonly Scene2D _scene;

        /// <summary>
        /// Creates a new OSM data layer.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="styleInterpreter"></param>
        public LayerGraph(IRoutingAlgorithmData<Edge> dataSource, 
                                         StyleInterpreter styleInterpreter)
        {
            _dataSource = dataSource;
            _styleInterpreter = styleInterpreter;

            _scene = new Scene2D(new OsmSharp.Math.Geo.Projections.WebMercator(), 16);
            _interpretedObjects = new Dictionary<int, HashSet<ArcId>>();
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
            _interpretedObjects.Clear();
            _requestedBoxes.Clear();

            this.BuildScene(map, zoomFactor, center, extraView);
        }

        #region Scene Building

        /// <summary>
        /// Holds al id's of all already interpreted objects.
        /// </summary>
        private readonly Dictionary<int, HashSet<ArcId>> _interpretedObjects;
        /// <summary>
        /// Holds all previously requested boxes.
        /// </summary>
        private HashSet<GeoCoordinateBox> _requestedBoxes = new HashSet<GeoCoordinateBox>();

        /// <summary>
        /// Builds the scene.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        private void BuildScene(Map map, float zoomFactor, GeoCoordinate center, View2D view)
        {
            // get the indexed object at this zoom.
            HashSet<ArcId> interpretedObjects;
            if (!_interpretedObjects.TryGetValue((int)zoomFactor, out interpretedObjects))
            {
                interpretedObjects = new HashSet<ArcId>();
                _interpretedObjects.Add((int)zoomFactor, interpretedObjects);
            }

            // build the boundingbox.
            var viewBox = view.OuterBox;
            var box = new GeoCoordinateBox(map.Projection.ToGeoCoordinates(viewBox.Min[0], viewBox.Min[1]),
                 map.Projection.ToGeoCoordinates(viewBox.Max[0], viewBox.Max[1]));
            foreach (var requestedBox in _requestedBoxes)
            {
                if (requestedBox.Contains(box))
                {
                    return;
                }
            }
            _requestedBoxes.Add(box);

            //// set the scene backcolor.
            //SimpleColor? color = _styleInterpreter.GetCanvasColor ();
            //_scene.BackColor = color.HasValue
            //                               ? color.Value.Value
            //                               : SimpleColor.FromArgb (0, 255, 255, 255).Value;

            // get data.
            foreach (var arc in _dataSource.GetEdges(box))
            {
                // translate each object into scene object.
                var arcId = new ArcId()
                {
                    Vertex1 = arc.Vertex1,
                    Vertex2 = arc.Vertex2
                };
                if (!interpretedObjects.Contains(arcId))
                {
                    interpretedObjects.Add(arcId);

                    // create nodes.
                    float latitude, longitude;
                    _dataSource.GetVertex(arcId.Vertex1, out latitude, out longitude);
                    var node1 = new Node();
                    node1.Id = arcId.Vertex1;
                    node1.Latitude = latitude;
                    node1.Longitude = longitude;
                    _dataSource.GetVertex(arcId.Vertex2, out latitude, out longitude);
                    var node2 = new Node();
                    node2.Id = arcId.Vertex2;
                    node2.Latitude = latitude;
                    node2.Longitude = longitude;

                    // create way.
                    var way = CompleteWay.Create(-1);
                    if (arc.EdgeData.Forward)
                    {
                        way.Nodes.Add(node1);
                        way.Nodes.Add(node2);
                    }
                    else
                    {
                        way.Nodes.Add(node2);
                        way.Nodes.Add(node1);
                    }
                    way.Tags.AddOrReplace(_dataSource.TagsIndex.Get(arc.EdgeData.Tags));

                    _styleInterpreter.Translate(_scene, map.Projection, way);
                    interpretedObjects.Add(arcId);
                }
            }
        }

        private struct ArcId
        {
            public uint Vertex1 { get; set; }

            public uint Vertex2 { get; set; }

            public override int GetHashCode()
            {
                return (this.Vertex1 + this.Vertex2).GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is ArcId)
                {
                    var other = (ArcId)(obj);
                    return (other.Vertex1 == this.Vertex2 &&
                    other.Vertex2 == this.Vertex1) ||
                    (other.Vertex1 == this.Vertex1 &&
                    other.Vertex2 == this.Vertex2);
                }
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Returns all primitives in the given mapview.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        protected internal override IEnumerable<Renderer.Primitives.Primitive2D> Get(float zoomFactor, View2D view)
        {
            throw new System.NotImplementedException();
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
