using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.DynamicGraph;

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
        /// Holds the data source.
        /// </summary>
        private IDynamicGraph<CHEdgeData> _data;

        /// <summary>
        /// Creates a new sparse ordering calculator.
        /// </summary>
        /// <param name="data"></param>
        public SparseOrdering(IDynamicGraph<CHEdgeData> data)
        {
            _data = data;
        }
        /// <summary>
        /// Calculates the ordering.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public float Calculate(uint vertex)
        {
            KeyValuePair<uint, CHEdgeData>[] neighbours = _data.GetArcs(vertex);

            uint forward = 0, backward = 0;
            foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours)
            {
                if (neighbour.Value.Forward)
                {
                    forward++;
                }
                if (neighbour.Value.Backward)
                {
                    backward++;
                }
            }

            if (forward == 2 &&
                backward == 2)
            {
                return -1;
            }
            else
            {
                return float.MaxValue;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Do nothing with this here!
        /// </summary>
        /// <param name="vertex"></param>
        public void NotifyContracted(uint vertex)
        {

        }
    }
}