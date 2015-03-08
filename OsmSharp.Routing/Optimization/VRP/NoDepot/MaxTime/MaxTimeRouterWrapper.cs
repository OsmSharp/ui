using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Routers;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Vehicles;

namespace OsmSharp.Routing.Optimization.VRP.NoDepot.MaxTime
{
    /// <summary>
    /// A wrapper for all the RouterMaxTime implementations.
    /// </summary>
    public class MaxTimeRouterWrapper : RouterVRPWrapper<RouterMaxTime>
    {
        /// <summary>
        /// Creates a new RouterMaxTime wrapper.
        /// </summary>
        /// <param name="routerVrp"></param>
        /// <param name="router"></param>
        public MaxTimeRouterWrapper(RouterMaxTime routerVrp, Router router)
            :base(routerVrp, router)
        {

        }

        /// <summary>
        /// Calculates the solution to the No-depot DVRP starting from the raw data.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="points"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public Route[] CalculateNoDepot(Vehicle vehicle, RouterPoint[] points, double[][] weights)
        {
            // build the points array.
            var locations = new GeoCoordinate[points.Length];
            for(int idx = 0; idx < points.Length; idx++)
            {
                locations[idx] = points[idx].Location;
            }

            // calculate the No-depot DVRP solution.
            int[][] solution = this.RouterVRP.CalculateNoDepot(weights, locations);

            // convert the solution.
            return this.ConvertSolution(vehicle, solution, points);
        }
    }
}