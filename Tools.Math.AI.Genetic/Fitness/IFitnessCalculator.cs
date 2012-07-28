using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Solvers;

namespace Tools.Math.AI.Genetic.Fitness
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
        /// <param name="individual"></param>
        /// <returns></returns>
        WeightType Fitness(
            ProblemType problem,
            Individual<GenomeType, ProblemType, WeightType> individual);

        /// <summary>
        /// Executes a fitness calculation.
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        WeightType Fitness(
            ProblemType problem,
            Individual<GenomeType, ProblemType, WeightType> individual, bool validate);

        /// <summary>
        /// Executes a fitness calculation.
        /// </summary>
        /// <param name="individual"></param>
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