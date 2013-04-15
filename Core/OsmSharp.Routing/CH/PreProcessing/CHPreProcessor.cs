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
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.CH.Routing;
using System.Diagnostics;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.DynamicGraph;
using OsmSharp.Tools.Collections;
//using System.Collections.Concurrent;

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
        private IDynamicGraph<CHEdgeData> _target;

        /// <summary>
        /// Holds the edge comparer.
        /// </summary>
        private CHEdgeDataComparer _comparer;

        /// <summary>
        /// Creates a new pre-processor.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="calculator"></param>
        /// <param name="witness_calculator"></param>
        /// <param name="max"></param>
        public CHPreProcessor(IDynamicGraph<CHEdgeData> target,
                INodeWeightCalculator calculator,
                INodeWitnessCalculator witness_calculator,
                int max)
        {
            _comparer = new CHEdgeDataComparer();

            _target = target;

            _calculator = calculator;
            _witness_calculator = witness_calculator;

            _queue = new CHPriorityQueue();
            _contracted = new bool[1000];
        }

        /// <summary>
        /// Creates a new pre-processor.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="calculator"></param>
        /// <param name="witness_calculator"></param>
        public CHPreProcessor(IDynamicGraph<CHEdgeData> target,
                INodeWeightCalculator calculator,
                INodeWitnessCalculator witness_calculator)
        {
            _comparer = new CHEdgeDataComparer();

            _target = target;

            _calculator = calculator;
            _witness_calculator = witness_calculator;

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
        private INodeWitnessCalculator _witness_calculator;

        /// <summary>
        /// Starts pre-processing all nodes
        /// </summary>
        public void Start()
        {
            _misses_queue = new Queue<bool>();
            _misses = 0;

            uint total = _target.VertexCount;
            uint current = 1;

#if !WINDOWS_PHONE
            System.Threading.Tasks.Parallel.ForEach(_target.GetVertices(), current_vertex =>
            {
                float priority = _calculator.Calculate(current_vertex);
                lock(_queue)
                {
                    _queue.Enqueue(current_vertex, priority);

                    if (current % 1000 == 0)
                    {
                        Tools.Output.OutputStreamHost.ReportProgress(current, total,
                            "CHPreProcessor", "Building CH Queue...");
                    }

                    current++;
                }
            });
#endif
#if WINDOWS_PHONE
            foreach (uint current_vertex in _target.GetVertices())
            {
                float priority = _calculator.Calculate(current_vertex);
                lock (_queue)
                {
                    _queue.Enqueue(current_vertex, priority);

                    if (current % 1000 == 0)
                    {
                        Tools.Output.OutputStreamHost.ReportProgress(current, total,
                            "CHPreProcessor", "Building CH Queue...");
                    }

                    current++;
                }  
            }
#endif
            //while (_all_nodes.MoveNext())
            //{ // keep enqueuing until the last node, or until no more nodes are left.
            //    this.ReQueue(_all_nodes.Current);

            //}

            // loop over the priority queue until it's empty.
            current = 1;
            uint? vertex = this.SelectNext();
            while (vertex != null)
            {
                // contract the nodes.
                this.Contract(vertex.Value);

                // select the next vertex.
                vertex = this.SelectNext();

                if (current % 1000 == 0)
                {
                    Tools.Output.OutputStreamHost.ReportProgress(current, total, "CHPreProcessor", "Pre-processing...");
                }
                current++;
            }

            // Remark: this is possible with SparseOrdering (not all vertices will be contracted!)
            //if (current != _target.VertexCount + 1)
            //{ // not all vertices have been contracted.
            //    throw new Exception(string.Format("Not all vertices have been contracted: only {0}/{1}!",
            //        current, _target.VertexCount));
            //}
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
            HashSet<uint> neighbours = new HashSet<uint>();

            // get all information from the source.
            KeyValuePair<uint, CHEdgeData>[] edges = _target.GetArcs(vertex);

            // report the before contraction event.
            this.OnBeforeContraction(vertex, edges);

            // remove the edges from the neighbours to the target.
            foreach (KeyValuePair<uint, CHEdgeData> edge in edges)
            { // remove the edge.
                _target.DeleteArc(edge.Key, vertex);

                // keep the neighbour.
                neighbours.Add(edge.Key);
            }

            // loop over each combination of edges just once.
            for (int x = 1; x < edges.Length; x++)
            { // loop over all elements first.
                KeyValuePair<uint, CHEdgeData> x_edge = edges[x];

                //System.Threading.Tasks.Parallel.For(0, x, y =>
                for (int y = 0; y < x; y++)
                { // loop over all elements.
                    KeyValuePair<uint, CHEdgeData> y_edge = edges[y];

                    // calculate the total weight.
                    float weight = x_edge.Value.Weight + y_edge.Value.Weight;

                    // add the combinations of these edges.
                    if ((x_edge.Value.Backward && y_edge.Value.Forward) ||
                        (y_edge.Value.Backward && x_edge.Value.Forward))
                    { // there is a connection from x to y and there is no witness path.

                        bool witness_x_to_y = _witness_calculator.Exists(x_edge.Key, y_edge.Key, vertex, weight, 100);
                        bool witness_y_to_x = _witness_calculator.Exists(y_edge.Key, x_edge.Key, vertex, weight, 100);

                        // create x-to-y data and edge.
                        CHEdgeData data_x_to_y = new CHEdgeData();
                        data_x_to_y.Forward = (x_edge.Value.Backward && y_edge.Value.Forward) &&
                            !witness_x_to_y;
                        data_x_to_y.Backward = (y_edge.Value.Backward && x_edge.Value.Forward) &&
                            !witness_y_to_x;
                        data_x_to_y.Weight = weight;
                        data_x_to_y.ContractedVertexId = vertex;
                        if ((data_x_to_y.Forward || data_x_to_y.Backward) ||
                            !_target.HasNeighbour(x_edge.Key, y_edge.Key))
                        { // add the edge if there is usefull info or if there needs to be a neighbour relationship.
                            _target.AddArc(x_edge.Key, y_edge.Key, data_x_to_y, _comparer);
                        }

                        // create y-to-x data and edge.
                        CHEdgeData data_y_to_x = new CHEdgeData();
                        data_y_to_x.Forward = (y_edge.Value.Backward && x_edge.Value.Forward) &&
                            !witness_y_to_x;
                        data_y_to_x.Backward = (x_edge.Value.Backward && y_edge.Value.Forward) &&
                            !witness_x_to_y;
                        data_y_to_x.Weight = weight;
                        data_y_to_x.ContractedVertexId = vertex;
                        if ((data_y_to_x.Forward || data_y_to_x.Backward) ||
                            !_target.HasNeighbour(y_edge.Key, x_edge.Key))
                        { // add the edge if there is usefull info or if there needs to be a neighbour relationship.
                            _target.AddArc(y_edge.Key, x_edge.Key, data_y_to_x, _comparer);
                        }
                    }
                }
                //});
            }

            //// re-enqueue all the neigbours.
            //foreach (uint neighbour in neighbours)
            //{
            //    if (_queue.Remove(neighbour))
            //    {
            //        this.ReQueue(neighbour);
            //    }
            //    if (_contracted.Length > neighbour && _contracted[neighbour])
            //    { // vertex was neighbour but already contracted (= impossible)
            //        throw new Exception(string.Format("Vertex {0} has contracted neighbour: {1}!",
            //            vertex, neighbour));
            //    }
            //}

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
                Debug.WriteLine("Contracted {0}", vertex);
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
#if !WINDOWS_PHONE
                    System.Collections.Concurrent.ConcurrentBag<KeyValuePair<uint, float>> recalculated_weights = 
                        new System.Collections.Concurrent.ConcurrentBag<KeyValuePair<uint, float>>();
                    System.Threading.Tasks.Parallel.ForEach(_queue, vertex =>
                    {
                        recalculated_weights.Add(
                            new KeyValuePair<uint,float>(vertex, _calculator.Calculate(vertex)));
                    });
#endif
#if WINDOWS_PHONE
                    HashSet<KeyValuePair<uint, float>> recalculated_weights =
                        new HashSet<KeyValuePair<uint, float>>();
                    foreach (uint vertex in _queue)
                    {
                        recalculated_weights.Add(
                            new KeyValuePair<uint, float>(vertex, _calculator.Calculate(vertex)));
                    }
#endif
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

            //// keep going over the enumerator and check.
            //while (_all_nodes.MoveNext())
            //{
            //    // get the next vertex.
            //    uint next = _all_nodes.Current;

            //    if (this.CanBeContracted(next))
            //    { // yes, this vertex can be contracted!
            //        _queue.Remove(next);
            //        return next;
            //    }
            //}

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
                return _witness_calculator;
            }
        }
        #endregion

    }
}
