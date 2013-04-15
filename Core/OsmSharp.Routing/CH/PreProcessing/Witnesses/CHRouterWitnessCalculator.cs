using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.CH.Routing;
using OsmSharp.Routing.Graph.DynamicGraph;

namespace OsmSharp.Routing.CH.PreProcessing.Witnesses
{
    /// <summary>
    /// Does witness calculations using the CHRouter implementation.
    /// </summary>
    public class CHRouterWitnessCalculator : INodeWitnessCalculator
    {
        /// <summary>
        /// Holds the data target.
        /// </summary>
        private CHRouter _router;

        /// <summary>
        /// Creates a new witness calculator.
        /// </summary>
        /// <param name="data"></param>
        public CHRouterWitnessCalculator(IDynamicGraph<CHEdgeData> data)
        {
            _router = new CHRouter(data);
        }

        /// <summary>
        /// Returns true if the given vertex has a witness calculator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="via"></param>
        /// <param name="weight"></param>
        /// <param name="max_settles"></param>
        /// <returns></returns>
        public bool Exists(uint from, uint to, uint via, float weight, int max_settles)
        {
            return _router.CalculateWeight(from, to, via, weight, max_settles) <= weight;
        }
    }
}