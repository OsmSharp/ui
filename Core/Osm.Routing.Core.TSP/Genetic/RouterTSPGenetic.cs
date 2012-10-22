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
using Tools.Math.VRP.Core.Routes;
using Tools.Core.Progress;
using Osm.Core;
using Tools.Math.TSP;
using Tools.Math.TSP.Genetic;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.Core.TSP.Genetic
{
    /// <summary>
    /// A TSP router using a genetic algorithm.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public class RouterTSPGenetic<ResolvedType> : RouterTSP<ResolvedType>
        where ResolvedType : IResolvedPoint
    {
        /// <summary>
        /// Creates a new TSP router;
        /// </summary>
        /// <param name="router"></param>
        public RouterTSPGenetic(IRouter<ResolvedType> router)
            :base(router)
        {

        }

        /// <summary>
        /// Creates a genetic solver.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        internal override ISolver DoCreateSolver()
        {
            return new GeneticSolver();
        }
    }
}
