using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Routers;
using OsmSharp.Math.TSP;
using OsmSharp.Math.Geo;
using OsmSharp.Math.VRP.Core.Routes;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Metrics.Time;

namespace OsmSharp.Routing.TSP
{
    /// <summary>
    /// A wrapper around the RouterVRP class.
    /// </summary>
    /// <typeparam name="TRouterTSPType"></typeparam>
    public class RouterTSPWrapper<TRouterTSPType>
        where TRouterTSPType : RouterTSP
    {
        /// <summary>
        /// Holds the router.
        /// </summary>
        private readonly Router _router;

        /// <summary>
        /// Holds the router TSP.
        /// </summary>
        private readonly TRouterTSPType _routerTSP;

        /// <summary>
        /// Interpreter for the routing network.
        /// </summary>
        private readonly IRoutingInterpreter _interpreter;

        /// <summary>
        /// Creates a new RouterTSPWrapper.
        /// </summary>
        /// <param name="routerTSP"></param>
        /// <param name="router"></param>
        public RouterTSPWrapper(TRouterTSPType routerTSP, Router router)
        {
            _router = router;
            _routerTSP = routerTSP;
        }

        /// <summary>
        /// Creates a new RouterTSPWrapper.
        /// </summary>
        /// <param name="routerTSP"></param>
        /// <param name="router"></param>
        /// <param name="interpreter"></param>
        public RouterTSPWrapper(TRouterTSPType routerTSP, Router router, IRoutingInterpreter interpreter)
        {
            _router = router;
            _routerTSP = routerTSP;
            _interpreter = interpreter;
        }

        /// <summary>
        /// Calculates a weight matrix for the given array of points.
        /// </summary>
        /// <param name="vehicle">The vehicle type.</param>
        /// <param name="points">The points to travel along.</param>
        /// <returns></returns>
        protected double[][] CalculateManyToManyWeight(Vehicle vehicle, RouterPoint[] points)
        {
            return _router.CalculateManyToManyWeight(vehicle, points, points);
        }

        /// <summary>
        /// Calculates the shortest route along all given points starting and ending at the given points.
        /// </summary>
        /// <param name="vehicle">The vehicle type.</param>
        /// <param name="points">The points to travel along.</param>
        /// <param name="first">The index of the point to start from.</param>
        /// <param name="isRound">Return back to the first point or not.</param>
        /// <returns></returns>
        public Route CalculateTSP(Vehicle vehicle, RouterPoint[] points, int first, bool isRound)
        {
            // calculate the weights.
            double[][] weights = this.CalculateManyToManyWeight(vehicle, points);
            
            // build the points array.
            var locations = new GeoCoordinate[points.Length];
            for(int idx = 0; idx < points.Length; idx++)
            {
                locations[idx] = points[idx].Location;
            }

            // calculates the TSP solution.
            IRoute tspSolution = _routerTSP.CalculateTSP(weights, locations, first, isRound);

            // calculate weight
            double weight = 0;
            foreach (Edge edge in tspSolution.Edges())
            {
                weight = weight + weights[edge.From][edge.To];
            }

            // concatenate the route(s).
            return this.BuildRoute(vehicle, points, tspSolution, weight);
        }

        /// <summary>
        /// Calculates the shortest route along all given points starting and ending at the given points.
        /// </summary>
        /// <param name="vehicle">The vehicle type.</param>
        /// <param name="points">The points to travel along.</param>
        /// <param name="first">The index of the point to start from.</param>
        /// <param name="last">The index of the point to end at.</param>
        /// <returns></returns>
        public Route CalculateTSP(Vehicle vehicle, RouterPoint[] points, int first, int last)
        {
            // calculate the weights.
            double[][] weights = this.CalculateManyToManyWeight(vehicle, points);
            
            // build the points array.
            var locations = new GeoCoordinate[points.Length];
            for(int idx = 0; idx < points.Length; idx++)
            {
                locations[idx] = points[idx].Location;
            }
            
            // call the RouterTSP.
            IRoute tspSolution = _routerTSP.CalculateTSP(weights, locations);

            // calculate weight
            double weight = 0;
            foreach (Edge edge in tspSolution.Edges())
            {
                weight = weight + weights[edge.From][edge.To];
            }

            // concatenate the route(s).
            return this.BuildRoute(vehicle, points, tspSolution, weight);
        }

        /// <summary>
        /// Calculates the shortest route along all given points.
        /// </summary>
        /// <param name="vehicle">The vehicle type.</param>
        /// <param name="points">The points to travel along.</param>
        /// <param name="isRound">Make the route return to the start-point or not.</param>
        /// <returns></returns>
        public Route CalculateTSP(Vehicle vehicle, RouterPoint[] points, bool isRound)
        {
            // calculate the weights.
            double[][] weights = this.CalculateManyToManyWeight(vehicle, points);

            // build the points array.
            var locations = new GeoCoordinate[points.Length];
            for (int idx = 0; idx < points.Length; idx++)
            {
                locations[idx] = points[idx].Location;
            }

            // calculate the TSP.
            IRoute tspSolution = _routerTSP.CalculateTSP(weights, locations, isRound);

            // calculate weight
            double weight = 0;
            foreach (Edge edge in tspSolution.Edges())
            {
                weight = weight + weights[edge.From][edge.To];
            }

            // concatenate the route(s).
            return this.BuildRoute(vehicle, points, tspSolution, weight);
        }

        /// <summary>
        /// Calculates the shortest route along all given points returning back to the first.
        /// </summary>
        /// <param name="vehicle">The vehicle type.</param>
        /// <param name="points">The points to travel along.</param>
        /// <returns></returns>
        public Route CalculateTSP(Vehicle vehicle, RouterPoint[] points)
        {
            return this.CalculateTSP(vehicle, points, true);
        }

        /// <summary>
        /// Builds an OsmSharRoute.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="points"></param>
        /// <param name="tspSolution"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public Route BuildRoute(Vehicle vehicle, RouterPoint[] points, IRoute tspSolution, double weight)
        {
            int[] solution = tspSolution.ToArray();

            Route tsp = null;
            Route route;
            for (int idx = 0; idx < solution.Length - 1; idx++)
            {
                route = _router.Calculate(vehicle, points[solution[idx]],
                    points[solution[idx + 1]]);
                if (tsp == null)
                { // first route = start
                    tsp = route;
                }
                else
                { // concatenate.
                    tsp = Route.Concatenate(tsp, route);
                }
            }
            if (tspSolution.IsRound)
            {
                // concatenate the route from the last to the first point again.
                route = _router.Calculate(vehicle, points[solution[solution.Length - 1]],
                            points[solution[0]]);
                tsp = Route.Concatenate(tsp, route);
            }

            if (tsp != null)
            {
                tsp.Vehicle = vehicle.UniqueName; // set the correct vehicle type.

                if (_interpreter != null)
                { // there is an interpreter set: calculate time/distance.
                    // calculate metrics.
                    var calculator = new TimeCalculator(_interpreter);
                    Dictionary<string, double> metrics = calculator.Calculate(tsp);
                    tsp.TotalDistance = metrics[TimeCalculator.DISTANCE_KEY];
                    tsp.TotalTime = metrics[TimeCalculator.TIME_KEY];
                }

                tsp.Tags = new RouteTags[1];
                tsp.Tags[0] = new RouteTags();
                tsp.Tags[0].Key = "internal_weight";
                tsp.Tags[0].Value = weight.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            return tsp;
        }

        #region Intermidiate Results

        /// <summary>
        /// Delegate to pass on an intermidiate solution.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="weight"></param>
        public delegate void IntermidiateDelegate(Route result, double weight);

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
        /// <param name="weight"></param>
        protected void RaiseIntermidiateResult(Route result, double weight)
        {
            if (IntermidiateResult != null)
            {
                this.IntermidiateResult(result, weight);
            }
        }

        #endregion
    }
}
