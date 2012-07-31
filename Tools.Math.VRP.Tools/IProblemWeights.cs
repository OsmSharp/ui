using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.VRP.Core
{
    /// <summary>
    /// The definition of the problem weights.
    /// </summary>
    public interface IProblemWeights
    {
        /// <summary>
        /// Returns the weight matrix if any, else returns null.
        /// </summary>
        float[][] WeightMatrix
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
        /// Returns the size.
        /// </summary>
        int Size
        {
            get;
        }

        /// <summary>
        /// Returns the 10 nearest neighbours of the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        NearestNeighbours10 Get10NearestNeighbours(int customer);
    }
}
