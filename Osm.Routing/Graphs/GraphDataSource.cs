//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Data;
//using Osm.Core;
//using Tools.Math.Geo;
//using Tools.Math.Graph;
//using Osm.Core.Filters;
//using Osm.Core.Factory;
//using Tools.Math.Shapes;
//using Tools.Math.Shapes.ResultHelpers;
//using Osm.Routing.Raw.Graphs.Interpreter;

//namespace Osm.Routing.Raw.Graphs
//{
//    /// <summary>
//    /// Class wrapping an osm data source to allow adding special routing nodes.
//    /// </summary>
//    internal class GraphDataSource
//    {
//        public IDataSourceReadOnly _source;

//        private Guid _guid;

//        private Dictionary<long, Node> _extra_nodes;

//        private Dictionary<long, Way> _extra_ways;

//        private Dictionary<long, List<Way>> _extra_ways_per_node_id;

//        private GraphInterpreterBase _interpreter;

//        internal GraphDataSource(
//            IDataSourceReadOnly source,
//            GraphInterpreterBase interpreter)
//        {
//            _source = source;
//            _extra_nodes = new Dictionary<long, Node>();
//            _extra_ways = new Dictionary<long, Way>();
//            _extra_ways_per_node_id = new Dictionary<long, List<Way>>();
//            _guid = Guid.NewGuid();
//            _interpreter = interpreter;
//        }

//        /// <summary>
//        /// The interpreter used in this source.
//        /// </summary>
//        public IGraphInterpreter<Way, Node> Interpreter
//        {
//            get
//            {
//                return _interpreter;
//            }
//        }

//        #region Resolve Routing Nodes

//        /// <summary>
//        /// Resolves a coordinate and returns the closest routable node.
//        /// </summary>
//        /// <param name="coordinate"></param>
//        /// <param name="search_radius"></param>
//        /// <returns></returns>
//        public GraphResolved ResolveAndAddNode(
//            GeoCoordinate coordinate,
//            double search_radius)
//        {
//            return this.ResolveAndAddNode(coordinate, search_radius, null);
//        }

//        /// <summary>
//        /// Resolves a coordinate and returns the closest routable node.
//        /// </summary>
//        /// <param name="coordinate"></param>
//        /// <param name="search_radius"></param>
//        /// <returns></returns>
//        public GraphResolved ResolveAndAddNode(
//            GeoCoordinate coordinate,
//            double search_radius,
//            IGraphResolverMatcher matcher)
//        {
//            GraphResolved node_way = new GraphResolved();

//            // get the delta.
//            double delta = search_radius;

//            // initialize the result and distance.
//            OsmGeo result = null;
//            double distance = double.MaxValue;

//            // construct a bounding box.
//            GeoCoordinate from_top = new GeoCoordinate(
//                coordinate.Latitude + delta,
//                coordinate.Longitude - delta);
//            GeoCoordinate from_bottom = new GeoCoordinate(
//                coordinate.Latitude - delta,
//                coordinate.Longitude + delta);
//            IList<OsmBase> query_result =
//                this.Get(new Tools.Math.Geo.GeoCoordinateBox(from_top, from_bottom), Filter.Any());
            
//            // loop over all found objects.
//            foreach (OsmBase base_obj in query_result)
//            {
//                // test if the object is a pysical geo object.
//                if (base_obj is OsmGeo)
//                {
//                    // calculate the distance.
//                    double dist = (base_obj as OsmGeo).Shape.Distance(coordinate);

//                    // take the current object as the new found object 
//                    // if the distance is smaller and if the interpreter 
//                    // says it is routable.
//                    if (dist < distance)
//                    {
//                        if (base_obj is Way)
//                        { // object is a way; see if the way is drivable.
//                            Way way = base_obj as Way;
//                            if (this.Interpreter.CanBeTraversed(way))
//                            {
//                                bool match = true;
//                                if (matcher != null)
//                                {
//                                    match = matcher.MatchWay(way);
//                                }

//                                if (match)
//                                {
//                                    distance = dist;
//                                    result = way;
//                                }
//                            }
//                        }
//                    }
//                }
//            }

//            // find the node/way to add.
//            Node resulting_node = null;
//            if (result != null)
//            {
//                Way closest_way = (result as Way);

//                ShapeF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> shape =
//                    closest_way.Shape;

//                DistanceResult<GeoCoordinate> distance_result = shape.DistanceDetailed(coordinate);
//                if (distance_result is LineProjectionResult<GeoCoordinate>)
//                {
//                    LineProjectionResult<GeoCoordinate> line_projection = (distance_result as LineProjectionResult<GeoCoordinate>);

//                    GeoCoordinate intersection_point = line_projection.ClosestPrimitive as GeoCoordinate;

//                    // insert a new node in the way and add it.
//                    Node new_node = OsmBaseFactory.CreateNode();
//                    new_node.Coordinate = intersection_point;
//                    this.AddExtra(new_node);
//                    Way new_way = OsmBaseFactory.CreateWay();
//                    new_way = closest_way.Copy();
//                    new_way.Nodes.Insert(line_projection.Idx + 1, new_node);
//                    this.AddExtra(new_way, _interpreter.GetResolvedWayTag());

//                    resulting_node = new_node;
//                }
//                else if (distance_result is PolygonProjectionResult<GeoCoordinate>)
//                {
//                    PolygonProjectionResult<GeoCoordinate> line_projection = (distance_result as PolygonProjectionResult<GeoCoordinate>);

//                    GeoCoordinate intersection_point = line_projection.ClosestPrimitive as GeoCoordinate;

//                    // insert a new node in the way and add it.
//                    Node new_node = OsmBaseFactory.CreateNode();
//                    new_node.Coordinate = intersection_point;
//                    this.AddExtra(new_node);
//                    Way new_way = OsmBaseFactory.CreateWay();
//                    new_way = closest_way.Copy();
//                    new_way.Nodes.Insert(line_projection.Idx + 1, new_node);
//                    this.AddExtra(new_way, _interpreter.GetResolvedWayTag());

//                    resulting_node = new_node;
//                }

//                node_way.Way = closest_way;
//                node_way.Node = resulting_node;
//            }

//            return node_way;
//        }


//        /// <summary>
//        /// Adds an extra node.
//        /// </summary>
//        /// <param name="node"></param>
//        private void AddExtra(Node node)
//        {
//            if (node.Coordinate == null)
//            {
//                throw new ArgumentNullException();
//            }
//            _extra_nodes.Add(node.Id, node);
//        }

//        /// <summary>
//        /// Adds an extra way, or a new version of an old one.
//        /// </summary>
//        /// <param name="way"></param>
//        private void AddExtra(Way way)
//        {
//            this.AddExtra(way, null);
//        }

//        /// <summary>
//        /// Adds an extra way.
//        /// </summary>
//        /// <param name="way">The extra way to add.</param>
//        /// <param name="extra_tags">The extra tags to add if any.</param>
//        private void AddExtra(Way way, KeyValuePair<string,string>? extra_tags)
//        {
//            // add the extra tags.
//            if (extra_tags != null
//                && !way.Tags.ContainsKey(extra_tags.Value.Key))
//            {
//                way.Tags.Add(extra_tags.Value.Key, extra_tags.Value.Value);
//            }

//            // check for new or replacement ways.
//            if (!_extra_ways.ContainsKey(way.Id))
//            { // the way is new.
//                _extra_ways.Add(way.Id, way);
//            }
//            else
//            { // the way is to replace another.
//                Way old_way = _extra_ways[way.Id];

//                // remove all previously indexed nodes.
//                foreach (Node node in old_way.Nodes)
//                {
//                    List<Way> ways = null;
//                    if (!_extra_ways_per_node_id.TryGetValue(node.Id, out ways))
//                    {
//                        throw new Exception("Way exists as an old version but was not correctly indexed!");
//                    }
//                    ways.Remove(way);
//                }

//                _extra_ways[way.Id] = way;
//            }

//            // index all the nodes in the new way.
//            foreach (Node node in way.Nodes)
//            {
//                List<Way> ways = null;
//                if (!_extra_ways_per_node_id.TryGetValue(node.Id, out ways))
//                {
//                    ways = new List<Way>();
//                    _extra_ways_per_node_id.Add(node.Id, ways);
//                }
//                ways.Add(way);
//            }
//        }

//        /// <summary>
//        /// Returns all objects within a given bounding box and for a given filter.
//        /// </summary>
//        /// <param name="box"></param>
//        /// <param name="filter"></param>
//        /// <returns></returns>
//        private IList<Osm.Core.OsmBase> Get(Tools.Math.Geo.GeoCoordinateBox box, Osm.Core.Filters.Filter filter)
//        {
//            IList<OsmBase> found = _source.Get(box, filter);
//            List<OsmBase> found_extra = new List<OsmBase>();
//            for (int idx = 0; idx < found.Count; idx++)
//            {
//                OsmBase found_object = found[idx];
//                if (found_object is Way)
//                {
//                    Way way = null;
//                    if (_extra_ways.TryGetValue(found_object.Id, out way))
//                    {
//                        // replace way.
//                        found[idx] = way;

//                        // add the nodes that are new to the result if they exist within the bb.
//                        foreach (Node node in way.Nodes)
//                        {
//                            if (_extra_nodes.ContainsKey(node.Id))
//                            {
//                                if (box.IsInside(node.Coordinate))
//                                {
//                                    found_extra.Add(node);
//                                }
//                            }
//                        }
//                    }
//                }
//            }

//            found_extra.AddRange(found);
//            return found_extra;
//        }

//        #endregion
        
//        #region Get Ways

//        /// <summary>
//        /// Returns all ways for given node.
//        /// </summary>
//        /// <param name="node"></param>
//        /// <returns></returns>
//        public IList<Osm.Core.Way> GetWaysFor(Osm.Core.Node node)
//        {
//            IList<Way> ways = null;
//            if(_interpreter.HasWeight(node))
//            { // the node has weight.
//                // convert the node.
//                this.ConvertNode(node);
//            }

//            // get all the ways for this (converted) node.
//            ways = this.GetWaysForNodeWithExtras(node);

//            // convert all the nodes that have weights and requery.
//            bool one_node_was_converted = false;
//            foreach (Way returned_way in ways)
//            {
//                foreach (Node returned_node in returned_way.Nodes)
//                {
//                    if (_interpreter.HasWeight(returned_node))
//                    { // convert the node; it has weight.
//                        one_node_was_converted = true;
//                        this.ConvertNode(returned_node);
//                    }
//                }
//            }

//            // if there was a conversion in one of the ways requery for the new versions of the ways.
//            if (one_node_was_converted)
//            {
//                ways = this.GetWaysForNodeWithExtras(node);
//            }

//            return ways;
//        }

//        /// <summary>
//        /// Holds the id's of the nodes already converted.
//        /// </summary>
//        private HashSet<long> _converted_nodes = new HashSet<long>();

//        /// <summary>
//        /// Converts a node part of a way that has to remain unchanged.
//        /// </summary>
//        /// <param name="node"></param>
//        /// <returns></returns>
//        private void ConvertNode(Node node)
//        {
//            if (!_converted_nodes.Contains<long>(node.Id))
//            {
//                while (this.DoConvertNode(node))
//                { // keep converting until no conversion is needed anymore.

//                }
//                _converted_nodes.Add(node.Id);
//            }
//        }

//        private bool DoConvertNode(Node node)
//        {
//            bool conversion_was_done = false;

//            // get all the ways where the node occurs.
//            IList<Way> ways = this.GetWaysForNodeWithExtras(node);

//            // for all the ways do the conversion if needed.
//            List<KeyValuePair<Node, KeyValuePair<Node,Way>>> converted_way_segments = new List<KeyValuePair<Node, KeyValuePair<Node,Way>>>();
//            bool keep_original = true;
//            foreach (Way way in ways)
//            {
//                converted_way_segments.AddRange(
//                    this.DoConvertNodeOnWay(way, node, way, keep_original));
//                keep_original = false;
//            }

//            // query the interperter and notify it of the changes.
//            Node via = node;
//            foreach (KeyValuePair<Node, KeyValuePair<Node, Way>> convert_way_segment_from in converted_way_segments)
//            {
//                foreach (KeyValuePair<Node, KeyValuePair<Node, Way>> convert_way_segment_to in converted_way_segments)
//                {
//                    Node from = convert_way_segment_from.Value.Key;
//                    Node to = convert_way_segment_to.Value.Key;
//                    Way way_from = convert_way_segment_from.Value.Value;
//                    Way way_to = convert_way_segment_to.Value.Value;

//                    if (from != to)
//                    {
//                        if (_interpreter.CanBeTraversed(from, way_from, via, way_to, to))
//                        {
//                            Way new_way = OsmBaseFactory.CreateWay();
//                            new_way.Nodes.Add(convert_way_segment_from.Key);
//                            new_way.Nodes.Add(convert_way_segment_to.Key);
//                            new_way.Tags.Add(_interpreter.GetWeighedNodeTag());
//                            new_way.Tags.Add("oneway", "yes");
//                            this.AddExtra(new_way);
//                        }
//                    }
//                }
//            }

//            return conversion_was_done;
//        }

//        /// <summary>
//        /// Converts a given node on a given way keep one original if requested.
//        /// </summary>
//        /// <param name="way"></param>
//        /// <param name="node"></param>
//        /// <param name="keep_original"></param>
//        /// <returns></returns>
//        private IList<KeyValuePair<Node, KeyValuePair<Node, Way>>> DoConvertNodeOnWay(Way way, Node node, Way original_way, bool keep_original)
//        {
//            List<KeyValuePair<Node, KeyValuePair<Node, Way>>> return_nodes = 
//                new List<KeyValuePair<Node, KeyValuePair<Node, Way>>>();

//            int idx = way.Nodes.IndexOf(node);

//            if (idx == 0 || idx == way.Nodes.Count - 1)
//            { // node at the edge.
//                Way new_way = way;
//                if (!keep_original)
//                {
//                    // create a new node for the edge.
//                    Node new_node = OsmBaseFactory.CreateNode();
//                    node.CopyTo(new_node);
//                    new_node.Coordinate = node.Coordinate;
//                    this.AddExtra(new_node);

//                    // create a copy of the original way.
//                    new_way = way.Copy();
//                    new_way.Nodes[idx] = new_node;
//                    this.AddExtra(new_way);
//                }

//                // get the two nodes to return.
//                KeyValuePair<Node, KeyValuePair<Node, Way>> pair;
//                if (idx == 0)
//                {
//                    pair = new KeyValuePair<Node, KeyValuePair<Node, Way>>(
//                        new_way.Nodes[idx],
//                        new KeyValuePair<Node,Way>(
//                            new_way.Nodes[idx + 1],
//                            original_way));
//                }
//                else
//                {
//                    pair = new KeyValuePair<Node, KeyValuePair<Node, Way>>(
//                        new_way.Nodes[idx],
//                        new KeyValuePair<Node,Way>(
//                            new_way.Nodes[idx - 1],
//                            original_way));
//                }
//                return_nodes.Add(pair);
//            }
//            else if(idx > 0)
//            { // node not at the edge.

//                // create the way before; use the original for this.
//                IList<Node> nodes_before = way.Nodes.GetRange(0, idx + 1);
//                Way way_before = way.Copy();
//                way_before.Nodes.Clear();
//                way_before.Nodes.AddRange(nodes_before);

//                // replace the node with new ones.
//                Node new_node_before = null;
//                if (keep_original)
//                { // do nothing the original is to be kept.

//                }
//                else
//                {
//                    // create a new node.
//                    new_node_before = OsmBaseFactory.CreateNode();
//                    node.CopyTo(new_node_before);
//                    new_node_before.Coordinate = node.Coordinate;
//                    this.AddExtra(new_node_before);

//                    // replace the node.
//                    way_before.Nodes[idx] = new_node_before;

//                    // add to the return list.
//                    return_nodes.Add(new KeyValuePair<Node, KeyValuePair<Node, Way>>(
//                        new_node_before, 
//                        new KeyValuePair<Node,Way>(way_before.Nodes[idx - 2],original_way)));
//                }
//                this.AddExtra(way_before);

//                // create the way after; use a new way for this.
//                IList<Node> nodes_after = way.Nodes.GetRange(idx, way.Nodes.Count - (idx + 1));
//                Way way_after = OsmBaseFactory.CreateWay();
//                way.CopyTo(way_after);
//                way_after.Nodes.Clear();
//                way_after.Nodes.AddRange(nodes_after);
                
//                // create a new node.
//                Node new_node_after = OsmBaseFactory.CreateNode();
//                node.CopyTo(new_node_after);
//                new_node_after.Coordinate = node.Coordinate;
//                this.AddExtra(new_node_after);

//                // replace the node.
//                way_after.Nodes[0] = new_node_after;
//                this.AddExtra(way_after);

//                // add to the return list.
//                return_nodes.Add(
//                    new KeyValuePair<Node, KeyValuePair<Node, Way>>(
//                        new_node_after, new KeyValuePair<Node,Way>(way_after.Nodes[1],original_way)));

//                // convert nodes in the way if needed again.
//                return_nodes.AddRange(
//                    this.DoConvertNodeOnWay(way_after, node,original_way, false));
//            }

//            return return_nodes;
//        }

//        private IList<Way> GetWaysForNodeWithExtras(Node node)
//        {
//            // get the actual extra ways.
//            IList<Way> ways = null;
//            List<Way> extra_ways = new List<Way>();
//            if (_extra_ways_per_node_id.TryGetValue(node.Id, out extra_ways))
//            {

//            }
//            ways = _source.GetWaysFor(node);

//            if (ways == null)
//            {
//                return extra_ways;
//            }
//            else if (extra_ways == null)
//            {
//                return ways;
//            }
//            else
//            {
//                // remove all ways where there exist new versions of.
//                List<Way> to_remove = new List<Way>();
//                foreach (Way way in ways)
//                {
//                    Way new_way = null;
//                    if (_extra_ways.TryGetValue(way.Id, out new_way))
//                    {
//                        if (!new_way.HasNode(node))
//                        {
//                            to_remove.Add(way);
//                        }
//                    }
//                }

//                // add extra ways/remove old ways.
//                foreach (Way new_way in extra_ways)
//                {
//                    bool found = false;
//                    for (int idx = 0; idx < ways.Count; idx++)
//                    {
//                        if (ways[idx].Id == new_way.Id)
//                        { // way was found.
//                            found = true;

//                            ways[idx] = new_way;
//                        }
//                    }

//                    if (!found)
//                    {
//                        ways.Add(new_way);
//                    }
//                }

//                // remove all ways to be removed.
//                foreach (Way way in to_remove)
//                {
//                    ways.Remove(way);
//                }
//            }

//            return ways;
//        }

//        #endregion

//        #region Get Relations

//        /// <summary>
//        /// Returns any relation containing the node.
//        /// </summary>
//        /// <param name="node"></param>
//        /// <returns></returns>
//        public IList<Relation> GetRelationsFor(Node node)
//        {
//            return _source.GetRelationsFor(node);
//        }

//        #endregion

//        internal Node GetNode(long id)
//        {
//            Node node = null;
//            if (!_extra_nodes.TryGetValue(id,out node))
//            {
//                node = _source.GetNode(id);
//            }
//            return node;
//        }

//        internal Way GetWay(long id)
//        {
//            Way way = null;
//            if (!_extra_ways.TryGetValue(id, out way))
//            {
//                way = _source.GetWay(id);
//            }
//            return way;
//        }
//    }
//}
