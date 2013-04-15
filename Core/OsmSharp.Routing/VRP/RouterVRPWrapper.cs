using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Route;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Routing.VRP
{
    /// <summary>
    /// A wrapper around the RouterVRP class.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    /// <typeparam name="RouterVRPType"></typeparam>
    public abstract class RouterVRPWrapper<ResolvedType, RouterVRPType>
        where ResolvedType : IRouterPoint
        where RouterVRPType : RouterVRP
    {
        /// <summary>
        /// Holds the router VRP type.
        /// </summary>
        private RouterVRPType _router_vrp;

        /// <summary>
        /// Holds the the router.
        /// </summary>
        private IRouter<ResolvedType> _router;

        /// <summary>
        /// Creates a router VRP wrapper.
        /// </summary>
        /// <param name="router_vrp"></param>
        /// <param name="router"></param>
        public RouterVRPWrapper(RouterVRPType router_vrp, IRouter<ResolvedType> router)
        {
            _router = router;
            _router_vrp = router_vrp;

            _router_vrp.IntermidiateResult += new VRP.RouterVRP.SolutionDelegate(_router_vrp_IntermidiateResult);
        }

        /// <summary>
        /// Returns the VRP solver of this wrapper.
        /// </summary>
        public RouterVRPType RouterVRP
        {
            get
            {
                return _router_vrp;
            }
        }

        /// <summary>
        /// Returns the router.
        /// </summary>
        public IRouter<ResolvedType> Router
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
        protected OsmSharpRoute[] ConvertSolution(VehicleEnum vehicle, int[][] solution, ResolvedType[] points)
        {
            OsmSharpRoute[] routes = new OsmSharpRoute[solution.Length];
            for (int route_idx = 0; route_idx < solution.Length; route_idx++)
            {
                // concatenate the route(s).
                OsmSharpRoute tsp = null;
                OsmSharpRoute route;
                for (int idx = 0; idx < solution[route_idx].Length - 1; idx++)
                {
                    route = _router.Calculate(VehicleEnum.Car, points[solution[route_idx][idx]],
                        points[solution[route_idx][idx + 1]]);
                    if (route != null && route.Entries.Length > 0)
                    {
                        if (tsp == null)
                        { // first route = start
                            tsp = route;
                        }
                        else
                        { // concatenate.
                            tsp = OsmSharpRoute.Concatenate(tsp, route);
                        }
                    }
                }

                // concatenate the route from the last to the first point again.
                route = _router.Calculate(vehicle, points[solution[route_idx][solution[route_idx].Length - 1]],
                            points[solution[route_idx][0]]);
                if (route.Entries.Length > 0)
                {
                    tsp = OsmSharpRoute.Concatenate(tsp, route);
                }

                // set the result.
                routes[route_idx] = tsp;

                if (routes[route_idx] != null)
                { // route exists!
                    List<RouteTags> tags = new List<RouteTags>();
                    RouteTags customer_count = new RouteTags();
                    customer_count.Key = "customer_count";
                    customer_count.Value = solution[route_idx].Length.ToString();
                    tags.Add(customer_count);
                    routes[route_idx].Tags = tags.ToArray();

                    // set the correct vehicle type.
                    routes[route_idx].Vehicle = vehicle;
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
        public delegate void OsmSharpRoutesDelegate(OsmSharpRoute[] result);

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
        protected void RaiseIntermidiateResult(OsmSharpRoute[] result)
        {
            if (IntermidiateResult != null)
            {
                this.IntermidiateResult(result);
            }
        }

        #endregion
    }
}