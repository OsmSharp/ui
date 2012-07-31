using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.TSP.Problems;
using Tools.Math.VRP.Core.Routes;
using Tools.Math.VRP.Core;

namespace Tools.Math.TSP
{
    /// <summary>
    /// Represents an improvement heuristic/solver.
    /// </summary>
    public interface IImprovement
    {
        /// <summary>
        /// Returns true if there was an improvement.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        bool Improve(IProblemWeights problem, IRoute route, out float difference);
    }
}
