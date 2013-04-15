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
using OsmSharp.Tools.Math.TSP.Genetic.Solver;

namespace OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Mutation
{
    /// <summary>
    /// Class mutating individuals in two ways:
    ///     - Switching two random nodes
    ///     - Switch order between two random nodes.
    /// </summary>
    public class DefaultMutationOperation :
        IMutationOperation<List<int>, GeneticProblem, Fitness>
    {
        /// <summary>
        /// Returns the name of this operation.
        /// </summary>
        public string Name
        {
            get
            {
                return "RAN";
            }
        }

        #region IMutationOperation<int,Problem> Members

        /// <summary>
        /// Mutates an idividual.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="mutating"></param>
        /// <returns></returns>
        public Individual<List<int>, GeneticProblem, Fitness> Mutate(
            Solver<List<int>, GeneticProblem, Fitness> solver,
            Individual<List<int>, GeneticProblem, Fitness> mutating)
        {
            List<int> genome = new List<int>(mutating.Genomes);

            if (solver.Random.Next(2) > 0)
            {
                // switch two nodes.
                int idx1 = solver.Random.Next(genome.Count);
                int idx2 = solver.Random.Next(genome.Count);

                if (idx1 != idx2)
                {
                    int temp = genome[idx1];
                    genome[idx1] = genome[idx2];
                    genome[idx2] = temp;
                }
            }
            else
            {
                // switch two nodes.
                int idx1 = solver.Random.Next(genome.Count);
                int idx2 = solver.Random.Next(genome.Count);

                // make sure idx1 < idx2.
                if (idx1 > idx2)
                {
                    int idx_temp = idx2;
                    idx2 = idx1;
                    idx1 = idx_temp;
                }

                // switch order.
                if (idx1 != idx2)
                {
                    List<int> genomes_to_change = new List<int>();
                    for (int idx = idx1; idx <= idx2; idx++)
                    {
                        genomes_to_change.Add(genome[idx]);
                    }
                    genomes_to_change.Reverse();
                    int idx_list = 0;
                    for (int idx = idx1; idx <= idx2; idx++)
                    {
                        genome[idx] = genomes_to_change[idx_list];
                        idx_list++;
                    }
                }
            }

            Individual individual = new Individual(genome);
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        #endregion
    }
}