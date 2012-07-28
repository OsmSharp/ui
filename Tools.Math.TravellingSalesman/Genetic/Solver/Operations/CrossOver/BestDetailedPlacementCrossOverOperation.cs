using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.CrossOver;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.AI.Genetic;
using Tools.Math.TSP.Genetic.Solver;
using Tools.Math.TSP.Genetic.Solver.Operations.Helpers;
using Tools.Math.Graph;

namespace Tools.Math.TSP.Genetic.Solver.Operations.CrossOver
{
    public class BestDetailedPlacementCrossOverOperation :
        ICrossOverOperation<List<int>, GeneticProblem, Fitness>
    {
        public BestDetailedPlacementCrossOverOperation()
        {     

        }

        public string Name
        {
            get
            {
                return "CI";
            }
        }

        #region ICrossOverOperation<int,Problem> Members

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
