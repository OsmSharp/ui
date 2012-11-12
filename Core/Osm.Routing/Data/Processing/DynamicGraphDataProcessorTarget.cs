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
using Routing.Core.Roads;
using Tools.Math.Geo;
using Routing.Core.Roads.Tags;
using Osm.Routing.Interpreter;
using Routing.Core.Graph;
using Tools.Math;
using Osm.Core;
using Routing.Core.Interpreter;
using Routing.Core.Graph.Router;
using Routing.Core.Interpreter.Roads;

namespace Osm.Routing.Data.Processing
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
        /// The interpreter for osm data.
        /// </summary>
        private IRoutingInterpreter _interpreter;

        /// <summary>
        /// Holds the tags index.
        /// </summary>
        private ITagsIndex _tags_index;

        /// <summary>
        /// True when this target is in pre-index mode.
        /// </summary>
        private bool _pre_index_mode;

        /// <summary>
        /// Creates a new processor target.
        /// </summary>
        /// <param name="dynamic_graph">The graph that will be filled.</param>
        /// <param name="interpreter">The interpreter to generate the edge data.</param>
        public DynamicGraphDataProcessorTarget(IDynamicGraph<EdgeData> dynamic_graph,
            IRoutingInterpreter interpreter)
            : this(dynamic_graph, interpreter, new OsmTagsIndex(), new Dictionary<long,uint>())
        {

        }

        /// <summary>
        /// Creates a new processor target.
        /// </summary>
        /// <param name="dynamic_graph">The graph that will be filled.</param>
        /// <param name="interpreter">The interpreter to generate the edge data.</param>
        public DynamicGraphDataProcessorTarget(IDynamicGraph<EdgeData> dynamic_graph,
            IRoutingInterpreter interpreter, ITagsIndex tags_index)
            :this(dynamic_graph, interpreter, tags_index, new Dictionary<long, uint>())
        {

        }

        /// <summary>
        /// Creates a new processor target.
        /// </summary>
        /// <param name="dynamic_graph">The graph that will be filled.</param>
        /// <param name="interpreter">The interpreter to generate the edge data.</param>
        public DynamicGraphDataProcessorTarget(IDynamicGraph<EdgeData> dynamic_graph,
            IRoutingInterpreter interpreter, ITagsIndex tags_index, IDictionary<long, uint> id_transformations)
        {
            _dynamic_graph = dynamic_graph;
            _interpreter = interpreter;

            _tags_index = tags_index;
            _id_transformations = id_transformations;
            _pre_index_mode = true;
            _pre_index = new HashSet<long>();
            _used_twice_or_more = new HashSet<long>();
        }

        /// <summary>
        /// Returns the tags index.
        /// </summary>
        public ITagsIndex TagsIndex
        {
            get
            {
                return _tags_index;
            }
        }

        /// <summary>
        /// Holds the coordinates.
        /// </summary>
        private Dictionary<long, float[]> _coordinates;

        /// <summary>
        /// Holds the index of all relevant nodes.
        /// </summary>
        private HashSet<long> _pre_index;

        /// <summary>
        /// Holds the id transformations.
        /// </summary>
        private IDictionary<long, uint> _id_transformations;

        /// <summary>
        /// Initializes the processing.
        /// </summary>
        public override void Initialize()
        {
            _coordinates = new Dictionary<long, float[]>();
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
            if (!_pre_index_mode)
            {
                if (_pre_index != null && _pre_index.Contains(node.Id.Value))
                { // only save the coordinates for relevant nodes.
                    // save the node-coordinates.
                    // add the relevant nodes.
                    //_id_transformations[node.Id.Value] =
                    //     _dynamic_graph.AddVertex((float)node.Latitude.Value, (float)node.Longitude.Value);
                    _coordinates[node.Id.Value] = new float[] { (float)node.Latitude.Value, (float)node.Longitude.Value };
                    if (_coordinates.Count == _pre_index.Count)
                    {
                        _pre_index.Clear();
                        _pre_index = null;
                    }
                }
            }
        }

        /// <summary>
        /// Holds a list of nodes used twice or more.
        /// </summary>
        private HashSet<long> _used_twice_or_more;

        /// <summary>
        /// Adds a given way.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(SimpleWay way)
        {
            // initialize the way interpreter.
            if (_interpreter.EdgeInterpreter.IsRoutable(way.Tags))
            { // the way is a road.
                if (_pre_index_mode)
                { // index only relevant nodes.
                    foreach(long node in way.Nodes)
                    {
                        if (_pre_index.Contains(node))
                        {
                            _used_twice_or_more.Add(node);
                        }
                        else
                        {
                            _pre_index.Add(node); // node is relevant.
                        }
                    }
                }
                else
                {
                // add the forward edges.
                //if (!interpreter.IsOneWayReverse())
                    if (true) // add backward edges too!
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
                                    if (!this.AddRoadEdge(way.Tags, true, from.Value, to.Value))
                                    {
                                        this.AddRoadEdge(way.Tags, true, to.Value, from.Value);
                                    }
                                }

                                from = to; // the to node becomes the from.
                            }
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
                float[] coordinates;
                if (_coordinates.TryGetValue(node_id, out coordinates))
                { // the coordinate is present.
                    id = _dynamic_graph.AddVertex(
                        coordinates[0], coordinates[1]);
                    _coordinates.Remove(node_id); // free the memory again!

                    if (_used_twice_or_more.Contains(node_id))
                    {
                        _id_transformations[node_id] = id;
                    }
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
        private bool AddRoadEdge(IDictionary<string, string> tags, bool forward, uint from, uint to)
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
                EdgeData edge_data = this.CalculateEdgeData(_interpreter.EdgeInterpreter, _tags_index, tags, forward, from_coordinate, to_coordinate);

                _dynamic_graph.AddArc(from, to, edge_data);

                if (edge_data.Forward && edge_data.Backward)
                {
                    _dynamic_graph.AddArc(to, from, edge_data);

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Calculates the edge data.
        /// </summary>
        /// <returns></returns>
        protected abstract EdgeData CalculateEdgeData(IEdgeInterpreter edge_interpreter, ITagsIndex tags_index, IDictionary<string, string> tags,
            bool direction_forward, GeoCoordinate from, GeoCoordinate to);

        /// <summary>
        /// Adds a given relation.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(SimpleRelation relation)
        {

        }

        public override void Close()
        {
            if (_pre_index_mode)
            {
                this.Source.Reset();
                _pre_index_mode = false;
                this.Pull();
            }
        }
    }
}
