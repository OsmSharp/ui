using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Generation;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.TravellingSalesman.LocalSearch.HillClimbing3Opt;
using Tools.Math.VRP.Core.Routes;

namespace Tools.Math.TSP.Genetic.Solver.Operations.Generation
{
    public class _3OptGenerationOperation :
        IGenerationOperation<int, GeneticProblem, Fitness>
    {
        public string Name
        {
            get
            {
                return "3OPT";
            }
        }

        #region IGenerationOperation<GenomeType> Members

        /// <summary>
        /// Generates a random individual.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        public Individual<int, GeneticProblem, Fitness> Generate(
            Solver<int, GeneticProblem, Fitness> solver)
        {
            ISolver lk_solver = new HillClimbing3OptSolver(true, true);
            IRoute route = lk_solver.Solve(solver.Problem.BaseProblem);

            List<int> new_genome = new List<int>();
            bool first_found = false;
            foreach (int customer in route)
            {
                if (first_found)
                {
                    new_genome.Add(customer);
                }
                if (customer == 0)
                {
                    first_found = true;
                }
            }
            foreach (int customer in route)
            {
                if (customer == 0)
                {
                    break;
                }
                new_genome.Add(customer);
            }

            Individual individual = new Individual();
            individual.Initialize(new_genome);
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        #endregion
    }
}
