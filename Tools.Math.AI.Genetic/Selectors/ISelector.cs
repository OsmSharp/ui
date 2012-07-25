using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Solvers;

namespace Tools.Math.AI.Genetic.Selectors
{
    /// <summary>
    /// Interface abstracting the implementation of selection of individuals.
    /// </summary>
    /// <typeparam name="GenomeType"></typeparam>
    public interface ISelector<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : IEquatable<GenomeType>
        where WeightType : IComparable
    {
        /// <summary>
        /// Selects an individual from the population to use for cross-over.
        /// </summary>
        /// <param name="population"></param>
        /// <returns></returns>
        Individual<GenomeType, ProblemType, WeightType> Select(
            Solver<GenomeType, ProblemType, WeightType> solver,
            Population<GenomeType, ProblemType, WeightType> population,
            HashSet<Individual<GenomeType, ProblemType, WeightType>> do_not_select_list);
    }
}