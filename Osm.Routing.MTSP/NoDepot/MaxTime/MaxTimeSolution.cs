using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core.Routes.ASymmetric;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime
{
    /// <summary>
    /// Represents a solution to the MaxTime problem.
    /// </summary>
    public class MaxTimeSolution : DynamicAsymmetricMultiRoute
    {
        /// <summary>
        /// Creates a new solution in the form of a DynamicAsymmetricMultiRoute.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="is_round"></param>
        public MaxTimeSolution(int size, bool is_round)
            : base(size, is_round)
        {

        }

        /// <summary>
        /// Creates a new solution based on an existing one.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="next_array"></param>
        /// <param name="is_round"></param>
        protected MaxTimeSolution(int[] first, int[] next_array, bool is_round)
            : base(first, next_array, is_round)
        {

        }

        /// <summary>
        /// Clones this solution.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new MaxTimeSolution(_first.Clone() as int[], _next_array.Clone() as int[], _is_round);
        }
    }
}
