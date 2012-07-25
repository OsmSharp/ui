using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.TSP.Genetic.Solver;
using Tools.Math.TSP.Genetic.Solver.Operations.Helpers;
using Tools.Math.Graph;

namespace Tools.Math.TSP.Genetic.Solver.Operations.Mutation
{
    public class BestDetailedPlacementMutationOperation :
        IMutationOperation<int, GeneticProblem, Fitness>
    {
        public BestDetailedPlacementMutationOperation()
        {

        }

        public string Name
        {
            get
            {
                return "CI";
            }
        }

        #region IMutationOperation<Node,Problem> Members

        public Individual<int, GeneticProblem, Fitness> Mutate(
            Solver<int, GeneticProblem, Fitness> solver,
            Individual<int, GeneticProblem, Fitness> mutating)
        {
            if (solver.Random.Next(100) > 60)
            {
                return MutateByTakingPiece(solver, mutating);
            }
            else
            {
                return MutateByRePlacement(solver, mutating);
            }
        }

        /// <summary>
        /// Take a piece of the genome and re-do best placement.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="mutating"></param>
        /// <returns></returns>
        private Individual<int, GeneticProblem, Fitness> MutateByTakingPiece(
            Solver<int, GeneticProblem, Fitness> solver,
            Individual<int, GeneticProblem, Fitness> mutating)
        {
            // take a random piece.
            int idx1 = 0;
            int idx2 = 0;
            while (idx2 - idx1 == 0)
            {
                idx1 = solver.Random.Next(mutating.Genomes.Count - 1) + 1;
                idx2 = solver.Random.Next(mutating.Genomes.Count - 1) + 1;
                if (idx1 > idx2)
                {
                    int temp = idx1;
                    idx1 = idx2;
                    idx2 = temp;
                }
            }

            // if the genome range is big take it from the best individual.
            Individual<int, GeneticProblem, Fitness> source =
                (mutating as Individual<int, GeneticProblem, Fitness>);
            Individual<int, GeneticProblem, Fitness> target =
                (mutating as Individual<int, GeneticProblem, Fitness>);

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

            Individual individual = new Individual();
            individual.Initialize(new_genome);
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        /// <summary>
        /// Re-places all the cities again to their own best place.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="mutating"></param>
        /// <returns></returns>
        private Individual<int, GeneticProblem, Fitness> MutateByRePlacement(
            Solver<int, GeneticProblem, Fitness> solver,
            Individual<int, GeneticProblem, Fitness> mutating)
        {
            List<int> nodes_to_re_place = mutating.Genomes.ToList<int>();
            List<int> current_placement = mutating.Genomes.ToList<int>();

            foreach (int node_to_place in nodes_to_re_place)
            {
                // take the node out.
                current_placement.Remove(node_to_place);

                // place the node back in.
                BestPlacementHelper helper = BestPlacementHelper.Instance();
                helper.Do(
                    solver.Problem,
                    solver.FitnessCalculator as FitnessCalculator,
                    current_placement, 
                    node_to_place);
            }

            Individual individual = new Individual();
            individual.Initialize(current_placement);
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        #endregion
    }
}
