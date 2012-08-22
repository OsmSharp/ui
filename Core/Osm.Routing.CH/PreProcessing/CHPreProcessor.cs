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
using Tools.Math.Geo;
using Osm.Data.Core.Processor.SimpleSource;

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
        /// <param name="calculator"></param>
        /// <param name="witness_calculator"></param>
        /// <param name="max"></param>
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
        /// <param name="calculator"></param>
        /// <param name="witness_calculator"></param>
        public CHPreProcessor(ICHData target,
            INodeWeightCalculator calculator,
            INodeWitnessCalculator witness_calculator)
        {
            _target = target;
            _current_level = 1;

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
                    if (weight < float.MaxValue)
                    {
                        _priority_queue.Enqueue(vertex.Id, weight);
                    }
                }
            }
        }

        /// <summary>
        /// Holds the current 'before recalculation value'.
        /// </summary>
        private int _k;

        /// <summary>
        /// Select the next vertex from the queue.
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
                    { // do the lazy updating.
                        // vertices are dequeued later.
                        //_priority_queue.Pop(); // dequeue the current vertex.

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
                foreach (CHVertexNeighbour n in
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

        #region Notifications

        /// <summary>
        /// The delegate for arc notifications.
        /// </summary>
        /// <param name="from_id"></param>
        /// <param name="to_id"></param>
        public delegate void ArcDelegate(long from_id, long to_id);

        /// <summary>
        /// The event.
        /// </summary>
        public event ArcDelegate NotifyArcEvent;

        /// <summary>
        /// Notifies a new arc.
        /// </summary>
        /// <param name="from_id"></param>
        /// <param name="to_id"></param>
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

        #region Updates

        /// <summary>
        /// Holds a simple source data.
        /// </summary>
        private ISimpleSourceData _simple_source_data;

        #region Nodes

        /// <summary>
        /// Processes a node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="change"></param>
        public void Process(SimpleNode node, SimpleChangeType change)
        {
            switch (change)
            {
                case SimpleChangeType.Create:
                    this.CreateNode(node);
                    break;
                case SimpleChangeType.Modify:
                    this.ModifyNode(node);
                    break;
                case SimpleChangeType.Delete:
                    this.DeleteNode(node);
                    break;
            }
        }

        /// <summary>
        /// A node was created, process the creation.
        /// </summary>
        /// <param name="node"></param>
        private void CreateNode(SimpleNode node)
        {
            _simple_source_data.AddNode(node);

            // when creating a new node:
            // create a CHVertex without any neighbours.
            // the node will be connected later using way modifications.
            CHVertex vertex = new CHVertex();
            vertex.Id = node.Id.Value;
            vertex.Latitude = node.Latitude.Value;
            vertex.Longitude = node.Longitude.Value;
            vertex.Level = int.MaxValue; // start at the highest possible level.

            _target.PersistCHVertex(vertex);
        }

        /// <summary>
        /// A node was deleted, process the deletion.
        /// </summary>
        /// <param name="node"></param>
        private void DeleteNode(SimpleNode node)
        {
            _simple_source_data.DeleteNode(node.Id.Value);

            // when deleting a node:
            // delete the actual CHVertex without taking into account 
            // it's neighbours referencing it.
            // the node will be remove from the neighbours list uwing way modifications.
            _target.DeleteCHVertex(node.Id.Value);
        }

        /// <summary>
        /// A node was modified, process the modifications.
        /// </summary>
        /// <param name="node"></param>
        private void ModifyNode(SimpleNode node)
        {
            _simple_source_data.UpdateNode(node);

            // when modifying a node:
            // update it's position.
            // other changes will be reflected when using way modifications.
            CHVertex vertex = _target.GetCHVertex(node.Id.Value);
            vertex.Latitude = node.Latitude.Value;
            vertex.Longitude = node.Longitude.Value;

            _target.PersistCHVertex(vertex);
        }

        #endregion

        #region Ways

        /// <summary>
        /// Processes a way.
        /// </summary>
        /// <param name="way"></param>
        /// <param name="change"></param>
        public void Process(SimpleWay way, SimpleChangeType change)
        {
            switch (change)
            {
                case SimpleChangeType.Create:
                    this.CreateWay(way);
                    break;
                case SimpleChangeType.Modify:
                    this.ModifyWay(way);
                    break;
                case SimpleChangeType.Delete:
                    this.DeleteWay(way);
                    break;
            }
        }

        /// <summary>
        /// A new way was create, process it.
        /// </summary>
        /// <param name="way"></param>
        private void CreateWay(SimpleWay way)
        {
            _simple_source_data.AddWay(way);

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

        /// <summary>
        /// A way was delete, process it.
        /// </summary>
        /// <param name="way"></param>
        private void DeleteWay(SimpleWay way)
        {
            _simple_source_data.DeleteWay(way.Id.Value);

            // initialize the changes list.
            UpdateList change_list = new UpdateList();

            // add the nodes to the update list.
            change_list.AddWay(way);

            // re-process the nodes in the update list.
            this.ProcessUpdates(change_list);
        }

        /// <summary>
        /// A way was modified, process it.
        /// </summary>
        /// <param name="way"></param>
        private void ModifyWay(SimpleWay way)
        {
            // get the old way.
            SimpleWay old_way = _simple_source_data.GetWay(way.Id.Value);

            // update way.
            _simple_source_data.UpdateWay(way);

            // initialize the changes list.
            UpdateList change_list = new UpdateList();

            // add the nodes to the update list.
            change_list.AddWay(way);
            change_list.AddWay(old_way);

            // re-process the nodes in the update list.
            this.ProcessUpdates(change_list);
        }

        #endregion

        /// <summary>
        /// Processes all the updated nodes.
        /// </summary>
        /// <param name="change_list"></param>
        private void ProcessUpdates(UpdateList change_list)
        {
            HashSet<long> queue = new HashSet<long>();
            HashSet<long> settled = new HashSet<long>();
            HashSet<long> unsettled = new HashSet<long>();

            // create the queue list.
            foreach (long node_id in change_list)
            { // do a search from each node until all non-settled nodes are at level float.MaxValue
                queue.Add(node_id);
                settled.Add(node_id);

                // loop until the unsettled nodes list is empty.
                do
                {
                    // get the neighbours.
                    foreach (long neighbours_id in this.GetNeighbours(node_id))
                    { // add to the unsetteled list.
                        if (!settled.Contains(neighbours_id))
                        {
                            unsettled.Add(neighbours_id);
                        }
                    }


                } while (unsettled.Count > 0);
            }

            // remove all deleted nodes.
            foreach (long long_id in change_list)
            {
                // get an existing node.
                SimpleNode node = _simple_source_data.GetNode(long_id);

                if (node == null)
                { // the node does not exist anymore: delete it.
                    _target.DeleteCHVertex(long_id);
                }
            }

            // queue the changed nodes and update.

        }

        /// <summary>
        /// Returns the neighbours for this given node.
        /// </summary>
        /// <param name="node_id"></param>
        private IEnumerable<long> GetNeighbours(long node_id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Process an updated node.
        /// </summary>
        /// <param name="long_id"></param>
        private void ProcessUpdatesForNode(long long_id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal class UpdateList : List<long>
    {
        public void AddWay(SimpleWay way)
        {
            this.AddRange(way.Nodes);
        }
    }
}
