using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core.Routes;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.InterRoute
{
    /// <summary>
    /// Represents an inter route improvement heuristic.
    /// </summary>
    public interface IInterRouteImprovement
    {
        /// <summary>
        /// Tries to improve the given solution given the given problem.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="difference"></param>
        /// <returns></returns>
        bool Improve(MaxTimeProblem problem, IRoute route1, IRoute route2, out float difference);
    }
}
