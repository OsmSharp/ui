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

using OsmSharp.Math.AI.Genetic;
using OsmSharp.Math.AI.Genetic.Operations;
using OsmSharp.Math.AI.Genetic.Solvers;

namespace OsmSharp.Routing.Optimization.VRP.NoDepot.MaxTime.Genetic.Generation
{
    /// <summary>
    /// Generates individuals based on another solver.
    /// </summary>
    internal class SolverGenerationOperation :
        IGenerationOperation<MaxTimeSolution, MaxTimeProblem, Fitness>
    {
        /// <summary>
        /// Holds the solver.
        /// </summary>
        private IMaxTimeSolver _solver;

        /// <summary>
        /// Creates a new solver generation operation.
        /// </summary>
        /// <param name="solver"></param>
        public SolverGenerationOperation(IMaxTimeSolver solver)
        {
            _solver = solver;
        }

        /// <summary>
        /// Returns the name of this operation.
        /// </summary>
        public string Name
        {
            get
            {
                return "SOL";
            }
        }

        /// <summary>
        /// Generates individuals based on a random first customer for each route.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        public Individual<MaxTimeSolution, MaxTimeProblem, Fitness> Generate(
            Solver<MaxTimeSolution, MaxTimeProblem, Fitness> solver)
        {
            return new Individual<MaxTimeSolution,MaxTimeProblem,Fitness>(
                _solver.Solve(solver.Problem));
        }
    }
}
