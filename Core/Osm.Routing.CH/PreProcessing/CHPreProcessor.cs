// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
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
using Osm.Data.Core.Sparse;
using Osm.Data.Core.Sparse.Primitives;
using Osm.Routing.CH.PreProcessing.Ordering;
using Osm.Routing.CH.PreProcessing.Witnesses;
using Osm.Routing.CH.Routing;
using Osm.Core.Simple;
using Osm.Routing.Core.Roads.Tags;
using Osm.Routing.Core;
using System.Diagnostics;
using Tools.Math.Geo;
using Osm.Data.Core.Processor.SimpleSource;
using Osm.Data.Core.DynamicGraph;

namespace Osm.Routing.CH.PreProcessing
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
        /// <param name="nodes"></param>
        public void Start()
        {
            // get all nodes from the source.
            _all_nodes = _target.GetVertices().GetEnumerator();

            // loop over the priority queue until it's empty.
            uint? vertex = this.SelectNext();
            while (vertex != null)
            {
                // contract the nodes.
                this.Contract(vertex.Value);

                // select the next vertex.
                vertex = this.SelectNext();
            }
        }

        /// <summary>
        /// Contracts the given vertex.
        /// </summary>
        /// <param name="vertex_id"></param>
        public void Contract(uint vertex)
        {
            // get all information from the source.
            KeyValuePair<uint, CHEdgeData>[] edges = _target.GetArcs(vertex);

            // remove the edges from the neighbours to the target.
            foreach (KeyValuePair<uint, CHEdgeData> edge in edges)
            { // remove the edge.
                _target.DeleteArc(edge.Key, vertex);
            }

            foreach (KeyValuePair<uint, CHEdgeData> from in edges) // loop over all from-neighbours.
            { // loop over all from neighbours.
                // skip forward edges.
                if (!from.Value.Backward) { continue; }

                foreach (KeyValuePair<uint, CHEdgeData> to in edges) // loop over all to-neighbours.
                { // loop over all to neighbours.
                    // skip backward edges.
                    if (!to.Value.Forward) { continue; }

                    // skip routes to self.
                    if (to.Key == from.Key) { continue; }

                    // add weights.
                    float weight = to.Value.Weight + from.Value.Weight;

                    // test for a witness.
                    if (!_witness_calculator.Exists(from.Key, to.Key, vertex, weight))
                    { // add edge from from to to.
                        CHEdgeData new_data = new CHEdgeData();
                        new_data.Forward = true;
                        new_data.Backward = from.Value.Forward && to.Value.Backward;
                        new_data.Weight = weight;
                        _target.AddArc(from.Key, to.Key, new_data);

                        this.NotifyArc(from.Key, to.Key); // notify a new arc.
                    }
                }
            }

            // mark the vertex as contracted.
            this.MarkContracted(vertex);

            // notify a contracted neighbour.
            _calculator.NotifyContracted(vertex);
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
        /// Enumerates all nodes.
        /// </summary>
        private IEnumerator<uint> _all_nodes;

        /// <summary>
        /// Select the next vertex from the queue.
        /// </summary>
        /// <returns></returns>
        private uint? SelectNext()
        {
            // first check the first of the current queue.
            if (_queue.Count > 0)
            { // get the first vertex and check.
                uint first_queued = _queue.Peek();

                if (this.CanBeContracted(first_queued))
                { // yet, this vertex can be contracted!
                    _queue.Remove(first_queued);
                    return first_queued;
                }
            }

            // keep going over the enumerator and check.
            while (_all_nodes.MoveNext())
            {
                // get the next vertex.
                uint next = _all_nodes.Current;

                if (this.CanBeContracted(next))
                { // yes, this vertex can be contracted!
                    _queue.Remove(next);
                    return next;
                }
            }

            // first check the first of the current queue.
            foreach (float weight in _queue.Weights)
            { // get the first vertex and check.
                foreach (uint vertex in new List<uint>(_queue.PeekAtWeight(weight)))
                { // get the first vertex and check.
                    if (this.CanBeContracted(vertex))
                    { // yet, this vertex can be contracted!
                        _queue.Remove(vertex);
                        return vertex;
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
            float priority = this.CalculatePriority(vertex);

            if (priority < float.MaxValue)
            {
                // enqueue the vertex.
                _queue.Enqueue(vertex, priority);

                // compare the priority with that of it's neighbours.
                foreach (KeyValuePair<uint, CHEdgeData> edge in _target.GetArcs(vertex))
                { // check the priority.
                    if (!this.IsContracted(edge.Key))
                    {
                        float edge_priority = this.CalculatePriority(edge.Key);
                        if (edge_priority < priority) // TODO: <= or <
                        { // there is a neighbour with lower priority.
                            return false;
                        }
                    }
                }
                return true;
            }
            return false; // priority is 'infinite'.
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


        #endregion

    }
}