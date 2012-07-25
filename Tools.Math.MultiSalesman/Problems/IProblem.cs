using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.VRP.MultiSalesman.Problems
{
    /// <summary>
    /// Represents an M-TSP problem.
    /// </summary>
    public interface IProblem : Tools.Math.AI.Genetic.IProblem
    {
        /// <summary>
        /// Returns the size of the problem.
        /// </summary>
        int Size
        {
            get;
        }

        /// <summary>
        /// Returns the weight between two customers.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        float Weight(int from, int to);

        /// <summary>
        /// Returns true if the problem is symmetric.
        /// </summary>
        bool Symmetric
        {
            get;
        }

        /// <summary>
        /// Returns true if the problem is euclidean.
        /// </summary>
        bool Euclidean
        {
            get;
        }
    }
}
