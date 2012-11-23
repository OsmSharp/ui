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
using OsmSharp.Routing.CH.Routing;
using OsmSharp.Routing.Core.Graph;
using OsmSharp.Routing.Core.Graph.Router;
using OsmSharp.Routing.Core.Graph.DynamicGraph;

namespace OsmSharp.Routing.CH.PreProcessing.Witnesses
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
        /// Creates a new witness calculator.
        /// </summary>
        /// <param name="data"></param>
        public DykstraWitnessCalculator(IDynamicGraph<CHEdgeData> data)
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
        /// <returns></returns>
        public bool Exists(uint from, uint to, uint via, float weight, int max_settles)
        {
            return _router.CalculateWeight(from, to, via, weight, max_settles) <= weight;
        }
    }
}
