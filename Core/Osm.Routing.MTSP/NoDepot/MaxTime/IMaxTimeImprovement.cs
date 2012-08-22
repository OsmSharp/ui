using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime
{
    /// <summary>
    /// Represents an improvement heurstics for the MaxTimeProblem.
    /// </summary>
    public interface IMaxTimeImprovement
    {
        /// <summary>
        /// Tries to improve the given solution given the given problem.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="difference"></param>
        /// <returns></returns>
        bool Improve(MaxTimeProblem problem, MaxTimeSolution route, out float difference);
    }
}
