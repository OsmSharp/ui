// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core.Routes.ASymmetric;

namespace Routing.Core.VRP.NoDepot.MaxTime
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
