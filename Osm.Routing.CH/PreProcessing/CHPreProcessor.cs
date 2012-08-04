using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.Sparse;
using Osm.Data.Core.CH;
using Osm.Data.Core.Sparse.Primitives;
using Osm.Routing.CH.PreProcessing.Ordering;
using Osm.Routing.CH.PreProcessing.Witnesses;
using Osm.Routing.CH.Routing;
using Osm.Data.Core.CH.Primitives;
using Osm.Core.Simple;
using Osm.Routing.Core.Roads.Tags;
using Osm.Routing.Core;
using System.Diagnostics;

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
        private ICHData _target;

        /// <summary>
        /// Holds the max of the recalculate queue.
        /// </summary>
        private int _max;

        /// <summary>
        /// Creates a new pre-processor.
        /// </summary>
        /// <param name="target"></param>
        public CHPreProcessor(ICHData target, 
            INodeWeightCalculator calculator,
            INodeWitnessCalculator witness_calculator,
            int max)
        {
            _target = target;
            _current_level = 1;

            //_calculator = new EdgeDifference(
            //    new DykstraWitnessCalculator(_target));
            _calculator = calculator;
            _witness_calculator = witness_calculator;
            _priority_queue = new CHPriorityQueue();

            _k = 0;
            _max = max;
        }

        /// <summary>
        /// Creates a new pre-processor.
        /// </summary>
        /// <param name="target"></param>
        public CHPreProcessor(ICHData target,
            INodeWeightCalculator calculator,
            INodeWitnessCalculator witness_calculator)
        {
            _target = target;
            _current_level = 1;

            //_calculator = new EdgeDifference(
            //    new DykstraWitnessCalculator(_target));
            _calculator = calculator;
            _witness_calculator = witness_calculator;
            _priority_queue = new CHPriorityQueue();

            _k = 0;
            _max = int.MaxValue;
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
        /// Holds the current level.
        /// </summary>
        private int _current_level;

        /// <summary>
        /// Returns the current level.
        /// </summary>
        public int Level
        {
            get
            {
                return _current_level;
            }
        }

        /// <summary>
        /// Starts pre-processing the nodes in the enumerator.
        /// </summary>
        /// <param name="nodes"></param>
        public void Start(IEnumerator<long> nodes)
        {
            // first enqueue some vertices.
            this.Enqueue(nodes);

            // loop over the priority queue until it's empty.
            long current_id = this.SelectNext();
            while (current_id > 0)
            {
                // dequeue.
                _priority_queue.Remove(current_id);

                // contract the nodes.
                //Console.Write(".");
                //Console.Write("Contracting [{0}]:{1}", _priority_queue.Count, current_id);
                this.Contract(current_id);
                //Console.WriteLine(" Done!");

                // select the next vertex.
                current_id = this.SelectNext();
            }
        }

        /// <summary>
        /// Enqueue some of the nodes.
        /// </summary>
        /// <param name="nodes"></param>
        public void Enqueue(IEnumerator<long> nodes)
        {
            // first enqueue some vertices.
            while (nodes.MoveNext())
            {
                long vertex_id = nodes.Current;

                CHVertex vertex = _target.GetCHVertex(vertex_id);
                if (vertex != null && 
                    vertex.Level == int.MaxValue)
                {
                    double weight = _calculator.Calculate(_current_level, vertex);

                    _priority_queue.Enqueue(vertex.Id, weight);
                }
            }
        }

        /// <summary>
        /// Holds the current 'before recalculation value'.
        /// </summary>
        private int _k;

        /// <summary>
        /// Selecte the next vertex from the queue.
        /// </summary>
        /// <returns></returns>
        public long SelectNext()
        {
            // loop over the priority queue until it's empty.
            while (_priority_queue.Count > 0)
            {
                // get the vertex and used lazy updating.
                long current_id = _priority_queue.Peek();
                double queued_weight = _priority_queue.Weight(current_id);

                // calculate weight.
                CHVertex vertex = _target.GetCHVertex(current_id);
                if (vertex.Level == int.MaxValue)
                {
                    double new_weight = this.CalculateWeight(_current_level, vertex);

                    if (new_weight <= queued_weight)
                    { // 
                        //_priority_queue.Pop();
                        return current_id;
                    }
                    else
                    { // enqueue again with new weight.
                        _k++;

                        _priority_queue.Enqueue(current_id, new_weight);

                        if (_k > _max)
                        { // calculate the entire queue.
                            List<long> redo_list = new List<long>(_priority_queue);
                            foreach (long redo in redo_list)
                            {
                                queued_weight = _priority_queue.Weight(redo);
                                CHVertex redo_vertex = _target.GetCHVertex(redo);
                                new_weight = this.CalculateWeight(_current_level, redo_vertex);
                                _priority_queue.Enqueue(redo, new_weight);
                            }
                            _k = 0;
                        }
                    }
                }
                else
                { // dequeue the vertex
                    _priority_queue.Pop();
                }
            }
            return -1;
        }

        /// <summary>
        /// Contracts the given vertex.
        /// </summary>
        /// <param name="vertex_id"></param>
        public void Contract(long vertex_id)
        {
            CHLocalCache local_cache = new CHLocalCache();

            // adjust the level of the vertex.
            CHVertex vertex = _target.GetCHVertex(vertex_id);
            vertex.Level = _current_level; // set the level.
            _target.PersistCHVertex(vertex);

            // increas(e the level.
            _current_level++;

            // loop over all from-neighbours.
            foreach (CHVertexNeighbour from in vertex.BackwardNeighbours)
            { // loop over all from neighbours.
                // get the from vertex.
                CHVertex from_vertex = local_cache.Get(_target, from.Id);

                // remove the neighbours.
                foreach(CHVertexNeighbour n in 
                    from_vertex.ForwardNeighbours.Where<CHVertexNeighbour>(n => n.Id == vertex_id))
                {
                    this.NotifyRemove(from_vertex.Id, n.Id);
                }
                from_vertex.ForwardNeighbours.RemoveAll(n => n.Id == vertex_id);

                foreach (CHVertexNeighbour to in vertex.ForwardNeighbours)
                { // loop over all to neighbours.
                    // notify contracted neighbour.
                    // get the from vertex.
                    CHVertex to_vertex = local_cache.Get(_target, to.Id);
                    foreach (CHVertexNeighbour n in
                        to_vertex.BackwardNeighbours.Where<CHVertexNeighbour>(n => n.Id == vertex_id))
                    {
                        this.NotifyRemove(from_vertex.Id, n.Id);
                    }
                    to_vertex.BackwardNeighbours.RemoveAll(n => n.Id == vertex_id);

                    if (from.Id != to.Id)
                    { // the nodes are different.
                        double weight = to.Weight + from.Weight;

                        // test for a witness.
                        if (!_witness_calculator.Exists(_current_level, from.Id,
                            to.Id, vertex_id, weight))
                        {
                            CHVertexNeighbour forward_neighbour = new CHVertexNeighbour();
                            forward_neighbour.ContractedVertexId = vertex_id;
                            forward_neighbour.Id = to.Id;
                            forward_neighbour.Weight = to.Weight + from.Weight;
                            from_vertex.ForwardNeighbours.Add(forward_neighbour);
                            this.NotifyArc(from.Id, to.Id);

                            CHVertexNeighbour backward_neighbour = new CHVertexNeighbour();
                            backward_neighbour.ContractedVertexId = vertex_id;
                            backward_neighbour.Id = from.Id;
                            backward_neighbour.Weight = to.Weight + from.Weight;
                            to_vertex.BackwardNeighbours.Add(backward_neighbour);
                            this.NotifyArc(to.Id, from.Id);
                        }
                    }

                }
            }

            // notify a contracted neighbour.
            _calculator.NotifyContracted(vertex);

            // notify neighbours
            foreach (CHVertex neighbour in local_cache.Vertices)
            {
                // persist the result(s).
                _target.PersistCHVertex(neighbour);

                // re-calculate the neighbours.
                //this.QueueNode(neighbour);
            }
        }

        /// <summary>
        /// Calculates the weight of the given vertex.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private double CalculateWeight(int level, CHVertex vertex)
        {
            return _calculator.Calculate(level, vertex);
        }

        #endregion

        #region Queue

        /// <summary>
        /// Holds the priority queue.
        /// </summary>
        private CHPriorityQueue _priority_queue;

        /// <summary>
        /// Returns the current priority queue.
        /// </summary>
        public CHPriorityQueue Queue
        {
            get
            {
                return _priority_queue;
            }
        }

        ///// <summary>
        ///// Queue the given vertex.
        ///// </summary>
        ///// <param name="vertex_id"></param>
        //private void QueueNode(long vertex_id)
        //{
        //    this.QueueNode(_target.GetCHVertex(vertex_id));
        //}
        
        ///// <summary>
        ///// Queue the given vertex.
        ///// </summary>
        ///// <param name="vertex"></param>
        //private void QueueNode(CHVertex vertex)
        //{
        //    if (vertex.Level == int.MaxValue)
        //    {
        //        double weight = _calculator.Calculate(_current_level, vertex);

        //        _priority_queue.Enqueue(vertex.Id, weight);
        //    }
        //}

        #endregion

        #region Osm Primitives Processings

        /// <summary>
        /// Processes a new node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="change_type"></param>
        internal void Process(SimpleNode node, SimpleChangeType change_type)
        {
            if (change_type != SimpleChangeType.Create)
            {
                throw new ArgumentOutOfRangeException(string.Format("Contraction Hierarchy processor does not support changes of type: {0}", change_type));
            }

            CHVertex vertex = new CHVertex();
            vertex.Id = node.Id.Value;
            vertex.Latitude = node.Latitude.Value;
            vertex.Longitude = node.Longitude.Value;
            vertex.Level = int.MaxValue;

            _target.PersistCHVertex(vertex);
        }

        /// <summary>
        /// Processes a new way.
        /// </summary>
        /// <param name="way"></param>
        /// <param name="change_type"></param>
        internal void Process(SimpleWay way, SimpleChangeType change_type)
        {
            if (change_type != SimpleChangeType.Create)
            {
                throw new ArgumentOutOfRangeException(string.Format("Contraction Hierarchy processor does not support changes of type: {0}", change_type));
            }
            
            RoadTagsInterpreterBase interpreter = new RoadTagsInterpreterBase(way.Tags);
            if (way.Nodes.Count > 1 && interpreter.IsRoad())
            {
                bool forward = !(interpreter.IsOneWay() || interpreter.IsOneWayReverse())
                    || interpreter.IsOneWay();
                bool backward = !(interpreter.IsOneWay() || interpreter.IsOneWayReverse())
                    || interpreter.IsOneWay();

                if (forward)
                {
                    CHVertex previous = _target.GetCHVertex(way.Nodes[0]);
                    for (int idx = 1; idx < way.Nodes.Count; idx++)
                    {
                        long current_id = way.Nodes[idx];

                        CHVertex current = _target.GetCHVertex(current_id);
                        if (current != null && previous != null)
                        {
                            float weight = (float)interpreter.Time(VehicleEnum.Car, previous.Location, current.Location);

                            // persist the arc.
                            CHVertexNeighbour arc = new CHVertexNeighbour();
                            arc.ContractedVertexId = -1;
                            //arc.VertexFromId = previous.Id;
                            arc.Id = current.Id;
                            arc.Weight = weight;
                            arc.Tags = way.Tags;
                            previous.ForwardNeighbours.Add(arc);
                            this.NotifyArc(previous.Id, arc.Id);

                            // persist the arc.
                            arc = new CHVertexNeighbour();
                            arc.ContractedVertexId = -1;
                            //arc.VertexFromId = previous.Id;
                            arc.Id = previous.Id;
                            arc.Weight = weight;
                            arc.Tags = way.Tags;
                            current.BackwardNeighbours.Add(arc);
                            this.NotifyArc(current.Id, arc.Id);
                        }
                        previous = current;
                    }
                }

                if (backward)
                {
                    CHVertex previous = _target.GetCHVertex(way.Nodes[way.Nodes.Count - 1]);
                    for (int idx = way.Nodes.Count - 2; idx >= 0; idx--)
                    {
                        long current_id = way.Nodes[idx];

                        CHVertex current = _target.GetCHVertex(current_id);
                        if (current != null && previous != null)
                        {
                            float weight = (float)interpreter.Time(VehicleEnum.Car, previous.Location, current.Location);

                            // persist the arc.
                            CHVertexNeighbour arc = new CHVertexNeighbour();
                            arc.ContractedVertexId = -1;
                            //arc.VertexFromId = previous.Id;
                            arc.Id = current.Id;
                            arc.Weight = weight;
                            arc.Tags = way.Tags;
                            previous.ForwardNeighbours.Add(arc);
                            this.NotifyArc(previous.Id, arc.Id);

                            // persist the arc.
                            arc = new CHVertexNeighbour();
                            arc.ContractedVertexId = -1;
                            //arc.VertexFromId = previous.Id;
                            arc.Id = previous.Id;
                            arc.Weight = weight;
                            arc.Tags = way.Tags;
                            current.BackwardNeighbours.Add(arc);
                            this.NotifyArc(current.Id, arc.Id);
                        }

                        previous = current;
                    }
                }
            }
        }

        #endregion

        #region Notifications

        /// <summary>
        /// The delegate for arc notifications.
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="contracted_id"></param>
        public delegate void ArcDelegate(long from_id, long to_id);

        /// <summary>
        /// The event.
        /// </summary>
        public event ArcDelegate NotifyArcEvent;

        /// <summary>
        /// Notifies a new arc.
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="contracted_id"></param>
        private void NotifyArc(long from_id, long to_id)
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
        private void NotifyRemove(long from_id, long to_id)
        {
            if (this.NotifyRemoveEvent != null)
            {
                this.NotifyRemoveEvent(from_id, to_id);
            }
        }


        #endregion
    }
}
