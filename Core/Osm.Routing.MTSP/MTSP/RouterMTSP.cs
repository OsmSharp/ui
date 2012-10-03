using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Core.Route;
using Osm.Core;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.Core.VRP.MTSP
{
    /// <summary>
    /// Class to solve for a specific class of VRP problems: Multiple Travelling Salesmen.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public abstract class RouterMTSP<ResolvedType> : RouterVRP<ResolvedType>
        where ResolvedType : IResolvedPoint
    {
        /// <summary>
        /// Creates an MTSP Router.
        /// </summary>
        /// <param name="router"></param>
        public RouterMTSP(IRouter<ResolvedType> router)
            :base(router)
        {

        }

        /// <summary>
        /// Calculates a number of routes to travel on along the given point(s) as efficiently as possible.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public OsmSharpRoute[] CalculateMTSP(ResolvedType[] points)
        {
            // calculate the weights.
            float[][] weights = this.CalculateManyToManyWeigth(points);

            // calculate the MTSP.
            int[][] mtsp_solution = this.CalculateMTSP(weights);

            // concatenate the route(s).
            OsmSharpRoute[] mtsp = new OsmSharpRoute[mtsp_solution.Length];
            for (int route_idx = 0; route_idx < mtsp_solution.Length; route_idx++)
            {
                OsmSharpRoute route;
                OsmSharpRoute tsp = null;
                for (int idx = 0; idx < mtsp_solution[route_idx].Length - 1; idx++)
                {
                    route = this.Calculate(points[mtsp_solution[route_idx][idx]],
                        points[mtsp_solution[route_idx][idx + 1]]);
                    if (tsp == null)
                    { // first route = start
                        tsp = route;
                    }
                    else
                    { // concatenate.
                        tsp = OsmSharpRoute.Concatenate(tsp, route);
                    }
                }

                // concatenate the route from the last to the first point again.
                route = this.Calculate(points[mtsp_solution[route_idx][mtsp_solution[route_idx].Length - 1]],
                            points[mtsp_solution[route_idx][0]]);
                tsp = OsmSharpRoute.Concatenate(tsp, route);

                // set the route.
                mtsp[route_idx] = tsp;
            }
            return mtsp;
        }

        #region Default Functions

        /// <summary>
        /// Calculates the actual MTSP solution.
        /// </summary>
        /// <param name="weights"></param>
        /// <returns></returns>
        internal abstract int[][] CalculateMTSP(float[][] weights);

        #endregion
    }
}
