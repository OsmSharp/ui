using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.VRP.Core;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.InterImprovements
{
    /// <summary>
    /// Abstract an inter-improvement algorithm that tries to exhange edges/customers between two routes to obtain a better result.
    /// </summary>
    public interface IInterImprovement
    {
        /// <summary>
        /// Returns the name of this inter-improvement algorithm.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Returns true if this inter-improvement is symmetric.
        /// </summary>
        bool IsSymmetric
        {
            get;
        }

        /// <summary>
        /// Returns true if there was an improvement.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="solution"></param>
        /// <param name="route1_idx"></param>
        /// <param name="route2_idx"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        bool Improve(MaxTimeProblem problem, MaxTimeSolution solution,
            int route1_idx, int route2_idx, double max);
    }
}