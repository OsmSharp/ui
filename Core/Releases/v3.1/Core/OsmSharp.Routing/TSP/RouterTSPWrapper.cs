using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Route;
using OsmSharp.Tools.Math.TSP;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Routing.TSP
{
    /// <summary>
    /// A wrapper around the RouterVRP class.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    /// <typeparam name="RouterTSPType"></typeparam>
    public class RouterTSPWrapper<ResolvedType, RouterTSPType>
        where ResolvedType : IRouterPoint
        where RouterTSPType : RouterTSP
    {
        /// <summary>
        /// Holds the router.
        /// </summary>
        private IRouter<ResolvedType> _router;

        /// <summary>
        /// Holds the router TSP.
        /// </summary>
        private RouterTSPType _router_tsp;

        /// <summary>
        /// Creates a new RouterTSPWrapper.
        /// </summary>
        /// <param name="router_tsp"></param>
        /// <param name="router"></param>
        public RouterTSPWrapper(RouterTSPType router_tsp, IRouter<ResolvedType> router)
        {
            _router = router;
            _router_tsp = router_tsp;
        }

        /// <summary>
        /// Calculates a weight matrix for the given array of points.
        /// </summary>
        /// <param name="vehicle">The vehicle type.</param>
        /// <param name="points">The points to travel along.</param>
        /// <returns></returns>
        protected double[][] CalculateManyToManyWeight(VehicleEnum vehicle, ResolvedType[] points)
        {
            return _router.CalculateManyToManyWeight(vehicle, points, points);
        }

        /// <summary>
        /// Calculates the shortest route along all given points starting and ending at the given points.
        /// </summary>
        /// <param name="vehicle">The vehicle type.</param>
        /// <param name="points">The points to travel along.</param>
        /// <param name="first">The index of the point to start from.</param>
        /// <param name="is_round">Return back to the first point or not.</param>
        /// <returns></returns>
        public OsmSharpRoute CalculateTSP(VehicleEnum vehicle, ResolvedType[] points, int first, bool is_round)
        {
            // calculate the weights.
            double[][] weights = this.CalculateManyToManyWeight(vehicle, points);
            
            // build the points array.
            GeoCoordinate[] locations = new GeoCoordinate[points.Length];
            for(int idx = 0; idx < points.Length; idx++)
            {
                locations[idx] = points[idx].Location;
            }

            // calculates the TSP solution.
            IRoute tsp_solution = _router_tsp.CalculateTSP(weights, locations, first, is_round);

            // calculate weight
            double weight = 0;
            foreach (Edge edge in tsp_solution.Edges())
            {
                weight = weight + weights[edge.From][edge.To];
            }

            // concatenate the route(s).
            return this.BuildRoute(points, tsp_solution, weight);
        }

        /// <summary>
        /// Calculates the shortest route along all given points starting and ending at the given points.
        /// </summary>
        /// <param name="vehicle">The vehicle type.</param>
        /// <param name="points">The points to travel along.</param>
        /// <param name="first">The index of the point to start from.</param>
        /// <param name="last">The index of the point to end at.</param>
        /// <returns></returns>
        public OsmSharpRoute CalculateTSP(VehicleEnum vehicle, ResolvedType[] points, int first, int last)
        {
            // calculate the weights.
            double[][] weights = this.CalculateManyToManyWeight(vehicle, points);
            
            // build the points array.
            GeoCoordinate[] locations = new GeoCoordinate[points.Length];
            for(int idx = 0; idx < points.Length; idx++)
            {
                locations[idx] = points[idx].Location;
            }
            
            // call the RouterTSP.
            IRoute tsp_solution = _router_tsp.CalculateTSP(weights, locations);

            // calculate weight
            double weight = 0;
            foreach (Edge edge in tsp_solution.Edges())
            {
                weight = weight + weights[edge.From][edge.To];
            }

            // concatenate the route(s).
            return this.BuildRoute(points, tsp_solution, weight);
        }

        /// <summary>
        /// Calculates the shortest route along all given points.
        /// </summary>
        /// <param name="vehicle">The vehicle type.</param>
        /// <param name="points">The points to travel along.</param>
        /// <param name="is_round">Make the route return to the start-point or not.</param>
        /// <returns></returns>
        public OsmSharpRoute CalculateTSP(VehicleEnum vehicle, ResolvedType[] points, bool is_round)
        {
            // calculate the weights.
            double[][] weights = this.CalculateManyToManyWeight(vehicle, points);

            // build the points array.
            GeoCoordinate[] locations = new GeoCoordinate[points.Length];
            for (int idx = 0; idx < points.Length; idx++)
            {
                locations[idx] = points[idx].Location;
            }

            // calculate the TSP.
            IRoute tsp_solution = _router_tsp.CalculateTSP(weights, locations, is_round);

            // calculate weight
            double weight = 0;
            foreach (Edge edge in tsp_solution.Edges())
            {
                weight = weight + weights[edge.From][edge.To];
            }

            // concatenate the route(s).
            return this.BuildRoute(points, tsp_solution, weight);
        }

        /// <summary>
        /// Calculates the shortest route along all given points returning back to the first.
        /// </summary>
        /// <param name="vehicle">The vehicle type.</param>
        /// <param name="points">The points to travel along.</param>
        /// <returns></returns>
        public OsmSharpRoute CalculateTSP(VehicleEnum vehicle, ResolvedType[] points)
        {
            return this.CalculateTSP(vehicle, points, true);
        }

        /// <summary>
        /// Builds an OsmSharRoute.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tsp_solution"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public OsmSharpRoute BuildRoute(ResolvedType[] points, IRoute tsp_solution, double weight)
        {
            int[] solution = tsp_solution.ToArray();

            OsmSharpRoute tsp = null;
            OsmSharpRoute route;
            for (int idx = 0; idx < solution.Length - 1; idx++)
            {
                route = _router.Calculate(VehicleEnum.Car, points[solution[idx]],
                    points[solution[idx + 1]]);
                if (tsp == null)
                { // first route = start
                    tsp = route;
                }
                else
                { // concatenate.
                    tsp = OsmSharpRoute.Concatenate(tsp, route);
                }
            }
            if (tsp_solution.IsRound)
            {
                // concatenate the route from the last to the first point again.
                route = _router.Calculate(VehicleEnum.Car, points[solution[solution.Length - 1]],
                            points[solution[0]]);
                tsp = OsmSharpRoute.Concatenate(tsp, route);
            }

            tsp.Tags = new RouteTags[1];
            tsp.Tags[0] = new RouteTags();
            tsp.Tags[0].Key = "internal_weight";
            tsp.Tags[0].Value = weight.ToString(System.Globalization.CultureInfo.InvariantCulture);

            return tsp;
        }

        #region Intermidiate Results

        /// <summary>
        /// Delegate to pass on an intermidiate solution.
        /// </summary>
        /// <param name="result"></param>
        public delegate void IntermidiateDelegate(OsmSharpRoute result, double weight);

        /// <summary>
        /// Raised when an intermidiate result is available.
        /// </summary>
        public event IntermidiateDelegate IntermidiateResult;

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
        protected void RaiseIntermidiateResult(OsmSharpRoute result, double weight)
        {
            if (IntermidiateResult != null)
            {
                this.IntermidiateResult(result, weight);
            }
        }

        #endregion
    }
}
