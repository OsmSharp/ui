using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.TSP.Genetic.Solver.Operations.Helpers;
using Tools.Math.TSP.Genetic.Solver;
using Tools.Math.Graph;

namespace Tools.Math.TSP.Genetic.Solver.Operations.Mutation
{
    public class BestPlacementMutationOperation :
        IMutationOperation<int, GeneticProblem, Fitness>
    {        
        public BestPlacementMutationOperation()
        {

        }

        public string Name
        {
            get
            {
                return "AI";
            }
        }

        #region IMutationOperation<int,Problem> Members

        public Individual<int, GeneticProblem, Fitness> Mutate(
            Solver<int, GeneticProblem, Fitness> solver,
            Individual<int, GeneticProblem, Fitness> mutating)
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

            Individual individual = new Individual();
            individual.Initialize(new_genome);
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        #endregion
    }
}
