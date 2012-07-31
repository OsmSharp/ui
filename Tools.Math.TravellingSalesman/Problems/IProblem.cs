using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core;

namespace Tools.Math.TSP.Problems
{
    /// <summary>
    /// Interface representing a generic TSP-problem.
    /// </summary>
    public interface IProblem : IProblemWeights
    {
        /// <summary>
        /// Returns the size of the problem.
        /// </summary>
        int Size
        {
            get;
        }

        /// <summary>
        /// Returns the first customer.
        /// </summary>
        int? First
        {
            get;
        }

        /// <summary>
        /// Returns the last customer.
        /// </summary>
        int? Last
        {
            get;
        }

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
