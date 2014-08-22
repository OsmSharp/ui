using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Routers;
using OsmSharp.Math.VRP.Core.Routes;

namespace OsmSharp.Routing.VRP
{
    /// <summary>
    /// A wrapper around the RouterVRP class.
    /// </summary>
    /// <typeparam name="TRouterVRPType"></typeparam>
    public abstract class RouterVRPWrapper<TRouterVRPType>
        where TRouterVRPType : RouterVRP
    {
        /// <summary>
        /// Holds the router VRP type.
        /// </summary>
        private readonly TRouterVRPType _routerVRP;

        /// <summary>
        /// Holds the the router.
        /// </summary>
        private readonly Router _router;

        /// <summary>
        /// Creates a router VRP wrapper.
        /// </summary>
        /// <param name="routerVRP"></param>
        /// <param name="router"></param>
        protected RouterVRPWrapper(TRouterVRPType routerVRP, Router router)
        {
            _router = router;
            _routerVRP = routerVRP;

            _routerVRP.IntermidiateResult += new VRP.RouterVRP.SolutionDelegate(_router_vrp_IntermidiateResult);
        }

        /// <summary>
        /// Returns the VRP solver of this wrapper.
        /// </summary>
        public TRouterVRPType RouterVRP
        {
            get
            {
                return _routerVRP;
            }
        }

        /// <summary>
        /// Returns the router.
        /// </summary>
        public Router Router
        {
            get
            {
                return _router;
            }
        }

        /// <summary>
        /// Returns the name of this router.
        /// </summary>
        public string Name
        {
            get
            {
                return this.RouterVRP.Name;
            }
        }

        /// <summary>
        /// Converts a simple VRP solution into a solution containing the actual routes.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="solution"></param>
        /// <param name="points"></param>
        protected Route[] ConvertSolution(Vehicle vehicle, int[][] solution, RouterPoint[] points)
        {
            var routes = new Route[solution.Length];
            for (int routeIdx = 0; routeIdx < solution.Length; routeIdx++)
            {
                // concatenate the route(s).
                Route tsp = null;
                Route route;
                for (int idx = 0; idx < solution[routeIdx].Length - 1; idx++)
                {
                    route = _router.Calculate(vehicle, points[solution[routeIdx][idx]],
                        points[solution[routeIdx][idx + 1]]);
                    if (route != null && route.Segments.Length > 0)
                    {
                        if (tsp == null)
                        { // first route = start
                            tsp = route;
                        }
                        else
                        { // concatenate.
                            tsp = Route.Concatenate(tsp, route);
                        }
                    }
                }

                // concatenate the route from the last to the first point again.
                route = _router.Calculate(vehicle, points[solution[routeIdx][solution[routeIdx].Length - 1]],
                            points[solution[routeIdx][0]]);
                if (route.Segments.Length > 0)
                {
                    tsp = Route.Concatenate(tsp, route);
                }

                // set the result.
                routes[routeIdx] = tsp;

                if (routes[routeIdx] != null)
                { // route exists!
                    var tags = new List<RouteTags>();
                    var customerCount = new RouteTags();
                    customerCount.Key = "customer_count";
                    customerCount.Value = solution[routeIdx].Length.ToString();
                    tags.Add(customerCount);
                    routes[routeIdx].Tags = tags.ToArray();

                    // set the correct vehicle type.
                    routes[routeIdx].Vehicle = vehicle.UniqueName;
                }
            }

            return routes;
        }

        #region Intermidiate Results

        /// <summary>
        /// Handles events from the VRP solver and converts them to real results.
        /// </summary>
        /// <param name="result"></param>
        void _router_vrp_IntermidiateResult(IEnumerable<int[]> result)
        {
            foreach (int[] route in result)
            {

            }
        }

        /// <summary>
        /// Delegate to pass on an array of routes.
        /// </summary>
        /// <param name="result"></param>
        public delegate void OsmSharpRoutesDelegate(Route[] result);

        /// <summary>
        /// Raised when an intermidiate result is available.
        /// </summary>
        public event OsmSharpRoutesDelegate IntermidiateResult;

        /// <summary>
        /// Returns true when the event has to be raised.
        /// </summary>
        /// <returns></returns>
        protected bool CanRaiseIntermidiateResult()
        {
            return this.IntermidiateResult != null;
        }

        /// <summary>
        /// Raises the intermidiate results event.
        /// </summary>
        /// <param name="result"></param>
        protected void RaiseIntermidiateResult(Route[] result)
        {
            if (IntermidiateResult != null)
            {
                this.IntermidiateResult(result);
            }
        }

        #endregion
    }
}
