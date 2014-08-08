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
        private CHEdgeDataComparer _comparer;

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
            _comparer = new CHEdgeDataComparer();

            _target = target;

            _calculator = calculator;
            _witnessCalculator = witnessCalculator;

            _queue = new CHPriorityQueue();
            _contracted = new bool[1000];
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
        private INodeWitnessCalculator _contractionWitnessCalculator = new OsmSharp.Routing.CH.PreProcessing.Witnesses.DykstraWitnessCalculator(100);

        /// <summary>
        /// Starts pre-processing all nodes
        /// </summary>
        public void Start()
        {
            _missesQueue = new Queue<bool>();
            _misses = 0;

            uint total = _target.VertexCount;
            uint current = 1;

            double latestProgress = 0;
            for (uint currentVertex = 1; currentVertex <= total; currentVertex++)
            {
                float priority = _calculator.Calculate(currentVertex);

                _queue.Enqueue(currentVertex, priority);

                float progress = (float)System.Math.Round((((double)current / (double)total) * 100));
                if (progress != latestProgress)
                {
                    OsmSharp.Logging.Log.TraceEvent("CHPreProcessor", TraceEventType.Information,
                        "Building CH Queue... {0}%", progress);
                    latestProgress = progress;
                }
                current++;
            }

            // loop over the priority queue until it's empty.
            current = 1;
            uint? vertex = this.SelectNext();
            latestProgress = 0;
            while (vertex != null)
            {
                // contract the nodes.
                this.Contract(vertex.Value);

                // select the next vertex.
                vertex = this.SelectNext();

                double realProgress = (double)current / (double)total;
                double progress = (float)System.Math.Floor(realProgress * 1000) / 10.0;
                if (progress != latestProgress)
                {
                    OsmSharp.Logging.Log.TraceEvent("CHPreProcessor", TraceEventType.Information,
                        "Pre-processing... {0}% [{1}/{2}]", progress, current, total);
                    latestProgress = progress;
                }
                current++;
            }

            OsmSharp.Logging.Log.TraceEvent("CHPreProcessor", TraceEventType.Information,
                "Pre-processing finsihed!");
        }

        /// <summary>
        /// Contracts the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public void Contract(uint vertex)
        {
            if (_contracted.Length > vertex && _contracted[vertex])
            {
                throw new Exception("Is already contracted!");
            }

            // keep the neighbours.
            var neighbours = new HashSet<KeyValuePair<uint, CHEdgeData>>();

            // get all information from the source.
            var edges = _target.GetEdges(vertex).ToList();

            // report the before contraction event.
            this.OnBeforeContraction(vertex, edges);

            // replace the adjacent edges with edges that are point up.
            var edgesForContractions = new List<Edge<CHEdgeData>>(edges.Count);
            foreach (var edge in edges)
            {
                if (!edge.EdgeData.ToLower && !edge.EdgeData.ToHigher)
                { // the edge is not to lower or higher.
                    // use this edge for contraction.
                    edgesForContractions.Add(edge);

                    // overwrite the old edge making it point 'to higher' only.
                    _target.AddEdge(vertex, edge.Neighbour,
                        new CHEdgeData(edge.EdgeData.Weight, edge.EdgeData.Forward, edge.EdgeData.Backward, true, edge.EdgeData.ContractedVertexId, edge.EdgeData.Tags), null);
                }
            }

            // loop over each combination of edges just once.
            for (int x = 1; x < edgesForContractions.Count; x++)
            { // loop over all elements first.
                var xEdge = edgesForContractions[x];

                for (int y = 0; y < x; y++)
                { // loop over all elements.
                    var yEdge = edgesForContractions[y];

                    // calculate the total weight.
                    var weight = xEdge.EdgeData.Weight + yEdge.EdgeData.Weight;

                    // add the combinations of these edges.
                    if (((xEdge.EdgeData.Backward && yEdge.EdgeData.Forward) ||
                        (yEdge.EdgeData.Backward && xEdge.EdgeData.Forward)) &&
                        (xEdge.Neighbour != yEdge.Neighbour))
                    { // there is a connection from x to y and there is no witness path.
                        var witnessXToY = _contractionWitnessCalculator.Exists(_target, xEdge.Neighbour, 
                            yEdge.Neighbour, vertex, weight, int.MaxValue);
                        var witnessYToX = _contractionWitnessCalculator.Exists(_target, yEdge.Neighbour,
                            xEdge.Neighbour, vertex, weight, int.MaxValue);

                        // create x-to-y data and edge.
                        var dataXToY = new CHEdgeData();
                        var forward = (xEdge.EdgeData.Backward && yEdge.EdgeData.Forward) &&
                            !witnessXToY;
                        var backward = (yEdge.EdgeData.Backward && xEdge.EdgeData.Forward) &&
                            !witnessYToX;
                        if ((forward || backward) ||
                            !_target.ContainsEdge(xEdge.Neighbour, yEdge.Neighbour))
                        { // add the edge if there is usefull info or if there needs to be a neighbour relationship.
                            dataXToY.SetDirection(forward, backward);
                            dataXToY.Weight = weight;
                            dataXToY.ContractedVertexId = vertex;

                            _target.AddEdge(xEdge.Neighbour, yEdge.Neighbour, dataXToY, null, _comparer);
                        }
                    }
                }
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
        private bool[] _contracted;

        /// <summary>
        /// Mark the vertex as contacted.
        /// </summary>
        /// <param name="vertex"></param>
        private void MarkContracted(uint vertex)
        {
            if (_contracted.Length <= vertex)
            {
                Array.Resize<bool>(ref _contracted, (int)vertex + 1000);
            }
            _contracted[vertex] = true;
        }

        /// <summary>
        /// Returns true if the vertex is contracted.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private bool IsContracted(uint vertex)
        {
            if (_contracted.Length > vertex)
            {
                return _contracted[vertex];
            }
            return false;
        }

        #endregion

        #region Selection

        /// <summary>
        /// Holds a queue of contraction priorities.
        /// </summary>
        private CHPriorityQueue _queue;

        /// <summary>
        /// The amount of queue 'misses' to recalculated.
        /// </summary>
        private int _k = 60;

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
                // TODO: peek and get the weight at the same time.
                uint first_queued = _queue.Peek();

                // the lazy updating part!
                // calculate priority
                float priority = _calculator.Calculate(first_queued);
                float current_priority = _queue.Weight(first_queued);
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
                    OsmSharp.Logging.Log.TraceEvent("CHPreProcessor", TraceEventType.Information,
                        "Recalculating queue...");

                    CHPriorityQueue new_queue = new CHPriorityQueue();

                    HashSet<KeyValuePair<uint, float>> recalculated_weights =
                        new HashSet<KeyValuePair<uint, float>>();
                    int totalCadinality = 0;
                    foreach (uint vertex in _queue)
                    {
                        recalculated_weights.Add(
                            new KeyValuePair<uint, float>(vertex, _calculator.Calculate(vertex)));
                        var arcs = _target.GetEdges(vertex).ToList();
                        if(arcs != null)
                        {
                            totalCadinality = arcs.Count + totalCadinality;
                        }
                    }
                    foreach (KeyValuePair<uint, float> pair in recalculated_weights)
                    {
                        new_queue.Enqueue(pair.Key, pair.Value);
                    }
                    OsmSharp.Logging.Log.TraceEvent("CHPreProcessor", TraceEventType.Information,
                        "Average card: {0}", (double)totalCadinality/(double)recalculated_weights.Count);
                    _queue = new_queue;
                    _missesQueue.Clear();
                    _misses = 0;
                }
                else
                { // no recalculation.
                    if (priority > current_priority)
                    { // re-enqueue the weight.
                        _queue.Enqueue(first_queued, priority);
                    }
                    else
                    {
                        //if (this.CanBeContracted(first_queued))
                        //{ // yet, this vertex can be contracted!
                        _queue.Remove(first_queued);
                        return first_queued;
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

        /// <summary>
        /// Returns true if the vertex can be contracted compared to it's neighbours.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private bool CanBeContracted(uint vertex)
        {
            // calculate the priority of the vertex first.
            float priority = this.ReQueue(vertex);

            if (priority < float.MaxValue)
            { // there is a valid priority.
                return this.CanBeContractedLocally(vertex, priority);
            }
            return false; // priority is 'infinite'.
        }

        /// <summary>
        /// Re-calculates the priority and queues the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private float ReQueue(uint vertex)
        {            
            // calculate the priority of the vertex first.
            //_queue.Remove(vertex);
            float priority = this.CalculatePriority(vertex);

            if (priority < float.MaxValue)
            {
                // enqueue the vertex.
                _queue.Enqueue(vertex, priority);
            }
            return priority;
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
                    float edge_priority = this.CalculatePriority(edge.Neighbour);
                    if (edge_priority < priority) // TODO: <= or <
                    { // there is a neighbour with lower priority.
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Calculates the priority of the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private float CalculatePriority(uint vertex)
        { // calculates the priority or gets it from the queue.
            if (_queue.Contains(vertex))
            { // gets the queued weight.
                float weight = _queue.Weight(vertex);

                if (weight == float.MaxValue)
                { // re-calculate the weight.
                    weight = _calculator.Calculate(vertex);

                    // update the queue.
                    if (weight == float.MaxValue)
                    { // do not enqueue again, node does not need to be contracted!
                        _queue.Enqueue(vertex, weight);
                    }
                }
                return weight;
            }
            else
            { // re-calculate the weight.
                return _calculator.Calculate(vertex);
            }
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