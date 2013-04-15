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
using OsmSharp.Tools.Math.AI.Genetic.Operations.Mutations;
using OsmSharp.Tools.Math.AI.Genetic.Operations.CrossOver;
using OsmSharp.Tools.Math.AI.Genetic.Operations.Generation;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;

namespace OsmSharp.Tools.Math.AI.Genetic.Operations
{
    /// <summary>
    /// Class combining the genetic operators.
    /// </summary>
    /// <typeparam name="GenomeType"></typeparam>
    /// <typeparam name="ProblemType"></typeparam>
    /// <typeparam name="WeightType"></typeparam>
    public class CombinedOperation<GenomeType, ProblemType, WeightType>
        : IOperation<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : class
        where WeightType : IComparable
    {
        /// <summary>
        /// Holds the mutation operator.
        /// </summary>
        private IMutationOperation<GenomeType, ProblemType, WeightType> _mutation_operator;

        /// <summary>
        /// Holds the cross over operator.
        /// </summary>
        private ICrossOverOperation<GenomeType, ProblemType, WeightType> _cross_over_operator;

        /// <summary>
        /// Holds the generation operator.
        /// </summary>
        private IGenerationOperation<GenomeType, ProblemType, WeightType> _generation_operation;

        /// <summary>
        /// Creates a new combined operation.
        /// </summary>
        /// <param name="mutation_operator"></param>
        /// <param name="cross_over_operator"></param>
        /// <param name="generator"></param>
        public CombinedOperation(
            IMutationOperation<GenomeType, ProblemType, WeightType> mutation_operator,
            ICrossOverOperation<GenomeType, ProblemType, WeightType> cross_over_operator,
            IGenerationOperation<GenomeType, ProblemType, WeightType> generator)
        {
            _mutation_operator = mutation_operator;
            _cross_over_operator = cross_over_operator;
            _generation_operation = generator;
        }

        /// <summary>
        /// Returns the name of this operation.
        /// </summary>
        public string Name
        {
            get
            {
                return "COM";
            }
        }
            
        #region ICrossOverOperation<GenomeType> Members

        /// <summary>
        /// Executes the cross over of two individuals.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="parent1"></param>
        /// <param name="parent2"></param>
        /// <returns></returns>
        public Individual<GenomeType, ProblemType, WeightType> CrossOver(
            Solver<GenomeType, ProblemType, WeightType> solver,
            Individual<GenomeType, ProblemType, WeightType> parent1,
            Individual<GenomeType, ProblemType, WeightType> parent2)
        {
            return _cross_over_operator.CrossOver(
                solver,
                parent1, parent2);
        }

        #endregion

        #region IMutationOperation<GenomeType> Members

        /// <summary>
        /// Executes the mutation of an individual.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="mutating"></param>
        /// <returns></returns>
        public Individual<GenomeType, ProblemType, WeightType> Mutate(
            Solver<GenomeType, ProblemType, WeightType> solver,
            Individual<GenomeType, ProblemType, WeightType> mutating)
        {
            return _mutation_operator.Mutate(
                solver,
                mutating);
        }

        #endregion

        #region IGenerationOperation<GenomeType> Members

        /// <summary>
        /// Executes the generation of a new individual.
        /// </summary>
        /// <returns></returns>
        public Individual<GenomeType, ProblemType, WeightType> Generate(
            Solver<GenomeType, ProblemType, WeightType> solver)
        {
            return _generation_operation.Generate(solver);
        }

        #endregion
    }
}
