//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.Graph.Helpers;
//using Tools.Core.Performance;
//using Tools.Math.Graph.Routing.Dykstra;

//namespace Tools.Math.Graph.Routing.AStar
//{
//    /// <summary>
//    /// Class implementing a version of the Dykstra route calculation algorithm working on a <see cref="Graph<EdgeType,VertexType>"/> object.
//    /// </summary>
//    /// <typeparam name="EdgeType"></typeparam>
//    /// <typeparam name="RouteType"></typeparam>
//    internal class AStarRouting<EdgeType, VertexType> : IPoint2PointRouter<EdgeType, VertexType>
//        where EdgeType : class
//        where VertexType : class, IEquatable<VertexType>
//    {
//        /// <summary>
//        /// The graph the routing is being done on.
//        /// </summary>
//        private Graph<EdgeType, VertexType> _graph;

//        /// <summary>
//        /// The controller to controle the progress of the algorithm.
//        /// </summary>
//        private IPoint2PointRouterDykstraController<EdgeType, VertexType> _controller;

//        /// <summary>
//        /// The maximum to count.
//        /// </summary>
//        private int _max_to_count;

//        /// <summary>
//        /// Creates a new instance of this class.
//        /// </summary>
//        /// <param name="graph"></param>
//        public AStarRouting(Graph<EdgeType, VertexType> graph)
//        {
//            _graph = graph;
//            _max_to_count = -1;

//            _controller = new DummyDykstraController<EdgeType, VertexType>();
//        }

//        /// <summary>
//        /// Creates a new instance of this class.
//        /// </summary>
//        /// <param name="graph"></param>
//        public AStarRouting(Graph<EdgeType, VertexType> graph,
//            int max_to_count)
//        {
//            _graph = graph;
//            _max_to_count = max_to_count;

//            _controller = new DummyDykstraController<EdgeType, VertexType>();
//        }

//        /// <summary>
//        /// Calculates a route from a route pointo the to route point.
//        /// </summary>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        public GraphRoute<EdgeType, VertexType> Calculate(
//            VertexType from,
//            VertexType to,
//            float max_range)
//        {
//            // dykstra should be used mainly for calculating routes from one to multiple nodes.
//            // this is just to ensure compatibility.
//            Dictionary<VertexType, GraphRoute<EdgeType, VertexType>> node_route =
//                this.Calculate(
//                    from,
//                    new List<VertexType> { to },
//                    max_range);
//            if (node_route.ContainsKey(to))
//            {
//                return node_route[to];
//            }
//            return null;
//        }

//        public GraphRoute<EdgeType, VertexType> Calculate(List<VertexType> in_order,
//            float max_range)
//        {
//            VertexType from = in_order[0];

//            List<GraphRouteEntry<EdgeType, VertexType>> entries = new List<GraphRouteEntry<EdgeType, VertexType>>();

//            float weight = 0;
//            for (int idx = 1; idx < in_order.Count; idx++)
//            {
//                // get start-end.
//                VertexType from_in_entry = in_order[idx - 1];
//                VertexType to_in_entry = in_order[idx];

//                // calculate/get route.
//                GraphRoute<EdgeType, VertexType> route = this.Calculate(from_in_entry, to_in_entry, max_range);
//                weight = route.Weight + weight;
//                entries.AddRange(route.Entries);
//            }

//            return new GraphRoute<EdgeType, VertexType>(
//                new GraphRouteEntryPoint<VertexType>(from, true), entries, weight);
//        }

//        /// <summary>
//        /// Calculates routes from the given point to all points in the target.
//        /// </summary>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        public Dictionary<VertexType, GraphRoute<EdgeType, VertexType>> Calculate(
//            VertexType from,
//            List<VertexType> to,
//            float max_range)
//        {
//            // start counting performance on this.
//            PerformanceCounter.RegisterCounter(1, "Calculate(VertexType from,List<VertexType> to)");
//            PerformanceCounter.Run(1);

//            // notify the contoller.
//            //_controller.NotifyRoutingAlgorithm(RoutingAlgorithmsEnum.Dykstra);
//            List<VertexType> vertices = new List<VertexType>();
//            vertices.AddRange(to);
//            vertices.Add(from);
//            _controller.NotifyGraph(_graph);
//            _controller.NotifyVerticesToRoute(vertices);

//            // initialize the data structures to hold the result.
//            Dictionary<VertexType, GraphRoute<EdgeType, VertexType>> route_node = new Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>();

//            // fill in the routes to the node that are the same.
//            if (route_node.Count < to.Count)
//            {
//                foreach (VertexType to_node in to)
//                {
//                    if (!route_node.ContainsKey(to_node))
//                    {
//                        if (to_node == from)
//                        {
//                            route_node.Add(to_node,
//                                new GraphRoute<EdgeType, VertexType>
//                                    (new GraphRouteEntryPoint<VertexType>(from, true),
//                                        new List<GraphRouteEntry<EdgeType, VertexType>>(),
//                                        0)
//                                    );
//                        }
//                    }
//                }
//            }

//            // intialize dyskstra data structures.
//            DykstraVisitList<EdgeType, VertexType> visit_list = new DykstraVisitList<EdgeType, VertexType>();
//            HashSet<VertexType> chosen_nodes = new HashSet<VertexType>();

//            // set the from node as the current node and put it in the correct data structures.
//            // intialize the source's neighbours.
//            VertexReference<EdgeType, VertexType> current =
//                new VertexReference<EdgeType, VertexType>(default(float), null, from, null);
//            HashSet<VertexAlongEdge<EdgeType, VertexType>> neighbours = _graph.GetNeighbours(
//                current.Vertex,
//                chosen_nodes);
//            chosen_nodes.Add(current.Vertex);

//            // loop until target is found and the route is the shortest!
//            while (true)
//            {
//                // update the visited nodes.
//                foreach (VertexAlongEdge<EdgeType, VertexType> node_way in neighbours)
//                {
//                    VertexReference<EdgeType, VertexType> ref_val;
//                    if (visit_list.TryGetRef(node_way.Vertex, out ref_val))
//                    { // a reference for the node was found.
//                        if (ref_val.Weight.CompareTo(node_way.Weight + current.Weight) > 0)
//                        { // the route to the node found was bigger than the route found here.
//                            // replace the original route.
//                            ref_val = new VertexReference<EdgeType, VertexType>(
//                                node_way.Weight + current.Weight,
//                                current,
//                                node_way.Vertex,
//                                node_way.Edge);
//                        }
//                    }
//                    else
//                    { // no reference for the node was found.
//                        float weight = node_way.Weight + current.Weight + this.EstimateDistanceToGoal(node_way.Vertex, to);
//                        ref_val = new VertexReference<EdgeType, VertexType>(weight,
//                            current,
//                            node_way.Vertex,
//                            node_way.Edge);
//                        visit_list.AddNode(weight,
//                            ref_val);
//                    }
//                }

//                while (visit_list.Count > 0)
//                {
//                    // get the first node.
//                    VertexReference<EdgeType, VertexType> first =
//                        visit_list.GetFirst();

//                    // notify the controller of the first vertex reference.
//                    if (!_controller.NotifyFirstVertex(first, visit_list))
//                    {
//                        break;
//                    }

//                    // notify for a new vertex.
//                    _controller.NotifyVertexSelected(first);

//                    // add a to result if the target is the first node.
//                    if (to.Contains(first.Vertex)
//                        && !route_node.ContainsKey(first.Vertex))
//                    {
//                        // a target was found!
//                        GraphRoute<EdgeType, VertexType> new_route = first.CreateRouteTo();
//                        route_node.Add(first.Vertex, new_route);

//                        if (route_node.Count == to.Count)
//                        {
//                            break;
//                        }
//                        if (_max_to_count > 0 && to.Count >= _max_to_count)
//                        {
//                            break;
//                        }

//                        // notify the controller of a found target.
//                        _controller.NotifyFound(new_route);
//                    }

//                    // check if it has neighbours.
//                    neighbours = _graph.GetNeighbours(
//                        first.From.Vertex,
//                        first.Edge,
//                        first.Vertex,
//                        chosen_nodes);
//                    visit_list.RemoveNode(first.Weight, first);
//                    if (neighbours.Count > 0)
//                    {
//                        current = first;

//                        // add the current node to the chosen nodes.
//                        chosen_nodes.Add(current.Vertex);

//                        break;
//                    }
//                }

//                // stop the search if no new node with neighbours was found or the routes found match the to-count.
//                if (current == null
//                    || to.Count == route_node.Count
//                    || (visit_list.Count == 0 && neighbours.Count == 0)
//                    || _controller.QueryStopConditions()
//                    || (_max_to_count > 0 && route_node.Count >= _max_to_count))
//                {
//                    // if the maxcount was set then
//                    if (_max_to_count > 0 && current != null && !_controller.QueryStopConditions())
//                    {
//                        // fill in the blanks.
//                        if (route_node.Count < to.Count)
//                        {
//                            foreach (VertexType to_node in to)
//                            {
//                                if (!route_node.ContainsKey(to_node))
//                                {
//                                    // add a route that is infinite
//                                    route_node.Add(to_node,
//                                        new GraphRoute<EdgeType, VertexType>
//                                            (new GraphRouteEntryPoint<VertexType>(from, true),
//                                            new List<GraphRouteEntry<EdgeType, VertexType>>(),
//                                            float.MaxValue));
//                                }
//                            }
//                        }
//                    }
//                    break;
//                }

//                PerformanceCounter.Pause(1);
//            }

//            //// check if any routes are found!
//            //if (route_node.Count == 0)
//            //{
//            //    throw new GraphRoutingException("No route found!");
//            //}

//            // fill in the blanks.
//            List<VertexType> not_found = new List<VertexType>();
//            if (route_node.Count < to.Count)
//            {
//                foreach (VertexType to_node in to)
//                {
//                    if (!route_node.ContainsKey(to_node))
//                    {
//                        if (to_node == from)
//                        {
//                            route_node.Add(to_node,
//                                new GraphRoute<EdgeType, VertexType>
//                                    (new GraphRouteEntryPoint<VertexType>(from, false),
//                                    new List<GraphRouteEntry<EdgeType, VertexType>>(),
//                                    0));
//                        }
//                        else
//                        {
//                            not_found.Add(to_node);
//                        }
//                    }
//                }
//            }

//            if (not_found.Count > 0)
//            {
//                Tools.Core.Output.OutputTextStreamHost.WriteLine("{0} routes not found!", not_found.Count);
//            }

//            return route_node;
//        }

//        /// <summary>
//        /// Checks the connectivity of a given vertex.
//        /// </summary>
//        /// <param name="from"></param>
//        /// <param name="connectivity_count"></param>
//        /// <returns></returns>
//        public bool CheckConnectivity(
//            VertexType from,
//            int connectivity_count)
//        {
//            // intialize dyskstra data structures.
//            DykstraVisitList<EdgeType, VertexType> visit_list = new DykstraVisitList<EdgeType, VertexType>();
//            HashSet<VertexType> chosen_nodes = new HashSet<VertexType>();

//            // set the from node as the current node and put it in the correct data structures.
//            // intialize the source's neighbours.
//            VertexReference<EdgeType, VertexType> current =
//                new VertexReference<EdgeType, VertexType>(default(float), null, from, null);
//            HashSet<VertexAlongEdge<EdgeType, VertexType>> neighbours = _graph.GetNeighboursReversed(
//                current.Vertex,
//                chosen_nodes);
//            chosen_nodes.Add(current.Vertex);

//            // loop until target is found and the route is the shortest!
//            while (true)
//            {
//                // update the visited nodes.
//                foreach (VertexAlongEdge<EdgeType, VertexType> node_way in neighbours)
//                {
//                    VertexReference<EdgeType, VertexType> ref_val;
//                    if (visit_list.TryGetRef(node_way.Vertex, out ref_val))
//                    { // a reference for the node was found.
//                        if (ref_val.Weight.CompareTo(node_way.Weight + current.Weight) > 0)
//                        { // the route to the node found was bigger than the route found here.
//                            // replace the original route.
//                            ref_val = new VertexReference<EdgeType, VertexType>(
//                                node_way.Weight + current.Weight,
//                                current,
//                                node_way.Vertex,
//                                node_way.Edge);
//                        }
//                    }
//                    else
//                    { // no reference for the node was found.
//                        float weight = node_way.Weight + current.Weight;
//                        ref_val = new VertexReference<EdgeType, VertexType>(weight,
//                            current,
//                            node_way.Vertex,
//                            node_way.Edge);
//                        visit_list.AddNode(weight,
//                            ref_val);
//                    }
//                }

//                while (visit_list.Count > 0)
//                {
//                    // get the first node.
//                    VertexReference<EdgeType, VertexType> first =
//                        visit_list.GetFirst();

//                    // check if it has neighbours.
//                    neighbours = _graph.GetNeighboursReversed(
//                        first.From.Vertex,
//                        first.Edge,
//                        first.Vertex,
//                        chosen_nodes);
//                    visit_list.RemoveNode(first.Weight, first);
//                    if (neighbours.Count > 0)
//                    {
//                        current = first;

//                        // add the current node to the chosen nodes.
//                        chosen_nodes.Add(current.Vertex);

//                        // check the chosen nodes list.
//                        if (connectivity_count <=
//                            neighbours.Count + chosen_nodes.Count)
//                        {
//                            // more or an equal numbers of nodes than connectivity_count can
//                            // lead to this route.
//                            return true;
//                        }

//                        break;
//                    }
//                }

//                // stop the search if no new node with neighbours was found or the routes found match the to-count.
//                if (visit_list.Count == 0
//                    && neighbours.Count == 0)
//                { // the connectivity test failed!
//                    break;
//                }
//            }

//            // return a negative result.
//            return false;
//        }

//        private float EstimateDistanceToGoal(VertexType from, List<VertexType> tos)
//        {
//            float min = float.MaxValue;

//            foreach (VertexType to in tos)
//            {
//                float weight = _graph.UnderestimateWeight(from, to);
//                if (weight < min)
//                {
//                    min = weight;
//                }
//            }

//            if (min == float.MaxValue)
//            {
//                return 0;
//            }
//            return min;
//        }

//        public SortedDictionary<float, Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>> CalculateClosest(VertexType from, List<VertexType> to)
//        {
//            SortedDictionary<float, Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>> sorted_routes =
//                new SortedDictionary<float, Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>>();

//            foreach (VertexType to_node in to)
//            {
//                GraphRoute<EdgeType, VertexType> route = this.Calculate(
//                    from, to_node, -1);

//                Dictionary<VertexType, GraphRoute<EdgeType, VertexType>> route_dic;
//                if (!sorted_routes.TryGetValue(route.Weight, out route_dic))
//                {
//                    route_dic = new Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>();
//                }
//                if (!route_dic.ContainsKey(to_node))
//                {
//                    route_dic.Add(to_node,
//                        route);
//                }
//            }

//            return sorted_routes;
//        }

//        //public float Calculate(List<VertexType> vertices)
//        //{
//        //    float distance = 0;
//        //    for (int idx = 0; idx < vertices.Count - 1; idx++)
//        //    {
//        //        distance =
//        //            this.CalculateRoute(vertices[idx], vertices[idx + 1]).
//        //            Weight + distance;
//        //    }
//        //    return distance;
//        //}

//        public float Distance(VertexType from, VertexType to)
//        {
//            throw new NotSupportedException();
//        }


//        #region IPoint2PointRouter<EdgeType,VertexType> Members

//        public void RegisterController(IPoint2PointRouterController<EdgeType, VertexType> controller)
//        {
//            if (controller == null)
//            {
//                throw new ArgumentNullException();
//            }
//            if (controller is IPoint2PointRouterDykstraController<EdgeType, VertexType>)
//            {
//                _controller = controller as IPoint2PointRouterDykstraController<EdgeType, VertexType>;
//            }
//            else
//            {
//                _controller = new ControllerWrapper<EdgeType, VertexType>(controller);
//            }
//        }

//        public void RegisterDykstraController(IPoint2PointRouterDykstraController<EdgeType, VertexType> controller)
//        {
//            if (controller == null)
//            {
//                throw new ArgumentNullException();
//            }
//            _controller = controller;
//        }

//        #endregion

//        #region IPoint2PointRouter<EdgeType,VertexType> Members


//        public HashSet<VertexType> CalculateRange(VertexType from, float weight)
//        {
//            throw new NotSupportedException();
//        }

//        #endregion
//    }
//}
