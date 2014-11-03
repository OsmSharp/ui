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

using OsmSharp.Collections;
using OsmSharp.Collections.Coordinates;
using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Collections.LongIndex;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Osm;
using OsmSharp.Osm.Cache;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.PreProcessor;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Interpreter.Roads;
using OsmSharp.Routing.Osm.Interpreter;
using System.Collections.Generic;

namespace OsmSharp.Routing.Osm.Streams.Graphs
{
    /// <summary>
    /// Data Processor Target to fill a dynamic graph object.
    /// </summary>
    public abstract class DynamicGraphOsmStreamWriter<TEdgeData> : OsmStreamTarget
        where TEdgeData : IGraphEdgeData 
    {
        /// <summary>
        /// Holds the dynamic graph.
        /// </summary>
        private readonly IDynamicGraphRouterDataSource<TEdgeData> _dynamicGraph;

        /// <summary>
        /// The interpreter for osm data.
        /// </summary>
        private readonly IOsmRoutingInterpreter _interpreter;

        /// <summary>
        /// Holds the tags index.
        /// </summary>
        private ITagsCollectionIndex _tagsIndex;

        /// <summary>
        /// Holds the osm data cache.
        /// </summary>
        private readonly OsmDataCache _dataCache;

        /// <summary>
        /// True when this target is in pre-index mode.
        /// </summary>
        private bool _preIndexMode;

        /// <summary>
        /// Holds the edge comparer.
        /// </summary>
        private readonly IDynamicGraphEdgeComparer<TEdgeData> _edgeComparer;

        /// <summary>
        /// Holds the collect intermediates flag.
        /// </summary>
        private bool _collectIntermediates;

        /// <summary>
        /// Creates a new processor target.
        /// </summary>
        /// <param name="dynamicGraph">The graph that will be filled.</param>
        /// <param name="interpreter">The interpreter to generate the edge data.</param>
        /// <param name="edgeComparer"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="idTransformations"></param>
        protected DynamicGraphOsmStreamWriter(IDynamicGraphRouterDataSource<TEdgeData> dynamicGraph,
            IOsmRoutingInterpreter interpreter, IDynamicGraphEdgeComparer<TEdgeData> edgeComparer, ITagsCollectionIndex tagsIndex)
            : this(dynamicGraph, interpreter, edgeComparer, tagsIndex, new HugeDictionary<long, uint>(), false, new CoordinateIndex())
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
        /// <param name="collectIntermediates"></param>
        /// <param name="coordinates"></param>
        protected DynamicGraphOsmStreamWriter(
            IDynamicGraphRouterDataSource<TEdgeData> dynamicGraph, IOsmRoutingInterpreter interpreter, IDynamicGraphEdgeComparer<TEdgeData> edgeComparer,
            ITagsCollectionIndex tagsIndex, HugeDictionary<long, uint> idTransformations, bool collectIntermediates, ICoordinateIndex coordinates)
        {
            _dynamicGraph = dynamicGraph;
            _interpreter = interpreter;
            _edgeComparer = edgeComparer;

            _tagsIndex = tagsIndex;
            _idTransformations = idTransformations;
            _preIndexMode = true;
            _preIndex = new OsmSharp.Collections.LongIndex.LongIndex.LongIndex();
            _relevantNodes = new OsmSharp.Collections.LongIndex.LongIndex.LongIndex();
            _restricedWays = new HashSet<long>();
            _collapsedNodes = new Dictionary<long, KeyValuePair<KeyValuePair<long, uint>, KeyValuePair<long, uint>>>();

            _collectIntermediates = collectIntermediates;
            _dataCache = new OsmDataCacheMemory();
            _coordinates = coordinates;
        }

        /// <summary>
        /// Returns the tags index.
        /// </summary>
        public ITagsCollectionIndex TagsIndex
        {
            get
            {
                return _tagsIndex;
            }
        }

        /// <summary>
        /// Returns the target graph.
        /// </summary>
        public IDynamicGraphRouterDataSource<TEdgeData> DynamicGraph
        {
            get { return _dynamicGraph; }
        }

        /// <summary>
        /// Returns the osm routing interpreter.
        /// </summary>
        public IOsmRoutingInterpreter Interpreter
        {
            get { return _interpreter; }
        }

        /// <summary>
        /// Returns the edge comparer.
        /// </summary>
        public IDynamicGraphEdgeComparer<TEdgeData> EdgeComparer
        {
            get { return _edgeComparer; }
        }

        /// <summary>
        /// Holds the coordinates.
        /// </summary>
        private ICoordinateIndex _coordinates;

        /// <summary>
        /// Holds the index of all relevant nodes.
        /// </summary>
        private ILongIndex _preIndex;

        /// <summary>
        /// Holds the id transformations.
        /// </summary>
        private readonly HugeDictionary<long, uint> _idTransformations;

        /// <summary>
        /// Initializes the processing.
        /// </summary>
        public override void Initialize()
        {
            _coordinates = new CoordinateIndex();
        }

        /// <summary>
        /// Adds the given node.
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(Node node)
        {
            if (!_preIndexMode)
            {
                if (_nodesToCache != null &&
                    _nodesToCache.Contains(node.Id.Value))
                { // cache this node?
                    _dataCache.AddNode(node);
                }

                if (_preIndex != null && _preIndex.Contains(node.Id.Value))
                { // only save the coordinates for relevant nodes.
                    // save the node-coordinates.
                    // add the relevant nodes.
                    _coordinates[node.Id.Value] = new GeoCoordinateSimple()
                    {
                        Latitude = (float)node.Latitude.Value,
                        Longitude = (float)node.Longitude.Value
                    };

                    // add the node as a possible restriction.
                    if (_interpreter.IsRestriction(OsmGeoType.Node, node.Tags))
                    { // tests quickly if a given node is possibly a restriction.
                        var vehicleTypes = _interpreter.CalculateRestrictions(node);
                        if (vehicleTypes != null &&
                            vehicleTypes.Count > 0)
                        { // add all the restrictions.
                            this._relevantNodes.Add(node.Id.Value);
                            var vertexId = this.AddRoadNode(node.Id.Value).Value; // will always exists, has just been added to coordinates.
                            var restriction = new uint[] { vertexId };
                            if (vehicleTypes.Contains(null))
                            { // restriction is valid for all vehicles.
                                _dynamicGraph.AddRestriction(restriction);
                            }
                            else
                            { // restriction is restricted to some vehicles only.
                                foreach (string vehicle in vehicleTypes)
                                {
                                    _dynamicGraph.AddRestriction(vehicle, restriction);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Holds a list of nodes used twice or more.
        /// </summary>
        private ILongIndex _relevantNodes;

        /// <summary>
        /// Holds all ways that have at least one restrictions.
        /// </summary>
        private HashSet<long> _restricedWays;

        /// <summary>
        /// Holds nodes that have been collapsed because they are considered irrelevant.
        /// </summary>
        private Dictionary<long, KeyValuePair<KeyValuePair<long, uint>, KeyValuePair<long, uint>>> _collapsedNodes;

        /// <summary>
        /// Adds a given way.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(Way way)
        {
            if (!_preIndexMode && _waysToCache != null &&
                _waysToCache.Contains(way.Id.Value))
            { // cache this way?
               _dataCache.AddWay(way);
            }

            // initialize the way interpreter.
            if (_interpreter.EdgeInterpreter.IsRoutable(way.Tags))
            { // the way is a road.
                if (_preIndexMode)
                { // index relevant and used nodes.
                    if (way.Nodes != null)
                    { // this way has nodes.
                        // add new routable tags type.
                        var routableWayTags = new TagsCollection(way.Tags);
                        routableWayTags.RemoveAll(x =>
                        {
                            return !_interpreter.IsRelevantRouting(x.Key) &&
                                !Vehicle.IsRelevantForOneOrMore(x.Key);
                        });
                        _tagsIndex.Add(routableWayTags);

                        int wayNodesCount = way.Nodes.Count;
                        for (int idx = 0; idx < wayNodesCount; idx++)
                        {
                            var node = way.Nodes[idx];
                            if (_preIndex.Contains(node))
                            { // node is relevant.
                                _relevantNodes.Add(node);
                            }
                            else
                            { // node is used.
                                _preIndex.Add(node);
                            }
                        }

                        if (wayNodesCount > 0)
                        { // first node is always relevant.
                            _relevantNodes.Add(way.Nodes[0]);
                            if (wayNodesCount > 1)
                            { // last node is always relevant.
                                _relevantNodes.Add(way.Nodes[wayNodesCount - 1]);
                            }
                        }
                    }
                }
                else
                { // add actual edges.
                    if (way.Nodes != null && way.Nodes.Count > 1)
                    { // way has at least two nodes.
                        if (this.CalculateIsTraversable(_interpreter.EdgeInterpreter, _tagsIndex,
                            way.Tags))
                        { // the edge is traversable, add the edges.
                            uint? from = this.AddRoadNode(way.Nodes[0]);
                            long fromNodeId = way.Nodes[0];
                            List<long> intermediates = new List<long>();
                            for (int idx = 1; idx < way.Nodes.Count; idx++)
                            { // the to-node.
                                long currentNodeId = way.Nodes[idx];
                                if (!_collectIntermediates ||
                                    _relevantNodes.Contains(currentNodeId) ||
                                    idx == way.Nodes.Count - 1)
                                { // node is an important node.
                                    uint? to = this.AddRoadNode(currentNodeId);
                                    long toNodeId = currentNodeId;

                                    // add the edge(s).
                                    if (from.HasValue && to.HasValue)
                                    { // add a road edge.
                                        while(from.Value == to.Value)
                                        {
                                            if(intermediates.Count > 0)
                                            {
                                                uint? dummy = this.AddRoadNode(intermediates[0]);
                                                intermediates.RemoveAt(0);
                                                if(dummy.HasValue && from.Value != dummy.Value)
                                                {
                                                    this.AddRoadEdge(way.Tags, true, from.Value, dummy.Value, null);
                                                    from = dummy;
                                                }
                                            }
                                            else
                                            { // no use to continue.
                                                break;
                                            }
                                        }
                                        // build coordinates.
                                        var intermediateCoordinates = new List<GeoCoordinateSimple>(intermediates.Count);
                                        for (int coordIdx = 0; coordIdx < intermediates.Count; coordIdx++)
                                        {
                                            ICoordinate coordinate;
                                            if (!_coordinates.TryGet(intermediates[coordIdx], out coordinate))
                                            {
                                                break;
                                            }
                                            intermediateCoordinates.Add(new GeoCoordinateSimple()
                                            {
                                                Latitude = coordinate.Latitude,
                                                Longitude = coordinate.Longitude
                                            });
                                        }

                                        if (intermediateCoordinates.Count == intermediates.Count &&
                                            from.Value != to.Value)
                                        { // all coordinates have been found.
                                            this.AddRoadEdge(way.Tags, true, from.Value, to.Value, intermediateCoordinates);
                                        }
                                    }

                                    // if this way has a restriction save the collapsed nodes information.
                                    if(_restricedWays.Contains(way.Id.Value) && to.HasValue && from.HasValue)
                                    { // loop over all intermediates and save.
                                        var collapsedInfo = new KeyValuePair<KeyValuePair<long, uint>, KeyValuePair<long, uint>>(
                                            new KeyValuePair<long, uint>(fromNodeId, from.Value), new KeyValuePair<long, uint>(toNodeId, to.Value));
                                        foreach(var intermedidate in intermediates)
                                        {
                                            _collapsedNodes[intermedidate] = collapsedInfo;
                                        }
                                    }

                                    from = to; // the to node becomes the from.
                                    intermediates.Clear();
                                }
                                else
                                { // this node is just an intermediate.
                                    intermediates.Add(currentNodeId);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the given node has an actual road node, meaning a relevant vertex, and outputs the vertex id.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="id">The vertex id.</param>
        /// <returns></returns>
        private bool TryGetRoadNode(long nodeId, out uint id)
        {
            return _idTransformations.TryGetValue(nodeId, out id);
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
                ICoordinate coordinates;
                if (_coordinates.TryGet(nodeId, out coordinates))
                { // the coordinate is present.
                    id = _dynamicGraph.AddVertex(
                        coordinates.Latitude, coordinates.Longitude);
                    _coordinates.Remove(nodeId); // free the memory again!

                    if (_relevantNodes.Contains(nodeId))
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
        /// <param name="intermediates"></param>
        protected virtual void AddRoadEdge(TagsCollectionBase tags, bool forward, uint from, uint to, List<GeoCoordinateSimple> intermediates)
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
                var edgeData = this.CalculateEdgeData(_interpreter.EdgeInterpreter, _tagsIndex, tags, forward, fromCoordinate, toCoordinate, intermediates);

                _dynamicGraph.AddEdge(from, to, edgeData, new CoordinateArrayCollection<GeoCoordinateSimple>(intermediates.ToArray()), _edgeComparer);
            }
        }

        /// <summary>
        /// Calculates the edge data.
        /// </summary>
        /// <returns></returns>
        protected abstract TEdgeData CalculateEdgeData(IEdgeInterpreter edgeInterpreter, ITagsCollectionIndex tagsIndex, TagsCollectionBase tags, 
            bool directionForward, GeoCoordinate from, GeoCoordinate to, List<GeoCoordinateSimple> intermediates);

        /// <summary>
        /// Returns true if the edge can be traversed.
        /// </summary>
        /// <param name="edgeInterpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        protected abstract bool CalculateIsTraversable(IEdgeInterpreter edgeInterpreter, ITagsCollectionIndex tagsIndex,
                                              TagsCollectionBase tags);

        /// <summary>
        /// Holds the ways to cache to complete the restriction reations.
        /// </summary>
        private HashSet<long> _waysToCache;

        /// <summary>
        /// Holds the node to cache to complete the restriction relations.
        /// </summary>
        private HashSet<long> _nodesToCache;

        /// <summary>
        /// Adds a given relation.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(Relation relation)
        {
            if (_interpreter.IsRestriction(OsmGeoType.Relation, relation.Tags))
            {
                // add the node as a possible restriction.
                if (!_preIndexMode)
                { // tests quickly if a given node is possibly a restriction.
                    // interpret the restriction using the complete object.
                    var vehicleRestrictions = _interpreter.CalculateRestrictions(relation, _dataCache);
                    if (vehicleRestrictions != null &&
                        vehicleRestrictions.Count > 0)
                    { // add all the restrictions.
                        foreach (var vehicleRestriction in vehicleRestrictions)
                        { // translated the restricted route in terms of node-id's to vertex ids.
                            var restriction = new List<uint>(vehicleRestriction.Value.Length);
                            KeyValuePair<KeyValuePair<long, uint>, KeyValuePair<long, uint>>? firstCollapsedInfo = null;
                            uint? previousRelevantId = null;
                            for (int idx = 0; idx < vehicleRestriction.Value.Length; idx++)
                            {
                                // check if relevant node.
                                uint relevantId;
                                if (this.TryGetRoadNode(vehicleRestriction.Value[idx], out relevantId))
                                { // ok, is relevant.
                                    if (firstCollapsedInfo.HasValue)
                                    { // there was an irrelevant node before this one.
                                        if (firstCollapsedInfo.Value.Key.Value == relevantId)
                                        { // ok, take the other relevant one.
                                            restriction.Add(firstCollapsedInfo.Value.Value.Value);
                                        }
                                        else if (firstCollapsedInfo.Value.Value.Value == relevantId)
                                        { // ok, take the other relevant one.
                                            restriction.Add(firstCollapsedInfo.Value.Key.Value);
                                        }
                                        else
                                        { // oeps, invalid info here.
                                            restriction = null;
                                            break;
                                        }
                                        firstCollapsedInfo = null;
                                    }
                                    if (!previousRelevantId.HasValue || previousRelevantId.Value != relevantId)
                                    { // ok, this one is new.
                                        previousRelevantId = relevantId;
                                        restriction.Add(relevantId);
                                    }
                                }
                                else
                                { // ok, not relevant, should be in the collapsed nodes.
                                    KeyValuePair<KeyValuePair<long, uint>, KeyValuePair<long, uint>> collapsedInfo;
                                    if(!_collapsedNodes.TryGetValue(vehicleRestriction.Value[idx], out collapsedInfo))
                                    { // one of the nodes was not found, this restriction is incomplete or invalid, skip it.
                                        restriction = null;
                                        break;
                                    }
                                    if(previousRelevantId.HasValue)
                                    { // ok, there is a previous relevant id, one of them should match collapsedInfo.
                                        if (collapsedInfo.Key.Value == previousRelevantId.Value)
                                        { // ok, take the other relevant one.
                                            restriction.Add(collapsedInfo.Value.Value);
                                        }
                                        else if (collapsedInfo.Value.Value == previousRelevantId.Value)
                                        { // ok, take the other relevant one.
                                            restriction.Add(collapsedInfo.Key.Value);
                                        }
                                        else
                                        { // oeps, invalid info here.
                                            restriction = null;
                                            break;
                                        }
                                    }
                                    else
                                    { // save the collapsedInfo for the first relevant node.
                                        firstCollapsedInfo = collapsedInfo;
                                    }
                                }
                            }
                            if (restriction != null)
                            { // restriction exists.
                                if (vehicleRestriction.Key == null)
                                { // this restriction is for all vehicles.
                                    _dynamicGraph.AddRestriction(restriction.ToArray());
                                }
                                else
                                { // this restriction is just for the given vehicle.
                                    _dynamicGraph.AddRestriction(vehicleRestriction.Key, restriction.ToArray());
                                }
                            }
                        }
                    }
                }
                else
                { // pre-index mode.
                    if (relation.Members != null && relation.Members.Count > 0)
                    { // there are members, keep them!
                        foreach (var member in relation.Members)
                        {
                            switch (member.MemberType.Value)
                            {
                                case OsmGeoType.Node:
                                    if (_nodesToCache == null)
                                    {
                                        _nodesToCache = new HashSet<long>();
                                    }
                                    _relevantNodes.Add(member.MemberId.Value);
                                    _nodesToCache.Add(member.MemberId.Value);
                                    break;
                                case OsmGeoType.Way:
                                    if (_waysToCache == null)
                                    {
                                        _waysToCache = new HashSet<long>();
                                    }
                                    _waysToCache.Add(member.MemberId.Value);
                                    _restricedWays.Add(member.MemberId.Value);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a pre-processor if needed.
        /// </summary>
        /// <returns></returns>
        public virtual IPreProcessor GetPreprocessor()
        {
            return null;
        }

        /// <summary>
        /// Registers the source for this target.
        /// </summary>
        /// <param name="source"></param>
        public override void RegisterSource(OsmStreamSource source)
        {
            // add filter to remove all irrelevant tags.
            OsmStreamFilterTagsFilter tagsFilter = new OsmStreamFilterTagsFilter((TagsCollectionBase tags) =>
            {
                List<Tag> tagsToRemove = new List<Tag>();
                foreach (Tag tag in tags)
                {
                    if (!_interpreter.IsRelevant(tag.Key, tag.Value))
                    {
                        tagsToRemove.Add(tag);
                    }
                }
                foreach (Tag tag in tagsToRemove)
                {
                    tags.RemoveKeyValue(tag.Key, tag.Value);
                }
            });
            tagsFilter.RegisterSource(source);

            base.RegisterSource(tagsFilter);
        }

        /// <summary>
        /// Called right before pull and right after initialization.
        /// </summary>
        /// <returns></returns>
        public override bool OnBeforePull()
        {
            // do the pull.
            this.DoPull(true, false, false);

            // reset the source.
            this.Source.Reset();

            // resize graph.
            // TODO: study avery cardinality and slightly overestimate here.
            long vertexEstimate = _relevantNodes.Count + (long)(_relevantNodes.Count * 0.1);
            _dynamicGraph.Resize(vertexEstimate, (long)(vertexEstimate * 4));

            // move out of pre-index mode.
            _preIndexMode = false;

            return true;
        }

        /// <summary>
        /// Called right after pull.
        /// </summary>
        public override void OnAfterPull()
        {
            base.OnAfterPull();

            // execute pre-processor.
            var preProcessor = this.GetPreprocessor();
            if(preProcessor != null)
            { // there is a pre-processor, trigger execution.
                preProcessor.Start();
            }

            // trim the graph.
            _dynamicGraph.Trim();
        }

        /// <summary>
        /// Closes this target.
        /// </summary>
        public override void Close()
        {
            _coordinates.Clear();
            _dataCache.Clear();
            _idTransformations.Clear();
            if(_nodesToCache != null)
            {
                _nodesToCache.Clear();
            }
            if (_waysToCache != null)
            {
                _waysToCache.Clear();
            }
            _restricedWays = null;
            _collapsedNodes = null;
            _preIndex = null;
            _relevantNodes = null;
            _tagsIndex = null;
        }
    }
}