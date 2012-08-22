using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.TSP.Problems;
using Tools.Math.VRP.Core.Routes;

namespace Tools.Math.TSP
{
    /// <summary>
    /// Interface representing a solver for the TSP.
    /// </summary>
    public interface ISolver
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Solves the TSP.
        /// </summary>
        /// <returns></returns>
        IRoute Solve(IProblem problem);

        /// <summary>
        /// Stops the executing of the solving process.
        /// </summary>
        void Stop();
        
        /// <summary>
        /// Raised when an intermidiate result is available.
        /// </summary>
        event SolverDelegates.IntermidiateDelegate IntermidiateResult;
    }

    public static class SolverDelegates
    {
        /// <summary>
        /// Delegate to pass on an intermidiate solution.
        /// </summary>
        /// <param name="result"></param>
        public delegate void IntermidiateDelegate(int[] result, float weight);
    }
}
