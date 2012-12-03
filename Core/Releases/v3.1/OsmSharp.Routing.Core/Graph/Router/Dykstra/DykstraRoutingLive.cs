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
using OsmSharp.Routing.Core.Graph;
using OsmSharp.Routing.Core.Graph.Path;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Routing.Core.Constraints;
using OsmSharp.Tools.Math;
using OsmSharp.Routing.Core.Graph.DynamicGraph;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Core.Router;
using OsmSharp.Tools.Core.Collections.PriorityQueues;

namespace OsmSharp.Routing.Core.Graph.Router.Dykstra
{
    /// <summary>
    /// A class containing a dykstra implementation suitable for a simple graph.
    /// </summary>
    /// <typeparam name="EdgeData"></typeparam>
    public class DykstraRoutingLive<EdgeData> : IBasicRouter<EdgeData>
        where EdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the tags index.
        /// </summary>
        private ITagsIndex _tags_index;

        /// <summary>
        /// Creates a new dykstra routing object.
        /// </summary>
        /// <param name="tags_index"></param>
        public DykstraRoutingLive(ITagsIndex tags_index)
        {
            _tags_index = tags_index;
        }

        /// <summary>
        /// Calculates the shortest path from the given vertex to the given vertex given the weights in the graph.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public PathSegment<long> Calculate(IDynamicGraphReadOnly<EdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            PathSegmentVisitList from, PathSegmentVisitList to, double max)
        {
            return this.CalculateToClosest(graph, interpreter, vehicle, from,
                new PathSegmentVisitList[] { to }, max);
        }

        /// <summary>
        /// Calculates the shortest path from the given vertex to the given vertex given the weights in the graph.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public double CalculateWeight(IDynamicGraphReadOnly<EdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            PathSegmentVisitList from, PathSegmentVisitList to, double max)
        {
            return this.CalculateToClosest(graph, interpreter, vehicle, from,
                new PathSegmentVisitList[] { to }, max).Weight;
        }

        /// <summary>
        /// Calculates a shortest path between the source vertex and any of the targets and returns the shortest.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="_interpreter"></param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public PathSegment<long> CalculateToClosest(IDynamicGraphReadOnly<EdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            PathSegmentVisitList from, PathSegmentVisitList[] targets, double max)
        {
            PathSegment<long>[] result = this.DoCalculation(graph, interpreter, vehicle,
                from, targets, max, true, false);
            if (result != null && result.Length == 1)
            {
                return result[0];
            }
            return null;
        }

        /// <summary>
        /// Calculates all routes from a given source to all given targets.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double[] CalculateOneToManyWeight(IDynamicGraphReadOnly<EdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            PathSegmentVisitList source, PathSegmentVisitList[] targets, double max)
        {
            PathSegment<long>[] many = this.DoCalculation(graph, interpreter, vehicle,
                   source, targets, max, false, false);

            double[] weights = new double[many.Length];
            for (int idx = 0; idx < many.Length; idx++)
            {
                if (many[idx] != null)
                {
                    weights[idx] = many[idx].Weight;
                }
                else
                {
                    weights[idx] = double.MaxValue;
                }
            }
            return weights;
        }

        /// <summary>
        /// Calculates all routes from a given sources to all given targets.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double[][] CalculateManyToManyWeight(IDynamicGraphReadOnly<EdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            PathSegmentVisitList[] sources, PathSegmentVisitList[] targets, double max)
        {
            double[][] results = new double[sources.Length][];
            for (int idx = 0; idx < sources.Length; idx++)
            {
                results[idx] = this.CalculateOneToManyWeight(graph, interpreter, vehicle, sources[idx], targets, max);

                // report progress.
                OsmSharp.Tools.Core.Output.OutputStreamHost.ReportProgress(idx, sources.Length, "Router.Core.Graph.Router.Dykstra.DykstraRouting<EdgeData>.CalculateManyToManyWeight",
                    "Calculating weights...");
            }
            return results;
        }

        /// <summary>
        /// Returns true, range calculation is supported.
        /// </summary>
        public bool IsCalculateRangeSupported
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Calculates all points that are at or close to the given weight.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="source"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public HashSet<long> CalculateRange(IDynamicGraphReadOnly<EdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            PathSegmentVisitList source, double weight)
        {
            PathSegment<long>[] result = this.DoCalculation(graph, interpreter, vehicle,
                   source, new PathSegmentVisitList[0], weight, false, true);

            HashSet<long> result_vertices = new HashSet<long>();
            for (int idx = 0; idx < result.Length; idx++)
            {
                result_vertices.Add(result[idx].VertexId);
            }
            return result_vertices;
        }

        /// <summary>
        /// Returns true if the search can move beyond the given weight.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="source"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool CheckConnectivity(IDynamicGraphReadOnly<EdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            PathSegmentVisitList source, double weight)
        {
            HashSet<long> range = this.CalculateRange(graph, interpreter, vehicle, source, weight);

            return range.Count > 0;
        }

        #region Implementation

        /// <summary>
        /// Does dykstra calculation(s) with several options.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <param name="weight"></param>
        /// <param name="stop_at_first"></param>
        /// <param name="return_at_weight"></param>
        /// <returns></returns>
        private PathSegment<long>[] DoCalculation(IDynamicGraphReadOnly<EdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            PathSegmentVisitList source, PathSegmentVisitList[] targets, double weight,
            bool stop_at_first, bool return_at_weight)
        {
            //  initialize the result data structures.
            List<PathSegment<long>> segments_at_weight = new List<PathSegment<long>>();
            PathSegment<long>[] segments_to_target = new PathSegment<long>[targets.Length]; // the resulting target segments.
            long found_targets = 0;

            // intialize dykstra data structures.
            //IPriorityQueue<PathSegment<long>> heap = new FibonacciQueue<PathSegment<long>>();
            IPriorityQueue<PathSegment<long>> heap = new BinairyHeap<PathSegment<long>>();
            HashSet<long> chosen_vertices = new HashSet<long>();
            Dictionary<long, IList<RoutingLabel>> labels = new Dictionary<long, IList<RoutingLabel>>();
            foreach (long vertex in source.GetVertices())
            {
                labels[vertex] = new List<RoutingLabel>();

                PathSegment<long> path = source.GetPathTo(vertex);
                heap.Enqueue(path, (float)path.Weight);
            }

            // set the from node as the current node and put it in the correct data structures.
            // intialize the source's neighbours.
            PathSegment<long> current = heap.Dequeue();
            while (current != null &&
                chosen_vertices.Contains(current.VertexId))
            { // keep dequeuing.
                current = heap.Dequeue();
            }

            // test each target for the source.
            // test each source for any of the targets.
            Dictionary<long, PathSegment<long>> paths_from_source = new Dictionary<long, PathSegment<long>>();
            foreach (long source_vertex in source.GetVertices())
            { // get the path to the vertex.
                PathSegment<long> source_path = source.GetPathTo(source_vertex); // get the source path.
                source_path = source_path.From;
                while (source_path != null)
                { // add the path to the paths from source.
                    paths_from_source[source_path.VertexId] = source_path;
                    source_path = source_path.From;
                }
            }
            // loop over all targets
            for (int idx = 0; idx < targets.Length; idx++)
            { // check for each target if there are paths to the source.
                foreach (long target_vertex in targets[idx].GetVertices())
                {
                    PathSegment<long> target_path = targets[idx].GetPathTo(target_vertex); // get the target path.
                    target_path = target_path.From;
                    while (target_path != null)
                    { // add the path to the paths from source.
                        PathSegment<long> path_from_source;
                        if (paths_from_source.TryGetValue(target_path.VertexId, out path_from_source))
                        { // a path is found.
                            // get the existing path if any.
                            PathSegment<long> existing = segments_to_target[idx];
                            if (existing == null)
                            { // a path did not exist yet!
                                segments_to_target[idx] = target_path.Reverse().ConcatenateAfter(path_from_source);
                                found_targets++;
                            }
                            else if (existing.Weight > target_path.Weight + path_from_source.Weight)
                            { // a new path is found with a lower weight.
                                segments_to_target[idx] = target_path.Reverse().ConcatenateAfter(path_from_source);
                            }
                        }
                        target_path = target_path.From;
                    }
                }
            }
            if (found_targets == targets.Length && targets.Length > 0)
            { // routing is finished!
                return segments_to_target.ToArray();
            }

            if (stop_at_first)
            { // only one entry is needed.
                if (found_targets > 0)
                { // targets found, return the shortest!
                    PathSegment<long> shortest = null;
                    foreach (PathSegment<long> found_target in segments_to_target)
                    {
                        if (shortest == null)
                        {
                            shortest = found_target;
                        }
                        else if (found_target != null &&
                            shortest.Weight > found_target.Weight)
                        {
                            shortest = found_target;
                        }
                    }
                    segments_to_target = new PathSegment<long>[1];
                    segments_to_target[0] = shortest;
                    return segments_to_target;
                }
                else
                { // not targets found yet!
                    segments_to_target = new PathSegment<long>[1];
                }
            }

            // test for identical start/end point.
            for (int idx = 0; idx < targets.Length; idx++)
            {
                PathSegmentVisitList target = targets[idx];
                if (return_at_weight)
                { // add all the reached vertices larger than weight to the results.
                    if (current.Weight > weight)
                    {
                        PathSegment<long> to_path = target.GetPathTo(current.VertexId);
                        to_path.Reverse();
                        to_path = to_path.ConcatenateAfter(current);
                        segments_at_weight.Add(to_path);
                    }
                }
                else if (target.Contains(current.VertexId))
                { // the current is a target!
                    PathSegment<long> to_path = target.GetPathTo(current.VertexId);
                    to_path = to_path.Reverse();
                    to_path = to_path.ConcatenateAfter(current);

                    if (stop_at_first)
                    { // stop at the first occurance.
                        segments_to_target[0] = to_path;
                        return segments_to_target;
                    }
                    else
                    { // normal one-to-many; add to the result.
                        // check if routing is finished.
                        if (segments_to_target[idx] == null)
                        { // make sure only the first route is set.
                            found_targets++;
                            segments_to_target[idx] = to_path;
                            if (found_targets == targets.Length)
                            { // routing is finished!
                                return segments_to_target.ToArray();
                            }
                        }
                        else if (segments_to_target[idx].Weight > to_path.Weight)
                        { // check if the second, third or later is shorter.
                            segments_to_target[idx] = to_path;
                        }
                    }
                }
            }

            // start OsmSharp.Routing.
            KeyValuePair<uint, EdgeData>[] arcs = graph.GetArcs(
                Convert.ToUInt32(current.VertexId));
            chosen_vertices.Add(current.VertexId);

            // loop until target is found and the route is the shortest!
            while (true)
            {
                // get the current labels list (if needed).
                IList<RoutingLabel> current_labels = null;
                if (interpreter.Constraints != null)
                { // there are constraints, get the labels.
                    current_labels = labels[current.VertexId];
                    labels.Remove(current.VertexId);
                }

                // update the visited nodes.
                foreach (KeyValuePair<uint, EdgeData> neighbour in arcs)
                {
                    // check the tags against the interpreter.
                    IDictionary<string, string> tags = _tags_index.Get(neighbour.Value.Tags);
                    if (interpreter.EdgeInterpreter.CanBeTraversedBy(tags, vehicle))
                    { // it's ok; the edge can be traversed by the given vehicle.
                        if (neighbour.Value.Forward &&
                            !chosen_vertices.Contains(neighbour.Key))
                        { // the neigbour is forward and is not settled yet!
                            // check the labels (if needed).
                            bool constraints_ok = true;
                            if (interpreter.Constraints != null)
                            { // check if the label is ok.
                                RoutingLabel neighbour_label = interpreter.Constraints.GetLabelFor(
                                    _tags_index.Get(neighbour.Value.Tags));

                                // only test labels if there is a change.
                                if (current_labels.Count == 0 || !neighbour_label.Equals(current_labels[current_labels.Count - 1]))
                                { // labels are different, test them!
                                    constraints_ok = interpreter.Constraints.ForwardSequenceAllowed(current_labels,
                                        neighbour_label);

                                    if (constraints_ok)
                                    { // update the labels.
                                        List<RoutingLabel> neighbour_labels = new List<RoutingLabel>(current_labels);
                                        neighbour_labels.Add(neighbour_label);

                                        labels[neighbour.Key] = neighbour_labels;
                                    }
                                }
                                else
                                { // set the same label(s).
                                    labels[neighbour.Key] = current_labels;
                                }
                            }

                            if (constraints_ok)
                            { // all constraints are validated or there are none.
                                // calculate neighbours weight.
                                double total_weight = current.Weight + neighbour.Value.Weight;

                                // update the visit list;
                                PathSegment<long> neighbour_route = new PathSegment<long>(neighbour.Key, total_weight, current);
                                heap.Enqueue(neighbour_route, (float)neighbour_route.Weight);
                            }
                        }
                    }
                }

                // while the visit list is not empty.
                current = null;
                if (heap.Count > 0)
                {
                    // choose the next vertex.
                    current = heap.Dequeue();
                    while (current != null &&
                        chosen_vertices.Contains(current.VertexId))
                    { // keep dequeuing.
                        current = heap.Dequeue();
                    }
                    if (current != null)
                    {
                        chosen_vertices.Add(current.VertexId);
                    }
                }
                while (current != null && current.Weight > weight)
                {
                    if (return_at_weight)
                    { // add all the reached vertices larger than weight to the results.
                        segments_at_weight.Add(current);
                    }

                    // choose the next vertex.
                    current = heap.Dequeue();
                    while (current != null &&
                        chosen_vertices.Contains(current.VertexId))
                    { // keep dequeuing.
                        current = heap.Dequeue();
                    }
                }

                if (current == null)
                { // route is not found, there are no vertices left
                    // or the search whent outside of the max bounds.
                    break;
                }

                // check target.
                for (int idx = 0; idx < targets.Length; idx++)
                {
                    PathSegmentVisitList target = targets[idx];
                    if (target.Contains(current.VertexId))
                    { // the current is a target!
                        PathSegment<long> to_path = target.GetPathTo(current.VertexId);
                        to_path = to_path.Reverse();
                        to_path = to_path.ConcatenateAfter(current);

                        if (stop_at_first)
                        { // stop at the first occurance.
                            segments_to_target[0] = to_path;
                            return segments_to_target;
                        }
                        else
                        { // normal one-to-many; add to the result.
                            // check if routing is finished.
                            if (segments_to_target[idx] == null)
                            { // make sure only the first route is set.
                                found_targets++;
                                segments_to_target[idx] = to_path;
                                if (found_targets == targets.Length)
                                { // routing is finished!
                                    return segments_to_target.ToArray();
                                }
                            }
                            else if (segments_to_target[idx].Weight > to_path.Weight)
                            { // check if the second, third or later is shorter.
                                segments_to_target[idx] = to_path;
                            }
                        }
                    }
                }

                // get the neigbours of the current node.
                arcs = graph.GetArcs(Convert.ToUInt32(current.VertexId));
            }

            // return the result.
            if (!return_at_weight)
            {
                return segments_to_target.ToArray();
            }
            return segments_at_weight.ToArray();
        }

        #endregion

        #region Search Closest
        /// <summary>
        /// Searches the data for a point on an edge closest to the given coordinate.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="matcher"></param>
        public SearchClosestResult SearchClosest(IBasicRouterDataSource<EdgeData> graph, VehicleEnum vehicle,
            GeoCoordinate coordinate, IEdgeMatcher matcher, double search_box_size)
        {
            // create the search box.
            GeoCoordinateBox search_box = new GeoCoordinateBox(new GeoCoordinate(
                coordinate.Latitude - search_box_size, coordinate.Longitude - search_box_size),
                                                               new GeoCoordinate(
                coordinate.Latitude + search_box_size, coordinate.Longitude + search_box_size));

            // get the arcs from the data source.
            KeyValuePair<uint, KeyValuePair<uint, EdgeData>>[] arcs = graph.GetArcs(search_box);

            // loop over all.
            SearchClosestResult closest_with_match = new SearchClosestResult(double.MaxValue, 0);
            SearchClosestResult closest_without_match = new SearchClosestResult(double.MaxValue, 0);
            foreach (KeyValuePair<uint, KeyValuePair<uint, EdgeData>> arc in arcs)
            {
                // test the two points.
                float from_latitude, from_longitude;
                float to_latitude, to_longitude;
                double distance;
                if (graph.GetVertex(arc.Key, out from_latitude, out from_longitude) &&
                    graph.GetVertex(arc.Value.Key, out to_latitude, out to_longitude))
                { // return the vertex.
                    GeoCoordinate from_coordinates = new GeoCoordinate(from_latitude, from_longitude);
                    distance = coordinate.Distance(from_coordinates);

                    if (distance < 0.00001)
                    { // the distance is smaller than the tolerance value.
                        closest_without_match = new SearchClosestResult(
                            distance, arc.Key);
                        break;

                        // try and match.
                        //if(matcher.Match(_
                    }

                    if (distance < closest_without_match.Distance)
                    { // the distance is smaller.
                        closest_without_match = new SearchClosestResult(
                            distance, arc.Key);

                        // try and match.
                        //if(matcher.Match(_
                    }
                    GeoCoordinate to_coordinates = new GeoCoordinate(to_latitude, to_longitude);
                    distance = coordinate.Distance(to_coordinates);

                    if (distance < closest_without_match.Distance)
                    { // the distance is smaller.
                        closest_without_match = new SearchClosestResult(
                            distance, arc.Value.Key);

                        // try and match.
                        //if(matcher.Match(_
                    }

                    // create a line.
                    double distance_total = from_coordinates.Distance(to_coordinates);
                    if (distance_total > 0)
                    { // the from/to are not the same location.
                        GeoCoordinateLine line = new GeoCoordinateLine(from_coordinates, to_coordinates, true, true);
                        distance = line.Distance(coordinate);

                        if (distance < closest_without_match.Distance)
                        { // the distance is smaller.
                            PointF2D projected_point =
                                line.ProjectOn(coordinate);

                            // calculate the position.
                            if (projected_point != null)
                            { // calculate the distance
                                double distance_point = from_coordinates.Distance(projected_point);
                                double position = distance_point / distance_total;

                                closest_without_match = new SearchClosestResult(
                                    distance, arc.Key, arc.Value.Key, position);

                                // try and match.
                                //if(matcher.Match(_
                            }
                        }
                    }
                }
            }

            // return the best result.
            if (closest_with_match.Distance < double.MaxValue)
            {
                return closest_with_match;
            }
            return closest_without_match;
        }

        #endregion
    }
}