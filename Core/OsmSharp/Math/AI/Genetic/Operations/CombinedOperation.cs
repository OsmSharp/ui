//// OsmSharp - OpenStreetMap tools & library.
//// Copyright (C) 2012 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Math.AI.Genetic.Operations.CrossOver;
//using OsmSharp.Math.AI.Genetic.Operations.Generation;
//using OsmSharp.Math.AI.Genetic.Operations.Mutations;
//using OsmSharp.Math.AI.Genetic.Solvers;
//using OsmSharp.Math.AI.Genetic.Operations.Mutations;
//using OsmSharp.Math.AI.Genetic.Operations.CrossOver;
//using OsmSharp.Math.AI.Genetic.Operations.Generation;
//using OsmSharp.Math.AI.Genetic.Solvers;

//namespace OsmSharp.Math.AI.Genetic.Operations
//{
//    /// <summary>
//    /// Class combining the genetic operators.
//    /// </summary>
//    /// <typeparam name="TGenome"></typeparam>
//    /// <typeparam name="TProblem"></typeparam>
//    /// <typeparam name="TWeight"></typeparam>
//    public class CombinedOperation<TGenome, TProblem, TWeight>
//        : IOperation<TGenome, TProblem, TWeight>
//        where TProblem : IProblem
//        where TGenome : class
//        where TWeight : IComparable
//    {
//        /// <summary>
//        /// Holds the mutation operator.
//        /// </summary>
//        private readonly IMutationOperation<TGenome, TProblem, TWeight> _mutationOperator;

//        /// <summary>
//        /// Holds the cross over operator.
//        /// </summary>
//        private readonly ICrossOverOperation<TGenome, TProblem, TWeight> _crossOverOperator;

//        /// <summary>
//        /// Holds the generation operator.
//        /// </summary>
//        private readonly IGenerationOperation<TGenome, TProblem, TWeight> _generationOperation;

//        /// <summary>
//        /// Creates a new combined operation.
//        /// </summary>
//        /// <param name="mutationOperator"></param>
//        /// <param name="crossOverOperator"></param>
//        /// <param name="generator"></param>
//        public CombinedOperation(
//            IMutationOperation<TGenome, TProblem, TWeight> mutationOperator,
//            ICrossOverOperation<TGenome, TProblem, TWeight> crossOverOperator,
//            IGenerationOperation<TGenome, TProblem, TWeight> generator)
//        {
//            _mutationOperator = mutationOperator;
//            _crossOverOperator = crossOverOperator;
//            _generationOperation = generator;
//        }

//        /// <summary>
//        /// Returns the name of this operation.
//        /// </summary>
//        public string Name
//        {
//            get
//            {
//                return "COM";
//            }
//        }
            
//        #region ICrossOverOperation<GenomeType> Members

//        /// <summary>
//        /// Executes the cross over of two individuals.
//        /// </summary>
//        /// <param name="solver"></param>
//        /// <param name="parent1"></param>
//        /// <param name="parent2"></param>
//        /// <returns></returns>
//        public Individual<TGenome, TProblem, TWeight> CrossOver(
//            Solver<TGenome, TProblem, TWeight> solver,
//            Individual<TGenome, TProblem, TWeight> parent1,
//            Individual<TGenome, TProblem, TWeight> parent2)
//        {
//            return _crossOverOperator.CrossOver(
//                solver,
//                parent1, parent2);
//        }

//        #endregion

//        #region IMutationOperation<GenomeType> Members

//        /// <summary>
//        /// Executes the mutation of an individual.
//        /// </summary>
//        /// <param name="solver"></param>
//        /// <param name="mutating"></param>
//        /// <returns></returns>
//        public Individual<TGenome, TProblem, TWeight> Mutate(
//            Solver<TGenome, TProblem, TWeight> solver,
//            Individual<TGenome, TProblem, TWeight> mutating)
//        {
//            return _mutationOperator.Mutate(
//                solver,
//                mutating);
//        }

//        #endregion

//        #region IGenerationOperation<GenomeType> Members

//        /// <summary>
//        /// Executes the generation of a new individual.
//        /// </summary>
//        /// <returns></returns>
//        public Individual<TGenome, TProblem, TWeight> Generate(
//            Solver<TGenome, TProblem, TWeight> solver)
//        {
//            return _generationOperation.Generate(solver);
//        }

//        #endregion
//    }
//}
