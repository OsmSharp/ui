using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.CH.Routing;
using Osm.Data.Core.CH;

namespace Osm.Routing.CH.PreProcessing.Witnesses
{
    /// <summary>
    /// A simple dykstra witness calculator.
    /// </summary>
    public class DykstraWitnessCalculator : INodeWitnessCalculator
    {
        /// <summary>
        /// Holds the data target.
        /// </summary>
        private CHRouter _router;

        /// <summary>
        /// The max vertices to settle before stopping the witness search.
        /// </summary>
        private int _max_settles;

        /// <summary>
        /// Creates a new witness calculator.
        /// </summary>
        /// <param name="data"></param>
        public DykstraWitnessCalculator(ICHData data)
        {
            _router = new CHRouter(data);
            _max_settles = int.MaxValue;
        }

        /// <summary>
        /// Creates a new witness calculator.
        /// </summary>
        /// <param name="data"></param>
        public DykstraWitnessCalculator(ICHData data, int max_settles)
        {
            _router = new CHRouter(data);
            _max_settles = max_settles;
        }

        /// <summary>
        /// Returns true if the given vertex has a witness calculator.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool Exists(int level, long from, long to, long via, double weight)
        {
            return _router.CalculateWeight(from, to, via, weight, _max_settles) <= weight;
        }
    }
}
