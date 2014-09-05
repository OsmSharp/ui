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

using OsmSharp.Logging;
using OsmSharp.Routing.Graph.PreProcessor;
using OsmSharp.Routing.Graph.Router;
using System;
using System.Collections.Generic;
using OsmSharp.Routing.Graph;
using System.Linq;
using OsmSharp.Collections.PriorityQueues;

namespace OsmSharp.Routing.CH.PreProcessing
{
    /// <summary>
    /// Pre-processor to construct a Contraction Hierarchy (CH).
    /// </summary>
    public class CHPreProcessor : IPreProcessor
    {
        /// <summary>
        /// Holds the data target.
        /// </summary>
        private IDynamicGraphRouterDataSource<CHEdgeData> _target;

        /// <summary>
        /// Holds the edge comparer.
        /// </summary>
        private IDynamicGraphEdgeComparer<CHEdgeData> _comparer;

        /// <summary>
        /// Creates a new pre-processor.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="calculator"></param>
        /// <param name="witnessCalculator"></param>
        public CHPreProcessor(IDynamicGraphRouterDataSource<CHEdgeData> target,
                INodeWeightCalculator calculator,
                INodeWitnessCalculator witnessCalculator)
        {
            _comparer = null;

            _target = target;

            _calculator = calculator;
            _witnessCalculator = witnessCalculator;

            _queue = new BinairyHeap<uint>(target.VertexCount + 1);
            _lowestPriorities = new float[target.VertexCount + 1];
            for(int idx = 0; idx < _lowestPriorities.Length; idx++)
            { // uncontracted = priority != float.MinValue.
                _lowestPriorities[idx] = float.MaxValue;
            }
        }

        #region Contraction

        /// <summary>
        /// Holds a weight calculator.
        /// </summary>
        private INodeWeightCalculator _calculator;

        /// <summary>
        /// Holds a witness calculator.
        /// </summary>
        private INodeWitnessCalculator _witnessCalculator;

        /// <summary>
        /// Holds a witness calculator just for contraction.
        /// </summary>
        private INodeWitnessCalculator _contractionWitnessCalculator = new OsmSharp.Routing.CH.PreProcessing.Witnesses.DykstraWitnessCalculator(20);

        /// <summary>
        /// Starts pre-processing all nodes
        /// </summary>
        public void Start()
        {
            _missesQueue = new Queue<bool>();
            _misses = 0;

            // calculate the entire queue.
            this.RecalculateQueue();

            // loop over the priority queue until it's empty.
            uint total = _target.VertexCount;
            uint current = 1;
            uint? vertex = this.SelectNext();
            float latestProgress = 0;
            while (vertex != null)
            {
                // contract the nodes.
                this.Contract(vertex.Value);

                // select the next vertex.
                vertex = this.SelectNext();

                // calculate and log progress.
                float progress = (float)(System.Math.Floor(((double)current / (double)total) * 1000) / 10.0);
                if (progress != latestProgress)
                {
                    OsmSharp.Logging.Log.TraceEvent("CHPreProcessor", TraceEventType.Information,
                        "Pre-processing... {0}% [{1}/{2}]", progress, current, total);
                    latestProgress = progress;
                    //if (progress % 1 == 0)
                    //{
                    //    int totalCardinality = 0;
                    //    int uncontracted = 0;
                    //    int maxCardinality = 0;
                    //    for (uint v = 0; v < _target.VertexCount; v++)
                    //    {
                    //        if (!this.IsContracted(v))
                    //        {
                    //            var edges = _target.GetEdges(v);
                    //            if (edges != null)
                    //            {
                    //                int edgesCount = 0;
                    //                foreach(var edge in edges)
                    //                {
                    //                    if(!edge.EdgeData.ToLower &&
                    //                        edge.EdgeData.Forward)
                    //                    {
                    //                        edgesCount++;
                    //                    }
                    //                }
                    //                totalCardinality = edgesCount + totalCardinality;
                    //                if (maxCardinality < edgesCount)
                    //                {
                    //                    maxCardinality = edgesCount;
                    //                }
                    //            }
                    //            uncontracted++;
                    //        }
                    //    }
                    //    OsmSharp.Logging.Log.TraceEvent("CHPreProcessor", TraceEventType.Information,
                    //        "Average card uncontracted vertices: {0} with max {1}", (double)totalCardinality / (double)uncontracted, maxCardinality);
                    //}
                }
                current++;
            }

            OsmSharp.Logging.Log.TraceEvent("CHPreProcessor", TraceEventType.Information,
                "Pre-processing finsihed!");
        }

        /// <summary>
        /// Recalculates the entire queue.
        /// </summary>
        public void RecalculateQueue()
        {
            OsmSharp.Logging.Log.TraceEvent("CHPreProcessor", TraceEventType.Information,
                "Recalculating queue...");
            uint total = _target.VertexCount;
            uint current = 1;

            double latestProgress = 0;
            _queue.Clear();
            for (uint currentVertex = 1; currentVertex <= total; currentVertex++)
            {
                if (!this.IsContracted(currentVertex))
                {
                    var priority = _calculator.Calculate(currentVertex);

                    _queue.Push(currentVertex, priority);
                    _lowestPriorities[currentVertex] = priority;

                    float progress = (float)System.Math.Round((((double)current / (double)total) * 100));
                    if (progress != latestProgress)
                    {
                        //OsmSharp.Logging.Log.TraceEvent("CHPreProcessor", TraceEventType.Information,
                        //    "Building CH Queue... {0}%", progress);
                        latestProgress = progress;
                    }
                    current++;
                }
            }
        }

        /// <summary>
        /// Contracts the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public void Contract(uint vertex)
        {
            if (this.IsContracted(vertex))
            {
                throw new Exception("Is already contracted!");
            }

            // get all information from the source.
            var edges = _target.GetEdges(vertex).ToList();

            // report the before contraction event.
            this.OnBeforeContraction(vertex, edges);

            // replace the adjacent edges with edges that are point up.
            var edgesForContractions = new List<Edge<CHEdgeData>>(edges.Count);
            var tos = new List<uint>(edges.Count);
            foreach (var edge in edges)
            {
                if (!edge.EdgeData.ToLower)
                { // the edge is not to lower or higher.
                    // use this edge for contraction.
                    edgesForContractions.Add(edge);
                    tos.Add(edge.Neighbour);

                    // overwrite the old edge making it point 'to higher' only.
                    var toHigherData = edge.EdgeData;
                    toHigherData.SetContractedDirection(true, false);
                    _target.AddEdge(vertex, edge.Neighbour, toHigherData, null);
                }
            }

            // loop over each combination of edges just once.
            int newEdge = 0;
            var witnesses = new bool[edgesForContractions.Count];
            var tosWeights = new List<float>(edgesForContractions.Count);
            var toRequeue = new HashSet<uint>();
            for (int x = 0; x < edgesForContractions.Count; x++)
            { // loop over all elements first.
                var xEdge = edgesForContractions[x];
                if (!xEdge.EdgeData.Backward) { continue; }

                // calculate max weight.
                tosWeights.Clear();
                for (int idx = 0; idx < edgesForContractions.Count; idx++)
                {
                    // update maxWeight.
                    var yEdge = edgesForContractions[idx];
                    if (xEdge.Neighbour != yEdge.Neighbour &&
                        yEdge.EdgeData.Forward)
                    {
                        // reset witnesses.
                        float weight = (float)xEdge.EdgeData.BackwardWeight + (float)yEdge.EdgeData.ForwardWeight;
                        witnesses[idx] = false;
                        tosWeights.Add(weight);
                    }
                    else
                    { // already set this to true, not use calculating it's witness.
                        witnesses[idx] = true;
                        tosWeights.Add(0);
                    }
                }

                _contractionWitnessCalculator.Exists(_target, xEdge.Neighbour, tos, tosWeights, int.MaxValue, ref witnesses);

                for (int y = 0; y < edgesForContractions.Count; y++)
                { // loop over all elements.
                    var yEdge = edgesForContractions[y];

                    // add the combinations of these edges.
                    if (yEdge.EdgeData.Forward &&
                        xEdge.Neighbour != yEdge.Neighbour)
                    { // there is a connection from x to y and there is no witness path.
                        // create x-to-y data and edge.
                        var forward = (xEdge.EdgeData.Backward && yEdge.EdgeData.Forward) && !witnesses[y];

                        if (forward)
                        { // add the edge if there is usefull info or if there needs to be a neighbour relationship.
                            // calculate the total weight.
                            var forwardWeight = xEdge.EdgeData.BackwardWeight + yEdge.EdgeData.ForwardWeight;

                            CHEdgeData data;
                            if (_target.GetEdge(xEdge.Neighbour, yEdge.Neighbour, out data))
                            { // there already is an edge evaluate for each direction.
                                if (forward && data.ForwardWeight > forwardWeight)
                                { // replace forward edge.
                                    toRequeue.Add(xEdge.Neighbour);
                                    newEdge++;
                                    data.ForwardWeight = forwardWeight;
                                    data.ForwardContractedId = vertex;
                                    _target.AddEdge(xEdge.Neighbour, yEdge.Neighbour, data, null, _comparer);
                                }
                            }
                            else
                            { // there is no edge, just add the data.
                                var dataXToY = new CHEdgeData();
                                dataXToY.SetContractedDirection(false, false);

                                toRequeue.Add(xEdge.Neighbour);
                                newEdge++;
                                dataXToY.ForwardWeight = forwardWeight;
                                dataXToY.ForwardContractedId = vertex;
                                _target.AddEdge(xEdge.Neighbour, yEdge.Neighbour, dataXToY, null, _comparer);

                            }
                        }
                    }
                }
            }

            // update priority of direct neighbours.
            foreach (var neighbour in toRequeue)
            {
                this.ReQueue(neighbour);
            }

            // mark the vertex as contracted.
            this.MarkContracted(vertex);

            // notify a contracted neighbour.
            _calculator.NotifyContracted(vertex);

            // report the after contraction event.
            this.OnAfterContraction(vertex, edges);
        }

        #endregion

        #region Contraction Status

        /// <summary>
        /// Keeps and array of the contraction status of vertices.
        /// </summary>
        private float[] _lowestPriorities;

        /// <summary>
        /// Mark the vertex as contacted.
        /// </summary>
        /// <param name="vertex"></param>
        private void MarkContracted(uint vertex)
        {
            _lowestPriorities[vertex] = float.MinValue;
        }

        /// <summary>
        /// Returns true if the vertex is contracted.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private bool IsContracted(uint vertex)
        {
            return _lowestPriorities[vertex] == float.MinValue;
        }

        #endregion

        #region Selection

        /// <summary>
        /// Holds a queue of contraction priorities.
        /// </summary>
        private BinairyHeap<uint> _queue;

        /// <summary>
        /// The amount of queue 'misses' to recalculated.
        /// </summary>
        private int _k = 30;

        /// <summary>
        /// Holds a counter of all misses.
        /// </summary>
        private int _misses;

        /// <summary>
        /// Holds the misses queue.
        /// </summary>
        private Queue<bool> _missesQueue;

        /// <summary>
        /// Select the next vertex from the queue.
        /// </summary>
        /// <returns></returns>
        private uint? SelectNext()
        {
            // first check the first of the current queue.
            while (_queue.Count > 0)
            { // get the first vertex and check.
                uint first_queued = _queue.Peek();
                if(this.IsContracted(first_queued))
                { // already contracted, priority was updated.
                    _queue.Pop();
                    continue;
                }
                float current_priority = _queue.PeekWeight();

                // the lazy updating part!
                // calculate priority
                float priority = _calculator.Calculate(first_queued);
                if (priority != current_priority)
                { // a succesfull update.
                    _missesQueue.Enqueue(true);
                    _misses++;
                }
                else
                { // an unsuccessfull update.
                    _missesQueue.Enqueue(false);
                }
                if (_missesQueue.Count > _k)
                { // dequeue and update the misses.
                    if (_missesQueue.Dequeue())
                    {
                        _misses--;
                    }
                }

                // if the misses are _k
                if (_misses == _k)
                { // recalculation.
                    this.RecalculateQueue();

                    //int totalCadinality = 0;
                    //for (uint vertex = 0; vertex < _target.VertexCount; vertex++)
                    //{
                    //    var arcs = _target.GetEdges(vertex);
                    //    if (arcs != null)
                    //    {
                    //        totalCadinality = arcs.Count() + totalCadinality;
                    //    }
                    //}
                    //OsmSharp.Logging.Log.TraceEvent("CHPreProcessor", TraceEventType.Information,
                    //    "Average card: {0}", (double)totalCadinality / (double)_target.VertexCount);

                    _missesQueue.Clear();
                    _misses = 0;

                    _target.Compress();
                }
                else
                { // no recalculation.
                    if (priority != current_priority)
                    { // re-enqueue the weight.
                        _queue.Pop();
                        _queue.Push(first_queued, priority);
                    }
                    else
                    {
                        //if (this.CanBeContracted(first_queued))
                        //{ // yet, this vertex can be contracted!
                        return _queue.Pop();
                    }
                }
            }

            // check the queue.
            if (_queue.Count > 0)
            {
                throw new Exception("Unqueued items left!, CanBeContracted is too restrictive!");
            }
            return null; // all nodes have been contracted.
        }

        ///// <summary>
        ///// Returns true if the vertex can be contracted compared to it's neighbours.
        ///// </summary>
        ///// <param name="vertex"></param>
        ///// <returns></returns>
        //private bool CanBeContracted(uint vertex)
        //{
        //    // calculate the priority of the vertex first.
        //    float priority = this.ReQueue(vertex);

        //    if (priority < float.MaxValue)
        //    { // there is a valid priority.
        //        return this.CanBeContractedLocally(vertex, priority);
        //    }
        //    return false; // priority is 'infinite'.
        //}

        /// <summary>
        /// Re-calculates the priority and queues the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private void ReQueue(uint vertex)
        {
            var priority = _calculator.Calculate(vertex);

            // enqueue the vertex.
            if (_lowestPriorities[vertex] < priority)
            { // only queue again when lower, vertex must be moved forward in the queue.
                _queue.Push(vertex, priority);
                _lowestPriorities[vertex] = priority;
            }
            else
            { // priority is higher, will be detected by lazy-updating.

            }
        }

        /// <summary>
        /// Returns true if the given vertex's neighbours have a higher priority.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        private bool CanBeContractedLocally(uint vertex, float priority)
        {
            // compare the priority with that of it's neighbours.
            foreach (var edge in _target.GetEdges(vertex))
            { // check the priority.
                if (!this.IsContracted(edge.Neighbour))
                {
                    float edge_priority = _calculator.Calculate(edge.Neighbour);
                    if (edge_priority < priority) // TODO: <= or <
                    { // there is a neighbour with lower priority.
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion

        #region Notifications

        /// <summary>
        /// The delegate for arc notifications.
        /// </summary>
        /// <param name="from_id"></param>
        /// <param name="to_id"></param>
        public delegate void ArcDelegate(uint from_id, uint to_id);

        /// <summary>
        /// The event.
        /// </summary>
        public event ArcDelegate NotifyArcEvent;

        /// <summary>
        /// Notifies a new arc.
        /// </summary>
        /// <param name="from_id"></param>
        /// <param name="to_id"></param>
        private void NotifyArc(uint from_id, uint to_id)
        {
            if (this.NotifyArcEvent != null)
            {
                this.NotifyArcEvent(from_id, to_id);
            }
        }
        /// <summary>
        /// The event.
        /// </summary>
        public event ArcDelegate NotifyRemoveEvent;

        /// <summary>
        /// Notifies an arc removal.
        /// </summary>
        /// <param name="from_id"></param>
        /// <param name="to_id"></param>
        private void NotifyRemove(uint from_id, uint to_id)
        {
            if (this.NotifyRemoveEvent != null)
            {
                this.NotifyRemoveEvent(from_id, to_id);
            }
        }

        /// <summary>
        /// The delegate for arc notifications.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="edges"></param>
        public delegate void VertexDelegate(uint vertex, List<Edge<CHEdgeData>> edges);

        /// <summary>
        /// The before contraction delegate.
        /// </summary>
        public event VertexDelegate OnBeforeContractionEvent;

        /// <summary>
        /// Notifies an arc removal.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="edges"></param>
        private void OnBeforeContraction(uint vertex, List<Edge<CHEdgeData>> edges)
        {
            if (this.OnBeforeContractionEvent != null)
            {
                this.OnBeforeContractionEvent(vertex, edges);
            }
        }

        /// <summary>
        /// The after contraction delegate.
        /// </summary>
        public event VertexDelegate OnAfterContractionEvent;

        /// <summary>
        /// Notifies an arc removal.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="edges"></param>
        private void OnAfterContraction(uint vertex, List<Edge<CHEdgeData>> edges)
        {
            if (this.OnAfterContractionEvent != null)
            {
                this.OnAfterContractionEvent(vertex, edges);
            }
        }


        #endregion

        #region Properties

        /// <summary>
        /// Returns the node weight calculator used by this pre-processor.
        /// </summary>
        public INodeWeightCalculator NodeWeightCalculator
        {
            get
            {
                return _calculator;
            }
        }

        /// <summary>
        /// Returns the node witness calculator used by this pre-processor.
        /// </summary>
        public INodeWitnessCalculator NodeWitnessCalculator
        {
            get
            {
                return _witnessCalculator;
            }
        }
        #endregion

    }
}