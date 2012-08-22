using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime
{
    /// <summary>
    /// Represents an abstraction of a max time solver.
    /// </summary>
    public interface IMaxTimeSolver
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Solves the max time problem.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        MaxTimeSolution Solve(MaxTimeProblem problem);
    }
}
