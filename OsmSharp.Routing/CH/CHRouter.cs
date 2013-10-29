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
using System.Linq;
using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Interpreter;

namespace OsmSharp.Routing.CH
{
    /// <summary>
    /// A router for CH.
    /// </summary>
    public class CHRouter : IBasicRouter<CHEdgeData>
    {
        /// <summary>
        /// Creates a new CH router on the givend data.
        /// </summary>
        public CHRouter()
        {

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
        public PathSegment<long> Calculate(IBasicRouterDataSource<CHEdgeData> graph, IRoutingInterpreter interpreter, 
            Vehicle vehicle, PathSegmentVisitList source, PathSegmentVisitList target, double max)
        {
            // do the basic CH calculations.
            CHResult result = this.DoCalculate(graph, interpreter, source, target, max, int.MaxValue, long.MaxValue);

            return this.ExpandBestResult(graph, result);
        }

        /// <summary>
        /// Calculates all routes between all sources and targets.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <param name="maxSearch"></param>
        /// <param name="graph"></param>
        /// <returns></returns>
        public PathSegment<long>[][] CalculateManyToMany(IBasicRouterDataSource<CHEdgeData> graph, IRoutingInterpreter interpreter, Vehicle vehicle, 
            PathSegmentVisitList[] sources, PathSegmentVisitList[] targets, double maxSearch)
        {
            var results = new PathSegment<long>[sources.Length][];
            for (int sourceIdx = 0; sourceIdx < sources.Length; sourceIdx++)
            {
                results[sourceIdx] = new PathSegment<long>[targets.Length];
                for (int targetIdx = 0; targetIdx < targets.Length; targetIdx++)
                {
                    results[sourceIdx][targetIdx] =
                        this.Calculate(graph, interpreter, vehicle, sources[sourceIdx], targets[targetIdx], maxSearch);
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
        public double CalculateWeight(IBasicRouterDataSource<CHEdgeData> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
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
        public PathSegment<long> CalculateToClosest(IBasicRouterDataSource<CHEdgeData> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
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
        public double[] CalculateOneToManyWeight(IBasicRouterDataSource<CHEdgeData> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
            PathSegmentVisitList source, PathSegmentVisitList[] targets, double max)
        {
            double[][] manyToManyResult = this.CalculateManyToManyWeight(
                graph, interpreter, vehicle, new PathSegmentVisitList[] { source }, targets, max);

            return manyToManyResult[0];
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
        public double[][] CalculateManyToManyWeight(IBasicRouterDataSource<CHEdgeData> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
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
        public HashSet<long> CalculateRange(IBasicRouterDataSource<CHEdgeData> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
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
        public bool CheckConnectivity(IBasicRouterDataSource<CHEdgeData> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
            PathSegmentVisitList source, double weight)
        {
            return this.DoCheckConnectivity(graph, source, weight, int.MaxValue);
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
        //        OsmSharp.IO.Output.OutputStreamHost.ReportProgress(idx, tos.Length, "Router.CH.CalculateManyToManyWeights",
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
        //        OsmSharp.IO.Output.OutputStreamHost.ReportProgress(idx, tos.Length, "Router.CH.CalculateManyToManyWeights",
        //            "Calculating forward...");
        //    }
        //    return weights;
        //}

        /// <summary>
        /// Searches backwards and puts the weigths from the to-vertex into the buckets list.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="buckets"></param>
        /// <param name="toVisitList"></param>
        /// <returns></returns>
        private long SearchBackwardIntoBucket(IBasicRouterDataSource<CHEdgeData> graph, Dictionary<long, Dictionary<long, double>> buckets, 
            PathSegmentVisitList toVisitList)
        {
            long? to = null;
            var settledVertices = new Dictionary<long, PathSegment<long>>();
            IPriorityQueue<PathSegment<long>> queue = new BinairyHeap<PathSegment<long>>();
            foreach (long vertex in toVisitList.GetVertices())
            {
                PathSegment<long> path = toVisitList.GetPathTo(vertex);
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
                        double existingWeight;
                        if (!bucket.TryGetValue(path.VertexId, out existingWeight) ||
                            existingWeight > path.Weight)
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
                while (current != null && settledVertices.ContainsKey(current.VertexId))
                {
                    current = queue.Pop();
                }

                if (current != null)
                { // a next vertex was found!
                    // add to the settled vertices.
                    PathSegment<long> previousLinkedRoute;
                    if (settledVertices.TryGetValue(current.VertexId, out previousLinkedRoute))
                    {
                        if (previousLinkedRoute.Weight > current.Weight)
                        {
                            // settle the vertex again if it has a better weight.
                            settledVertices[current.VertexId] = current;
                        }
                    }
                    else
                    {
                        // settled the vertex.
                        settledVertices[current.VertexId] = current;
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
                    KeyValuePair<uint, CHEdgeData>[] neighbours = graph.GetArcs(Convert.ToUInt32(current.VertexId));

                    // add the neighbours to the queue.
                    foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours.Where<KeyValuePair<uint, CHEdgeData>>(
                        a => a.Value.Backward))
                    {
                        if (!settledVertices.ContainsKey(neighbour.Key))
                        {
                            // if not yet settled.
                            var routeToNeighbour = new PathSegment<long>(
                                neighbour.Key, current.Weight + neighbour.Value.Weight, current);
                            queue.Push(routeToNeighbour, (float)routeToNeighbour.Weight);
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
        /// <param name="graph"></param>
        /// <param name="buckets"></param>
        /// <param name="fromVisitList"></param>
        /// <param name="tos"></param>
        private Dictionary<long, double> SearchForwardFromBucket(IBasicRouterDataSource<CHEdgeData> graph, Dictionary<long, Dictionary<long, double>> buckets,
            PathSegmentVisitList fromVisitList, long[] tos)
        {
            long? from = null;
            // intialize weights.
            var results = new Dictionary<long, double>();
            //HashSet<long> permanent_results = new HashSet<long>();
            var tentativeResults = new Dictionary<long, double>();

            var settledVertices =
                new Dictionary<long, PathSegment<long>>();
            //CHPriorityQueue queue = new CHPriorityQueue();
            IPriorityQueue<PathSegment<long>> queue = new BinairyHeap<PathSegment<long>>();
            foreach (long vertex in fromVisitList.GetVertices())
            {
                PathSegment<long> path = fromVisitList.GetPathTo(vertex);
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
                        foreach (KeyValuePair<long, double> bucketEntry in bucket)
                        {
                            double foundDistance = bucketEntry.Value + path.Weight;
                            double tentativeDistance;
                            if (tentativeResults.TryGetValue(bucketEntry.Key, out tentativeDistance))
                            {
                                if (foundDistance < tentativeDistance)
                                {
                                    tentativeResults[bucketEntry.Key] = foundDistance;
                                }
                            }
                            else
                            { // there was no result yet!
                                tentativeResults[bucketEntry.Key] = foundDistance;
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
                while (current != null && settledVertices.ContainsKey(current.VertexId))
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
                    PathSegment<long> previousLinkedRoute;
                    if (settledVertices.TryGetValue(current.VertexId, out previousLinkedRoute))
                    {
                        if (previousLinkedRoute.Weight > current.Weight)
                        {
                            // settle the vertex again if it has a better weight.
                            settledVertices[current.VertexId] = current;
                        }
                    }
                    else
                    {
                        // settled the vertex.
                        settledVertices[current.VertexId] = current;
                    }

                    // search the bucket.
                    Dictionary<long, double> bucket;
                    if (buckets.TryGetValue(current.VertexId, out bucket))
                    {
                        // there is a bucket!
                        foreach (KeyValuePair<long, double> bucketEntry in bucket)
                        {
                            //if (!permanent_results.Contains(bucket_entry.Key))
                            //{
                            double foundDistance = bucketEntry.Value + current.Weight;
                            double tentativeDistance;
                            if (tentativeResults.TryGetValue(bucketEntry.Key, out tentativeDistance))
                            {
                                if (foundDistance < tentativeDistance)
                                {
                                    tentativeResults[bucketEntry.Key] = foundDistance;
                                }

                                if (tentativeDistance < current.Weight)
                                {
                                    tentativeResults.Remove(bucketEntry.Key);
                                    results[bucketEntry.Key] = tentativeDistance;
                                }
                            }
                            else if(!results.ContainsKey(bucketEntry.Key))
                            { // there was no result yet!
                                tentativeResults[bucketEntry.Key] = foundDistance;
                            }
                            //}
                        }
                    }

                    // get neighbours.
                    KeyValuePair<uint, CHEdgeData>[] neighbours = graph.GetArcs(Convert.ToUInt32(current.VertexId));

                    // add the neighbours to the queue.
                    foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours.Where<KeyValuePair<uint, CHEdgeData>>(
                        a => a.Value.Forward))
                    {
                        if (!settledVertices.ContainsKey(neighbour.Key))
                        {
                            // if not yet settled.
                            var routeToNeighbour = new PathSegment<long>(
                                neighbour.Key, current.Weight + neighbour.Value.Weight, current);
                            queue.Push(routeToNeighbour, (float)routeToNeighbour.Weight);
                        }
                    }
                }
            }

            foreach (long to in tos)
            {
                if (!results.ContainsKey(to) && tentativeResults.ContainsKey(to))
                {
                    results[to] = tentativeResults[to];
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
        private CHResult DoCalculate(IBasicRouterDataSource<CHEdgeData> graph, IRoutingInterpreter interpreter,
            PathSegmentVisitList source, PathSegmentVisitList target, double max, int max_settles, long exception)
        {
            // keep settled vertices.
            var settledVertices = new CHQueue();

            // initialize the queues.
            IPriorityQueue<PathSegment<long>> queueForward = new BinairyHeap<PathSegment<long>>();
            IPriorityQueue<PathSegment<long>> queueBackward = new BinairyHeap<PathSegment<long>>();
            //CHPriorityQueue queue_forward = new CHPriorityQueue();
            //CHPriorityQueue queue_backward = new CHPriorityQueue();

            // add the sources to the forward queue.
            var resolvedSettles = new Dictionary<long, PathSegment<long>>();
            foreach (long sourceVertex in source.GetVertices())
            {
                PathSegment<long> path = source.GetPathTo(sourceVertex);
                queueForward.Push(path, (float)path.Weight);
                path = path.From;
                while (path != null)
                { // keep looping.
                    PathSegment<long> existingSource = null;
                    if (!resolvedSettles.TryGetValue(path.VertexId, out existingSource) ||
                        existingSource.Weight > path.Weight)
                    { // the new path is better.
                        resolvedSettles[path.VertexId] = path;
                    }
                    path = path.From;
                }
            }

            // add the sources to the settled vertices.
            foreach (KeyValuePair<long, PathSegment<long>> resolvedSettled 
                in resolvedSettles)
            {
                settledVertices.AddForward(resolvedSettled.Value);
            }

            // add the to(s) vertex to the backward queue.resolved_settles = 
            resolvedSettles = new Dictionary<long, PathSegment<long>>();
            foreach (long targetVertex in target.GetVertices())
            {
                PathSegment<long> path = target.GetPathTo(targetVertex);
                queueBackward.Push(path, (float)path.Weight);
                path = path.From;
                while (path != null)
                { // keep looping.
                    PathSegment<long> existingSource = null;
                    if (!resolvedSettles.TryGetValue(path.VertexId, out existingSource) ||
                        existingSource.Weight > path.Weight)
                    { // the new path is better.
                        resolvedSettles[path.VertexId] = path;
                    }
                    path = path.From;
                }
            }

            // add the sources to the settled vertices.
            foreach (KeyValuePair<long, PathSegment<long>> resolvedSettled
                in resolvedSettles)
            {
                settledVertices.AddBackward(resolvedSettled.Value);
            }

            // keep looping until stopping conditions are met.
            CHBest best = this.CalculateBest(settledVertices);

            // calculate stopping conditions.
            double queueBackwardWeight = queueBackward.PeekWeight();
            double queueForwardWeight = queueForward.PeekWeight();
            while (true)
            { // keep looping until stopping conditions.
                if (queueBackward.Count == 0 && queueForward.Count == 0)
                { // stop the search; both queues are empty.
                    break;
                }
                if (max < queueBackwardWeight && max < queueForwardWeight)
                { // stop the search: the max search weight has been reached.
                    break;
                }
                if (max_settles < (settledVertices.Forward.Count + settledVertices.Backward.Count))
                { // stop the search: the max settles cound has been reached.
                    break;
                }
                if (best.Weight < queueForwardWeight && best.Weight < queueBackwardWeight)
                { // stop the search: it now became impossible to find a shorter route.
                    break;
                }

                // do a forward search.
                if (queueForward.Count > 0)
                {
                    this.SearchForward(graph, settledVertices, queueForward, exception);
                }

                // do a backward search.
                if (queueBackward.Count > 0)
                {
                    this.SearchBackward(graph, settledVertices, queueBackward, exception);
                }

                // calculate the new best if any.
                best = this.CalculateBest(settledVertices);

                // calculate stopping conditions.
                if (queueForward.Count > 0)
                {
                    queueForwardWeight = queueForward.PeekWeight();
                }
                if (queueBackward.Count > 0)
                {
                    queueBackwardWeight = queueBackward.PeekWeight();
                }
            }

            // return forward/backward routes.
            var result = new CHResult();
            if (!best.Found)
            {
                // no route was found!
            }
            else
            {
                // construct the existing route.
                result.Forward = settledVertices.Forward[best.VertexId];
                result.Backward = settledVertices.Backward[best.VertexId];
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
        /// <param name="maxSettles"></param>
        /// <returns></returns>
        private double[][] DoCalculateManyToMany(IBasicRouterDataSource<CHEdgeData> graph, IRoutingInterpreter interpreter,
            PathSegmentVisitList[] sources, PathSegmentVisitList[] targets, double max, int maxSettles)
        {
            // TODO: implement switching of from/to when to < from.

            // keep a list of distances to the given vertices while doing backward search.
            var buckets = new Dictionary<long, Dictionary<long, double>>();
            var targetIds = new long[sources.Length];
            for (int idx = 0; idx < sources.Length; idx++)
            {
                targetIds[idx] = 
                    this.SearchBackwardIntoBucket(graph, buckets, sources[idx]);

                OsmSharp.Logging.Log.TraceEvent("CHRouter", System.Diagnostics.TraceEventType.Information, "Calculating backward... {0}%",
                    (int)(((float)idx / (float)targets.Length) * 100));
            }

            // conduct a forward search from each source.
            var weights = new double[sources.Length][];
            for (int idx = 0; idx < sources.Length; idx++)
            {
                // calculate all from's.
                Dictionary<long, double> result =
                    this.SearchForwardFromBucket(graph, buckets, sources[idx], targetIds);

                var toWeights = new double[targetIds.Length];
                for (int toIdx = 0; toIdx < targetIds.Length; toIdx++)
                {
                    if (result.ContainsKey(targetIds[toIdx]))
                    {
                        toWeights[toIdx] = result[targetIds[toIdx]];
                    }
                }

                weights[idx] = toWeights;
                result.Clear();
                
                OsmSharp.Logging.Log.TraceEvent("CHRouter", System.Diagnostics.TraceEventType.Information, "Calculating forward... {0}%",
                    (int)(((float)idx / (float)sources.Length) * 100));
            }
            return weights;
        }

        /// <summary>
        /// Calculates the weight from from to to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public double CalculateWeight(IBasicRouterDataSource<CHEdgeData> graph, uint from, uint to, uint exception)
        {
            return this.CalculateWeight(graph, from, to, exception, double.MaxValue);
        }

        /// <summary>
        /// Calculates the weight from from to to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="exception"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double CalculateWeight(IBasicRouterDataSource<CHEdgeData> graph, uint from, uint to, uint exception, double max)
        {
            // calculate the result.
            CHResult result = this.CalculateInternal(graph, from, to, exception, max, int.MaxValue);

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
        /// <param name="maxSettles"></param>
        /// <returns></returns>
        public double CalculateWeight(IBasicRouterDataSource<CHEdgeData> graph, uint from, uint to, uint exception, double max, int maxSettles)
        {
            // calculate the result.
            CHResult result = this.CalculateInternal(graph, from, to, exception, max, maxSettles);

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
        public bool CheckConnectivity(IBasicRouterDataSource<CHEdgeData> graph, PathSegmentVisitList source, double max)
        {
            return this.DoCheckConnectivity(graph, source, max, int.MaxValue);
        }

        /// <summary>
        /// Checks connectivity of a vertex.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="maxSettles"></param>
        /// <returns></returns>
        public bool CheckConnectivity(IBasicRouterDataSource<CHEdgeData> graph, PathSegmentVisitList source, int maxSettles)
        {
            return this.DoCheckConnectivity(graph, source, double.MaxValue, maxSettles);
        }

        /// <summary>
        /// Calculates a shortest path between the two given vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="exception"></param>
        /// <param name="max"></param>
        /// <param name="maxSettles"></param>
        /// <returns></returns>
        private CHResult CalculateInternal(IBasicRouterDataSource<CHEdgeData> graph, uint from, uint to, uint exception, double max, int maxSettles)
        {
            // keep settled vertices.
            var settledVertices = new CHQueue();

            // initialize the queues.
            IPriorityQueue<PathSegment<long>> queueForward = new BinairyHeap<PathSegment<long>>();
            IPriorityQueue<PathSegment<long>> queueBackward = new BinairyHeap<PathSegment<long>>();

            // add the from vertex to the forward queue.
            queueForward.Push(new PathSegment<long>(from), 0);

            // add the from vertex to the backward queue.
            queueBackward.Push(new PathSegment<long>(to), 0);

            // keep looping until stopping conditions are met.
            CHBest best = this.CalculateBest(settledVertices);

            // calculate stopping conditions.
            double queueBackwardWeight = queueBackward.PeekWeight();
            double queueForwardWeight = queueForward.PeekWeight();
            while (true)
            { // keep looping until stopping conditions.
                if (queueBackward.Count == 0 && queueForward.Count == 0)
                { // stop the search; both queues are empty.
                    break;
                }
                if (max < queueBackwardWeight && max < queueForwardWeight)
                { // stop the search: the max search weight has been reached.
                    break;
                }
                if (maxSettles < (settledVertices.Forward.Count + settledVertices.Backward.Count))
                { // stop the search: the max settles cound has been reached.
                    break;
                }
                if (best.Weight < queueForwardWeight && best.Weight < queueBackwardWeight)
                { // stop the search: it now became impossible to find a shorter route.
                    break;
                }

                // do a forward search.
                if (queueForward.Count > 0)
                {
                    this.SearchForward(graph, settledVertices, queueForward, exception);
                }

                // do a backward search.
                if (queueBackward.Count > 0)
                {
                    this.SearchBackward(graph, settledVertices, queueBackward, exception);
                }

                // calculate the new best if any.
                best = this.CalculateBest(settledVertices);

                // calculate stopping conditions.
                if (queueForward.Count > 0)
                {
                    queueForwardWeight = queueForward.PeekWeight();
                }
                if (queueBackward.Count > 0)
                {
                    queueBackwardWeight = queueBackward.PeekWeight();
                }
            }

            // return forward/backward routes.
            var result = new CHResult();
            if (!best.Found)
            {
                // no route was found!
            }
            else
            {
                // construct the existing route.
                result.Forward = settledVertices.Forward[best.VertexId];
                result.Backward = settledVertices.Backward[best.VertexId];
            }
            return result;
        }

        /// <summary>
        /// Checks if the given vertex is connected to others.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="max"></param>
        /// <param name="maxSettles"></param>
        /// <returns></returns>
        private bool DoCheckConnectivity(IBasicRouterDataSource<CHEdgeData> graph, PathSegmentVisitList source, double max, int maxSettles)
        {
            // keep settled vertices.
            CHQueue settledVertices = new CHQueue();

            // initialize the queues.
            IPriorityQueue<PathSegment<long>> queueForward = new BinairyHeap<PathSegment<long>>();
            IPriorityQueue<PathSegment<long>> queueBackward = new BinairyHeap<PathSegment<long>>();
            //CHPriorityQueue queue_forward = new CHPriorityQueue();
            //CHPriorityQueue queue_backward = new CHPriorityQueue();

            // add the sources to the forward queue.
            foreach (long sourceVertex in source.GetVertices())
            {
                PathSegment<long> path = source.GetPathTo(sourceVertex);
                queueForward.Push(path, (float)path.Weight);
                //queue_forward.Push(source.GetPathTo(source_vertex));
            }

            // add the to(s) vertex to the backward queue.
            foreach (long targetVertex in source.GetVertices())
            {
                PathSegment<long> path = source.GetPathTo(targetVertex);
                queueBackward.Push(path, (float)path.Weight);
                //queue_backward.Push(source.GetPathTo(target_vertex));
            }

            // calculate stopping conditions.
            double queueBackwardWeight = queueBackward.PeekWeight();
            double queueForwardWeight = queueForward.PeekWeight();
            while (true) // when the queue is empty the connectivity test fails!
            { // keep looping until stopping conditions.
                if(queueBackward.Count == 0 && queueForward.Count == 0)
                { // stop the search; both queues are empty.
                    break;
                }
                if(max < queueBackwardWeight && max < queueForwardWeight)
                { // stop the search: the max search weight has been reached.
                    break;
                }
                if(maxSettles < (settledVertices.Forward.Count + settledVertices.Backward.Count))
                { // stop the search: the max settles cound has been reached.
                    break;
                }

                // do a forward search.
                if (queueForward.Count > 0)
                {
                    this.SearchForward(graph, settledVertices, queueForward, -1);
                }

                // do a backward search.
                if (queueBackward.Count > 0)
                {
                    this.SearchBackward(graph, settledVertices, queueBackward, -1);
                }

                // calculate stopping conditions.
                if (queueForward.Count > 0)
                {
                    queueForwardWeight = queueForward.PeekWeight();
                }
                if (queueBackward.Count > 0)
                {
                    queueBackwardWeight = queueBackward.PeekWeight();
                }
            }
            return (max <= queueBackwardWeight && max <= queueForwardWeight) || // the search has continued until both weights exceed the maximum.
                maxSettles <= (settledVertices.Forward.Count + settledVertices.Backward.Count); // or until the max settled vertices have been reached.
        }

        /// <summary>
        /// Test stopping conditions and output the best tentative route.
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        private CHBest CalculateBest(CHQueue queue)
        {
            var best = new CHBest();
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
        /// <param name="settledQueue"></param>
        /// <param name="queue"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private void SearchForward(IBasicRouterDataSource<CHEdgeData> graph, CHQueue settledQueue, IPriorityQueue<PathSegment<long>> queue,
                                   long exception)
        {
            // get the current vertex with the smallest weight.
            PathSegment<long> current = queue.Pop();
            while (current != null && settledQueue.Forward.ContainsKey(
                current.VertexId))
            { // keep trying.
                current = queue.Pop();
            }

            if (current != null)
            { // there is a next vertex found.
                // add to the settled vertices.
                PathSegment<long> previousLinkedRoute;
                if (settledQueue.Forward.TryGetValue(current.VertexId, out previousLinkedRoute))
                {
                    if (previousLinkedRoute.Weight > current.Weight)
                    {
                        // settle the vertex again if it has a better weight.
                        settledQueue.AddForward(current);
                    }
                }
                else
                {
                    // settled the vertex.
                    settledQueue.AddForward(current);
                }

                // get neighbours.
                KeyValuePair<uint, CHEdgeData>[] neighbours = graph.GetArcs(Convert.ToUInt32(current.VertexId));

                // add the neighbours to the queue.
                foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours)
                {
                    if (neighbour.Value.Forward &&
                        !settledQueue.Forward.ContainsKey(neighbour.Key) &&
                        (exception == 0 || (exception != neighbour.Key &&
                        exception != neighbour.Value.ContractedVertexId)))
                    {
                        // if not yet settled.
                        var routeToNeighbour = new PathSegment<long>(
                            neighbour.Key, current.Weight + neighbour.Value.Weight, current);
                        //queue.Push(route_to_neighbour);
                        queue.Push(routeToNeighbour, (float)routeToNeighbour.Weight);
                    }
                    else if (neighbour.Value.Forward &&
                        (exception == 0 || (exception != neighbour.Key &&
                        exception != neighbour.Value.ContractedVertexId)))
                    {
                        // node was settled before: make sure this route is not shorter.
                        var routeToNeighbour = new PathSegment<long>(
                            neighbour.Key, current.Weight + neighbour.Value.Weight, current);

                        // remove from the queue again when there is a shorter route found.
                        if (settledQueue.Forward[neighbour.Key].Weight > routeToNeighbour.Weight)
                        {
                            settledQueue.Forward.Remove(neighbour.Key);
                            //queue.Push(route_to_neighbour);
                            queue.Push(routeToNeighbour, (float)routeToNeighbour.Weight);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Do one backward search step.
        /// </summary>
        /// <param name="settledQueue"></param>
        /// <param name="queue"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private void SearchBackward(IBasicRouterDataSource<CHEdgeData> graph, CHQueue settledQueue, IPriorityQueue<PathSegment<long>> queue,
                                    long exception)
        {
            // get the current vertex with the smallest weight.
            //PathSegment<long> current = queue.Pop();
            PathSegment<long> current = queue.Pop();
            while (current != null && settledQueue.Backward.ContainsKey(
                current.VertexId))
            { // keep trying.
                current = queue.Pop();
            }

            if (current != null)
            {
                // add to the settled vertices.
                PathSegment<long> previousLinkedRoute;
                if (settledQueue.Backward.TryGetValue(current.VertexId, out previousLinkedRoute))
                {
                    if (previousLinkedRoute.Weight > current.Weight)
                    {
                        // settle the vertex again if it has a better weight.
                        settledQueue.AddBackward(current);
                    }
                }
                else
                {
                    // settled the vertex.
                    settledQueue.AddBackward(current);
                }

                // get neighbours.
                KeyValuePair<uint, CHEdgeData>[] neighbours = graph.GetArcs(Convert.ToUInt32(current.VertexId));

                // add the neighbours to the queue.
                foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours)
                {
                    if (neighbour.Value.Backward &&
                        !settledQueue.Backward.ContainsKey(neighbour.Key)
                        && (exception == 0 || (exception != neighbour.Key &&
                        exception != neighbour.Value.ContractedVertexId)))
                    {
                        // if not yet settled.
                        var routeToNeighbour = new PathSegment<long>(
                            neighbour.Key, current.Weight + neighbour.Value.Weight, current);
                        //queue.Push(route_to_neighbour);
                        queue.Push(routeToNeighbour, (float)routeToNeighbour.Weight);
                    }
                    else if (neighbour.Value.Backward &&
                        (exception == 0 || (exception != neighbour.Key &&
                        exception != neighbour.Value.ContractedVertexId)))
                    {
                        // node was settled before: make sure this route is not shorter.
                        var routeToNeighbour = new PathSegment<long>(
                            neighbour.Key, current.Weight + neighbour.Value.Weight, current);

                        // remove from the queue again when there is a shorter route found.
                        if (settledQueue.Backward[neighbour.Key].Weight > routeToNeighbour.Weight)
                        {
                            settledQueue.Backward.Remove(neighbour.Key);
                            queue.Push(routeToNeighbour, (float)routeToNeighbour.Weight);
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
        private PathSegment<long> ExpandBestResult(IBasicRouterDataSource<CHEdgeData> graph, CHResult result)
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
            return this.ExpandPath(graph, route);
        }

        /// <summary>
        /// Converts the CH paths to complete paths in the orginal network.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private PathSegment<long> ExpandPath(IBasicRouterDataSource<CHEdgeData> graph, PathSegment<long> path)
        {
            // construct the full CH path.
            PathSegment<long> current = path;
            PathSegment<long> expandedPath = null;

            if (current != null && current.From == null)
            { // path contains just a single point.
                expandedPath = current;
            }
            else
            { // path containts at least two points or none at all.
                while (current != null && current.From != null)
                {
                    // recursively convert edge.
                    var localPath =
                        new PathSegment<long>(current.VertexId, -1, new PathSegment<long>(
                                                                    current.From.VertexId));
                    PathSegment<long> expandedArc = this.ConvertArc(graph, localPath);
                    if (expandedPath != null)
                    {
                        expandedPath = expandedPath.ConcatenateAfter(expandedArc);
                    }
                    else
                    {
                        expandedPath = expandedArc;
                    }

                    current = current.From;
                }
            }
            return expandedPath;
        }

        /// <summary>
        /// Converts the given edge and expands it if needed.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        private PathSegment<long> ConvertArc(IBasicRouterDataSource<CHEdgeData> graph, PathSegment<long> edge)
        {
            if (edge.VertexId < 0 || edge.From.VertexId < 0)
            { // these edges are not part of the regular network!
                return edge;
            }

            // get the edge by querying the forward neighbours of the from-vertex.
            //CHVertex from_vertex = _data.GetCHVertex(edge.From.VertexId);
            KeyValuePair<uint, CHEdgeData>[] neighbours = graph.GetArcs(Convert.ToUInt32(edge.From.VertexId));

            // find the edge with lowest weight.
            var arc = new KeyValuePair<uint, CHEdgeData>(0, null);
            foreach (KeyValuePair<uint, CHEdgeData> forwardArc in neighbours.Where<KeyValuePair<uint, CHEdgeData>>(
                a => a.Key == edge.VertexId && a.Value.Forward))
            {
                if (arc.Value == null)
                {
                    arc = forwardArc;
                }
                else if (arc.Value.Weight > forwardArc.Value.Weight)
                {
                    arc = forwardArc;
                }
            }
            if (arc.Value == null)
            {
                neighbours = graph.GetArcs(Convert.ToUInt32(edge.VertexId));
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
                return this.ConvertArc(graph, edge, arc.Key, Convert.ToUInt32(arc.Value.ContractedVertexId), Convert.ToUInt32(edge.VertexId));
            }
            else
            {
                return this.ConvertArc(graph, edge, Convert.ToUInt32(edge.From.VertexId), arc.Value.ContractedVertexId, arc.Key);
            }
        }

        private PathSegment<long> ConvertArc(IBasicRouterDataSource<CHEdgeData> graph, PathSegment<long> edge,
                                         uint vertexFromId, uint vertexContractedId, uint vertexToId)
        {
            // check if the arc is a shortcut.
            if (vertexContractedId > 0)
            {
                // arc is a shortcut.
                var firstPath = new PathSegment<long>(vertexToId, -1,
                                                             new PathSegment<long>(vertexContractedId));
                PathSegment<long> firstPathExpanded = this.ConvertArc(graph, firstPath);

                var secondPath = new PathSegment<long>(vertexContractedId, -1,
                                                              new PathSegment<long>(vertexFromId));
                PathSegment<long> secondPathExpanded = this.ConvertArc(graph, secondPath);


                // link the two paths.
                firstPathExpanded = firstPathExpanded.ConcatenateAfter(secondPathExpanded);

                return firstPathExpanded;
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
        /// <param name="verticesOnly"></param>
        public SearchClosestResult SearchClosest(IBasicRouterDataSource<CHEdgeData> graph, IRoutingInterpreter interpreter,
            Vehicle vehicle, GeoCoordinate coordinate, float delta, IEdgeMatcher matcher, TagsCollection pointTags)
        {
            return this.SearchClosest(graph, interpreter, vehicle, coordinate, delta, matcher, pointTags, false);
        }

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
        /// <param name="verticesOnly"></param>
        public SearchClosestResult SearchClosest(IBasicRouterDataSource<CHEdgeData> graph, IRoutingInterpreter interpreter,
            Vehicle vehicle, GeoCoordinate coordinate, float delta, IEdgeMatcher matcher, TagsCollection pointTags, bool verticesOnly)
        {
            double searchBoxSize = delta;
            // build the search box.
            var searchBox = new GeoCoordinateBox(new GeoCoordinate(
                coordinate.Latitude - searchBoxSize, coordinate.Longitude - searchBoxSize),
                                                               new GeoCoordinate(
                coordinate.Latitude + searchBoxSize, coordinate.Longitude + searchBoxSize));

            // get the arcs from the data source.
            KeyValuePair<uint, KeyValuePair<uint, CHEdgeData>>[] arcs = graph.GetArcs(searchBox);

            // loop over all.
            var closestWithMatch = new SearchClosestResult(double.MaxValue, 0);
            var closestWithoutMatch = new SearchClosestResult(double.MaxValue, 0);
            if (!verticesOnly)
            {
                foreach (KeyValuePair<uint, KeyValuePair<uint, CHEdgeData>> arc in arcs)
                {
                    TagsCollection arcTags = graph.TagsIndex.Get(arc.Value.Value.Tags);
                    // test the two points.
                    float fromLatitude, fromLongitude;
                    float toLatitude, toLongitude;
                    double distance;
                    if (graph.GetVertex(arc.Key, out fromLatitude, out fromLongitude) &&
                        graph.GetVertex(arc.Value.Key, out toLatitude, out toLongitude))
                    { // return the vertex.
                        var fromCoordinates = new GeoCoordinate(fromLatitude, fromLongitude);
                        distance = coordinate.Distance(fromCoordinates);

                        if (distance < 0.00001)
                        { // the distance is smaller than the tolerance value.
                            closestWithoutMatch = new SearchClosestResult(
                                distance, arc.Key);
                            if (matcher == null ||
                                (pointTags == null || pointTags.Count == 0) ||
                                matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                            {
                                closestWithMatch = new SearchClosestResult(
                                    distance, arc.Key);
                                break;
                            }
                        }

                        if (distance < closestWithoutMatch.Distance)
                        { // the distance is smaller.
                            closestWithoutMatch = new SearchClosestResult(
                                distance, arc.Key);

                            // try and match.
                            //if(matcher.Match(_
                        }
                        var toCoordinates = new GeoCoordinate(toLatitude, toLongitude);
                        distance = coordinate.Distance(toCoordinates);

                        if (distance < closestWithoutMatch.Distance)
                        { // the distance is smaller.
                            closestWithoutMatch = new SearchClosestResult(
                                distance, arc.Value.Key);

                            // try and match.
                            //if(matcher.Match(_
                        }

                        // get the uncontracted arc from the contracted vertex.
                        KeyValuePair<uint, KeyValuePair<uint, CHEdgeData>> uncontracted = arc;
                        while (uncontracted.Value.Value.HasContractedVertex)
                        { // try to inflate the contracted vertex.
                            KeyValuePair<uint, CHEdgeData>[] contractedArcs =
                                graph.GetArcs(uncontracted.Value.Value.ContractedVertexId);

                            foreach (KeyValuePair<uint, CHEdgeData> contractedArc in contractedArcs)
                            { // loop over all contracted arcs.
                                if (contractedArc.Key == uncontracted.Key)
                                { // the edge is and edge to the target.
                                    var data = new CHEdgeData();
                                    data.Direction = contractedArc.Value.Direction;
                                    data.ContractedVertexId = contractedArc.Value.ContractedVertexId;
                                    data.Tags = contractedArc.Value.Tags;
                                    data.Weight = contractedArc.Value.Weight;

                                    uncontracted = new KeyValuePair<uint, KeyValuePair<uint, CHEdgeData>>(
                                        uncontracted.Key, new KeyValuePair<uint, CHEdgeData>(
                                            contractedArc.Key, data));
                                    break;
                                }
                            }
                        }
                        // try the to-vertex of the non-contracted arc.
                        if (arc.Value.Key != uncontracted.Value.Key)
                        { // the to-vertex was contracted, not anymore!
                            if (graph.GetVertex(uncontracted.Value.Key, out toLatitude, out toLongitude))
                            { // the to vertex was found
                                toCoordinates = new GeoCoordinate(toLatitude, toLongitude);
                                distance = coordinate.Distance(toCoordinates);

                                if (distance < closestWithoutMatch.Distance)
                                { // the distance is smaller.
                                    closestWithoutMatch = new SearchClosestResult(
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
                        double distanceTotal = fromCoordinates.Distance(toCoordinates);
                        if (distanceTotal > 0)
                        { // the from/to are not the same location.
                            var line = new GeoCoordinateLine(fromCoordinates, toCoordinates, true, true);
                            distance = line.Distance(coordinate);

                            if (distance < closestWithoutMatch.Distance)
                            { // the distance is smaller.
                                PointF2D projectedPoint =
                                    line.ProjectOn(coordinate);

                                // calculate the position.
                                if (projectedPoint != null)
                                { // calculate the distance
                                    double distancePoint = fromCoordinates.Distance(projectedPoint);
                                    double position = distancePoint / distanceTotal;

                                    closestWithoutMatch = new SearchClosestResult(
                                        distance, uncontracted.Key, uncontracted.Value.Key, position);

                                    // try and match.
                                    //if(matcher.Match(_
                                }
                            }
                        }
                    }
                }
            }
            else
            { // only find closest vertices.
                // loop over all.
                foreach (KeyValuePair<uint, KeyValuePair<uint, CHEdgeData>> arc in arcs)
                {
                    float fromLatitude, fromLongitude;
                    float toLatitude, toLongitude;
                    if (graph.GetVertex(arc.Key, out fromLatitude, out fromLongitude) &&
                        graph.GetVertex(arc.Value.Key, out toLatitude, out toLongitude))
                    {
                        var vertexCoordinate = new GeoCoordinate(fromLatitude, fromLongitude);
                        double distance = coordinate.Distance(vertexCoordinate);
                        if (distance < closestWithoutMatch.Distance)
                        { // the distance found is closer.
                            closestWithoutMatch = new SearchClosestResult(
                                distance, arc.Key);
                        }

                        vertexCoordinate = new GeoCoordinate(toLatitude, toLongitude);
                        distance = coordinate.Distance(vertexCoordinate);
                        if (distance < closestWithoutMatch.Distance)
                        { // the distance found is closer.
                            closestWithoutMatch = new SearchClosestResult(
                                distance, arc.Value.Key);
                        }
                    }
                }
            }

            // return the best result.
            if (closestWithMatch.Distance < double.MaxValue)
            {
                return closestWithMatch;
            }
            return closestWithoutMatch;
        }

        #endregion

    }
}