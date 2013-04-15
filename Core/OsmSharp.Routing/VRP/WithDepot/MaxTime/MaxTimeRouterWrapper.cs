using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Route;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Routing.VRP.WithDepot.MaxTime
{
    /// <summary>
    /// A wrapper for all the RouterMaxTime implementations.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public class MaxTimeRouterWrapper<ResolvedType> : RouterVRPWrapper<ResolvedType, RouterMaxTime>
        where ResolvedType : IRouterPoint
    {
        /// <summary>
        /// Creates a new RouterMaxTime wrapper.
        /// </summary>
        /// <param name="router_vrp"></param>
        /// <param name="router"></param>
        public MaxTimeRouterWrapper(RouterMaxTime router_vrp, IRouter<ResolvedType> router)
            : base(router_vrp, router)
        {

        }

        /// <summary>
        /// Calculates the solution to the No-depot DVRP starting from the raw data.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="points"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public OsmSharpRoute[] CalculateNoDepot(VehicleEnum vehicle, ResolvedType[] points, double[][] weights)
        {
            // build the points array.
            GeoCoordinate[] locations = new GeoCoordinate[points.Length];
            for (int idx = 0; idx < points.Length; idx++)
            {
                locations[idx] = points[idx].Location;
            }

            // calculate the No-depot DVRP solution.
            int[][] solution = this.RouterVRP.CalculateDepot(weights, locations);

            // convert the solution.
            return this.ConvertSolution(vehicle, solution, points);
        }
    }
}
