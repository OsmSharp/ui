using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Solvers;

namespace Tools.Math.AI.Genetic.Operations.Generation
{
    /// <summary>
    /// Interface abstracting the implementation of the generation of new individuals.
    /// </summary>
    /// <typeparam name="GenomeType"></typeparam>
    public interface IGenerationOperation<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : IEquatable<GenomeType>
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
        /// Executes the generation of a new individual.
        /// </summary>
        /// <returns></returns>
        Individual<GenomeType, ProblemType, WeightType> Generate(
            Solver<GenomeType, ProblemType, WeightType> solver);
    }
}
