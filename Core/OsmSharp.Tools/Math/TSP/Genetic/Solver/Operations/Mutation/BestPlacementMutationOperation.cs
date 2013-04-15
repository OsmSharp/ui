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
using OsmSharp.Tools.Math.AI.Genetic.Operations.Mutations;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Helpers;
using OsmSharp.Tools.Math.TSP.Genetic.Solver;

namespace OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Mutation
{
    /// <summary>
    /// A best placement mutation operation.
    /// </summary>
    public class BestPlacementMutationOperation :
        IMutationOperation<List<int>, GeneticProblem, Fitness>
    {       
        /// <summary>
        /// Creates a best placement mutation.
        /// </summary>
        public BestPlacementMutationOperation()
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

        #region IMutationOperation<int,Problem> Members

        /// <summary>
        /// Mutates a given individual.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="mutating"></param>
        /// <returns></returns>
        public Individual<List<int>, GeneticProblem, Fitness> Mutate(
            Solver<List<int>, GeneticProblem, Fitness> solver,
            Individual<List<int>, GeneticProblem, Fitness> mutating)
        {
            // take a random piece.
            int idx = solver.Random.Next(mutating.Genomes.Count);

            List<int> new_genome = new List<int>(mutating.Genomes);
            int customer = new_genome[idx];
            new_genome.RemoveAt(idx);

            // apply best placement algorithm to place the selected genomes.
            BestPlacementHelper helper = BestPlacementHelper.Instance();
            helper.Do(
                solver.Problem,
                solver.FitnessCalculator as FitnessCalculator,
                new_genome,
                customer);

            Individual individual = new Individual(new_genome);
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        #endregion
    }
}
