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
using OsmSharp.Osm.Simple;
using OsmSharp.Osm.Data.Core.Processor;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Graph;
using OsmSharp.Tools.Math;
using OsmSharp.Osm;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Interpreter.Roads;
using OsmSharp.Routing.Graph.DynamicGraph;
using OsmSharp.Tools.Collections;

namespace OsmSharp.Routing.Osm.Data.Processing
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
        /// The bounding box to limit nodes if any.
        /// </summary>
        private GeoCoordinateBox _box;

        /// <summary>
        /// Holds the edge comparer.
        /// </summary>
        private IDynamicGraphEdgeComparer<EdgeData> _edge_comparer;

        /// <summary>
        /// Creates a new processor target.
        /// </summary>
        /// <param name="dynamic_graph">The graph that will be filled.</param>
        /// <param name="interpreter">The interpreter to generate the edge data.</param>
        /// <param name="edge_comparer"></param>
        public DynamicGraphDataProcessorTarget(IDynamicGraph<EdgeData> dynamic_graph,
            IRoutingInterpreter interpreter, IDynamicGraphEdgeComparer<EdgeData> edge_comparer)
            : this(dynamic_graph, interpreter, edge_comparer, new OsmTagsIndex(), new Dictionary<long, uint>())
        {

        }

        /// <summary>
        /// Creates a new processor target.
        /// </summary>
        /// <param name="dynamic_graph">The graph that will be filled.</param>
        /// <param name="interpreter">The interpreter to generate the edge data.</param>
        /// <param name="edge_comparer"></param>
        /// <param name="tags_index"></param>
        public DynamicGraphDataProcessorTarget(IDynamicGraph<EdgeData> dynamic_graph,
            IRoutingInterpreter interpreter, IDynamicGraphEdgeComparer<EdgeData> edge_comparer, ITagsIndex tags_index)
            : this(dynamic_graph, interpreter, edge_comparer, tags_index, new Dictionary<long, uint>())
        {

        }

        /// <summary>
        /// Creates a new processor target.
        /// </summary>
        /// <param name="dynamic_graph">The graph that will be filled.</param>
        /// <param name="interpreter">The interpreter to generate the edge data.</param>
        /// <param name="edge_comparer"></param>
        /// <param name="tags_index"></param>
        /// <param name="id_transformations"></param>
        public DynamicGraphDataProcessorTarget(IDynamicGraph<EdgeData> dynamic_graph,
            IRoutingInterpreter interpreter, IDynamicGraphEdgeComparer<EdgeData> edge_comparer, ITagsIndex tags_index, 
            IDictionary<long, uint> id_transformations)
            : this(dynamic_graph, interpreter, edge_comparer, tags_index, id_transformations, null)
        {

        }

        /// <summary>
        /// Creates a new processor target.
        /// </summary>
        /// <param name="dynamic_graph">The graph that will be filled.</param>
        /// <param name="interpreter">The interpreter to generate the edge data.</param>
        /// <param name="edge_comparer"></param>
        /// <param name="tags_index"></param>
        /// <param name="id_transformations"></param>
        /// <param name="box"></param>
        public DynamicGraphDataProcessorTarget(
            IDynamicGraph<EdgeData> dynamic_graph, IRoutingInterpreter interpreter, IDynamicGraphEdgeComparer<EdgeData> edge_comparer, 
            ITagsIndex tags_index, IDictionary<long, uint> id_transformations,
            GeoCoordinateBox box)
        {
            _dynamic_graph = dynamic_graph;
            _interpreter = interpreter;
            _edge_comparer = edge_comparer;
            _box = box;

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
        private OsmSharp.Tools.Collections.Huge.HugeDictionary<long, float[]> _coordinates;

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
            _coordinates = new OsmSharp.Tools.Collections.Huge.HugeDictionary<long, float[]>();
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

                    if (_box == null || _box.IsInside(new GeoCoordinate((float)node.Latitude.Value, (float)node.Longitude.Value)))
                    { // the coordinate is acceptable.
                        _coordinates[node.Id.Value] = new float[] { (float)node.Latitude.Value, (float)node.Longitude.Value };
                        if (_coordinates.Count == _pre_index.Count)
                        {
                            _pre_index.Clear();
                            _pre_index = null;
                        }
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

                                if (this.CalculateIsTraversable(_interpreter.EdgeInterpreter, _tags_index, way.Tags))
                                { // the edge is traversable, add the edges.
                                    // add the edge(s).
                                    if (from.HasValue && to.HasValue)
                                    {
                                        // 
                                        if (!this.AddRoadEdge(way.Tags, true, from.Value, to.Value))
                                        {
                                            this.AddRoadEdge(way.Tags, false, to.Value, from.Value);
                                        }
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
        /// <param name="forward"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="tags"></param>
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

                _dynamic_graph.AddArc(from, to, edge_data, _edge_comparer);
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
        /// Returns true if the edge can be traversed.
        /// </summary>
        /// <param name="edge_interpreter"></param>
        /// <param name="tags_index"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        protected abstract bool CalculateIsTraversable(IEdgeInterpreter edge_interpreter, ITagsIndex tags_index,
                                              IDictionary<string, string> tags);

        /// <summary>
        /// Adds a given relation.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(SimpleRelation relation)
        {

        }

        /// <summary>
        /// Closes this target.
        /// </summary>
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
