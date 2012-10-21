// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.CH.Routing;
using Osm.Data.Core.DynamicGraph;

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
        public DykstraWitnessCalculator(IDynamicGraph<CHEdgeData> data)
        {
            _router = new CHRouter(data);
            _max_settles = int.MaxValue;
        }

        /// <summary>
        /// Creates a new witness calculator.
        /// </summary>
        /// <param name="data"></param>
        public DykstraWitnessCalculator(IDynamicGraph<CHEdgeData> data, int max_settles)
        {
            _router = new CHRouter(data);
            _max_settles = max_settles;
        }

        /// <summary>
        /// Returns true if the given vertex has a witness calculator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="via"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool Exists(uint from, uint to, uint via, float weight)
        {
            return _router.CalculateWeight(from, to, via, weight, _max_settles) <= weight;
        }
    }
}
