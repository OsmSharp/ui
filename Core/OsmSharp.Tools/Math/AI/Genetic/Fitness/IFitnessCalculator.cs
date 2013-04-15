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
using OsmSharp.Tools.Math.AI.Genetic.Solvers;

namespace OsmSharp.Tools.Math.AI.Genetic.Fitness
{
    /// <summary>
    /// Interface abstracting the fitness calculation.
    /// </summary>
    public interface IFitnessCalculator<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : class
        where WeightType : IComparable
    {
        /// <summary>
        /// Returns the margin of error in the fitness calculation.
        /// </summary>
        double Epsilon
        {
            get;
        }

        /// <summary>
        /// Executes a fitness calculation.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="individual"></param>
        /// <returns></returns>
        WeightType Fitness(
            ProblemType problem,
            Individual<GenomeType, ProblemType, WeightType> individual);

        /// <summary>
        /// Executes a fitness calculation.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="individual"></param>
        /// <param name="validate"></param>
        /// <returns></returns>
        WeightType Fitness(
            ProblemType problem,
            Individual<GenomeType, ProblemType, WeightType> individual, bool validate);

        /// <summary>
        /// Executes a fitness calculation.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="genomes"></param>
        /// <returns></returns>
        WeightType Fitness(
            ProblemType problem,
            GenomeType genomes);

        /// <summary>
        /// Calculates the average fitness.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="population"></param>
        /// <returns></returns>
        WeightType AverageFitness(
            ProblemType problem,
            IEnumerable<Individual<GenomeType, ProblemType, WeightType>> population);

        #region Partial Calculations

        ///// <summary>
        ///// Executes a fitness calculation given only two parts of the genome.
        ///// </summary>
        ///// <param name="problem"></param>
        ///// <param name="first"></param>
        ///// <param name="second"></param>
        ///// <returns></returns>
        //WeightType FitnessPart(
        //    ProblemType problem,
        //    GenomeType first,
        //    GenomeType second);

        ///// <summary>
        ///// Executes a fitness calculation only on a part of the genome.
        ///// 
        ///// Mainly used to prevent duplicate calculations.
        ///// </summary>
        ///// <param name="individual"></param>
        ///// <returns></returns>
        //WeightType FitnessFirstPart(
        //    ProblemType problem,
        //    IList<GenomeType> genome_part);

        ///// <summary>
        ///// Executes a fitness calculation only on a part of the genome.
        ///// 
        ///// Mainly used to prevent duplicate calculations.
        ///// </summary>
        ///// <param name="individual"></param>
        ///// <returns></returns>
        //WeightType FitnessLastPart(
        //    ProblemType problem,
        //    IList<GenomeType> genome_part);

        ///// <summary>
        ///// Executes a fitness calculation only on a part of the genome.
        ///// 
        ///// Mainly used to prevent duplicate calculations.
        ///// </summary>
        ///// <param name="individual"></param>
        ///// <returns></returns>
        //WeightType FitnessPart(
        //    ProblemType problem,
        //    IList<GenomeType> genome_part);

        #endregion
    }
}