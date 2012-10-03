using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.Sparse.Primitives;

namespace Osm.Routing.CH.PreProcessing
{
    /// <summary>
    /// A witness calculator.
    /// </summary>
    public interface INodeWitnessCalculator
    {
        /// <summary>
        /// Return true if a witness exists for the given graph vertex 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="via"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        bool Exists(uint from, uint to, uint via, float weight);
    }
}
