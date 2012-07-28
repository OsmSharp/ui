using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Solvers;

namespace Tools.Math.AI.Genetic.Operations.Mutations
{
    /// <summary>
    /// Interface abstracting the usage of a specific mutation implementation.
    /// </summary>
    /// <typeparam name="GenomeType"></typeparam>
    public interface IMutationOperation<GenomeType, ProblemType, WeightType>
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
        /// Executes a mutation operation.
        /// </summary>
        /// <param name="mutating"></param>
        /// <returns></returns>
        Individual<GenomeType, ProblemType, WeightType> Mutate(
            Solver<GenomeType, ProblemType, WeightType> solver,
            Individual<GenomeType, ProblemType, WeightType> mutating);
    }
}
