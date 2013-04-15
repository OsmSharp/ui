//// OsmSharp - OpenStreetMap tools & library.
//// Copyright (C) 2012 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Routing.Graph.Path;

//namespace OsmSharp.Routing.CH.Routing
//{
//    /// <summary>
//    /// Internal data structure reprenting a visit list,
//    /// </summary>
//    internal class CHPriorityQueue
//    {
//        /// <summary>
//        /// Holds all visited vertices sorted by weight.
//        /// </summary>
//        private SortedList<double, HashSet<PathSegment<long>>> _visit_list;

//        /// <summary>
//        /// Holds all visited vertices.
//        /// </summary>
//        private Dictionary<long, double> _visited;

//        /// <summary>
//        /// Creates a new visit list.
//        /// </summary>
//        public CHPriorityQueue()
//        {
//            _visit_list = new SortedList<double, HashSet<PathSegment<long>>>();
//            _visited = new Dictionary<long, double>();
//        }

//        /// <summary>
//        /// Updates a vertex in this visit list.
//        /// </summary>
//        /// <param name="vertex"></param>
//        /// <param name="weight"></param>
//        public void Push(PathSegment<long> vertex)
//        {
//            long vertex_id = vertex.VertexId;
//            double weight = vertex.Weight;

//            double current_weight;
//            if (_visited.TryGetValue(vertex_id, out current_weight))
//            { // the vertex was already in this list.
//                if (current_weight <= weight)
//                { // do not add weights higher or equal to the current weight.
//                    return;
//                }
//                HashSet<PathSegment<long>> current_weight_vertices = _visit_list[current_weight];
//                current_weight_vertices.Remove(vertex);
//                if (current_weight_vertices.Count == 0)
//                {
//                    _visit_list.Remove(current_weight);
//                }
//            }

//            // add/update everthing.
//            HashSet<PathSegment<long>> vertices_at_weight;
//            if (!_visit_list.TryGetValue(weight, out vertices_at_weight))
//            {
//                vertices_at_weight = new HashSet<PathSegment<long>>();
//                _visit_list.Add(weight, vertices_at_weight);
//            }
//            vertices_at_weight.Add(vertex);
//            _visited[vertex_id] = weight;
//        }

//        /// <summary>
//        /// Returns the vertex with the lowest weight and removes it.
//        /// </summary>
//        /// <returns></returns>
//        public PathSegment<long> Pop()
//        {
//            if (_visit_list.Count > 0)
//            {
//                double weight = _visit_list.Keys[0];
//                HashSet<PathSegment<long>> first_set = _visit_list[weight];
//                PathSegment<long> vertex =
//                    first_set.First<PathSegment<long>>();

//                // remove the vertex.
//                first_set.Remove(vertex);
//                if (first_set.Count == 0)
//                {
//                    _visit_list.Remove(weight);
//                }
//                _visited.Remove(vertex.VertexId);

//                return vertex;
//            }
//            throw new InvalidOperationException("The Queue is empty!");
//        }

//        /// <summary>
//        /// Returns the vertex with the lowest weight and removes it.
//        /// </summary>
//        /// <returns></returns>
//        public PathSegment<long> Peek()
//        {
//            if (_visit_list.Count > 0)
//            {
//                double weight = _visit_list.Keys[0];
//                HashSet<PathSegment<long>> first_set = _visit_list[weight];
//                PathSegment<long> vertex =
//                    first_set.First<PathSegment<long>>();

//                return vertex;
//            }
//            throw new InvalidOperationException("The Queue is empty!");
//        }

//        /// <summary>
//        /// Returns the lowest weight.
//        /// </summary>
//        /// <returns></returns>
//        public double PeekWeight()
//        {
//            return _visit_list.Keys[0];
//        }

//        /// <summary>
//        /// Returns the element count in this list.
//        /// </summary>
//        public int Count
//        {
//            get
//            {
//                return _visited.Count;
//            }
//        }
//    }
//}
