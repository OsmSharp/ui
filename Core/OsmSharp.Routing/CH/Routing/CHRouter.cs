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
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math.Shapes;
using OsmSharp.Tools.Math.Geo.Factory;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Path;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Graph.DynamicGraph;
using OsmSharp.Routing.Router;
using OsmSharp.Tools.Math;
using OsmSharp.Tools.Collections.PriorityQueues;
using OsmSharp.Routing;
using OsmSharp.Tools.Collections;

namespace OsmSharp.Routing.CH.Routing
{
    /// <summary>
    /// A router for CH.
    /// </summary>
    public class CHRouter : IBasicRouter<CHEdgeData>
    {
        /// <summary>
        /// The CH data.
        /// </summary>
        private IDynamicGraphReadOnly<CHEdgeData> _data;

        /// <summary>
        /// Creates a new CH router on the givend data.
        /// </summary>
        /// <param name="data"></param>
        public CHRouter(IDynamicGraphReadOnly<CHEdgeData> data)
        {
            _data = data;
        }

        /// <summary>
        /// Calculates the shortest path from the given vertex to the given vertex given the weights in the graph.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public PathSegment<long> Calculate(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            PathSegmentVisitList source, PathSegmentVisitList target, double max)
        {
            // do the basic CH calculations.
            CHResult result = this.DoCalculate(graph, interpreter, source, target, max, int.MaxValue, long.MaxValue);

            return this.ExpandBestResult(result);
        }

        /// <summary>
        /// Calculates all routes between all sources and targets.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <param name="max_search"></param>
        /// <param name="graph"></param>
        /// <returns></returns>
        public PathSegment<long>[][] CalculateManyToMany(IBasicRouterDataSource<CHEdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle, 
            PathSegmentVisitList[] sources, PathSegmentVisitList[] targets, double max_search)
        {
            PathSegment<long>[][] results = new PathSegment<long>[sources.Length][];
            for (int source_idx = 0; source_idx < sources.Length; source_idx++)
            {
                results[source_idx] = new PathSegment<long>[targets.Length];
                for (int target_idx = 0; target_idx < targets.Length; target_idx++)
                {
                    results[source_idx][target_idx] =
                        this.Calculate(graph, interpreter, vehicle, sources[source_idx], targets[target_idx], max_search);
                }
            }

            return results;
        }


        /// <summary>
        /// Calculates the weight of shortest path from the given vertex to the given vertex given the weights in the graph.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double CalculateWeight(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            PathSegmentVisitList source, PathSegmentVisitList target, double max)
        {
            // do the basic CH calculations.
            CHResult result = this.DoCalculate(graph, interpreter, source, target, max, int.MaxValue, long.MaxValue);

            if (result.Backward != null && result.Forward != null)
            {
                return result.Backward.Weight + result.Forward.Weight;
            }
            return double.MaxValue;
        }

        /// <summary>
        /// Calculate route to the closest.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public PathSegment<long> CalculateToClosest(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            PathSegmentVisitList source, PathSegmentVisitList[] targets, double max)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calculates all weights from one source to multiple targets.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double[] CalculateOneToManyWeight(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            PathSegmentVisitList source, PathSegmentVisitList[] targets, double max)
        {
            double[][] many_to_many_result = this.CalculateManyToManyWeight(
                graph, interpreter, vehicle, new PathSegmentVisitList[] { source }, targets, max);

            return many_to_many_result[0];
        }

        /// <summary>
        /// Calculates all weights from multiple sources to multiple targets.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double[][] CalculateManyToManyWeight(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            PathSegmentVisitList[] sources, PathSegmentVisitList[] targets, double max)
        {
            return this.DoCalculateManyToMany(
                   graph, interpreter, sources, targets, max, int.MaxValue);
        }

        /// <summary>
        /// Returns false.
        /// </summary>
        public bool IsCalculateRangeSupported
        {
            get { return false; }
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public HashSet<long> CalculateRange(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            PathSegmentVisitList source, double weight)
        {
            throw new NotSupportedException("Check IsCalculateRangeSupported before using this functionality!");
        }

        /// <summary>
        /// Returns true if the search can move beyond the given weight.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool CheckConnectivity(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            PathSegmentVisitList source, double weight)
        {
            return this.DoCheckConnectivity(source, weight, int.MaxValue);
        }

        #region Implementation

        #region Bi-directional Many-to-Many

        ///// <summary>
        ///// Calculates all the weights between all the vertices.
        ///// </summary>
        ///// <param name="froms"></param>
        ///// <param name="tos"></param>
        ///// <returns></returns>
        //public float[][] CalculateManyToManyWeights(uint[] froms, uint[] tos)
        //{
        //    // TODO: implement switching of from/to when to < from.

        //    // keep a list of distances to the given vertices while performance backward search.
        //    Dictionary<uint, Dictionary<uint, float>> buckets = new Dictionary<uint, Dictionary<uint, float>>();
        //    for (int idx = 0; idx < tos.Length; idx++)
        //    {
        //        this.SearchBackwardIntoBucket(buckets, tos[idx]);

        //        // report progress.
        //        OsmSharp.Tools.Output.OutputStreamHost.ReportProgress(idx, tos.Length, "Router.CH.CalculateManyToManyWeights",
        //            "Calculating backward...");
        //    }

        //    // conduct a forward search from each source.
        //    float[][] weights = new float[froms.Length][];
        //    for (int idx = 0; idx < froms.Length; idx++)
        //    {
        //        uint from = froms[idx];

        //        // calculate all from's.
        //        Dictionary<uint, float> result =
        //            this.SearchForwardFromBucket(buckets, from, tos);

        //        float[] to_weights = new float[tos.Length];
        //        for (int to_idx = 0; to_idx < tos.Length; to_idx++)
        //        {
        //            to_weights[to_idx] = result[tos[to_idx]];
        //        }

        //        weights[idx] = to_weights;
        //        result.Clear();

        //        // report progress.
        //        OsmSharp.Tools.Output.OutputStreamHost.ReportProgress(idx, tos.Length, "Router.CH.CalculateManyToManyWeights",
        //            "Calculating forward...");
        //    }
        //    return weights;
        //}

        /// <summary>
        /// Searches backwards and puts the weigths from the to-vertex into the buckets list.
        /// </summary>
        /// <param name="buckets"></param>
        /// <param name="to_visit_list"></param>
        /// <returns></returns>
        private long SearchBackwardIntoBucket(Dictionary<long, Dictionary<long, double>> buckets, 
            PathSegmentVisitList to_visit_list)
        {
            long? to = null;
            Dictionary<long, PathSegment<long>> settled_vertices =
                new Dictionary<long, PathSegment<long>>();
            IPriorityQueue<PathSegment<long>> queue = new BinairyHeap<PathSegment<long>>();
            foreach (long vertex in to_visit_list.GetVertices())
            {
                PathSegment<long> path = to_visit_list.GetPathTo(vertex);
                if (!to.HasValue)
                {
                    to = path.First().VertexId;
                }
                queue.Push(path, (float)path.Weight);

                // also add the from paths.
                path = path.From;
                while (path != null)
                { // keep adding paths.
                    Dictionary<long, double> bucket = null;
                    if (buckets.TryGetValue(to.Value, out bucket))
                    { // an existing bucket was found!
                        double existing_weight;
                        if (!bucket.TryGetValue(path.VertexId, out existing_weight) ||
                            existing_weight > path.Weight)
                        { // there already exists a weight 
                            bucket.Add(path.VertexId, path.Weight);
                        }
                    }
                    else
                    { // add new bucket.
                        bucket = new Dictionary<long, double>();
                        bucket.Add(path.VertexId, path.Weight);
                        buckets.Add(to.Value, bucket);
                    }
                    path = path.From; // get the next one.
                }
            }

            // get the current vertex with the smallest weight.
            while (queue.Count > 0) // TODO: work on a stopping condition?
            {
                //PathSegment<long> current = queue.Pop();
                PathSegment<long> current = queue.Pop();
                while (current != null && settled_vertices.ContainsKey(current.VertexId))
                {
                    current = queue.Pop();
                }

                if (current != null)
                { // a next vertex was found!
                    // add to the settled vertices.
                    PathSegment<long> previous_linked_route;
                    if (settled_vertices.TryGetValue(current.VertexId, out previous_linked_route))
                    {
                        if (previous_linked_route.Weight > current.Weight)
                        {
                            // settle the vertex again if it has a better weight.
                            settled_vertices[current.VertexId] = current;
                        }
                    }
                    else
                    {
                        // settled the vertex.
                        settled_vertices[current.VertexId] = current;
                    }

                    // add to bucket.
                    Dictionary<long, double> bucket;
                    if (!buckets.TryGetValue(current.VertexId, out bucket))
                    {
                        bucket = new Dictionary<long, double>();
                        buckets.Add(Convert.ToUInt32(current.VertexId), bucket);
                    }
                    bucket[to.Value] = current.Weight;

                    // get neighbours.
                    KeyValuePair<uint, CHEdgeData>[] neighbours = _data.GetArcs(Convert.ToUInt32(current.VertexId));

                    // add the neighbours to the queue.
                    foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours.Where<KeyValuePair<uint, CHEdgeData>>(
                        a => a.Value.Backward))
                    {
                        if (!settled_vertices.ContainsKey(neighbour.Key))
                        {
                            // if not yet settled.
                            PathSegment<long> route_to_neighbour = new PathSegment<long>(
                                neighbour.Key, current.Weight + neighbour.Value.Weight, current);
                            queue.Push(route_to_neighbour, (float)route_to_neighbour.Weight);
                            //queue.Push(route_to_neighbour);
                        }
                    }
                }
            }

            return to.Value;
        }

        /// <summary>
        /// Searches forward and uses the bucket to calculate smallest weights.
        /// </summary>
        /// <param name="buckets"></param>
        /// <param name="from_visit_list"></param>
        /// <param name="tos"></param>
        private Dictionary<long, double> SearchForwardFromBucket(Dictionary<long, Dictionary<long, double>> buckets,
            PathSegmentVisitList from_visit_list, long[] tos)
        {
            long? from = null;
            // intialize weights.
            Dictionary<long, double> results = new Dictionary<long, double>();
            //HashSet<long> permanent_results = new HashSet<long>();
            Dictionary<long, double> tentative_results = new Dictionary<long, double>();

            Dictionary<long, PathSegment<long>> settled_vertices =
                new Dictionary<long, PathSegment<long>>();
            //CHPriorityQueue queue = new CHPriorityQueue();
            IPriorityQueue<PathSegment<long>> queue = new BinairyHeap<PathSegment<long>>();
            foreach (long vertex in from_visit_list.GetVertices())
            {
                PathSegment<long> path = from_visit_list.GetPathTo(vertex);
                if (!from.HasValue)
                {
                    from = path.First().VertexId;
                }
                queue.Push(path, (float)path.Weight);

                // also add the from paths.
                path = path.From;
                while (path != null)
                { // keep adding paths.
                    // search the bucket.
                    Dictionary<long, double> bucket;
                    if (buckets.TryGetValue(path.VertexId, out bucket))
                    {
                        // there is a bucket!
                        foreach (KeyValuePair<long, double> bucket_entry in bucket)
                        {
                            double found_distance = bucket_entry.Value + path.Weight;
                            double tentative_distance;
                            if (tentative_results.TryGetValue(bucket_entry.Key, out tentative_distance))
                            {
                                if (found_distance < tentative_distance)
                                {
                                    tentative_results[bucket_entry.Key] = found_distance;
                                }

                                //if (tentative_distance < current.Weight)
                                //{
                                //    tentative_results.Remove(bucket_entry.Key);
                                //    results[bucket_entry.Key] = tentative_distance;
                                //}
                            }
                            else
                            { // there was no result yet!
                                tentative_results[bucket_entry.Key] = found_distance;
                            }
                        }
                    }

                    path = path.From; // get the next one.
                }
            }

            // get the current vertex with the smallest weight.
            int k = 0;
            while (queue.Count > 0) // TODO: work on a stopping condition?
            {
                //PathSegment<long> current = queue.Pop();
                PathSegment<long> current = queue.Pop();
                while (current != null && settled_vertices.ContainsKey(current.VertexId))
                {
                    current = queue.Pop();
                }

                if (current != null)
                { // a next vertex was found!
                    k++;

                    // stop search if all results found.
                    if (results.Count == tos.Length)
                    {
                        break;
                    }
                    // add to the settled vertices.
                    PathSegment<long> previous_linked_route;
                    if (settled_vertices.TryGetValue(current.VertexId, out previous_linked_route))
                    {
                        if (previous_linked_route.Weight > current.Weight)
                        {
                            // settle the vertex again if it has a better weight.
                            settled_vertices[current.VertexId] = current;
                        }
                    }
                    else
                    {
                        // settled the vertex.
                        settled_vertices[current.VertexId] = current;
                    }

                    // search the bucket.
                    Dictionary<long, double> bucket;
                    if (buckets.TryGetValue(current.VertexId, out bucket))
                    {
                        // there is a bucket!
                        foreach (KeyValuePair<long, double> bucket_entry in bucket)
                        {
                            //if (!permanent_results.Contains(bucket_entry.Key))
                            //{
                            double found_distance = bucket_entry.Value + current.Weight;
                            double tentative_distance;
                            if (tentative_results.TryGetValue(bucket_entry.Key, out tentative_distance))
                            {
                                if (found_distance < tentative_distance)
                                {
                                    tentative_results[bucket_entry.Key] = found_distance;
                                }

                                if (tentative_distance < current.Weight)
                                {
                                    tentative_results.Remove(bucket_entry.Key);
                                    results[bucket_entry.Key] = tentative_distance;
                                }
                            }
                            else if(!results.ContainsKey(bucket_entry.Key))
                            { // there was no result yet!
                                tentative_results[bucket_entry.Key] = found_distance;
                            }
                            //}
                        }
                    }

                    // get neighbours.
                    KeyValuePair<uint, CHEdgeData>[] neighbours = _data.GetArcs(Convert.ToUInt32(current.VertexId));

                    // add the neighbours to the queue.
                    foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours.Where<KeyValuePair<uint, CHEdgeData>>(
                        a => a.Value.Forward))
                    {
                        if (!settled_vertices.ContainsKey(neighbour.Key))
                        {
                            // if not yet settled.
                            PathSegment<long> route_to_neighbour = new PathSegment<long>(
                                neighbour.Key, current.Weight + neighbour.Value.Weight, current);
                            queue.Push(route_to_neighbour, (float)route_to_neighbour.Weight);
                        }
                    }
                }

                //foreach (uint to in tos)
                //{
                //    if (!tentative_results.ContainsKey(to))
                //    {
                //        if (results.ContainsKey(to))
                //        {
                //            tentative_results[to] = results[to];
                //        }
                //        else
                //        {
                //            tentative_results[to] = double.MaxValue;
                //        }
                //    }
                //}
            }

            foreach (uint to in tos)
            {
                if (!results.ContainsKey(to) && tentative_results.ContainsKey(to))
                {
                    results[to] = tentative_results[to];
                }
            }

            return results;
        }

        #endregion

        #region Bi-directional Point-To-Point

        /// <summary>
        /// Calculates a shortest path between the two given vertices.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="max"></param>
        /// <param name="max_settles"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private CHResult DoCalculate(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter,
            PathSegmentVisitList source, PathSegmentVisitList target, double max, int max_settles, long exception)
        {
            // keep settled vertices.
            CHQueue settled_vertices = new CHQueue();

            // initialize the queues.
            IPriorityQueue<PathSegment<long>> queue_forward = new BinairyHeap<PathSegment<long>>();
            IPriorityQueue<PathSegment<long>> queue_backward = new BinairyHeap<PathSegment<long>>();
            //CHPriorityQueue queue_forward = new CHPriorityQueue();
            //CHPriorityQueue queue_backward = new CHPriorityQueue();

            // add the sources to the forward queue.
            Dictionary<long, PathSegment<long>> resolved_settles = 
                new Dictionary<long, PathSegment<long>>();
            foreach (long source_vertex in source.GetVertices())
            {
                PathSegment<long> path = source.GetPathTo(source_vertex);
                queue_forward.Push(path, (float)path.Weight);
                path = path.From;
                while (path != null)
                { // keep looping.
                    PathSegment<long> existing_source = null;
                    if (!resolved_settles.TryGetValue(path.VertexId, out existing_source) ||
                        existing_source.Weight > path.Weight)
                    { // the new path is better.
                        resolved_settles[path.VertexId] = path;
                    }
                    path = path.From;
                }
            }

            // add the sources to the settled vertices.
            foreach (KeyValuePair<long, PathSegment<long>> resolved_settled 
                in resolved_settles)
            {
                settled_vertices.AddForward(resolved_settled.Value);
            }

            // add the to(s) vertex to the backward queue.resolved_settles = 
            resolved_settles = new Dictionary<long, PathSegment<long>>();
            foreach (long target_vertex in target.GetVertices())
            {
                PathSegment<long> path = target.GetPathTo(target_vertex);
                queue_backward.Push(path, (float)path.Weight);
                path = path.From;
                while (path != null)
                { // keep looping.
                    PathSegment<long> existing_source = null;
                    if (!resolved_settles.TryGetValue(path.VertexId, out existing_source) ||
                        existing_source.Weight > path.Weight)
                    { // the new path is better.
                        resolved_settles[path.VertexId] = path;
                    }
                    path = path.From;
                }
            }

            // add the sources to the settled vertices.
            foreach (KeyValuePair<long, PathSegment<long>> resolved_settled
                in resolved_settles)
            {
                settled_vertices.AddBackward(resolved_settled.Value);
            }

            // keep looping until stopping conditions are met.
            CHBest best = this.CalculateBest(settled_vertices);

            // calculate stopping conditions.
            double queue_backward_weight = queue_backward.PeekWeight();
            double queue_forward_weight = queue_forward.PeekWeight();
            while (true)
            { // keep looping until stopping conditions.
                if (queue_backward.Count == 0 && queue_forward.Count == 0)
                { // stop the search; both queues are empty.
                    break;
                }
                if (max < queue_backward_weight && max < queue_forward_weight)
                { // stop the search: the max search weight has been reached.
                    break;
                }
                if (max_settles < (settled_vertices.Forward.Count + settled_vertices.Backward.Count))
                { // stop the search: the max settles cound has been reached.
                    break;
                }
                if (best.Weight < queue_forward_weight && best.Weight < queue_backward_weight)
                { // stop the search: it now became impossible to find a shorter route.
                    break;
                }

                // do a forward search.
                if (queue_forward.Count > 0)
                {
                    this.SearchForward(settled_vertices, queue_forward, exception);
                }

                // do a backward search.
                if (queue_backward.Count > 0)
                {
                    this.SearchBackward(settled_vertices, queue_backward, exception);
                }

                // calculate the new best if any.
                best = this.CalculateBest(settled_vertices);

                // calculate stopping conditions.
                if (queue_forward.Count > 0)
                {
                    queue_forward_weight = queue_forward.PeekWeight();
                }
                if (queue_backward.Count > 0)
                {
                    queue_backward_weight = queue_backward.PeekWeight();
                }
            }

            // return forward/backward routes.
            CHResult result = new CHResult();
            if (!best.Found)
            {
                // no route was found!
            }
            else
            {
                // construct the existing route.
                result.Forward = settled_vertices.Forward[best.VertexId];
                result.Backward = settled_vertices.Backward[best.VertexId];
            }
            return result;
        }

        /// <summary>
        /// Calculates all shortest paths between the given vertices.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <param name="max_settles"></param>
        /// <returns></returns>
        private double[][] DoCalculateManyToMany(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter,
            PathSegmentVisitList[] sources, PathSegmentVisitList[] targets, double max, int max_settles)
        {
            // TODO: implement switching of from/to when to < from.

            // keep a list of distances to the given vertices while doing backward search.
            Dictionary<long, Dictionary<long, double>> buckets = new Dictionary<long, Dictionary<long, double>>();
            long[] target_ids = new long[sources.Length];
            for (int idx = 0; idx < sources.Length; idx++)
            {
                target_ids[idx] = 
                    this.SearchBackwardIntoBucket(buckets, sources[idx]);

                // report progress.
                OsmSharp.Tools.Output.OutputStreamHost.ReportProgress(idx, targets.Length, "Router.CH.CalculateManyToManyWeights",
                    "Calculating backward...");
            }

            // conduct a forward search from each source.
            double[][] weights = new double[sources.Length][];
            for (int idx = 0; idx < sources.Length; idx++)
            {
                // calculate all from's.
                Dictionary<long, double> result =
                    this.SearchForwardFromBucket(buckets, sources[idx], target_ids);

                double[] to_weights = new double[target_ids.Length];
                for (int to_idx = 0; to_idx < target_ids.Length; to_idx++)
                {
                    if (result.ContainsKey(target_ids[to_idx]))
                    {
                        to_weights[to_idx] = result[target_ids[to_idx]];
                    }
                }

                weights[idx] = to_weights;
                result.Clear();

                // report progress.
                OsmSharp.Tools.Output.OutputStreamHost.ReportProgress(idx, sources.Length, "Router.CH.CalculateManyToManyWeights",
                    "Calculating forward...");
            }
            return weights;
        }

        ///// <summary>
        ///// Calculates the actual route from from to to.
        ///// </summary>
        ///// <param name="from"></param>
        ///// <param name="to"></param>
        ///// <returns></returns>
        //public PathSegment<long> Calculate(uint from, uint to)
        //{
        //    // calculate the result.
        //    CHResult result = this.CalculateInternal(from, to, 0, double.MaxValue, int.MaxValue);

        //    // construct the route.
        //    PathSegment<long> route = result.Forward;
        //    PathSegment<long> next = result.Backward;
        //    while (next != null && next.From != null)
        //    {
        //        route = new PathSegment<long>(next.From.VertexId,
        //                                  next.Weight + route.Weight, route);
        //        next = next.From;
        //    }

        //    // report a path segment.
        //    this.NotifyPathSegment(route);

        //    return this.ExpandPath(route);
        //}

        ///// <summary>
        ///// Calculates the weight from from to to.
        ///// </summary>
        ///// <param name="from"></param>
        ///// <param name="to"></param>
        ///// <returns></returns>
        //public double CalculateWeight(uint from, uint to)
        //{
        //    // calculate the result.
        //    CHResult result = this.CalculateInternal(from, to, 0, double.MaxValue, int.MaxValue);

        //    // construct the route.
        //    return result.Forward.Weight + result.Backward.Weight;
        //}

        /// <summary>
        /// Calculates the weight from from to to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public double CalculateWeight(uint from, uint to, uint exception)
        {
            return this.CalculateWeight(from, to, exception, double.MaxValue);
        }

        /// <summary>
        /// Calculates the weight from from to to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="exception"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double CalculateWeight(uint from, uint to, uint exception, double max)
        {
            // calculate the result.
            CHResult result = this.CalculateInternal(from, to, exception, max, int.MaxValue);

            // construct the route.
            if (result.Forward != null && result.Backward != null)
            {
                return result.Forward.Weight + result.Backward.Weight;
            }
            return double.MaxValue;
        }

        /// <summary>
        /// Calculates the weight from from to to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="exception"></param>
        /// <param name="max"></param>
        /// <param name="max_settles"></param>
        /// <returns></returns>
        public double CalculateWeight(uint from, uint to, uint exception, double max, int max_settles)
        {
            // calculate the result.
            CHResult result = this.CalculateInternal(from, to, exception, max, max_settles);

            // construct the route.
            if (result.Forward != null && result.Backward != null)
            {
                return result.Forward.Weight + result.Backward.Weight;
            }
            return double.MaxValue;
        }

        /// <summary>
        /// Checks connectivity of a vertex.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public bool CheckConnectivity(PathSegmentVisitList source, double max)
        {
            return this.DoCheckConnectivity(source, max, int.MaxValue);
        }

        /// <summary>
        /// Checks connectivity of a vertex.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="max_settles"></param>
        /// <returns></returns>
        public bool CheckConnectivity(PathSegmentVisitList source, int max_settles)
        {
            return this.DoCheckConnectivity(source, double.MaxValue, max_settles);
        }

        /// <summary>
        /// Calculates a shortest path between the two given vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="exception"></param>
        /// <param name="max"></param>
        /// <param name="max_settles"></param>
        /// <returns></returns>
        private CHResult CalculateInternal(uint from, uint to, uint exception, double max, int max_settles)
        {
            // keep settled vertices.
            CHQueue settled_vertices = new CHQueue();

            // initialize the queues.
            IPriorityQueue<PathSegment<long>> queue_forward = new BinairyHeap<PathSegment<long>>();
            IPriorityQueue<PathSegment<long>> queue_backward = new BinairyHeap<PathSegment<long>>();

            // add the from vertex to the forward queue.
            queue_forward.Push(new PathSegment<long>(from), 0);

            // add the from vertex to the backward queue.
            queue_backward.Push(new PathSegment<long>(to), 0);

            // keep looping until stopping conditions are met.
            CHBest best = this.CalculateBest(settled_vertices);

            // calculate stopping conditions.
            double queue_backward_weight = queue_backward.PeekWeight();
            double queue_forward_weight = queue_forward.PeekWeight();
            while (true)
            { // keep looping until stopping conditions.
                if (queue_backward.Count == 0 && queue_forward.Count == 0)
                { // stop the search; both queues are empty.
                    break;
                }
                if (max < queue_backward_weight && max < queue_forward_weight)
                { // stop the search: the max search weight has been reached.
                    break;
                }
                if (max_settles < (settled_vertices.Forward.Count + settled_vertices.Backward.Count))
                { // stop the search: the max settles cound has been reached.
                    break;
                }
                if (best.Weight < queue_forward_weight && best.Weight < queue_backward_weight)
                { // stop the search: it now became impossible to find a shorter route.
                    break;
                }

                // do a forward search.
                if (queue_forward.Count > 0)
                {
                    this.SearchForward(settled_vertices, queue_forward, exception);
                }

                // do a backward search.
                if (queue_backward.Count > 0)
                {
                    this.SearchBackward(settled_vertices, queue_backward, exception);
                }

                // calculate the new best if any.
                best = this.CalculateBest(settled_vertices);

                // calculate stopping conditions.
                if (queue_forward.Count > 0)
                {
                    queue_forward_weight = queue_forward.PeekWeight();
                }
                if (queue_backward.Count > 0)
                {
                    queue_backward_weight = queue_backward.PeekWeight();
                }
            }

            // return forward/backward routes.
            CHResult result = new CHResult();
            if (!best.Found)
            {
                // no route was found!
            }
            else
            {
                // construct the existing route.
                result.Forward = settled_vertices.Forward[best.VertexId];
                result.Backward = settled_vertices.Backward[best.VertexId];
            }
            return result;
        }

        /// <summary>
        /// Checks if the given vertex is connected to others.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="max"></param>
        /// <param name="max_settles"></param>
        /// <returns></returns>
        private bool DoCheckConnectivity(PathSegmentVisitList source, double max, int max_settles)
        {
            // keep settled vertices.
            CHQueue settled_vertices = new CHQueue();

            // initialize the queues.
            IPriorityQueue<PathSegment<long>> queue_forward = new BinairyHeap<PathSegment<long>>();
            IPriorityQueue<PathSegment<long>> queue_backward = new BinairyHeap<PathSegment<long>>();
            //CHPriorityQueue queue_forward = new CHPriorityQueue();
            //CHPriorityQueue queue_backward = new CHPriorityQueue();

            // add the sources to the forward queue.
            foreach (long source_vertex in source.GetVertices())
            {
                PathSegment<long> path = source.GetPathTo(source_vertex);
                queue_forward.Push(path, (float)path.Weight);
                //queue_forward.Push(source.GetPathTo(source_vertex));
            }

            // add the to(s) vertex to the backward queue.
            foreach (long target_vertex in source.GetVertices())
            {
                PathSegment<long> path = source.GetPathTo(target_vertex);
                queue_backward.Push(path, (float)path.Weight);
                //queue_backward.Push(source.GetPathTo(target_vertex));
            }

            // calculate stopping conditions.
            double queue_backward_weight = queue_backward.PeekWeight();
            double queue_forward_weight = queue_forward.PeekWeight();
            while (true) // when the queue is empty the connectivity test fails!
            { // keep looping until stopping conditions.
                if(queue_backward.Count == 0 && queue_forward.Count == 0)
                { // stop the search; both queues are empty.
                    break;
                }
                if(max < queue_backward_weight && max < queue_forward_weight)
                { // stop the search: the max search weight has been reached.
                    break;
                }
                if(max_settles < (settled_vertices.Forward.Count + settled_vertices.Backward.Count))
                { // stop the search: the max settles cound has been reached.
                    break;
                }

                // do a forward search.
                if (queue_forward.Count > 0)
                {
                    this.SearchForward(settled_vertices, queue_forward, -1);
                }

                // do a backward search.
                if (queue_backward.Count > 0)
                {
                    this.SearchBackward(settled_vertices, queue_backward, -1);
                }

                // calculate stopping conditions.
                if (queue_forward.Count > 0)
                {
                    queue_forward_weight = queue_forward.PeekWeight();
                }
                if (queue_backward.Count > 0)
                {
                    queue_backward_weight = queue_backward.PeekWeight();
                }
            }
            return (max <= queue_backward_weight && max <= queue_forward_weight) || // the search has continued until both weights exceed the maximum.
                max_settles <= (settled_vertices.Forward.Count + settled_vertices.Backward.Count); // or until the max settled vertices have been reached.
        }

        /// <summary>
        /// Test stopping conditions and output the best tentative route.
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        private CHBest CalculateBest(CHQueue queue)
        {
            CHBest best = new CHBest();
            best.VertexId = 0;
            best.Weight = double.MaxValue;

            // loop over all intersections.
            foreach (KeyValuePair<long, double> vertex in queue.Intersection)
            {
                double weight = vertex.Value;
                if (weight < best.Weight)
                {
                    best = new CHBest();
                    best.VertexId = vertex.Key;
                    best.Found = true;
                    best.Weight = weight;
                }
            }
            return best;
        }

        /// <summary>
        /// Holds the result.
        /// </summary>
        private struct CHBest
        {
            /// <summary>
            /// The vertex in the 'middle' of the best route yet.
            /// </summary>
            public long VertexId { get; set; }

            /// <summary>
            /// The weight of the best route yet.
            /// </summary>
            public double Weight { get; set; }

            /// <summary>
            /// The result that was found.
            /// </summary>
            public bool Found { get; set; }
        }

        /// <summary>
        /// Do one forward search step.
        /// </summary>
        /// <param name="settled_queue"></param>
        /// <param name="queue"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private void SearchForward(CHQueue settled_queue, IPriorityQueue<PathSegment<long>> queue,
                                   long exception)
        {
            // get the current vertex with the smallest weight.
            PathSegment<long> current = queue.Pop();
            while (current != null && settled_queue.Forward.ContainsKey(
                current.VertexId))
            { // keep trying.
                current = queue.Pop();
            }

            if (current != null)
            { // there is a next vertex found.
                // add to the settled vertices.
                PathSegment<long> previous_linked_route;
                if (settled_queue.Forward.TryGetValue(current.VertexId, out previous_linked_route))
                {
                    if (previous_linked_route.Weight > current.Weight)
                    {
                        // settle the vertex again if it has a better weight.
                        settled_queue.AddForward(current);
                    }
                }
                else
                {
                    // settled the vertex.
                    settled_queue.AddForward(current);
                }

                // get neighbours.
                KeyValuePair<uint, CHEdgeData>[] neighbours = _data.GetArcs(Convert.ToUInt32(current.VertexId));

                // add the neighbours to the queue.
                foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours)
                {
                    if (neighbour.Value.Forward &&
                        !settled_queue.Forward.ContainsKey(neighbour.Key) &&
                        (exception == 0 || (exception != neighbour.Key &&
                        exception != neighbour.Value.ContractedVertexId)))
                    {
                        // if not yet settled.
                        PathSegment<long> route_to_neighbour = new PathSegment<long>(
                            neighbour.Key, current.Weight + neighbour.Value.Weight, current);
                        //queue.Push(route_to_neighbour);
                        queue.Push(route_to_neighbour, (float)route_to_neighbour.Weight);
                    }
                    else if (neighbour.Value.Forward &&
                        (exception == 0 || (exception != neighbour.Key &&
                        exception != neighbour.Value.ContractedVertexId)))
                    {
                        // node was settled before: make sure this route is not shorter.
                        PathSegment<long> route_to_neighbour = new PathSegment<long>(
                            neighbour.Key, current.Weight + neighbour.Value.Weight, current);

                        // remove from the queue again when there is a shorter route found.
                        if (settled_queue.Forward[neighbour.Key].Weight > route_to_neighbour.Weight)
                        {
                            settled_queue.Forward.Remove(neighbour.Key);
                            //queue.Push(route_to_neighbour);
                            queue.Push(route_to_neighbour, (float)route_to_neighbour.Weight);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Do one backward search step.
        /// </summary>
        /// <param name="settled_queue"></param>
        /// <param name="queue"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private void SearchBackward(CHQueue settled_queue, IPriorityQueue<PathSegment<long>> queue,
                                    long exception)
        {
            // get the current vertex with the smallest weight.
            //PathSegment<long> current = queue.Pop();
            PathSegment<long> current = queue.Pop();
            while (current != null && settled_queue.Backward.ContainsKey(
                current.VertexId))
            { // keep trying.
                current = queue.Pop();
            }

            if (current != null)
            {
                // add to the settled vertices.
                PathSegment<long> previous_linked_route;
                if (settled_queue.Backward.TryGetValue(current.VertexId, out previous_linked_route))
                {
                    if (previous_linked_route.Weight > current.Weight)
                    {
                        // settle the vertex again if it has a better weight.
                        settled_queue.AddBackward(current);
                    }
                }
                else
                {
                    // settled the vertex.
                    settled_queue.AddBackward(current);
                }

                // get neighbours.
                KeyValuePair<uint, CHEdgeData>[] neighbours = _data.GetArcs(Convert.ToUInt32(current.VertexId));

                // add the neighbours to the queue.
                foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours)
                {
                    if (neighbour.Value.Backward &&
                        !settled_queue.Backward.ContainsKey(neighbour.Key)
                        && (exception == 0 || (exception != neighbour.Key &&
                        exception != neighbour.Value.ContractedVertexId)))
                    {
                        // if not yet settled.
                        PathSegment<long> route_to_neighbour = new PathSegment<long>(
                            neighbour.Key, current.Weight + neighbour.Value.Weight, current);
                        //queue.Push(route_to_neighbour);
                        queue.Push(route_to_neighbour, (float)route_to_neighbour.Weight);
                    }
                    else if (neighbour.Value.Backward &&
                        (exception == 0 || (exception != neighbour.Key &&
                        exception != neighbour.Value.ContractedVertexId)))
                    {
                        // node was settled before: make sure this route is not shorter.
                        PathSegment<long> route_to_neighbour = new PathSegment<long>(
                            neighbour.Key, current.Weight + neighbour.Value.Weight, current);

                        // remove from the queue again when there is a shorter route found.
                        if (settled_queue.Backward[neighbour.Key].Weight > route_to_neighbour.Weight)
                        {
                            settled_queue.Backward.Remove(neighbour.Key);
                            queue.Push(route_to_neighbour, (float)route_to_neighbour.Weight);
                            //queue.Push(route_to_neighbour);
                        }
                    }
                }
            }
        }

        #endregion

        #region Path Expansion

        /// <summary>
        /// Expands a ch results into an expanded path segment.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private PathSegment<long> ExpandBestResult(CHResult result)
        {
            // construct the route.
            PathSegment<long> route = result.Forward;
            PathSegment<long> next = result.Backward;
            while (next != null && next.From != null)
            {
                route = new PathSegment<long>(next.From.VertexId,
                                          next.Weight + route.Weight, route);
                next = next.From;
            }

            // expand the CH path to a regular path.
            return this.ExpandPath(route);
        }

        /// <summary>
        /// Converts the CH paths to complete paths in the orginal network.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private PathSegment<long> ExpandPath(PathSegment<long> path)
        {
            // construct the full CH path.
            PathSegment<long> current = path;
            PathSegment<long> expanded_path = null;

            if (current != null && current.From == null)
            { // path contains just a single point.
                expanded_path = current;
            }
            else
            { // path containts at least two points or none at all.
                while (current != null && current.From != null)
                {
                    // recursively convert edge.
                    PathSegment<long> local_path =
                        new PathSegment<long>(current.VertexId, -1, new PathSegment<long>(
                                                                    current.From.VertexId));
                    PathSegment<long> expanded_arc = this.ConvertArc(local_path);
                    if (expanded_path != null)
                    {
                        expanded_path = expanded_path.ConcatenateAfter(expanded_arc);
                    }
                    else
                    {
                        expanded_path = expanded_arc;
                    }

                    current = current.From;
                }
            }
            return expanded_path;
        }

        /// <summary>
        /// Converts the given edge and expands it if needed.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        private PathSegment<long> ConvertArc(PathSegment<long> edge)
        {
            if (edge.VertexId < 0 || edge.From.VertexId < 0)
            { // these edges are not part of the regular network!
                return edge;
            }

            // get the edge by querying the forward neighbours of the from-vertex.
            //CHVertex from_vertex = _data.GetCHVertex(edge.From.VertexId);
            KeyValuePair<uint, CHEdgeData>[] neighbours = _data.GetArcs(Convert.ToUInt32(edge.From.VertexId));

            // find the edge with lowest weight.
            KeyValuePair<uint, CHEdgeData> arc = new KeyValuePair<uint, CHEdgeData>(0, null);
            foreach (KeyValuePair<uint, CHEdgeData> forward_arc in neighbours.Where<KeyValuePair<uint, CHEdgeData>>(
                a => a.Key == edge.VertexId && a.Value.Forward))
            {
                if (arc.Value == null)
                {
                    arc = forward_arc;
                }
                else if (arc.Value.Weight > forward_arc.Value.Weight)
                {
                    arc = forward_arc;
                }
            }
            if (arc.Value == null)
            {
                neighbours = _data.GetArcs(Convert.ToUInt32(edge.VertexId));
                foreach (KeyValuePair<uint, CHEdgeData> backward in neighbours.Where<KeyValuePair<uint, CHEdgeData>>(
                    a => a.Key == edge.From.VertexId && a.Value.Backward))
                {
                    if (arc.Value == null)
                    {
                        arc = backward;
                    }
                    else if (arc.Value.Weight > backward.Value.Weight)
                    {
                        arc = backward;
                    }
                }
                return this.ConvertArc(edge, arc.Key, Convert.ToUInt32(arc.Value.ContractedVertexId), Convert.ToUInt32(edge.VertexId));
            }
            else
            {
                return this.ConvertArc(edge, Convert.ToUInt32(edge.From.VertexId), arc.Value.ContractedVertexId, arc.Key);
            }
        }

        private PathSegment<long> ConvertArc(PathSegment<long> edge,
                                         uint vertex_from_id, uint vertex_contracted_id, uint vertex_to_id)
        {
            // check if the arc is a shortcut.
            if (vertex_contracted_id > 0)
            {
                // arc is a shortcut.
                PathSegment<long> first_path = new PathSegment<long>(vertex_to_id, -1,
                                                             new PathSegment<long>(vertex_contracted_id));
                PathSegment<long> first_path_expanded = this.ConvertArc(first_path);

                PathSegment<long> second_path = new PathSegment<long>(vertex_contracted_id, -1,
                                                              new PathSegment<long>(vertex_from_id));
                PathSegment<long> second_path_expanded = this.ConvertArc(second_path);


                // link the two paths.
                first_path_expanded = first_path_expanded.ConcatenateAfter(second_path_expanded);

                return first_path_expanded;
            }
            return edge;
        }

        #endregion

        #region Notifications

        /// <summary>
        /// The delegate for arc notifications.
        /// </summary>
        /// <param name="route"></param>
        public delegate void NotifyPathSegmentDelegate(PathSegment<long> route);

        /// <summary>
        /// The event.
        /// </summary>
        public event NotifyPathSegmentDelegate NotifyPathSegmentEvent;

        /// <summary>
        /// Notifies the arc.
        /// </summary>
        /// <param name="route"></param>
        private void NotifyPathSegment(PathSegment<long> route)
        {
            if (this.NotifyPathSegmentEvent != null)
            {
                this.NotifyPathSegmentEvent(route);
            }
        }

        #endregion

        #endregion

        private struct CHResult
        {
            public PathSegment<long> Forward { get; set; }

            public PathSegment<long> Backward { get; set; }
        }

        #region Search Closest

        /// <summary>
        /// Searches the data for a point on an edge closest to the given coordinate.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="coordinate"></param>
        /// <param name="delta"></param>
        /// <param name="matcher"></param>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="pointTags"></param>
        public SearchClosestResult SearchClosest(IBasicRouterDataSource<CHEdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            GeoCoordinate coordinate, float delta, IEdgeMatcher matcher, IDictionary<string, string> pointTags)
        {
            double search_box_size = delta;
            // build the search box.
            GeoCoordinateBox search_box = new GeoCoordinateBox(new GeoCoordinate(
                coordinate.Latitude - search_box_size, coordinate.Longitude - search_box_size),
                                                               new GeoCoordinate(
                coordinate.Latitude + search_box_size, coordinate.Longitude + search_box_size));

            // get the arcs from the data source.
            KeyValuePair<uint, KeyValuePair<uint, CHEdgeData>>[] arcs = graph.GetArcs(search_box);

            // loop over all.
            SearchClosestResult closest_with_match = new SearchClosestResult(double.MaxValue, 0);
            SearchClosestResult closest_without_match = new SearchClosestResult(double.MaxValue, 0);
            foreach (KeyValuePair<uint, KeyValuePair<uint, CHEdgeData>> arc in arcs)
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

                    // get the uncontracted arc from the contracted vertex.
                    KeyValuePair<uint, KeyValuePair<uint, CHEdgeData>> uncontracted = arc;
                    while (uncontracted.Value.Value.HasContractedVertex)
                    { // try to inflate the contracted vertex.
                        KeyValuePair<uint, CHEdgeData>[] contracted_arcs = 
                            _data.GetArcs(uncontracted.Value.Value.ContractedVertexId);

                        foreach (KeyValuePair<uint, CHEdgeData> contracted_arc in contracted_arcs)
                        { // loop over all contracted arcs.
                            if (contracted_arc.Key == uncontracted.Key)
                            { // the edge is and edge to the target.
                                CHEdgeData data = new CHEdgeData();
                                data.Forward = contracted_arc.Value.Backward;
                                data.Backward = contracted_arc.Value.Forward;
                                data.ContractedVertexId = contracted_arc.Value.ContractedVertexId;
                                data.Tags = contracted_arc.Value.Tags;
                                data.Weight = contracted_arc.Value.Weight;

                                uncontracted = new KeyValuePair<uint, KeyValuePair<uint, CHEdgeData>>(
                                    uncontracted.Key, new KeyValuePair<uint, CHEdgeData>(
                                        contracted_arc.Key, data));
                                break;
                            }
                        }
                    }
                    // try the to-vertex of the non-contracted arc.
                    if (arc.Value.Key != uncontracted.Value.Key)
                    { // the to-vertex was contracted, not anymore!
                        if (graph.GetVertex(uncontracted.Value.Key, out to_latitude, out to_longitude))
                        { // the to vertex was found
                            to_coordinates = new GeoCoordinate(to_latitude, to_longitude);
                            distance = coordinate.Distance(to_coordinates);

                            if (distance < closest_without_match.Distance)
                            { // the distance is smaller.
                                closest_without_match = new SearchClosestResult(
                                    distance, arc.Key);

                                // try and match.
                                //if(matcher.Match(_
                            }
                        }
                        else
                        { // the to vertex was not found!
                            break;
                        }
                    }

                    // by now the arc is uncontracted.
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
                                    distance, uncontracted.Key, uncontracted.Value.Key, position);

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
