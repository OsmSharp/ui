using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Generation;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.VRP.Core.Routes.ASymmetric;
using Tools.Math.VRP.Core.Routes;
using Tools.Math.VRP.Core.BestPlacement;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.Genetic.Generation
{
    /// <summary>
    /// Generates individuals based on another solver.
    /// </summary>
    internal class SolverGenerationOperation :
        IGenerationOperation<MaxTimeSolution, MaxTimeProblem, Fitness>
    {
        /// <summary>
        /// Holds the solver.
        /// </summary>
        private IMaxTimeSolver _solver;

        /// <summary>
        /// Creates a new solver generation operation.
        /// </summary>
        /// <param name="solver"></param>
        public SolverGenerationOperation(IMaxTimeSolver solver)
        {
            _solver = solver;
        }

        /// <summary>
        /// Returns the name of this operation.
        /// </summary>
        public string Name
        {
            get
            {
                return "SOL";
            }
        }

        /// <summary>
        /// Generates individuals based on a random first customer for each route.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        public Individual<MaxTimeSolution, MaxTimeProblem, Fitness> Generate(
            Solver<MaxTimeSolution, MaxTimeProblem, Fitness> solver)
        {
            return new Individual<MaxTimeSolution,MaxTimeProblem,Fitness>(
                _solver.Solve(solver.Problem));
        }
    }
}
