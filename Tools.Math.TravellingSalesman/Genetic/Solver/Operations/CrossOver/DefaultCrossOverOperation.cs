using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.CrossOver;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.AI.Genetic;
using Tools.Math.TSP.Genetic.Solver;
using Tools.Math.Graph;

namespace Tools.Math.TSP.Genetic.Solver.Operations.CrossOver
{
    public class DefaultCrossOverOperation :
        ICrossOverOperation<int, GeneticProblem, Fitness>
    {
        public string Name
        {
            get
            {
                return "DE";
            }
        }

        #region ICrossOverOperation<int,Problem> Members

        public Individual<int, GeneticProblem, Fitness> CrossOver(
            Solver<int, GeneticProblem, Fitness> solver,
            Individual<int, GeneticProblem, Fitness> parent1,
            Individual<int, GeneticProblem, Fitness> parent2)
        {
            // take a random piece.
            int idx1 = solver.Random.Next(parent1.Genomes.Count - 1) + 1;
            int idx2 = solver.Random.Next(parent1.Genomes.Count - 1) + 1;
            if (idx1 > idx2)
            {
                int temp = idx1;
                idx1 = idx2;
                idx2 = temp;
            }

            // if the genome range is big take it from the best individual.
            Individual<int, GeneticProblem, Fitness> source =
                (parent1 as Individual<int, GeneticProblem, Fitness>);
            Individual<int, GeneticProblem, Fitness> target =
                (parent2 as Individual<int, GeneticProblem, Fitness>);

            if (idx2 - idx1 < parent1.Genomes.Count / 2)
            { // the range is small; take the worste genomes.
                if (source.Fitness.CompareTo(target.Fitness) > 0)
                {
                    Individual<int, GeneticProblem, Fitness> temp = source;
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
                    Individual<int, GeneticProblem, Fitness> temp = source;
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

            // insert the source_piece at index1
            new_genome.InsertRange(idx1, source_piece);

            Individual individual = new Individual();
            individual.Initialize(new List<int>(new_genome));
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        #endregion
    }
}
