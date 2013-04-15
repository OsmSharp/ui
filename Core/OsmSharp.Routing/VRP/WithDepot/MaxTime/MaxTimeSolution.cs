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
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Routing.VRP.WithDepot.MaxTime
{
    /// <summary>
    /// Represents a solution to the MaxTime problem.
    /// </summary>
    public class MaxTimeSolution : DepotDynamicAsymmetricMultiRoute
    {
        /// <summary>
        /// Creates a new solution in the form of a DynamicAsymmetricMultiRoute.
        /// </summary>
        /// <param name="size"></param>
        public MaxTimeSolution(int size)
            : base(size)
        {
            _weights = new List<double>();
        }

        /// <summary>
        /// Creates a new solution based on an existing one.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="next_array"></param>
        protected MaxTimeSolution(int[] first, int[] next_array)
            : base(first, next_array)
        {
            _weights = new List<double>();
        }

        /// <summary>
        /// Creates a deep-copy of this solution.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            int[] first = new int[this.Count];
            for (int idx = 0; idx < this.Count; idx++)
            {
                IRoute route = this.Route(idx);
                if (route.Count > 1)
                {
                    first[idx] = route.ElementAt<int>(1);
                }
                else
                {
                    first[idx] = -1;
                }
            }
            return new MaxTimeSolution(first, _next_array.Clone() as int[]);
        }

        #region Weights

        /// <summary>
        /// Keeps all the weights of the routes.
        /// </summary>
        private List<double> _weights;

        /// <summary>
        /// Gets/sets the weight of the route at the given index.
        /// </summary>
        /// <param name="route_idx"></param>
        /// <returns></returns>
        public double this[int route_idx]
        {
            get
            {
                if (route_idx < _weights.Count)
                {
                    return _weights[route_idx];
                }
                return 0;
            }
            set
            {
                while (!(route_idx < _weights.Count))
                { // add elements recursively.
                    _weights.Add(0);
                }
                _weights[route_idx] = value;
            }
        }

        /// <summary>
        /// Removes the weight for the given route.
        /// </summary>
        /// <param name="route_idx"></param>
        public void RemoveWeight(int route_idx)
        {
            if (route_idx < _weights.Count)
            {
                _weights.RemoveAt(route_idx);
            }
        }

        #endregion
    }
}
