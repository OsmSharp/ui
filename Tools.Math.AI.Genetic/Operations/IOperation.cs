using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.CrossOver;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic.Operations.Generation;

namespace Tools.Math.AI.Genetic.Operations
{
    /// <summary>
    /// Interface abstracting the usage of mutation and cross over operations.
    /// </summary>
    public interface IOperation<GenomeType, ProblemType, WeightType> :
        ICrossOverOperation<GenomeType, ProblemType, WeightType>,
        IMutationOperation<GenomeType, ProblemType, WeightType>,
        IGenerationOperation<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : IEquatable<GenomeType>
        where WeightType : IComparable
    {

    }
}
