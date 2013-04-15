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
using OsmSharp.Tools.Math.AI.Genetic.Operations.Generation;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.TSP.LK;

namespace OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Generation
{
    /// <summary>
    /// A LK generation operation.
    /// </summary>
    public class LKGenerationOperation :
        IGenerationOperation<List<int>, GeneticProblem, Fitness>
    {
        /// <summary>
        /// Returns the name of this operation.
        /// </summary>
        public string Name
        {
            get
            {
                return "LK";
            }
        }

        #region IGenerationOperation<GenomeType> Members

        /// <summary>
        /// Generates a random individual.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        public Individual<List<int>, GeneticProblem, Fitness> Generate(
            Solver<List<int>, GeneticProblem, Fitness> solver)
        {
            ISolver lk_solver = new LinKernighanSolver();
            IRoute route = lk_solver.Solve(solver.Problem.BaseProblem);

            Individual individual = new Individual(new List<int>(route));
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        #endregion
    }
}
