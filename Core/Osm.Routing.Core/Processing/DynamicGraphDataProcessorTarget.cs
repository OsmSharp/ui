// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using Osm.Core.Simple;
using Osm.Data.Core.Processor;
using Osm.Data.Core.DynamicGraph;
using Osm.Routing.Core.Roads;
using Tools.Math.Geo;
using Osm.Routing.Core.Roads.Tags;

namespace Osm.Routing.Core.Processor
{
    /// <summary>
    /// Data Processor Target to fill a dynamic graph object.
    /// </summary>
    public abstract class DynamicGraphDataProcessorTarget<EdgeData> : DataProcessorTarget
        where EdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the dynamic graph.
        /// </summary>
        private IDynamicGraph<EdgeData> _dynamic_graph;

        /// <summary>
        /// Creates a new processor target.
        /// </summary>
        /// <param name="dynamic_graph">The graph that will be filled.</param>
        public DynamicGraphDataProcessorTarget(IDynamicGraph<EdgeData> dynamic_graph)
        {
            _dynamic_graph = dynamic_graph;
        }

        /// <summary>
        /// Holds the coordinates.
        /// </summary>
        private Dictionary<long, GeoCoordinate> _coordinates;

        /// <summary>
        /// Holds the id transformations.
        /// </summary>
        private Dictionary<long, uint> _id_transformations;

        /// <summary>
        /// Initializes the processing.
        /// </summary>
        public override void Initialize()
        {
            _coordinates = new Dictionary<long, GeoCoordinate>();
            _id_transformations = new Dictionary<long, uint>();
        }

        /// <summary>
        /// Applies the changes in the changeset.
        /// </summary>
        /// <param name="change"></param>
        public override void ApplyChange(SimpleChangeSet change)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Adds the given node.
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(SimpleNode node)
        {
            // save the node-coordinates.
            _coordinates[node.Id.Value] = 
                new GeoCoordinate(node.Latitude.Value, node.Longitude.Value);
        }

        /// <summary>
        /// Adds a given way.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(SimpleWay way)
        {
            // initialize the way interpreter.
            Roads.Tags.RoadTagsInterpreterBase interpreter = new Roads.Tags.RoadTagsInterpreterBase(
                way.Tags);
            if (interpreter.IsRoad())
            { // the way is a road.
                // add the forward edges.
                if (!interpreter.IsOneWayReverse())
                { // loop over all edges.
                    if (way.Nodes.Count > 1)
                    { // way has at least two nodes.
                        uint? from = this.AddRoadNode(way.Nodes[0]);
                        for (int idx = 1; idx < way.Nodes.Count; idx++)
                        { // the to-node.
                            uint? to = this.AddRoadNode(way.Nodes[idx]);

                            // add the edge(s).
                            if (from.HasValue && to.HasValue)
                            { // 
                                this.AddRoadEdge(from.Value, to.Value,
                                    interpreter);
                            }

                            from = to; // the to node becomes the from.
                        }
                    }
                }

                // add the backward edges.
                if (!interpreter.IsOneWay())
                { // loop over all edges.
                    if (way.Nodes.Count > 1)
                    { // way has at least two nodes.
                        uint? from = this.AddRoadNode(way.Nodes[way.Nodes.Count - 1]);
                        for (int idx = way.Nodes.Count - 2; idx >= 0; idx--)
                        { // the to-node.
                            uint? to = this.AddRoadNode(way.Nodes[idx]);

                            // add the edge(s).
                            if (from.HasValue && to.HasValue)
                            { // 
                                this.AddRoadEdge(from.Value, to.Value, 
                                    interpreter);
                            }

                            from = to; // the to node becomes the from.
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a node that is at least part of one road.
        /// </summary>
        /// <param name="node_id"></param>
        /// <returns></returns>
        private uint? AddRoadNode(long node_id)
        {
            uint id;
            // try and get existing node.
            if (!_id_transformations.TryGetValue(node_id, out id))
            {
                // get coordinates.
                GeoCoordinate coordinates;
                if (_coordinates.TryGetValue(node_id, out coordinates))
                { // the coordinate is present.
                    id = _dynamic_graph.AddVertex(
                        (float)coordinates.Latitude, (float)coordinates.Longitude);
                    _id_transformations[node_id] = id;
                    return id;
                }
                return null;
            }
            return id;
        }

        /// <summary>
        /// Adds an edge.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="interpreter"></param>
        private void AddRoadEdge(
            uint from, uint to,
            RoadTagsInterpreterBase interpreter)
        {
            float latitude;
            float longitude;
            GeoCoordinate from_coordinate = null;
            if (_dynamic_graph.GetVertex(from, out latitude, out longitude))
            { // 
                from_coordinate = new GeoCoordinate(latitude, longitude);
            }
            GeoCoordinate to_coordinate = null;
            if (_dynamic_graph.GetVertex(to, out latitude, out longitude))
            { // 
                to_coordinate = new GeoCoordinate(latitude, longitude);
            }

            if (from_coordinate != null && to_coordinate != null)
            { // calculate the edge data.
                EdgeData edge_data = this.CalculateEdgeData(from_coordinate, to_coordinate,
                    interpreter);

                _dynamic_graph.AddArc(from, to, edge_data);
            }
        }

        /// <summary>
        /// Calculates the edge data.
        /// </summary>
        /// <returns></returns>
        protected abstract EdgeData CalculateEdgeData(GeoCoordinate from, GeoCoordinate to,
            RoadTagsInterpreterBase interpreter);

        /// <summary>
        /// Adds a given relation.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(SimpleRelation relation)
        {

        }
    }
}
