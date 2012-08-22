using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core.Routes;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.InterRoute
{
    /// <summary>
    /// CROSS-exchange heuristic.
    /// </summary>
    public class CrossExchangeImprovement : IInterRouteImprovement
    {
        /// <summary>
        /// Tries to improve the existing route by exchanging the end-parts.
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
