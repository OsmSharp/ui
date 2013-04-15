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

namespace OsmSharp.Tools.Math.VRP.Core.LocalSearch
{
    /// <summary>
    /// Responsible for an local search strategy doing best-placement.
    /// </summary>
    public class RePlace2OptHelper
    {
        /// <summary>
        /// Applies a local search strategy.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public static void CalculateRePlaceOptHelper(
            IProblemWeights problem, IRoute route)
        {
            throw new NotImplementedException();
        }
    }
}
