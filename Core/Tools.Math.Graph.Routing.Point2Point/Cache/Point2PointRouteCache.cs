//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph.Routing.Cache
//{
//    /// <summary>
//    /// Represents a cache for routes that are used muliple times (for example in another algorithm).
//    /// </summary>
//    /// <typeparam name="EdgeType"></typeparam>
//    /// <typeparam name="VertexType"></typeparam>
//    internal class Point2PointRouteCache<EdgeType, VertexType> : IPoint2PointRouter<EdgeType, VertexType>
//        where EdgeType : class
//        where VertexType : class, IEquatable<VertexType>
//    {
//        /// <summary>
//        /// The routes calculated between the cities.
//        /// </summary>
//        private Dictionary<VertexType,
//            Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>> _routes;

//        /// <summary>
//        /// The route calculator.
//        /// </summary>
//        private IPoint2PointRouter<EdgeType, VertexType> _router;

//        /// <summary>
//        /// The node the points are located in.
//        /// </summary>
//        private HashSet<VertexType> _nodes;

//        /// <summary>
//        /// Creates a new point 2 point cache.
//        /// </summary>
//        /// <param name="router"></param>
//        /// <param name="nodes"></param>
//        public Point2PointRouteCache(
//            IPoint2PointRouter<EdgeType, VertexType> router,
//            List<VertexType> nodes)
//        {
//            _router = router;
//            _routes = new Dictionary<VertexType, Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>>();
//            _nodes = new HashSet<VertexType>(nodes);
//        }

//        /// <summary>
//        /// Returns the route between node1 and node2.
//        /// </summary>
//        /// <param name="city1"></param>
//        /// <param name="city2"></param>
//        /// <returns></returns>
//        public GraphRoute<EdgeType, VertexType> GetRoute(VertexType node1, VertexType node2)
//        {
//            Dictionary<VertexType, GraphRoute<EdgeType, VertexType>> value1 = null;
//            GraphRoute<EdgeType, VertexType> value;
//            if (_routes.TryGetValue(node1, out value1)
//                && value1.TryGetValue(node2, out value))
//            {

//            }
//            else
//            {
//                value = this.CalculateRoute(node1, node2);
//            }

//            return value;
//        }

//        #region Calculations

//        /// <summary>
//        /// Returns all the important vertices.
//        /// </summary>
//        private HashSet<VertexType> Nodes
//        {
//            get
//            {
//                return _nodes;
//            }
//        }

//        int _loaded = 0;

//        /// <summary>
//        /// Calculates a new route.
//        /// </summary>
//        /// <param name="node1"></param>
//        /// <param name="node2"></param>
//        /// <returns></returns>
//        private GraphRoute<EdgeType, VertexType> CalculateRoute(VertexType node1, VertexType node2)
//        {
//            // Calculate the routes between node1 and all other nodes.
//            Dictionary<VertexType, GraphRoute<EdgeType, VertexType>> routes = _router.Calculate(
//                node1,
//                this.Nodes.ToList<VertexType>());

//            //// throw exception when a node is missing.
//            //if (routes.Count != this.Nodes.Count)
//            //{
//            //    throw new Exception();
//            //}

//            // add the result to the cache.
//            foreach (KeyValuePair<VertexType, GraphRoute<EdgeType, VertexType>> pair in routes)
//            {
//                this.AddTo(node1, pair.Key, pair.Value);

//                _loaded++;
//                if (_loaded % 100 == 0)
//                {
//                    Tools.Core.Output.OutputTextStreamHost.WriteLine("Calculated {0} weights!",
//                        _loaded);
//                }
//            }

//            if (_routes.ContainsKey(node1)
//                && _routes[node1].ContainsKey(node2))
//            {
//                return _routes[node1][node2];
//            }
//            return null;
//        }

//        private void AddTo(VertexType node1, VertexType node2, GraphRoute<EdgeType, VertexType> value)
//        {
//            Dictionary<VertexType, GraphRoute<EdgeType, VertexType>> value_dic;
//            if (!_routes.TryGetValue(node1, out value_dic))
//            {
//                value_dic = new Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>();
//                _routes.Add(node1, value_dic);
//                value_dic.Add(node2, value);
//            }
//            else
//            {
//                if (!value_dic.ContainsKey(node2))
//                {
//                    value_dic.Add(node2, value);
//                }
//            }
//        }

//        #endregion

//        #region IPoint2PointRouter Members

//        GraphRoute<EdgeType, VertexType> IPoint2PointRouter<EdgeType, VertexType>.CalculateRoute(VertexType from, VertexType to)
//        {
//            return this.GetRoute(from, to);
//        }

//        Dictionary<VertexType, GraphRoute<EdgeType, VertexType>> IPoint2PointRouter<EdgeType, VertexType>.Calculate(
//            VertexType from,
//            List<VertexType> to)
//        {
//            Dictionary<VertexType, GraphRoute<EdgeType, VertexType>> routes = new Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>();

//            foreach (VertexType to_node in to)
//            {
//                if (!routes.ContainsKey(to_node))
//                {
//                    routes.Add(to_node,
//                        this.GetRoute(from, to_node));
//                }
//            }

//            return routes;
//        }



//        public SortedDictionary<float, Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>> CalculateClosest(VertexType from, List<VertexType> to)
//        {
//            SortedDictionary<float, Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>> sorted_routes =
//                new SortedDictionary<float, Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>>();

//            foreach (VertexType to_node in to)
//            {
//                GraphRoute<EdgeType, VertexType> route = this.GetRoute(
//                    from, to_node);

//                Dictionary<VertexType, GraphRoute<EdgeType, VertexType>> route_dic;
//                if (!sorted_routes.TryGetValue(route.Weight, out route_dic))
//                {
//                    route_dic = new Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>();
//                    sorted_routes.Add(route.Weight, route_dic);
//                }
//                if (!route_dic.ContainsKey(to_node))
//                {
//                    route_dic.Add(to_node,
//                        route);
//                }
//            }

//            return sorted_routes;
//        }

//        public float Calculate(List<VertexType> nodes)
//        {
//            float distance = 0;
//            for (int idx = 0; idx < nodes.Count - 1; idx++)
//            {
//                distance =
//                    this.GetRoute(nodes[idx], nodes[idx + 1]).Weight + distance;
//            }
//            return distance;
//        }

//        public GraphRoute<EdgeType, VertexType> CalculateRoute(List<VertexType> in_order)
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
//                GraphRoute<EdgeType, VertexType> route =
//                    (this as IPoint2PointRouter<EdgeType, VertexType>).CalculateRoute(from_in_entry, to_in_entry);
//                weight = route.Weight + weight;
//                entries.AddRange(route.Entries);
//            }

//            return new GraphRoute<EdgeType, VertexType>(new GraphRouteEntryPoint<VertexType>(from, true), entries, weight);
//        }

//        #endregion

//        #region IPoint2PointRouter<EdgeType,VertexType> Members

//        public void RegisterController(IPoint2PointRouterController<EdgeType, VertexType> controller)
//        {
//            _router.RegisterController(controller);
//        }

//        #endregion
//    }
//}
