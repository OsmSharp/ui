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
using OsmSharp.Osm.Data.Streams;
using OsmSharp.Osm;
using OsmSharp.Osm.Data.Core.Processor;
using OsmSharp.Routing.Graph;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Interpreter.Roads;

namespace OsmSharp.Routing.Osm.Data.Processing
{
    /// <summary>
    /// Data Processor Target to fill a dynamic graph object.
    /// </summary>
    public abstract class DynamicGraphOsmStreamWriter<TEdgeData> : OsmStreamTarget
        where TEdgeData : IDynamicGraphEdgeData 
    {
        /// <summary>
        /// Holds the dynamic graph.
        /// </summary>
        private readonly IDynamicGraph<TEdgeData> _dynamicGraph;

        /// <summary>
        /// The interpreter for osm data.
        /// </summary>
        private readonly IRoutingInterpreter _interpreter;

        /// <summary>
        /// Holds the tags index.
        /// </summary>
        private readonly ITagsIndex _tagsIndex;

        /// <summary>
        /// True when this target is in pre-index mode.
        /// </summary>
        private bool _preIndexMode;

        /// <summary>
        /// The bounding box to limit nodes if any.
        /// </summary>
        private readonly GeoCoordinateBox _box;

        /// <summary>
        /// Holds the edge comparer.
        /// </summary>
        private readonly IDynamicGraphEdgeComparer<TEdgeData> _edgeComparer;

        /// <summary>
        /// Creates a new processor target.
        /// </summary>
        /// <param name="dynamicGraph">The graph that will be filled.</param>
        /// <param name="interpreter">The interpreter to generate the edge data.</param>
        /// <param name="edgeComparer"></param>
        protected DynamicGraphOsmStreamWriter(IDynamicGraph<TEdgeData> dynamicGraph,
            IRoutingInterpreter interpreter, IDynamicGraphEdgeComparer<TEdgeData> edgeComparer)
            : this(dynamicGraph, interpreter, edgeComparer, new SimpleTagsIndex(), new Dictionary<long, uint>())
        {

        }

        /// <summary>
        /// Creates a new processor target.
        /// </summary>
        /// <param name="dynamicGraph">The graph that will be filled.</param>
        /// <param name="interpreter">The interpreter to generate the edge data.</param>
        /// <param name="edgeComparer"></param>
        /// <param name="tagsIndex"></param>
        protected DynamicGraphOsmStreamWriter(IDynamicGraph<TEdgeData> dynamicGraph,
            IRoutingInterpreter interpreter, IDynamicGraphEdgeComparer<TEdgeData> edgeComparer, ITagsIndex tagsIndex)
            : this(dynamicGraph, interpreter, edgeComparer, tagsIndex, new Dictionary<long, uint>())
        {

        }

        /// <summary>
        /// Creates a new processor target.
        /// </summary>
        /// <param name="dynamicGraph">The graph that will be filled.</param>
        /// <param name="interpreter">The interpreter to generate the edge data.</param>
        /// <param name="edgeComparer"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="idTransformations"></param>
        protected DynamicGraphOsmStreamWriter(IDynamicGraph<TEdgeData> dynamicGraph,
            IRoutingInterpreter interpreter, IDynamicGraphEdgeComparer<TEdgeData> edgeComparer, ITagsIndex tagsIndex,
            IDictionary<long, uint> idTransformations)
            : this(dynamicGraph, interpreter, edgeComparer, tagsIndex, idTransformations, null)
        {

        }

        /// <summary>
        /// Creates a new processor target.
        /// </summary>
        /// <param name="dynamicGraph">The graph that will be filled.</param>
        /// <param name="interpreter">The interpreter to generate the edge data.</param>
        /// <param name="edgeComparer"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="idTransformations"></param>
        /// <param name="box"></param>
        protected DynamicGraphOsmStreamWriter(
            IDynamicGraph<TEdgeData> dynamicGraph, IRoutingInterpreter interpreter, IDynamicGraphEdgeComparer<TEdgeData> edgeComparer, 
            ITagsIndex tagsIndex, IDictionary<long, uint> idTransformations,
            GeoCoordinateBox box)
        {
            _dynamicGraph = dynamicGraph;
            _interpreter = interpreter;
            _edgeComparer = edgeComparer;
            _box = box;

            _tagsIndex = tagsIndex;
            _idTransformations = idTransformations;
            _preIndexMode = true;
            _preIndex = new HashSet<long>();
            _usedTwiceOrMore = new HashSet<long>();
        }

        /// <summary>
        /// Returns the tags index.
        /// </summary>
        public ITagsIndex TagsIndex
        {
            get
            {
                return _tagsIndex;
            }
        }

        /// <summary>
        /// Holds the coordinates.
        /// </summary>
        private OsmSharp.Collections.HugeDictionary<long, float[]> _coordinates;

        /// <summary>
        /// Holds the index of all relevant nodes.
        /// </summary>
        private HashSet<long> _preIndex;

        /// <summary>
        /// Holds the id transformations.
        /// </summary>
        private readonly IDictionary<long, uint> _idTransformations;

        /// <summary>
        /// Initializes the processing.
        /// </summary>
        public override void Initialize()
        {
            _coordinates = new OsmSharp.Collections.HugeDictionary<long, float[]>();
        }

        /// <summary>
        /// Adds the given node.
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(Node node)
        {
            if (!_preIndexMode)
            {
                if (_preIndex != null && _preIndex.Contains(node.Id.Value))
                { // only save the coordinates for relevant nodes.
                    // save the node-coordinates.
                    // add the relevant nodes.

                    if (_box == null || _box.Contains(new GeoCoordinate((float)node.Latitude.Value, (float)node.Longitude.Value)))
                    { // the coordinate is acceptable.
                        _coordinates[node.Id.Value] = new float[] { (float)node.Latitude.Value, (float)node.Longitude.Value };
                        if (_coordinates.Count == _preIndex.Count)
                        {
                            _preIndex.Clear();
                            _preIndex = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Holds a list of nodes used twice or more.
        /// </summary>
        private readonly HashSet<long> _usedTwiceOrMore;

        /// <summary>
        /// Adds a given way.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(Way way)
        {
            // initialize the way interpreter.
            if (_interpreter.EdgeInterpreter.IsRoutable(way.Tags))
            { // the way is a road.
                if (_preIndexMode)
                { // index only relevant nodes.
                    if (way.Nodes != null)
                    {
                        foreach (long node in way.Nodes)
                        {
                            if (_preIndex.Contains(node))
                            {
                                _usedTwiceOrMore.Add(node);
                            }
                            else
                            {
                                _preIndex.Add(node); // node is relevant.
                            }
                        }
                    }
                }
                else
                {
                // add the forward edges.
                //if (!interpreter.IsOneWayReverse())
                    if (true) // add backward edges too!
                    { // loop over all edges.
                        if (way.Nodes != null && way.Nodes.Count > 1)
                        { // way has at least two nodes.
                            // keep the relevant tags.
                            TagsCollection relevantTags = new SimpleTagsCollection();
                            foreach (var relevantTag in way.Tags)
                            {
                                if (_interpreter.IsRelevant(relevantTag.Key))
                                {
                                    relevantTags.Add(relevantTag);
                                }
                            }


                            if (this.CalculateIsTraversable(_interpreter.EdgeInterpreter, _tagsIndex,
                                relevantTags))
                            { // the edge is traversable, add the edges.
                                uint? from = this.AddRoadNode(way.Nodes[0]);
                                for (int idx = 1; idx < way.Nodes.Count; idx++)
                                { // the to-node.
                                    uint? to = this.AddRoadNode(way.Nodes[idx]);
                                    // add the edge(s).
                                    if (from.HasValue && to.HasValue)
                                    { // add a road edge.
                                        if (!this.AddRoadEdge(relevantTags, true, from.Value, to.Value))
                                        { // add the reverse too if it has been indicated that this was needed.
                                            this.AddRoadEdge(relevantTags, false, to.Value, from.Value);
                                        }
                                    }
                                    from = to; // the to node becomes the from.
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a node that is at least part of one road.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        private uint? AddRoadNode(long nodeId)
        {
            uint id;
            // try and get existing node.
            if (!_idTransformations.TryGetValue(nodeId, out id))
            {
                // get coordinates.
                float[] coordinates;
                if (_coordinates.TryGetValue(nodeId, out coordinates))
                { // the coordinate is present.
                    id = _dynamicGraph.AddVertex(
                        coordinates[0], coordinates[1]);
                    _coordinates.Remove(nodeId); // free the memory again!

                    if (_usedTwiceOrMore.Contains(nodeId))
                    {
                        _idTransformations[nodeId] = id;
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
        private bool AddRoadEdge(TagsCollection tags, bool forward, uint from, uint to)
        {
            float latitude;
            float longitude;
            GeoCoordinate fromCoordinate = null;
            if (_dynamicGraph.GetVertex(from, out latitude, out longitude))
            { // 
                fromCoordinate = new GeoCoordinate(latitude, longitude);
            }
            GeoCoordinate toCoordinate = null;
            if (_dynamicGraph.GetVertex(to, out latitude, out longitude))
            { // 
                toCoordinate = new GeoCoordinate(latitude, longitude);
            }

            if (fromCoordinate != null && toCoordinate != null)
            { // calculate the edge data.
                TEdgeData edgeData = this.CalculateEdgeData(_interpreter.EdgeInterpreter, _tagsIndex, tags, forward, fromCoordinate, toCoordinate);

                _dynamicGraph.AddArc(from, to, edgeData, _edgeComparer);
            }
            return false;
        }

        /// <summary>
        /// Calculates the edge data.
        /// </summary>
        /// <returns></returns>
        protected abstract TEdgeData CalculateEdgeData(IEdgeInterpreter edgeInterpreter, ITagsIndex tagsIndex, TagsCollection tags,
            bool directionForward, GeoCoordinate from, GeoCoordinate to);

        /// <summary>
        /// Returns true if the edge can be traversed.
        /// </summary>
        /// <param name="edgeInterpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        protected abstract bool CalculateIsTraversable(IEdgeInterpreter edgeInterpreter, ITagsIndex tagsIndex,
                                              TagsCollection tags);

        /// <summary>
        /// Adds a given relation.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(Relation relation)
        {

        }

        /// <summary>
        /// Closes this target.
        /// </summary>
        public override void Close()
        {
            if (_preIndexMode)
            {
                this.Reader.Reset();
                _preIndexMode = false;
                this.Pull();
            }
        }
    }
}
