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
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Helpers;
using OsmSharp.Tools.Math.TSP.Genetic.Solver;

namespace OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Generation
{
    /// <summary>
    /// Generates a random individual.
    /// </summary>
    public class BestPlacementGenerationOperation :
        IGenerationOperation<List<int>, GeneticProblem, Fitness>
    {
        /// <summary>
        /// Creates a best placement operation.
        /// </summary>
        public BestPlacementGenerationOperation()
        {

        }

        /// <summary>
        /// Returns the name of this operation.
        /// </summary>
        public string Name
        {
            get
            {
                return "AI";
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
            // create new genomes list.
            List<int> genome = new List<int>();

            // build cities to place (in a thread-safe way!)
            List<int> cities_to_place = new List<int>();
            lock (solver)
            {
                for (int idx = 0; idx < solver.Problem.Along.Count; idx++)
                {
                    cities_to_place.Add(solver.Problem.Along[idx]);
                }
            }

            BestPlacementHelper helper = BestPlacementHelper.Instance();
            helper.DoFast(
                solver.Problem,
                solver.FitnessCalculator as FitnessCalculator,
                genome,
                cities_to_place);

            Individual individual = new Individual(genome);
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        #endregion
    }
}
