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
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Routing.Core.VRP.NoDepot.MaxTime.InterRoute
{
    /// <summary>
    /// CROSS-exchange heuristic.
    /// </summary>
    public class CrossExchangeImprovement : IInterRouteImprovement
    {
        /// <summary>
        /// Tries to improve the existing route by exchanging the end-parts.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route1"></param>
        /// <param name="route2"></param>
        /// <param name="difference"></param>
        /// <returns></returns>
        public bool Improve(MaxTimeProblem problem, IRoute route1, IRoute route2, out double difference)
        {
            difference = 0;
            return false;
        }
    }
}
