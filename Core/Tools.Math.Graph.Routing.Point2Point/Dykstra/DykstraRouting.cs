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
using Tools.Core.Performance;
using Tools.Math.Graph.Routing.DataStructures;
using Tools.Math.Graph.Routing.Point2Point.Exceptions;

namespace Tools.Math.Graph.Routing.Dykstra
{
    /// <summary>
    /// Class implementing a version of the Dykstra route calculation algorithm working on a <see cref="Graph<EdgeType,VertexType>"/> object.
    /// </summary>
    public abstract class DykstraRouting<VertexType> : IRouter
        where VertexType : class, IEquatable<VertexType>
    {  
        /// <summary>
        /// The graph the routing is being done on.
        /// </summary>
        private IGraphDirected<VertexType> _graph;
        
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="graph"></param>
        public DykstraRouting(IGraphDirected<VertexType> graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Calculates the actual route between the given nodes.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public RouteLinked Calculate(long from, long to)
        {            
            // intialize dyskstra data structures.
            SortedVisitList visit_list = new SortedVisitList();
            HashSet<long> chosen_nodes = new HashSet<long>();

            // set the from node as the current node and put it in the correct data structures.
            // intialize the source's neighbours.
            RouteLinked current =
                new RouteLinked(from);

            // test for identical start/end point.
            if (from == to)
            {
                return current;
            }
            // start routing.
            Dictionary<long, float> neighbours = _graph.GetNeighbours(
                current.VertexId, null);
            chosen_nodes.Add(current.VertexId);

            // loop until target is found and the route is the shortest!
            while (true)
            {
                // update the visited nodes.
                foreach (KeyValuePair<long, float> neighbour in neighbours)
                {
                    // calculate neighbours weight.
                    float total_weight = current.Weight + neighbour.Value;

                    // update the visit list;
                    RouteLinked neighbour_route = new RouteLinked(neighbour.Key, total_weight, current);
                    visit_list.UpdateVertex(neighbour_route);
                }

                // while the visit list is not empty.
                if (visit_list.Count > 0)
                {
                    // choose the next vertex.
                    current = visit_list.GetFirst();
                    chosen_nodes.Add(current.VertexId);

                    // check target.
                    if (to == current.VertexId)
                    {
                        break;
                    }

                    // check stopping conditions.
                    if (this.StopCalculate(from, to, current.VertexId, chosen_nodes.Count))
                    {
                        break;
                    }

                    // get the neigbours of the current node.
                    neighbours = _graph.GetNeighbours(
                        current.VertexId, chosen_nodes);
                }
                else
                { // the visit list is emtpy and the algorithm will stop without a result.
                    if (!this.NotFoundCalculate(from, to, chosen_nodes))
                    {
                        throw new RoutingException(from, to);
                    }
                }
            }

            if (current.VertexId != to)
            { // target was not found!
                if (!this.NotFoundCalculate(from, to, chosen_nodes))
                {
                    throw new RoutingException(from, to);
                }
            }

            // return the result.
            return current;
        }

        /// <summary>
        /// Calculates the weight between the given nodes.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public float CalculateWeight(long from, long to)
        {
            RouteLinked linked_route = this.Calculate(from, to);
            if (linked_route != null)
            {
                return linked_route.Weight;
            }
            // route not found.
            throw new RoutingException(from, to);
        }

        /// <summary>
        /// Calculates all weights between one vertex and a list of others.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public float[] CalculateOneToMany(long from, long[] tos)
        {            
            // intialize dyskstra data structures.
            SortedVisitList visit_list = new SortedVisitList();
            HashSet<long> chosen_nodes = new HashSet<long>();

            // initialize the target list.
            int unique_tos_count = 0;
            int found_tos_count = 0;
            Dictionary<long, RouteLinked> to_dictionary = new Dictionary<long,RouteLinked>();
            foreach(long to in tos)
            {
                if(to != from && !to_dictionary.ContainsKey(to))
                {
                    to_dictionary.Add(to, null);
                    unique_tos_count++;
                }
            }

            // set the from node as the current node and put it in the correct data structures.
            // intialize the source's neighbours.
            RouteLinked current =
                new RouteLinked(from);
            Dictionary<long, float> neighbours = _graph.GetNeighbours(
                current.VertexId, null);
            chosen_nodes.Add(current.VertexId);

            // loop until target is found and the route is the shortest!
            while (true)
            {
                // update the visited nodes.
                foreach (KeyValuePair<long, float> neighbour in neighbours)
                {
                    // calculate neighbours weight.
                    float total_weight = current.Weight + neighbour.Value;

                    // update the visit list;
                    RouteLinked neighbour_route = new RouteLinked(neighbour.Key, total_weight, current);
                    visit_list.UpdateVertex(neighbour_route);
                }

                // while the visit list is not empty.
                if (visit_list.Count > 0)
                {
                    // choose the next vertex.
                    current = visit_list.GetFirst();

                    // check stopping conditions.
                    if (to_dictionary.ContainsKey(current.VertexId))
                    {
                        to_dictionary[current.VertexId] = current;
                        found_tos_count++;

                        if(found_tos_count == unique_tos_count)
                        {
                            break;
                        }
                    }

                    // check stopping conditions.
                    if (this.StopCalculate(from, tos, current.VertexId, chosen_nodes.Count))
                    {
                        break;
                    }

                    // get the neigbours of the current node.
                    neighbours = _graph.GetNeighbours(
                        current.VertexId, chosen_nodes);
                    chosen_nodes.Add(current.VertexId);
                }
                else
                { // the visit list is emtpy and the algorithm will stop without a result.
                    if (!this.NotFoundCalculateOneToMany(from, tos, chosen_nodes))
                    {
                        HashSet<long> tos_not_found = new HashSet<long>();
                        for (int idx = 0; idx < tos.Length; idx++)
                        {
                            long to = tos[idx];
                            RouteLinked route;
                            if (!to_dictionary.TryGetValue(to, out route)
                                || route != null)
                            {
                                //tos_not_found.Add(to);
                            }
                        }

                        //throw new RoutingException(from, tos_not_found.ToArray<long>());
                    }
                    break;
                }
            }

            // construct the result.
            float[] routes = new float[tos.Length];
            for(int idx = 0; idx < tos.Length; idx++)
            {
                long to = tos[idx];
                RouteLinked route;
                if (to_dictionary.TryGetValue(to, out route)
                    && route != null)
                {
                    routes[idx] = route.Weight;
                }
                else
                    routes[idx] = to == from ? 0 : float.MaxValue;
            }

            // return the result.
            return routes;
        }

        /// <summary>
        /// Calculates all weights between all vertices given.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="tos"></param>
        /// <returns></returns>
        public float[][] CalculateManyToMany(long[] from, long[] tos)
        {
            float[][] results = new float[from.Length][];

            //System.Threading.Tasks.Parallel.For(0, from.Length, new Action<int>((idx) =>
            //{
            //    results[idx] = this.CalculateOneToMany(from[idx], tos);
            //    System.Diagnostics.Debug.WriteLine("{0}/{1}", idx, from.Length);
            //}));

            for (int idx = 0; idx < from.Length; idx++)
            {
                results[idx] = this.CalculateOneToMany(from[idx], tos);
                //System.Diagnostics.Debug.WriteLine("{0}/{1}", idx, from.Length);
            }

            return results;
        }

        /// <summary>
        /// Returns true if the given vertex is reversed-connected to at least count other vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="connectivity_count"></param>
        /// <returns></returns>
        public bool CheckConnectivity(long from, int connectivity_count)
        {
            // intialize dyskstra data structures.
            SortedVisitList visit_list = new SortedVisitList();
            HashSet<long> chosen_nodes = new HashSet<long>();

            // set the from node as the current node and put it in the correct data structures.
            // intialize the source's neighbours.
            RouteLinked current =
                new RouteLinked(from);
            Dictionary<long, float> neighbours = _graph.GetNeighboursReversed(
                current.VertexId, null);
            chosen_nodes.Add(current.VertexId);

            // loop until target is found and the route is the shortest!
            while (true)
            {
                // update the visited nodes.
                foreach (KeyValuePair<long, float> neighbour in neighbours)
                {
                    // calculate neighbours weight.
                    float total_weight = current.Weight + neighbour.Value;

                    // update the visit list;
                    RouteLinked neighbour_route = new RouteLinked(neighbour.Key, total_weight, current);
                    visit_list.UpdateVertex(neighbour_route);
                }

                // while the visit list is not empty.
                if (visit_list.Count > 0)
                {
                    // choose the next vertex.
                    current = visit_list.GetFirst();
                    
                    // check stopping conditions.
                    if (chosen_nodes.Count >= connectivity_count)
                    {
                        break;
                    }

                    // get the neigbours of the current node.
                    neighbours = _graph.GetNeighboursReversed(
                        current.VertexId, chosen_nodes);
                    chosen_nodes.Add(current.VertexId);
                }
            }

            // return the result.
            return chosen_nodes.Count >= connectivity_count;
        }

        /// <summary>
        /// Returns true if the given vertex is reversed-connected to at least a radius of the given weight.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="max_weight"></param>
        /// <returns></returns>
        public bool CheckConnectivity(long from, float max_weight)
        {
            float reached_weight = 0;

            // intialize dyskstra data structures.
            SortedVisitList visit_list = new SortedVisitList();
            HashSet<long> chosen_nodes = new HashSet<long>();

            // set the from node as the current node and put it in the correct data structures.
            // intialize the source's neighbours.
            RouteLinked current =
                new RouteLinked(from);
            Dictionary<long, float> neighbours = _graph.GetNeighboursReversed(
                current.VertexId, null);
            chosen_nodes.Add(current.VertexId);

            // loop until target is found and the route is the shortest!
            while (true)
            {
                // update the visited nodes.
                foreach (KeyValuePair<long, float> neighbour in neighbours)
                {
                    // calculate neighbours weight.
                    float total_weight = current.Weight + neighbour.Value;

                    // update the visit list;
                    RouteLinked neighbour_route = new RouteLinked(neighbour.Key, total_weight, current);
                    visit_list.UpdateVertex(neighbour_route);
                }

                // while the visit list is not empty.
                if (visit_list.Count > 0)
                {
                    // choose the next vertex.
                    current = visit_list.GetFirst();
                    reached_weight = current.Weight;
                    while (current != null && current.Weight > max_weight)
                    {
                        current = visit_list.GetFirst();
                        if (current != null)
                        {
                            reached_weight = current.Weight;
                        }
                    }

                    if (current == null)
                    {
                        break;
                    }

                    // get the neigbours of the current node.
                    neighbours = _graph.GetNeighboursReversed(
                        current.VertexId, chosen_nodes);
                    chosen_nodes.Add(current.VertexId);
                }
                else
                {
                    break;
                }
            }

            // return the result.
            return reached_weight > max_weight;
        }

        #region Error Handling

        /// <summary>
        /// Called when some routes in the OneToMany calculation are not found.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="tos"></param>
        /// <param name="chosen_nodes"></param>
        protected virtual bool NotFoundCalculateOneToMany(long from, long[] tos, HashSet<long> chosen_nodes)
        {
            return false;
        }
        
        /// <summary>
        /// Called when the route is not found.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="chosen_nodes"></param>
        protected virtual bool NotFoundCalculate(long from, long to, HashSet<long> chosen_nodes)
        {
            return false;
        }

        #endregion

        #region Stopping Conditions
        
        /// <summary>
        /// Returns true if the algorithm should give up searching.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="current"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected virtual bool StopCalculate(long from, long to, long current, int count)
        { // in the default keep searching!
            return false;
        }

        /// <summary>
        /// Returns true if the algorithm should given up searching.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="tos"></param>
        /// <param name="current"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected virtual bool StopCalculate(long from, long[] tos, long current, int count)
        { // in the default keep searching!
            return false;
        }

        #endregion

    }
}
