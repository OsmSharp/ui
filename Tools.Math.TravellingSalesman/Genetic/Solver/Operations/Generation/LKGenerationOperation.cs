using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Generation;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.VRP.Core.Routes;
using Tools.Math.TSP.LK;

namespace Tools.Math.TSP.Genetic.Solver.Operations.Generation
{
    public class LKGenerationOperation :
        IGenerationOperation<int, GeneticProblem, Fitness>
    {
        public string Name
        {
            get
            {
                return "LK";
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
            ISolver lk_solver = new LinKernighanSolver();
            IRoute route = lk_solver.Solve(solver.Problem.BaseProblem);

            Individual individual = new Individual();
            individual.Initialize(new List<int>(route));
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        #endregion
    }
}
