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
using System.Diagnostics;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Logging;

namespace OsmSharp.Routing.CH.PreProcessing
{
    /// <summary>
    /// Pre-processor to construct a Contraction Hierarchy (CH).
    /// </summary>
    public class CHPreProcessor
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
        /// Holds the keep direct neighbours flag.
        /// </summary>
        private bool _keepDirectNeighbours = true;


        /// <summary>
        /// Creates a new pre-processor.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="calculator"></param>
        /// <param name="witnessCalculator"></param>
        /// <param name="keepDirectNeighbours"></param>
        public CHPreProcessor(IDynamicGraphRouterDataSource<CHEdgeData> target,
                INodeWeightCalculator calculator,
                INodeWitnessCalculator witnessCalculator, 
                bool keepDirectNeighbours)
        {
            _comparer = new CHEdgeDataComparer();

            _keepDirectNeighbours = keepDirectNeighbours;

            _target = target;

            _calculator = calculator;
            _witnessCalculator = witnessCalculator;

            _queue = new CHPriorityQueue();
            _contracted = new bool[1000];
        }

        /// <summary>
        /// Creates a new pre-processor.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="calculator"></param>
        /// <param name="witnessCalculator"></param>
        public CHPreProcessor(IDynamicGraphRouterDataSource<CHEdgeData> target,
                INodeWeightCalculator calculator,
                INodeWitnessCalculator witnessCalculator)
            : this(target, calculator, witnessCalculator, true) { }

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
        /// Starts pre-processing all nodes
        /// </summary>
        /// <remarks>Returns the last-contracted vertex.</remarks>
        public void Start()
        {
            _misses_queue = new Queue<bool>();
            _misses = 0;

            uint total = _target.VertexCount;
            uint current = 1;

            float latestProgress = 0;
            for(uint current_vertex = 1; current_vertex <= total; current_vertex++)
            {
                float priority = _calculator.Calculate(current_vertex);
                //lock (_queue)
                //{
                    _queue.Enqueue(current_vertex, priority);

                    float progress = (float)System.Math.Round((((double)current / (double)total) * 100));
                    if (progress != latestProgress)
                    {
                        OsmSharp.Logging.Log.TraceEvent("CHPreProcessor", TraceEventType.Information,
                            "Building CH Queue... {0}%", progress);
                        latestProgress = progress;
                    }
                    current++;
                //}
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

                float progress = (float)System.Math.Round((((double)current / (double)total) * 100));
                if (progress != latestProgress)
                {
                    OsmSharp.Logging.Log.TraceEvent("CHPreProcessor", TraceEventType.Information,
                        "Pre-processing... {0}%", progress);
                    latestProgress = progress;
                }
                current++;
            }
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
            HashSet<KeyValuePair<uint, CHEdgeData>> neighbours = 
                new HashSet<KeyValuePair<uint, CHEdgeData>>();

            // get all information from the source.
            KeyValuePair<uint, CHEdgeData>[] edges = _target.GetArcs(vertex);

            // remove all informative edges.
            edges = edges.RemoveInformativeEdges();

            // report the before contraction event.
            this.OnBeforeContraction(vertex, edges);

            // remove the edges from the neighbours to the target.
            foreach (KeyValuePair<uint, CHEdgeData> edge in edges)
            { // remove the edge.
                _target.DeleteArc(edge.Key, vertex);

                // keep the neighbour.
                if (_keepDirectNeighbours && !edge.Value.HasContractedVertex)
                { // edge does represent a neighbour relation.
                    neighbours.Add(
                        new KeyValuePair<uint, CHEdgeData>(edge.Key, edge.Value.ConvertToInformative()));
                }
            }

            // loop over each combination of edges just once.
            for (int x = 1; x < edges.Length; x++)
            { // loop over all elements first.
                KeyValuePair<uint, CHEdgeData> xEdge = edges[x];
                if (xEdge.Value.IsInformative) { continue; }

                for (int y = 0; y < x; y++)
                { // loop over all elements.
                    KeyValuePair<uint, CHEdgeData> yEdge = edges[y];
                    if (yEdge.Value.IsInformative) { continue; }

                    // calculate the total weight.
                    float weight = xEdge.Value.Weight + yEdge.Value.Weight;

                    // add the combinations of these edges.
                    if (((xEdge.Value.Backward && yEdge.Value.Forward) ||
                        (yEdge.Value.Backward && xEdge.Value.Forward)) &&
                        (xEdge.Key != yEdge.Key))
                    { // there is a connection from x to y and there is no witness path.
                        bool witnessXToY = _witnessCalculator.Exists(_target, xEdge.Key, 
                            yEdge.Key, vertex, weight, 100);
                        bool witnessYToX = _witnessCalculator.Exists(_target, yEdge.Key, 
                            xEdge.Key, vertex, weight, 100);

                        // create x-to-y data and edge.
                        CHEdgeData dataXToY = new CHEdgeData();
                        bool forward = (xEdge.Value.Backward && yEdge.Value.Forward) &&
                            !witnessXToY;
                        bool backward = (yEdge.Value.Backward && xEdge.Value.Forward) &&
                            !witnessYToX;
                        dataXToY.SetDirection(forward, backward, true);
                        dataXToY.Weight = weight;
                        dataXToY.ContractedVertexId = vertex;
                        if ((dataXToY.Forward || dataXToY.Backward) ||
                            !_target.HasArc(xEdge.Key, yEdge.Key))
                        { // add the edge if there is usefull info or if there needs to be a neighbour relationship.
                            _target.AddArc(xEdge.Key, yEdge.Key, dataXToY, _comparer);
                        }

                        // create y-to-x data and edge.
                        CHEdgeData dataYToX = new CHEdgeData();
                        forward = (yEdge.Value.Backward && xEdge.Value.Forward) &&
                            !witnessYToX;
                        backward = (xEdge.Value.Backward && yEdge.Value.Forward) &&
                            !witnessXToY;
                        dataYToX.SetDirection(forward, backward, true);
                        dataYToX.Weight = weight;
                        dataYToX.ContractedVertexId = vertex;
                        if ((dataYToX.Forward || dataYToX.Backward) ||
                            !_target.HasArc(yEdge.Key, xEdge.Key))
                        { // add the edge if there is usefull info or if there needs to be a neighbour relationship.
                            _target.AddArc(yEdge.Key, xEdge.Key, dataYToX, _comparer);
                        }
                    }
                }
            }

            // mark the vertex as contracted.
            this.MarkContracted(vertex);

            // notify a contracted neighbour.
            _calculator.NotifyContracted(vertex);

            // add contracted neighbour edges again.
            if (_keepDirectNeighbours)
            {
                foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours)
                {
                    _target.AddArc(neighbour.Key, vertex, neighbour.Value, null);
                }
            }

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
        private int _k = 40;

        /// <summary>
        /// Holds a counter of all misses.
        /// </summary>
        private int _misses;

        /// <summary>
        /// Holds the misses queue.
        /// </summary>
        private Queue<bool> _misses_queue;

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
                    _misses_queue.Enqueue(true);
                    _misses++;
                }
                else
                { // an unsuccessfull update.
                    _misses_queue.Enqueue(false);
                }
                if (_misses_queue.Count > _k)
                { // dequeue and update the misses.
                    if (_misses_queue.Dequeue())
                    {
                        _misses--;
                    }
                }

                // if the misses are _k
                if (_misses == _k)
                { // recalculation.
                    CHPriorityQueue new_queue = new CHPriorityQueue();

                    HashSet<KeyValuePair<uint, float>> recalculated_weights =
                        new HashSet<KeyValuePair<uint, float>>();
                    foreach (uint vertex in _queue)
                    {
                        recalculated_weights.Add(
                            new KeyValuePair<uint, float>(vertex, _calculator.Calculate(vertex)));
                    }
                    foreach (KeyValuePair<uint, float> pair in recalculated_weights)
                    {
                        new_queue.Enqueue(pair.Key, pair.Value);
                    }
                    _queue = new_queue;
                    _misses_queue.Clear();
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
            //// compare the priority with that of it's neighbours.
            //foreach (KeyValuePair<uint, CHEdgeData> edge in _target.GetArcs(vertex))
            //{ // check the priority.
            //    if (!this.IsContracted(edge.Key))
            //    {
            //        float edge_priority = this.CalculatePriority(edge.Key);
            //        if (edge_priority < priority) // TODO: <= or <
            //        { // there is a neighbour with lower priority.
            //            return false;
            //        }
            //    }
            //}
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
        public delegate void VertexDelegate(uint vertex, KeyValuePair<uint, CHEdgeData>[] edges);

        /// <summary>
        /// The before contraction delegate.
        /// </summary>
        public event VertexDelegate OnBeforeContractionEvent;

        /// <summary>
        /// Notifies an arc removal.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="edges"></param>
        private void OnBeforeContraction(uint vertex, KeyValuePair<uint, CHEdgeData>[] edges)
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
        private void OnAfterContraction(uint vertex, KeyValuePair<uint, CHEdgeData>[] edges)
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
