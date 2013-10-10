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
using OsmSharp.Math.VRP.Core.Routes;
using OsmSharp.Progress;
using OsmSharp.Math.TSP;
using OsmSharp.Math.TSP.Genetic;
using OsmSharp.Routing;

namespace OsmSharp.Routing.TSP.Genetic
{
    /// <summary>
    /// A TSP router using a genetic algorithm.
    /// </summary>
    public class RouterTSPGenetic : RouterTSP
    {
        /// <summary>
        /// Creates a new TSP router;
        /// </summary>
        public RouterTSPGenetic()
        {

        }

        /// <summary>
        /// Creates a genetic solver.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        internal override ISolver DoCreateSolver(int size)
        {
            return new GeneticSolver();
        }
    }
}
