using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Solvers;

namespace Tools.Math.AI.Genetic.Operations.CrossOver
{
    /// <summary>
    /// Interface abstracting the implementation of crossing over individuals.
    /// </summary>
    /// <typeparam name="GenomeType"></typeparam>
    public interface ICrossOverOperation<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : class
        where WeightType : IComparable
    {
        /// <summary>
        /// Returns the name.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Executes a cross-over using the two given parents.
        /// </summary>
        /// <param name="parent1"></param>
        /// <param name="parent2"></param>
        /// <returns></returns>
        Individual<GenomeType, ProblemType, WeightType> CrossOver(
            Solver<GenomeType, ProblemType, WeightType> solver,
            Individual<GenomeType, ProblemType, WeightType> parent1,
            Individual<GenomeType, ProblemType, WeightType> parent2);
    }
}
