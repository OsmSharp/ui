using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core.Routes;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.InterRoute
{
    /// <summary>
    /// Exchange improvement heuristic.
    /// </summary>
    public class ExchangeImprovement : IInterRouteImprovement
    {
        /// <summary>
        /// Tries to improve the existing routes by swapping simultaniously two customers in the two given routes.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route1"></param>
        /// <param name="route2"></param>
        /// <param name="difference"></param>
        /// <returns></returns>
        public bool Improve(MaxTimeProblem problem, IRoute route1, IRoute route2, out float difference)
        {
            difference = 0;
            return false;
        }
    }
}
