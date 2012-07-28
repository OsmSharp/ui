using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Generation;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.AI.Genetic;
using Tools.Math.TSP.Genetic.Solver.Operations.Helpers;
using Tools.Math.TSP.Genetic.Solver;
using Tools.Math.Graph;

namespace Tools.Math.TSP.Genetic.Solver.Operations.Generation
{
    /// <summary>
    /// Generates a random individual.
    /// </summary>
    /// <typeparam name="GenomeType"></typeparam>
    public class BestPlacementGenerationOperation :
        IGenerationOperation<List<int>, GeneticProblem, Fitness>
    {
        public BestPlacementGenerationOperation()
        {

        }

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
