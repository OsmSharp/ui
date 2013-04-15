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
using OsmSharp.Tools;
using OsmSharp.Tools.Math.AI.Genetic.Fitness;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;

namespace OsmSharp.Tools.Math.AI.Genetic
{
    /// <summary>
    /// Class representing an individual in the population.
    /// </summary>
    /// <typeparam name="GenomeType"></typeparam>
    /// <typeparam name="ProblemType"></typeparam>
    /// <typeparam name="WeightType"></typeparam>
    public class Individual<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : class
        where WeightType : IComparable
    {
        /// <summary>
        /// Creates a new individual.
        /// </summary>
        /// <param name="genomes"></param>
        public Individual(GenomeType genomes)
        {
            _genomes = genomes;
        }

        /// <summary>
        /// Creates a new individual.
        /// </summary>
        /// <param name="fitness_calculated"></param>
        /// <param name="fitness"></param>
        protected Individual(bool fitness_calculated, 
            WeightType fitness)
        {
            _fitness_calculated = fitness_calculated;
            _fitness = fitness;
        }

        /// <summary>
        /// The description of this individual.
        /// </summary>
        public string Description { get; set; }

        #region Genomes

        /// <summary>
        /// Holds the list for genomes in this individual.
        /// </summary>
        private GenomeType _genomes;

        /// <summary>
        /// Returns the genomes of this individual.
        /// </summary>
        public GenomeType Genomes
        {
            get
            {
                return this._genomes;
            }
        }

        #endregion

        //#region Initialisation

        ///// <summary>
        ///// Initializes this individual.
        ///// </summary>
        //public void Initialize(List<GenomeType> genomes)
        //{
        //    // this is the only way to inject genomes into an individual.
        //    // only the individual factory should call this function.
        //    // the responsability of injecting correct genomes lies with the operators defined at problem level.
        //    // validation is forced at population level.
        //    _genomes = genomes;
        //}

        //#endregion

        #region Fitness

        /// <summary>
        /// Hold the fitness if calculated.
        /// </summary>
        private WeightType _fitness;

        /// <summary>
        /// Flag indication if the fitness was calculated or not.
        /// </summary>
        private bool _fitness_calculated;

        /// <summary>
        /// Calculates the fitness for this individual.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="op"></param>
        public void CalculateFitness(
            ProblemType problem,
            IFitnessCalculator<GenomeType, ProblemType, WeightType> op)
        {
            this.CalculateFitness(problem, op, true);
        }

        /// <summary>
        /// Calculates the fitness for this individual.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="problem"></param>
        /// <param name="validate"></param>
        public void CalculateFitness(
            ProblemType problem,
            IFitnessCalculator<GenomeType, ProblemType, WeightType> op,
            bool validate)
        {
            _fitness_calculated = true;
            _fitness = op.Fitness(problem, this, validate);
        }

        /// <summary>
        /// Calculates fitness.
        /// </summary>
        /// <param name="fitness"></param>
        public void CalculateFitness(
            WeightType fitness)
        {
            _fitness = fitness;
            _fitness_calculated = true;
        }

        /// <summary>
        /// Returns true if the fitness was already calculated.
        /// </summary>
        public bool FitnessCalculated
        {
            get
            {
                return _fitness_calculated;
            }
        }

        /// <summary>
        /// Returns the fitness.
        /// </summary>
        public WeightType Fitness
        {
            get
            {
                if (!_fitness_calculated)
                {
                    throw new InvalidOperationException();
                }
                return _fitness;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Operator returning true if the two given individuals are genetically identical.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static Boolean operator ==(
            Individual<GenomeType, ProblemType, WeightType> i1, Individual<GenomeType, ProblemType, WeightType> i2)
        {
            if (((object)i1 == null) && ((object)i2 == null))
            {
                return true;
            }
            else if (((object)i1 == null) || ((object)i2 == null))
            {
                return false;
            }
            return i1._genomes.Equals(i2._genomes);
        }

        /// <summary>
        /// Operator returning false if the individuals are genetically different.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static Boolean operator !=(Individual<GenomeType, ProblemType, WeightType> i1,
            Individual<GenomeType, ProblemType, WeightType> i2)
        {
            if (((object)i1 == null) && ((object)i2 == null))
            {
                return false;
            }
            else if (((object)i1 == null) || ((object)i2 == null))
            {
                return true;
            }
            return !i1._genomes.Equals(i2._genomes);
        }

        #region Overrides

        /// <summary>
        /// Returns true if the given individual is genetically identical to this one.
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        public override bool Equals(Object individual)
        {
            return this == (individual as Individual<GenomeType, ProblemType, WeightType>);
        }

        /// <summary>
        /// Returns a hashcode for this individual based on it's genomes only.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Genomes.GetHashCode();
        }

        /// <summary>
        /// Returns a description of this individual.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            //foreach (GenomeType genome in this.Genomes)
            //{
            builder.Append(this.Genomes.ToString());
                //builder.Append(Environment.NewLine);
            //}
            //return this.Description + Environment.NewLine + 
            //    this.Fitness.ToString() + Environment.NewLine +
            //    builder.ToString();
            return this.Description + Environment.NewLine +
                this.Fitness.ToString();
        }

        #endregion

        #endregion

        //#region Copy

        ///// <summary>
        ///// Creates an exact copy of this individual.
        ///// </summary>
        ///// <returns></returns>
        //public virtual Individual<GenomeType, ProblemType, WeightType> Copy()
        //{
        //    Individual<GenomeType, ProblemType, WeightType> new_individual
        //        = new Individual<GenomeType, ProblemType, WeightType>(this.FitnessCalculated,this.Fitness);
        //    new_individual.Description = this.Description;
        //    new_individual.Initialize(
        //        this.Genomes);

        //    return new_individual;
        //}

        //#endregion

        /// <summary>
        /// Validates.
        /// </summary>
        /// <param name="problem"></param>
        public virtual void Validate(ProblemType problem)
        {

        }
    }
}
