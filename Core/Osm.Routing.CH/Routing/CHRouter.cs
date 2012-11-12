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
using Tools.Math.Geo;
using Tools.Math.Shapes;
using Tools.Math.Geo.Factory;
using Routing.CH.PreProcessing;
using Routing.Core.Graph;
using Routing.Core.Graph.Router;
using Routing.Core.Graph.Path;
using Routing.Core.Interpreter;

namespace Routing.CH.Routing
{
    /// <summary>
    /// A router for CH.
    /// </summary>
    public class CHRouter : IDynamicGraphRouter<CHEdgeData>
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
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public PathSegment<long> Calculate(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter, 
            PathSegmentVisitList source, PathSegmentVisitList target, double max)
        {
            // do the basic CH calculations.
            CHResult result = this.DoCalculate(graph, interpreter, source, target, max, int.MaxValue, long.MaxValue);

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
        /// Calculates the weight of shortest path from the given vertex to the given vertex given the weights in the graph.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double CalculateWeight(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter,
            PathSegmentVisitList source, PathSegmentVisitList target, double max)
        {
            // do the basic CH calculations.
            CHResult result = this.DoCalculate(graph, interpreter, source, target, max, int.MaxValue, long.MaxValue);

            return result.Backward.Weight + result.Forward.Weight;
        }

        /// <summary>
        /// Calculate route to the closest.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public PathSegment<long> CalculateToClosest(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter, 
            PathSegmentVisitList source, PathSegmentVisitList[] targets, double max)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calculates all weights from one source to multiple targets.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double[] CalculateOneToManyWeight(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter, 
            PathSegmentVisitList source, PathSegmentVisitList[] targets, double max)
        {
            double[][] many_to_many_result = this.CalculateManyToManyWeight(
                graph, interpreter, new PathSegmentVisitList[] { source }, targets, max);

            return many_to_many_result[0];
        }

        /// <summary>
        /// Calculates all weights from multiple sources to multiple targets.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double[][] CalculateManyToManyWeight(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter, 
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
        /// <param name="source"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public HashSet<long> CalculateRange(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter,
            PathSegmentVisitList source, double weight)
        {
            throw new NotSupportedException("Check IsCalculateRangeSupported before using this functionality!");
        }

        /// <summary>
        /// Returns true if the search can move beyond the given weight.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="source"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool CheckConnectivity(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter,
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
        //        Tools.Core.Output.OutputStreamHost.ReportProgress(idx, tos.Length, "Router.CH.CalculateManyToManyWeights",
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
        //        Tools.Core.Output.OutputStreamHost.ReportProgress(idx, tos.Length, "Router.CH.CalculateManyToManyWeights",
        //            "Calculating forward...");
        //    }
        //    return weights;
        //}

        /// <summary>
        /// Searches backwards and puts the weigths from the to-vertex into the buckets list.
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        private long SearchBackwardIntoBucket(Dictionary<long, Dictionary<long, double>> buckets, PathSegmentVisitList to_visit_list)
        {
            long? to = null;
            Dictionary<long, PathSegment<long>> settled_vertices =
                new Dictionary<long, PathSegment<long>>();
            CHPriorityQueue queue = new CHPriorityQueue();
            foreach (long vertex in to_visit_list.GetVertices())
            {
                PathSegment<long> path = to_visit_list.GetPathTo(vertex);
                if (!to.HasValue)
                {
                    to = path.First().VertexId;
                }
                queue.Push(path);
            }

            // get the current vertex with the smallest weight.
            while (queue.Count > 0) // TODO: work on a stopping condition?
            {
                PathSegment<long> current = queue.Pop();

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
                        queue.Push(route_to_neighbour);
                    }
                }
            }

            return to.Value;
        }

        /// <summary>
        /// Searches forward and uses the bucket to calculate smallest weights.
        /// </summary>
        /// <param name="buckets"></param>
        /// <param name="from"></param>
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
            CHPriorityQueue queue = new CHPriorityQueue();
            foreach (long vertex in from_visit_list.GetVertices())
            {
                PathSegment<long> path = from_visit_list.GetPathTo(vertex);
                if (!from.HasValue)
                {
                    from = path.First().VertexId;
                }
                queue.Push(path);
            }

            // get the current vertex with the smallest weight.
            int k = 0;
            while (queue.Count > 0) // TODO: work on a stopping condition?
            {
                PathSegment<long> current = queue.Pop();
                k++;

                //// remove from the tentative results list.
                //if (k > 1)
                //{
                //    HashSet<long> to_remove_set = new HashSet<long>();
                //    foreach (KeyValuePair<long, float> result in tentative_results)
                //    {
                //        if (result.Value < current.Weight)
                //        {
                //            to_remove_set.Add(result.Key);
                //            if (!results.ContainsKey(result.Key))
                //            {
                //                results.Add(result.Key, result.Value);
                //            }
                //        }
                //    }
                //    foreach (long to_remove in to_remove_set)
                //    {
                //        tentative_results.Remove(to_remove);
                //    }
                //    k = 0;
                //}

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
                if (buckets.TryGetValue(Convert.ToUInt32(current.VertexId), out bucket))
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
                        //}
                    }
                }

                // get neighbours.
                //CHResolvedPoint vertex = this.GetCHVertex(current.VertexId);
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
                        queue.Push(route_to_neighbour);
                    }
                }
            }

            foreach (uint to in tos)
            {
                if (!tentative_results.ContainsKey(to))
                {
                    if (results.ContainsKey(to))
                    {
                        tentative_results[to] = results[to];
                    }
                    else
                    {
                        tentative_results[to] = double.MaxValue;
                    }
                }
            }

            return tentative_results;
        }

        #endregion

        #region Bi-directional Point-To-Point

        /// <summary>
        /// Calculates a shortest path between the two given vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private CHResult DoCalculate(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter,
            PathSegmentVisitList source, PathSegmentVisitList target, double max, int max_settles, long exception)
        {
            // keep settled vertices.
            CHQueue settled_vertices = new CHQueue();

            // initialize the queues.
            CHPriorityQueue queue_forward = new CHPriorityQueue();
            CHPriorityQueue queue_backward = new CHPriorityQueue();

            // add the sources to the forward queue.
            foreach (long source_vertex in source.GetVertices())
            {
                queue_forward.Push(source.GetPathTo(source_vertex));
            }

            // add the to(s) vertex to the backward queue.
            foreach (long target_vertex in target.GetVertices())
            {
                queue_backward.Push(target.GetPathTo(target_vertex));
            }

            // keep looping until stopping conditions are met.
            CHBest best = this.CalculateBest(settled_vertices);

            // calculate stopping conditions.
            double queue_backward_weight = queue_backward.PeekWeight();
            double queue_forward_weight = queue_forward.PeekWeight();
            while (!(queue_backward.Count == 0 && queue_forward.Count == 0) &&
                   (best.Weight > queue_backward_weight && best.Weight > queue_forward_weight) &&
                   (max >= queue_backward_weight && max >= queue_forward_weight) &&
                   (max_settles >= (settled_vertices.Forward.Count + settled_vertices.Backward.Count)))
            {
                // keep looping until stopping conditions.

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
            if (best.VertexId <= 0)
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
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="max"></param>
        /// <param name="max_settles"></param>
        /// <returns></returns>
        private double[][] DoCalculateManyToMany(IDynamicGraphReadOnly<CHEdgeData> graph, IRoutingInterpreter interpreter,
            PathSegmentVisitList[] sources, PathSegmentVisitList[] targets, double max, int max_settles)
        {
            // TODO: implement switching of from/to when to < from.

            // keep a list of distances to the given vertices while performance backward search.
            Dictionary<long, Dictionary<long, double>> buckets = new Dictionary<long, Dictionary<long, double>>();
            long[] target_ids = new long[sources.Length];
            for (int idx = 0; idx < sources.Length; idx++)
            {
                target_ids[idx] = 
                    this.SearchBackwardIntoBucket(buckets, sources[idx]);

                // report progress.
                Tools.Core.Output.OutputStreamHost.ReportProgress(idx, targets.Length, "Router.CH.CalculateManyToManyWeights",
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
                Tools.Core.Output.OutputStreamHost.ReportProgress(idx, sources.Length, "Router.CH.CalculateManyToManyWeights",
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
        /// <param name="from"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public bool CheckConnectivity(PathSegmentVisitList source, double max)
        {
            return this.DoCheckConnectivity(source, max, int.MaxValue);
        }

        /// <summary>
        /// Checks connectivity of a vertex.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="max"></param>
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
        /// <returns></returns>
        private CHResult CalculateInternal(uint from, uint to, uint exception, double max, int max_settles)
        {
            // keep settled vertices.
            CHQueue settled_vertices = new CHQueue();

            // initialize the queues.
            CHPriorityQueue queue_forward = new CHPriorityQueue();
            CHPriorityQueue queue_backward = new CHPriorityQueue();

            // add the from vertex to the forward queue.
            queue_forward.Push(new PathSegment<long>(from));

            // add the from vertex to the backward queue.
            queue_backward.Push(new PathSegment<long>(to));

            // keep looping until stopping conditions are met.
            CHBest best = this.CalculateBest(settled_vertices);

            // calculate stopping conditions.
            double queue_backward_weight = queue_backward.PeekWeight();
            double queue_forward_weight = queue_forward.PeekWeight();
            while (!(queue_backward.Count == 0 && queue_forward.Count == 0) &&
                   (best.Weight > queue_backward_weight && best.Weight > queue_forward_weight) &&
                   (max >= queue_backward_weight && max >= queue_forward_weight) &&
                   (max_settles >= (settled_vertices.Forward.Count + settled_vertices.Backward.Count)))
            {
                // keep looping until stopping conditions.

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
            if (best.VertexId <= 0)
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
        /// <param name="from"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private bool DoCheckConnectivity(PathSegmentVisitList source, double max, int max_settles)
        {
            // keep settled vertices.
            CHQueue settled_vertices = new CHQueue();

            // initialize the queues.
            CHPriorityQueue queue_forward = new CHPriorityQueue();
            CHPriorityQueue queue_backward = new CHPriorityQueue();

            // add the sources to the forward queue.
            foreach (long source_vertex in source.GetVertices())
            {
                queue_forward.Push(source.GetPathTo(source_vertex));
            }

            // add the to(s) vertex to the backward queue.
            foreach (long target_vertex in source.GetVertices())
            {
                queue_backward.Push(source.GetPathTo(target_vertex));
            }

            // calculate stopping conditions.
            double queue_backward_weight = queue_backward.PeekWeight();
            double queue_forward_weight = queue_forward.PeekWeight();
            while (!(queue_backward.Count == 0 && queue_forward.Count == 0) && // when the queue is empty the connectivity test fails!
                   (max >= queue_backward_weight && max >= queue_forward_weight) &&
                   (max_settles >= (settled_vertices.Forward.Count + settled_vertices.Backward.Count)))
            {
                // keep looping until stopping conditions.

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
        }

        /// <summary>
        /// Do one forward search step.
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        private void SearchForward(CHQueue settled_queue, CHPriorityQueue queue,
                                   long exception)
        {
            // get the current vertex with the smallest weight.
            PathSegment<long> current = queue.Pop();

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
                    queue.Push(route_to_neighbour);
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
                        queue.Push(route_to_neighbour);
                    }
                }
            }
        }

        /// <summary>
        /// Do one backward search step.
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        private void SearchBackward(CHQueue settled_queue, CHPriorityQueue queue,
                                    long exception)
        {
            // get the current vertex with the smallest weight.
            PathSegment<long> current = queue.Pop();

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
                    queue.Push(route_to_neighbour);
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
                        queue.Push(route_to_neighbour);
                    }
                }
            }
        }

        #endregion

        #region Path Expansion

        /// <summary>
        /// Converts the CH paths to complete paths in the orginal network.
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="backward"></param>
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

        //#region ICHData

        ///// <summary>
        ///// Holds all sparse vertices.
        ///// </summary>
        //private Dictionary<long, CHResolvedPoint> _ch_vertices;

        //#region Resolve Points

        ///// <summary>
        ///// Holds a counter 
        ///// </summary>
        //private int _next_id = -1;

        //public CHResolvedPoint ResolveAt(long vertex_id)
        //{
        //    return this.GetCHVertex(vertex_id);
        //}

        /////// <summary>
        /////// Returns a resolved point.
        /////// </summary>
        /////// <param name="coordinate"></param>
        /////// <returns></returns>
        ////public CHResolvedPoint Resolve(GeoCoordinate coordinate)
        ////{
        ////    return this.Resolve(coordinate, 0.001, null);
        ////}

        /////// <summary>
        /////// Resolves a point and returns the closest vertex.
        /////// </summary>
        /////// <param name="coordinate"></param>
        /////// <returns></returns>
        ////private CHResolvedPoint Resolve(GeoCoordinate coordinate, double search_radius, ICHVertexMatcher matcher)
        ////{
        ////    //// get the delta.
        ////    //double delta = search_radius;

        ////    //// initialize the result and distance.
        ////    //PotentialResolvedHit best_hit = new PotentialResolvedHit();
        ////    //best_hit.Distance = double.MaxValue;

        ////    //// keep searching until at least one closeby hit is found.
        ////    //while (best_hit.Distance == double.MaxValue && ((matcher != null && delta <= search_radius * 2)
        ////    //                                                || (matcher == null && delta < search_radius * 200)))
        ////    //{
        ////    //    // construct a bounding box.
        ////    //    GeoCoordinate from_top = new GeoCoordinate(
        ////    //        coordinate.Latitude + delta,
        ////    //        coordinate.Longitude - delta);
        ////    //    GeoCoordinate from_bottom = new GeoCoordinate(
        ////    //        coordinate.Latitude - delta,
        ////    //        coordinate.Longitude + delta);

        ////    //    // query datasource.
        ////    //    IEnumerable<CHResolvedPoint> query_result =
        ////    //        this.GetCHVertices(new GeoCoordinateBox(from_top, from_bottom));
        ////    //    foreach (CHResolvedPoint vertex in query_result)
        ////    //    {
        ////    //        // get the neighbours
        ////    //        foreach (CHVertexNeighbour neighbour in vertex.ForwardNeighbours)
        ////    //        {
        ////    //            if (neighbour.ContractedVertexId < 0)
        ////    //            {
        ////    //                // only do the contracted versions.
        ////    //                // resolve between the two neighbours.
        ////    //                PotentialResolvedHit potential_hit = this.ResolveBetween(coordinate,
        ////    //                                                                         vertex.Id, neighbour.Id,
        ////    //                                                                         vertex, this.GetCHVertex(neighbour.Id));

        ////    //                // keep the result only if it is better.
        ////    //                if (potential_hit.Distance < best_hit.Distance)
        ////    //                {
        ////    //                    best_hit = potential_hit;
        ////    //                }
        ////    //            }
        ////    //        }
        ////    //        foreach (CHVertexNeighbour neighbour in vertex.BackwardNeighbours)
        ////    //        {
        ////    //            if (neighbour.ContractedVertexId < 0)
        ////    //            {
        ////    //                // only do the contracted versions.
        ////    //                // resolve between the two neighbours.
        ////    //                PotentialResolvedHit potential_hit = this.ResolveBetween(coordinate,
        ////    //                                                                         neighbour.Id, vertex.Id,
        ////    //                                                                         vertex, this.GetCHVertex(vertex.Id));

        ////    //                // keep the result only if it is better.
        ////    //                if (potential_hit.Distance < best_hit.Distance)
        ////    //                {
        ////    //                    best_hit = potential_hit;
        ////    //                }
        ////    //            }
        ////    //        }

        ////    //        // resolve the vertex itself.
        ////    //        double result_distance = vertex.Location.DistanceEstimate(coordinate).Value;
        ////    //        if (result_distance <= best_hit.Distance)
        ////    //        {
        ////    //            best_hit = new PotentialResolvedHit();
        ////    //            best_hit.Distance = result_distance;
        ////    //            best_hit.Vertex = vertex;
        ////    //        }
        ////    //    }

        ////    //    // get the resolved versions.

        ////    //    delta = delta * 2;
        ////    //}

        ////    //// process the best result.
        ////    //CHResolvedPoint result = null;
        ////    //if (best_hit.Vertex != null)
        ////    //{
        ////    //    // the best result was a sparse vertex.
        ////    //    result = best_hit.Vertex;
        ////    //}
        ////    //else
        ////    //{
        ////    //    // the best result was somewhere in between vertices.
        ////    //    LineProjectionResult<GeoCoordinate> line_projection = best_hit.LineProjection;

        ////    //    // get the actual intersection point.
        ////    //    GeoCoordinate intersection_point = line_projection.ClosestPrimitive as GeoCoordinate;

        ////    //    // process the projection.
        ////    //    double latitude_diff = best_hit.Neighbour2.Latitude - best_hit.Neighbour1.Latitude;
        ////    //    double latitude_diff_small = intersection_point.Latitude - best_hit.Neighbour1.Latitude;
        ////    //    float position = (float)System.Math.Max(System.Math.Min((latitude_diff_small / latitude_diff), 1), 0);
        ////    //    if (latitude_diff == 0 && latitude_diff_small == 0)
        ////    //    {
        ////    //        position = 0;
        ////    //    }

        ////    //    // create the result.
        ////    //    if (position == 0)
        ////    //    {
        ////    //        // the position is at the first node @ line_projection.Idx.
        ////    //        if (line_projection.Idx == 0)
        ////    //        {
        ////    //            // the closest one is the sparse vertex.
        ////    //            result = best_hit.Neighbour1;
        ////    //        }
        ////    //        else if (line_projection.Idx == 1)
        ////    //        {
        ////    //            // the closest one is another vertex.
        ////    //            result = best_hit.Neighbour2;
        ////    //        }
        ////    //    }
        ////    //    else if (position == 1)
        ////    //    {
        ////    //        // the position is at the first node @ line_projection.Idx + 1.
        ////    //        if (line_projection.Idx == 1)
        ////    //        {
        ////    //            // the closest node is neighbour2
        ////    //            result = best_hit.Neighbour2;
        ////    //        }
        ////    //        else
        ////    //        {
        ////    //            // the closest one is another vertex.
        ////    //            result = best_hit.Neighbour1;
        ////    //        }
        ////    //    }
        ////    //    else
        ////    //    {
        ////    //        // the best location is not located at an existing vertex.
        ////    //        // the position is somewhere in between.
        ////    //        result = best_hit.Neighbour1;

        ////    //        //result = new CHVertex();
        ////    //        //result.Id = _next_id;
        ////    //        //_next_id--;
        ////    //        //result.Level = -1;
        ////    //        //result.Latitude = intersection_point.Latitude;
        ////    //        //result.Longitude = intersection_point.Longitude;
        ////    //        //_ch_vertices[result.Id] = result;

        ////    //        //// get the forward arcs.
        ////    //        //CHArc neighbour_forward = null;
        ////    //        //HashSet<CHArc> neighbours_forward = this.GetCHArcs(best_hit.Neighbour1.Id, -1);
        ////    //        //foreach (CHArc potential_neighbour in neighbours_forward)
        ////    //        //{ // loop over all forward neighbours.
        ////    //        //    if (potential_neighbour.VertexToId == best_hit.Neighbour2.Id)
        ////    //        //    { // do not add the unmodified neighbours.
        ////    //        //        neighbour_forward = potential_neighbour;
        ////    //        //    }
        ////    //        //    else
        ////    //        //    { // add the unmodified neighbours.
        ////    //        //        this.ResolvedArc(potential_neighbour);
        ////    //        //    }
        ////    //        //}                    

        ////    //        //// adjust the neighbour.
        ////    //        //double forward_weight = neighbour_forward.Weight * position;

        ////    //        //// add a forward neighbour to the new vertex.
        ////    //        //CHArc forward1 = new CHArc();
        ////    //        //forward1.Weight = forward_weight;
        ////    //        //forward1.VertexFromId = neighbour_forward.VertexFromId;
        ////    //        //forward1.VertexToId = result.Id;
        ////    //        //forward1.Tags = neighbour_forward.Tags;
        ////    //        //this.ResolvedArc(forward1);

        ////    //        //// add a forward neighbour to the new vertex.
        ////    //        //CHArc forward2 = new CHArc();
        ////    //        //forward2.Weight = neighbour_forward.Weight - forward_weight;
        ////    //        //forward2.VertexFromId = result.Id;
        ////    //        //forward2.VertexToId = neighbour_forward.VertexToId;
        ////    //        //forward2.Tags = neighbour_forward.Tags;
        ////    //        //this.ResolvedArc(forward2);

        ////    //        //CHArc neighbour_backward = null;
        ////    //        //HashSet<CHArc> neighbours_backward = this.GetCHArcsReversed(best_hit.Neighbour1.Id, -1);
        ////    //        //foreach (CHArc potential_neighbour in neighbours_backward)
        ////    //        //{ // loop over all backward neighbours.
        ////    //        //    if (potential_neighbour.VertexFromId == best_hit.Neighbour2.Id)
        ////    //        //    {
        ////    //        //        neighbour_backward = potential_neighbour;
        ////    //        //    }
        ////    //        //    else
        ////    //        //    { // add the unmodified neighbours.
        ////    //        //        this.ResolvedArc(potential_neighbour);
        ////    //        //    }
        ////    //        //}

        ////    //        //// adjust the neighbour.
        ////    //        //double backward_weight = neighbour_backward.Weight * (1.0 - position);

        ////    //        //// add a forward neighbour to the new vertex.
        ////    //        //CHArc backward1 = new CHArc();
        ////    //        //backward1.Weight = forward_weight;
        ////    //        //backward1.VertexFromId = neighbour_forward.VertexFromId;
        ////    //        //backward1.VertexToId = result.Id;
        ////    //        //backward1.Tags = neighbour_forward.Tags;
        ////    //        //this.ResolvedArc(backward1);

        ////    //        //// add a forward neighbour to the new vertex.
        ////    //        //CHArc backward2 = new CHArc();
        ////    //        //backward2.Weight = neighbour_forward.Weight - forward_weight;
        ////    //        //backward2.VertexFromId = result.Id;
        ////    //        //backward2.VertexToId = neighbour_forward.VertexToId;
        ////    //        //backward2.Tags = neighbour_forward.Tags;
        ////    //        //this.ResolvedArc(backward2);

        ////    //        //// get the forward arcs.
        ////    //        //neighbours_forward = this.GetCHArcs(best_hit.Neighbour2.Id, -1);
        ////    //        //foreach (CHArc potential_neighbour in neighbours_forward)
        ////    //        //{ // loop over all forward neighbours.
        ////    //        //    if (potential_neighbour.VertexToId == best_hit.Neighbour1.Id)
        ////    //        //    { // do not add the unmodified neighbours.
        ////    //        //        neighbour_forward = potential_neighbour;
        ////    //        //    }
        ////    //        //    else
        ////    //        //    { // add the unmodified neighbours.
        ////    //        //        this.ResolvedArc(potential_neighbour);
        ////    //        //    }
        ////    //        //}

        ////    //        //// adjust the neighbour.
        ////    //        //forward_weight = neighbour_forward.Weight * position;

        ////    //        //// add a forward neighbour to the new vertex.
        ////    //        //forward1 = new CHArc();
        ////    //        //forward1.Weight = forward_weight;
        ////    //        //forward1.VertexFromId = neighbour_forward.VertexFromId;
        ////    //        //forward1.VertexToId = result.Id;
        ////    //        //forward1.Tags = neighbour_forward.Tags;
        ////    //        //this.ResolvedArc(forward1);

        ////    //        //// add a forward neighbour to the new vertex.
        ////    //        //forward2 = new CHArc();
        ////    //        //forward2.Weight = neighbour_forward.Weight - forward_weight;
        ////    //        //forward2.VertexFromId = result.Id;
        ////    //        //forward2.VertexToId = neighbour_forward.VertexToId;
        ////    //        //forward2.Tags = neighbour_forward.Tags;
        ////    //        //this.ResolvedArc(forward2);

        ////    //        //neighbours_backward = this.GetCHArcsReversed(best_hit.Neighbour2.Id, -1);
        ////    //        //foreach (CHArc potential_neighbour in neighbours_backward)
        ////    //        //{ // loop over all backward neighbours.
        ////    //        //    if (potential_neighbour.VertexFromId == best_hit.Neighbour1.Id)
        ////    //        //    {
        ////    //        //        neighbour_backward = potential_neighbour;
        ////    //        //    }
        ////    //        //    else
        ////    //        //    { // add the unmodified neighbours.
        ////    //        //        this.ResolvedArc(potential_neighbour);
        ////    //        //    }
        ////    //        //}

        ////    //        //// adjust the neighbour.
        ////    //        //backward_weight = neighbour_backward.Weight * (1.0 - position);

        ////    //        //// add a forward neighbour to the new vertex.
        ////    //        //backward1 = new CHArc();
        ////    //        //backward1.Weight = forward_weight;
        ////    //        //backward1.VertexFromId = neighbour_forward.VertexFromId;
        ////    //        //backward1.VertexToId = result.Id;
        ////    //        //backward1.Tags = neighbour_forward.Tags;
        ////    //        //this.ResolvedArc(backward1);

        ////    //        //// add a forward neighbour to the new vertex.
        ////    //        //backward2 = new CHArc();
        ////    //        //backward2.Weight = neighbour_forward.Weight - forward_weight;
        ////    //        //backward2.VertexFromId = result.Id;
        ////    //        //backward2.VertexToId = neighbour_forward.VertexToId;
        ////    //        //backward2.Tags = neighbour_forward.Tags;
        ////    //        //this.ResolvedArc(backward2);
        ////    //    }
        ////    //}
        ////    //return result;
        ////}

        /////// <summary>
        /////// Resolves the closest point between the given neighbours.
        /////// </summary>
        /////// <param name="neighbour1_id"></param>
        /////// <param name="neighbour2_id"></param>
        /////// <param name="neighbour1"></param>
        /////// <param name="neighbour2"></param>
        /////// <returns></returns>
        ////private PotentialResolvedHit ResolveBetween(GeoCoordinate coordinate, long neighbour1_id, long neighbour2_id,
        ////                                            CHResolvedPoint neighbour1, CHResolvedPoint neighbour2)
        ////{
        ////    // intersect with the neighbours line.
        ////    List<GeoCoordinate> points = new List<GeoCoordinate>();
        ////    points.Add(neighbour1.Location);
        ////    points.Add(neighbour2.Location);

        ////    // define the line and intersect.
        ////    ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> line =
        ////        new ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(PrimitiveGeoFactory.Instance,
        ////                                                                               points.ToArray());
        ////    LineProjectionResult<GeoCoordinate> line_projection =
        ////        (line.DistanceDetailed(coordinate) as LineProjectionResult<GeoCoordinate>);

        ////    PotentialResolvedHit best_hit = new PotentialResolvedHit();
        ////    best_hit.Distance = line_projection.Distance;
        ////    best_hit.LineProjection = line_projection;
        ////    best_hit.Neighbour1 = neighbour1;
        ////    best_hit.Neighbour2 = neighbour2;
        ////    return best_hit;
        ////}

        /////// <summary>
        /////// Potential resolved hit.
        /////// </summary>
        ////private struct PotentialResolvedHit
        ////{
        ////    public double Distance { get; set; }

        ////    public CHResolvedPoint Vertex { get; set; }

        ////    #region Non-Direct hits

        ////    public LineProjectionResult<GeoCoordinate> LineProjection { get; set; }

        ////    public uint Neighbour1 { get; set; }

        ////    public uint Neighbour2 { get; set; }

        ////    #endregion
        ////}

        //#endregion

        //#region ICHData

        ///// <summary>
        ///// Returns the vertex with the given id.
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public CHResolvedPoint GetCHVertex(long id)
        //{
        //    CHResolvedPoint vertex = null;
        //    if (_ch_vertices.TryGetValue(id, out vertex))
        //    {
        //        return vertex;
        //    }
        //    return new CHResolvedPoint(id);
        //}

        /////// <summary>
        /////// Returns all vertices without a set level.
        /////// </summary>
        /////// <returns></returns>
        ////public IEnumerable<CHVertex> GetCHVerticesNoLevel()
        ////{
        ////    return _data.GetCHVerticesNoLevel();
        ////}

        /////// <summary>
        /////// Persists the given vertex.
        /////// </summary>
        /////// <param name="vertex"></param>
        ////public void PersistCHVertex(CHVertex vertex)
        ////{
        ////    _data.PersistCHVertex(vertex);
        ////}

        //// TODO: broke SQLite changes; fix!
        ////public void PersistCHVertexNeighbour(CHVertex vertex, CHVertexNeighbour arc, bool forward)
        ////{
        ////    _data.PersistCHVertexNeighbour(vertex, arc, forward);
        ////}

        /////// <summary>
        /////// Deletes the vertex with the given id.
        /////// </summary>
        /////// <param name="id"></param>
        ////public void DeleteCHVertex(long id)
        ////{
        ////    _data.DeleteCHVertex(id);
        ////}


        //// TODO: broke SQLite changes; fix!
        ////public void DeleteNeighbours(long vertexid)
        ////{
        ////    _data.DeleteNeighbours(vertexid);
        ////}


        //// TODO: broke SQLite changes; fix!
        ////public void DeleteNeighbour(CHVertex vertex, CHVertexNeighbour neighbour, bool forward)
        ////{
        ////    _data.DeleteNeighbour(vertex, neighbour, forward);
        ////}

        ///// <summary>
        ///// Returns the vertices inside the given box.
        ///// </summary>
        ///// <param name="box"></param>
        ///// <returns></returns>
        //public IEnumerable<CHResolvedPoint> GetCHVertices(GeoCoordinateBox box)
        //{
        //    HashSet<CHResolvedPoint> vertices_in_box = new HashSet<CHResolvedPoint>();
        //    //foreach (CHResolvedPoint vertex in _ch_vertices.Values)
        //    //{
        //    //    if (box.IsInside(vertex.Location))
        //    //    {
        //    //        vertices_in_box.Add(vertex);
        //    //    }
        //    //}
        //    //IEnumerable<CHVertex> vertices = _data.GetCHVertices(box);
        //    //if (vertices != null)
        //    //{
        //    //    foreach (CHVertex vertex in vertices)
        //    //    {
        //    //        vertices_in_box.Add(vertex);
        //    //    }
        //    //}
        //    return vertices_in_box;
        //}

        //#endregion

        //#endregion

        #region Notifications

        /// <summary>
        /// The delegate for arc notifications.
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="contracted_id"></param>
        public delegate void NotifyPathSegmentDelegate(PathSegment<long> route);

        /// <summary>
        /// The event.
        /// </summary>
        public event NotifyPathSegmentDelegate NotifyPathSegmentEvent;

        /// <summary>
        /// Notifies the arc.
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="contracted_id"></param>
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
    }
}
