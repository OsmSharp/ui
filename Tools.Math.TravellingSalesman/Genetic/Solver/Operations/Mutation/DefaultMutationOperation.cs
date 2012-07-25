using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.TSP.Genetic.Solver;
using Tools.Math.Graph;

namespace Tools.Math.TSP.Genetic.Solver.Operations.Mutation
{
    /// <summary>
    /// Class mutating individuals in two ways:
    ///     - Switching two random nodes
    ///     - Switch order between two random nodes.
    /// </summary>
    public class DefaultMutationOperation :
        IMutationOperation<int, GeneticProblem, Fitness>
    {

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
        public Individual<int, GeneticProblem, Fitness> Mutate(
            Solver<int, GeneticProblem, Fitness> solver,
            Individual<int, GeneticProblem, Fitness> mutating)
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

            Individual individual = new Individual();
            individual.Initialize(genome);
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        #endregion
    }
}