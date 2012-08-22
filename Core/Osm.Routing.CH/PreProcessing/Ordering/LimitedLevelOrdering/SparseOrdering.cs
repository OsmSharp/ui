using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.CH.Primitives;

namespace Osm.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering
{
    /// <summary>
    /// Orders the vertices putting the sparse vertices first, any other float.MaxValue.
    /// 
    /// This should result in a sparser graph without nodes with exactly 2 neighbours.
    /// </summary>
    public class SparseOrdering : INodeWeightCalculator
    {
        /// <summary>
        /// Calculates the ordering.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public float Calculate(int level, CHVertex u)
        {
            if (u.BackwardNeighbours.Count == 2 &&
                u.ForwardNeighbours.Count == 2)
            {
                return -1;
            }
            else
            {
                return float.MaxValue;
            }
        }

        /// <summary>
        /// Do nothing with this here!
        /// </summary>
        /// <param name="vertex"></param>
        public void NotifyContracted(CHVertex vertex)
        {

        }
    }
}