using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.CrossOver;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.AI.Genetic;
using Tools.Math.TSP;
using Tools.Math.TSP.Genetic.Solver;
using Tools.Math.TSP.Genetic.Solver.Operations.Helpers;
using Tools.Math.Graph;

namespace Tools.Math.TSP.Genetic.Solver.Operations.CrossOver
{
    public class BestPlacementCrossOverOperation :
        ICrossOverOperation<int, GeneticProblem, Fitness>
    {
        public BestPlacementCrossOverOperation()
        {

        }

        public string Name
        {
            get
            {
                return "AI";
            }
        }

        #region ICrossOverOperation<int,Problem> Members

        public Individual<int, GeneticProblem, Fitness>
            CrossOver(Solver<int, GeneticProblem, Fitness> solver,
            Individual<int, GeneticProblem, Fitness> parent1,
            Individual<int, GeneticProblem, Fitness> parent2)
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

            // insert the source_piece at the best location.
            List<int> best_genome = null;
            Fitness best_fitness = null;
            Fitness best_distance = null;

            for (int idx = 0; idx <= new_genome.Count; idx++)
            {
                // create temp genome.
                List<int> temp_genome = new List<int>(new_genome);
                if (idx < new_genome.Count)
                {
                    temp_genome.InsertRange(idx, source_piece);
                }
                else
                {
                    temp_genome.AddRange(source_piece);
                }

                // calculate weight.
                Fitness temp_fitness = solver.FitnessCalculator.Fitness(solver.Problem, temp_genome);
                Fitness temp_distance = null;

                // select or not.
                if (temp_fitness.CompareTo(best_fitness) > 0)
                {
                    temp_distance = solver.FitnessCalculator.Fitness(solver.Problem, temp_genome);
                    best_distance = temp_distance;
                    best_fitness = temp_fitness;
                    best_genome = temp_genome;
                }
            }

            //new_genome.InsertRange(idx1, source_piece);

            Individual individual = new Individual();
            individual.Initialize(new List<int>(best_genome));
            return individual;
        }

        #endregion
    }
}
