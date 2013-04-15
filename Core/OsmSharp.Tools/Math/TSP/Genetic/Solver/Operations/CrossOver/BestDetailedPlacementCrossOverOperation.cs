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
using OsmSharp.Tools.Math.AI.Genetic.Operations.CrossOver;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.TSP.Genetic.Solver;
using OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Helpers;

namespace OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.CrossOver
{
    /// <summary>
    /// A best detailed placement crossover operation.
    /// </summary>
    public class BestDetailedPlacementCrossOverOperation :
        ICrossOverOperation<List<int>, GeneticProblem, Fitness>
    {
        /// <summary>
        /// Creates a best detailed placement crossover operation.
        /// </summary>
        public BestDetailedPlacementCrossOverOperation()
        {     

        }

        /// <summary>
        /// Returns the name of this operation.
        /// </summary>
        public string Name
        {
            get
            {
                return "CI";
            }
        }

        #region ICrossOverOperation<int,Problem> Members

        /// <summary>
        /// Applies this crossover.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="parent1"></param>
        /// <param name="parent2"></param>
        /// <returns></returns>
        public Individual<List<int>, GeneticProblem, Fitness>
            CrossOver(Solver<List<int>, GeneticProblem, Fitness> solver,
            Individual<List<int>, GeneticProblem, Fitness> parent1,
            Individual<List<int>, GeneticProblem, Fitness> parent2)
        {
            // take a random piece.
            int idx1 = 0;
            int idx2 = 0;
            while (idx2 - idx1 == 0)
            {
                idx1 = solver.Random.Next(parent1.Genomes.Count - 1) + 1;
                idx2 = solver.Random.Next(parent2.Genomes.Count - 1) + 1;
                if (idx1 > idx2)
                {
                    int temp = idx1;
                    idx1 = idx2;
                    idx2 = temp;
                }
            }

            // if the genome range is big take it from the best individual.
            Individual<List<int>, GeneticProblem, Fitness> source =
                (parent1 as Individual<List<int>, GeneticProblem, Fitness>);
            Individual<List<int>, GeneticProblem, Fitness> target =
                (parent2 as Individual<List<int>, GeneticProblem, Fitness>);

            if (idx2 - idx1 < parent1.Genomes.Count / 2)
            { // the range is small; take the worste genomes.
                if (source.Fitness.CompareTo(target.Fitness) > 0)
                {
                    Individual<List<int>, GeneticProblem, Fitness> temp = source;
                    source = target;
                    target = temp;
                }
                else
                {
                    // do nothing.
                }
            }
            else
            { // the range is big; take the good genomes.
                if (source.Fitness.CompareTo(target.Fitness) > 0)
                {
                    // do nothing.
                }
                else
                {
                    Individual<List<int>, GeneticProblem, Fitness> temp = source;
                    source = target;
                    target = temp;
                }
            }
            List<int> source_piece = source.Genomes.GetRange(idx1, idx2 - idx1);
            List<int> new_genome = target.Genomes.GetRange(0, target.Genomes.Count);

            // insert the piece into the worst individual.
            // remove nodes in the source_piece.
            foreach (int source_node in source_piece)
            {
                new_genome.Remove(source_node);
            }

            // apply best placement algorithm to place the selected genomes.
            List<int> genome = new List<int>();
            BestPlacementHelper helper = BestPlacementHelper.Instance();
            helper.DoFast(
                solver.Problem,
                solver.FitnessCalculator as FitnessCalculator,
                new_genome, 
                source_piece);

            // return a new individual based on the new genome list.
            Individual individual = new Individual(new_genome);
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        #endregion
    }
}
