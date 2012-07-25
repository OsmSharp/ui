using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Random;
using Tools.Math.AI.Genetic.Solvers;

namespace Tools.Math.AI.Genetic.Operations.Mutations
{
    public class CombinedMutation<GenomeType, ProblemType, WeightType> : IMutationOperation<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : IEquatable<GenomeType>
        where WeightType : IComparable
    {
        private IList<IMutationOperation<GenomeType, ProblemType, WeightType>> _operations;

        private IList<double> _probabilities;

        private IRandomGenerator _generator;

        public CombinedMutation(
            IRandomGenerator generator,
            IList<IMutationOperation<GenomeType, ProblemType, WeightType>> operations,
            IList<double> probabilities)
        {
            _operations = operations;
            _probabilities = probabilities;
            _generator = generator;
        }

        protected IList<double> Probabilities
        {
            get
            {
                return _probabilities;
            }
            set
            {
                _probabilities = value;
            }
        }

        public string Name
        {
            get
            {
                return "COM";
            }
        }

        public virtual Individual<GenomeType, ProblemType, WeightType> Mutate(
            Solver<GenomeType, ProblemType, WeightType> solver, Individual<GenomeType, ProblemType, WeightType> mutating)
        {
            double val = _generator.Generate(1.0);
            double prob = 0;
            for (int idx = 0; idx < _probabilities.Count; idx++)
            {
                prob = prob + _probabilities[idx];
                if (prob > val)
                {
                    return _operations[idx].Mutate(solver, mutating);
                }
            }
            throw new Exception("invalid probabilities!");
        }
    }
}
