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
using OsmSharp.Tools.Progress;
using OsmSharp.Tools.Math.TSP;
using OsmSharp.Tools.Math.TSP.Genetic;
using OsmSharp.Tools.Math.TSP.EdgeAssemblyGenetic;
using OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Generation;
using OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.CrossOver;
using OsmSharp.Routing;

namespace OsmSharp.Routing.TSP.Genetic
{
    /// <summary>
    /// A TSP router using a genetic algorithm.
    /// </summary>
    public class RouterTSPAEXGenetic : RouterTSP
    {
        /// <summary>
        /// Holds the population count.
        /// </summary>
        private int _population;

        /// <summary>
        /// Holds the stagnation count.
        /// </summary>
        private int _stagnation;

        /// <summary>
        /// Creates a new TSP router;
        /// </summary>
        public RouterTSPAEXGenetic()
        {
            _population = 300;
            _stagnation = 100;
        }

        /// <summary>
        /// Creates a new TSP router;
        /// </summary>
        public RouterTSPAEXGenetic(int population, int stagnation)
        {
            _population = population;
            _stagnation = stagnation;
        }

        /// <summary>
        /// Creates a genetic solver.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        internal override ISolver DoCreateSolver(int size)
        {
            return new EdgeAssemblyCrossOverSolver(_population, _stagnation,
                     new _3OptGenerationOperation(),
                      new EdgeAssemblyCrossover(30,
                             EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom,
                             true));
        }
    }
}