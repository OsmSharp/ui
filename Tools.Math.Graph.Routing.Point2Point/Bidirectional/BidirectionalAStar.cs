//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.Graph.Routing.Dykstra;
//using Tools.Math.Graph.Helpers;

//namespace Tools.Math.Graph.Routing.Bidirectional
//{
//    /// <summary>
//    /// Class implementing a version of the Dykstra route calculation algorithm working on a <see cref="Graph<EdgeType,VertexType>"/> object.
//    /// </summary>
//    /// <typeparam name="EdgeType"></typeparam>
//    /// <typeparam name="RouteType"></typeparam>
//    internal class BidirectionalAStarRouting<EdgeType, VertexType> : IPoint2PointRouter<EdgeType, VertexType>
//        where EdgeType : class
//        where VertexType : class, IEquatable<VertexType>
//    {
//        /// <summary>
//        /// The graph the routing is being done on.
//        /// </summary>
//        private Graph<EdgeType, VertexType> _graph;

//        /// <summary>
//        /// The maximum to count.
//        /// </summary>
//        private int _max_to_count;

//        /// <summary>
//        /// Creates a new instance of this class.
//        /// </summary>
//        /// <param name="graph"></param>
//        public BidirectionalAStarRouting(Graph<EdgeType, VertexType> graph)
//        {
//            _graph = graph;
//            _max_to_count = -1;
//        }

//        /// <summary>
//        /// Creates a new instance of this class.
//        /// </summary>
//        /// <param name="graph"></param>
//        public BidirectionalAStarRouting(Graph<EdgeType, VertexType> graph,
//            int max_to_count)
//        {
//            _graph = graph;
//            _max_to_count = max_to_count;
//        }

//        /// <summary>
//        /// Calculates a route from a route pointo the to route point.
//        /// </summary>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        public GraphRoute<EdgeType, VertexType> Calculate(
//            VertexType from,
//            VertexType to)
//        {
//            VertexReference<EdgeType, VertexType> forward_result = null;
//            VertexReference<EdgeType, VertexType> backward_result = null;
//            float mu = float.MaxValue;

//            // initialize the data structures to hold the result.
//            Dictionary<VertexType, GraphRoute<EdgeType, VertexType>> route_node =
//                new Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>();

//            // fill in the routes to the node that are the same.
//            if (to == from)
//            {
//                route_node.Add(to,
//                    new GraphRoute<EdgeType, VertexType>
//                        (new GraphRouteEntryPoint<VertexType>(from, true),
//                            new List<GraphRouteEntry<EdgeType, VertexType>>(),
//                            0)
//                        );
//            }

//            // intialize data structures.
//            DykstraVisitList<EdgeType, VertexType> forward_visit_list = new DykstraVisitList<EdgeType, VertexType>();
//            BidirectionAStarVisitList<EdgeType, VertexType> forward_chosen_nodes = new BidirectionAStarVisitList<EdgeType, VertexType>();
//            DykstraVisitList<EdgeType, VertexType> backward_visit_list = new DykstraVisitList<EdgeType, VertexType>();
//            BidirectionAStarVisitList<EdgeType, VertexType> backward_chosen_nodes = new BidirectionAStarVisitList<EdgeType, VertexType>();

//            // set the from node as the current node and put it in the correct data structures.
//            // intialize the source's neighbours.
//            VertexReference<EdgeType, VertexType> forward_current =
//                new VertexReference<EdgeType, VertexType>(default(float), null, from, null);
//            HashSet<VertexAlongEdge<EdgeType, VertexType>> forward_neighbours = _graph.GetNeighbours(
//                forward_current.Vertex,
//                forward_chosen_nodes.Vertices);
//            forward_chosen_nodes.Add(forward_current);

//            // set the to node as the current node and put it in the correct data structures.
//            // intialize the targets's neighbours.
//            VertexReference<EdgeType, VertexType> backward_current =
//                new VertexReference<EdgeType, VertexType>(default(float), null, to, null);
//            HashSet<VertexAlongEdge<EdgeType, VertexType>> backward_neighbours = _graph.GetNeighboursReversed(
//                backward_current.Vertex,
//                backward_chosen_nodes.Vertices);
//            backward_chosen_nodes.Add(backward_current);

//            // loop until a match is found in the chosen nodes!
//            while (true)
//            {
//                // update the forward visited nodes.
//                foreach (VertexAlongEdge<EdgeType, VertexType> node_way in forward_neighbours)
//                {
//                    VertexReference<EdgeType, VertexType> ref_val;
//                    if (forward_visit_list.TryGetRef(node_way.Vertex, out ref_val))
//                    { // a reference for the node was found.
//                        if (ref_val.Weight.CompareTo(node_way.Weight + forward_current.Weight) > 0)
//                        { // the route to the node found was bigger than the route found here.
//                            // replace the original route.
//                            ref_val = new VertexReference<EdgeType, VertexType>(
//                                node_way.Weight + forward_current.Weight,
//                                forward_current,
//                                node_way.Vertex,
//                                node_way.Edge);
//                        }
//                    }
//                    else
//                    { // no reference for the node was found.
//                        float weight = node_way.Weight + forward_current.Weight + 
//                            this.EstimateDistanceToGoal(node_way.Vertex, to);
//                        ref_val = new VertexReference<EdgeType, VertexType>(weight,
//                            forward_current,
//                            node_way.Vertex,
//                            node_way.Edge);
//                        forward_visit_list.AddNode(weight,
//                            ref_val);
//                    }
//                }

//                // update the backward visited nodes.
//                foreach (VertexAlongEdge<EdgeType, VertexType> node_way in backward_neighbours)
//                {
//                    VertexReference<EdgeType, VertexType> ref_val;
//                    if (backward_visit_list.TryGetRef(node_way.Vertex, out ref_val))
//                    { // a reference for the node was found.
//                        if (ref_val.Weight.CompareTo(node_way.Weight + backward_current.Weight) > 0)
//                        { // the route to the node found was bigger than the route found here.
//                            // replace the original route.
//                            ref_val = new VertexReference<EdgeType, VertexType>(
//                                node_way.Weight + backward_current.Weight,
//                                backward_current,
//                                node_way.Vertex,
//                                node_way.Edge);
//                        }
//                    }
//                    else
//                    { // no reference for the node was found.
//                        float weight = node_way.Weight + backward_current.Weight + 
//                            this.EstimateDistanceToGoal(node_way.Vertex, to);
//                        ref_val = new VertexReference<EdgeType, VertexType>(weight,
//                            backward_current,
//                            node_way.Vertex,
//                            node_way.Edge);
//                        backward_visit_list.AddNode(weight,
//                            ref_val);
//                    }
//                }

//                while (forward_visit_list.Count > 0 &&
//                    backward_visit_list.Count > 0)
//                {
//                    // get the first node.
//                    VertexReference<EdgeType, VertexType> forward_first =
//                        forward_visit_list.GetFirst();
//                    VertexReference<EdgeType, VertexType> backward_first =
//                        backward_visit_list.GetFirst();
                    
//                    // test stop criteria.
//                    if (forward_chosen_nodes.Vertices.Contains(backward_current.Vertex))
//                    {
//                        // get the weight.
//                        VertexReference<EdgeType, VertexType> from_forward =
//                            forward_chosen_nodes.GetForVertex(backward_current.Vertex);
//                        VertexReference<EdgeType, VertexType> from_backward = backward_current;

//                        float weight = from_backward.Weight + from_forward.Weight;
//                        float total_weight = forward_first.Weight + backward_first.Weight;
//                        if (total_weight >= weight)
//                        { // stopping criteria is met.
//                            forward_result = from_forward;
//                            backward_result = from_backward;
//                            break;
//                        }
//                    }                    
//                    // test stop criteria.
//                    if (backward_chosen_nodes.Vertices.Contains(forward_current.Vertex))
//                    {
//                        // get the weight.
//                        VertexReference<EdgeType, VertexType> from_forward = forward_current;
//                        VertexReference<EdgeType, VertexType> from_backward =
//                            backward_chosen_nodes.GetForVertex(forward_current.Vertex);

//                        float weight = from_backward.Weight + from_forward.Weight;
//                        float total_weight = forward_first.Weight + backward_first.Weight;
//                        if (total_weight >= weight)
//                        { // stopping criteria is met.
//                            forward_result = from_forward;
//                            backward_result = from_backward;
//                            break;
//                        }
//                    }

//                    // check if it has forward neighbours.
//                    forward_neighbours = _graph.GetNeighbours(
//                        forward_first.From.Vertex,
//                        forward_first.Edge,
//                        forward_first.Vertex,
//                        forward_chosen_nodes.Vertices);
//                    forward_visit_list.RemoveNode(forward_first.Weight, forward_first);
//                    if (forward_neighbours.Count > 0)
//                    {
//                        forward_current = forward_first;

//                        // add the current node to the chosen nodes.
//                        forward_chosen_nodes.Add(forward_current);

//                        break;
//                    }

//                    // check if it has backward neighbours.
//                    backward_neighbours = _graph.GetNeighbours(
//                        backward_first.From.Vertex,
//                        backward_first.Edge,
//                        backward_first.Vertex,
//                        backward_chosen_nodes.Vertices);
//                    backward_visit_list.RemoveNode(backward_first.Weight, backward_first);
//                    if (backward_neighbours.Count > 0)
//                    {
//                        backward_current = backward_first;

//                        // add the current node to the chosen nodes.
//                        backward_chosen_nodes.Add(backward_current);

//                        break;
//                    }
//                }

//                // stop the search if no new node with neighbours was found or the routes found match the to-count.
//                if (forward_current == null
//                    || (forward_result != null && backward_result != null)
//                    || (forward_visit_list.Count == 0 && forward_neighbours.Count == 0)
//                    || (_max_to_count > 0 && route_node.Count >= _max_to_count))
//                {

//                    break;
//                }
//            }

//            if ((forward_current == null || backward_current == null))
//            {
//                Tools.Core.Output.OutputTextStreamHost.WriteLine("Route not found!");
//            }

//            // construct route.
//            GraphRoute<EdgeType, VertexType> route = forward_current.CreateRouteTo();
//            return route;
//        }

//        public GraphRoute<EdgeType, VertexType> Calculate(List<VertexType> in_order)
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
//                GraphRoute<EdgeType, VertexType> route = this.Calculate(from_in_entry, to_in_entry);
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
//            List<VertexType> to)
//        {
//            Dictionary<VertexType, GraphRoute<EdgeType, VertexType>> routes = 
//                new Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>();
//            foreach (VertexType to_vertex in to)
//            {
//                routes[to_vertex] = this.Calculate(from, to_vertex);
//            }
//            return routes;
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

//        private float EstimateDistanceToGoal(VertexType from, VertexType to)
//        {
//            float min = float.MaxValue;

//            float weight = _graph.UnderestimateWeight(from, to);
//            if (weight < min)
//            {
//                min = weight;
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
//                    from, to_node);

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

//        public float Distance(VertexType from, VertexType to)
//        {
//            throw new NotSupportedException();
//        }


//        #region IPoint2PointRouter<EdgeType,VertexType> Members

//        public void RegisterController(IPoint2PointRouterController<EdgeType, VertexType> controller)
//        {
//            //if (controller == null)
//            //{
//            //    throw new ArgumentNullException();
//            //}
//            //if (controller is IPoint2PointRouterDykstraController<EdgeType, VertexType>)
//            //{
//            //    _controller = controller as IPoint2PointRouterDykstraController<EdgeType, VertexType>;
//            //}
//            //else
//            //{
//            //    _controller = new ControllerWrapper<EdgeType, VertexType>(controller);
//            //}
//        }

//        public void RegisterDykstraController(IPoint2PointRouterDykstraController<EdgeType, VertexType> controller)
//        {
//            if (controller == null)
//            {
//                throw new ArgumentNullException();
//            }
//        }

//        #endregion

//        #region IPoint2PointRouter<EdgeType,VertexType> Members


//        public HashSet<VertexType> CalculateRange(VertexType from, float weight)
//        {
//            throw new NotSupportedException();
//        }

//        #endregion

//        public GraphRoute<EdgeType, VertexType> Calculate(VertexType from, VertexType to, float max_range)
//        {
//            throw new NotImplementedException();
//        }

//        public GraphRoute<EdgeType, VertexType> Calculate(List<VertexType> in_order, float max_range)
//        {
//            throw new NotImplementedException();
//        }

//        public Dictionary<VertexType, GraphRoute<EdgeType, VertexType>> Calculate(VertexType from, List<VertexType> to, float max_range)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
