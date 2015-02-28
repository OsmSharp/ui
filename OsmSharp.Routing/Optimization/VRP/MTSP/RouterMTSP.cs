// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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
using OsmSharp.Routing;
using OsmSharp.Math.Geo;

namespace OsmSharp.Routing.Optimization.VRP.MTSP
{
    /// <summary>
    /// Class to solve for a specific class of VRP problems: Multiple Travelling Salesmen.
    /// </summary>
    public abstract class RouterMTSP: RouterVRP
    {
        /// <summary>
        /// Creates an MTSP Router.
        /// </summary>
        public RouterMTSP()
            :base()
        {

        }

        /// <summary>
        /// Calculates a number of routes to travel on along the given point(s) as efficiently as possible.
        /// </summary>
        /// <param name="weights">The weights between all customer pairs.</param>
        /// <param name="locations">The location of all customers.</param>
        /// <returns></returns>
        public int[][] CalculateMTSP(double[][] weights, GeoCoordinate[] locations)
        {
            return this.CalculateMTSP(weights);
        }

        #region Default Functions

        /// <summary>
        /// Calculates the actual MTSP solution.
        /// </summary>
        /// <param name="weights"></param>
        /// <returns></returns>
        internal abstract int[][] CalculateMTSP(double[][] weights);

        #endregion
    }
}
