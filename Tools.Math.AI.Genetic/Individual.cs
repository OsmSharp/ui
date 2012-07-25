using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Core;
using Tools.Math.AI.Genetic.Fitness;
using Tools.Math.AI.Genetic.Solvers;

namespace Tools.Math.AI.Genetic
{
    /// <summary>
    /// Class representing an individual in the population.
    /// </summary>
    /// <typeparam name="GenomeType"></typeparam>
    public class Individual<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : IEquatable<GenomeType>
        where WeightType : IComparable
    {
        public Individual()
        {

        }

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
        private List<GenomeType> _genomes;

        /// <summary>
        /// Returns the genomes of this individual.
        /// </summary>
        public List<GenomeType> Genomes
        {
            get
            {
                return this._genomes;
            }
        }

        #endregion

        #region Initialisation

        /// <summary>
        /// Initializes this individual.
        /// </summary>
        public void Initialize(List<GenomeType> genomes)
        {
            // this is the only way to inject genomes into an individual.
            // only the individual factory should call this function.
            // the responsability of injecting correct genomes lies with the operators defined at problem level.
            // validation is forced at population level.
            _genomes = genomes;
        }

        #endregion

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
        public void CalculateFitness(
            ProblemType problem,
            IFitnessCalculator<GenomeType, ProblemType, WeightType> op,
            bool validate)
        {
            _fitness_calculated = true;
            _fitness = op.Fitness(problem, this, validate);
        }

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
            return i1._genomes.EqualValues(i2._genomes);
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
            return !i1._genomes.EqualValues(i2._genomes);
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
            foreach (GenomeType genome in this.Genomes)
            {
                builder.Append(genome.ToString());
                builder.Append(Environment.NewLine);
            }
            return this.Description + Environment.NewLine + 
                this.Fitness.ToString() + Environment.NewLine + 
                builder.ToString();
        }

        #endregion

        #endregion

        #region Copy

        /// <summary>
        /// Creates an exact copy of this individual.
        /// </summary>
        /// <returns></returns>
        public virtual Individual<GenomeType, ProblemType, WeightType> Copy()
        {
            Individual<GenomeType, ProblemType, WeightType> new_individual
                = new Individual<GenomeType, ProblemType, WeightType>(this.FitnessCalculated,this.Fitness);
            new_individual.Description = this.Description;
            new_individual.Initialize(new List<GenomeType>(
                this.Genomes));

            return new_individual;
        }

        #endregion

        public virtual void Validate(ProblemType problem)
        {

        }
    }
}
