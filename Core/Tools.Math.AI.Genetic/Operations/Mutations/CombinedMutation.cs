// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.Random;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;

namespace OsmSharp.Tools.Math.AI.Genetic.Operations.Mutations
{
    public class CombinedMutation<GenomeType, ProblemType, WeightType> : IMutationOperation<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : class
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
