using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic;
using Tools.Math.Graph;

namespace Tools.Math.TSP.Genetic.Solver
{
    public class Individual : Individual<List<int>, GeneticProblem, Fitness>
    {
        public Individual(List<int> genomes)
            :base(genomes)
        {

        }

        protected Individual(bool fitness_calculated, Fitness fitness)
            : base(fitness_calculated, fitness)
        {

        }

        /// <summary>
        /// Returns the number of genomes in this individual.
        /// </summary>
        public int Count
        {
            get
            {
                return this.Genomes.Count;
            }
        }

        /// <summary>
        /// Returns a description of this individual.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.FitnessCalculated)
            {
                return string.Format("Individual: #{0} [{1}]",
                    this.Count,
                    this.Fitness.ToString());
            }
            else
            {
                return string.Format("Individual: #{0} [Not Calculated!]",
                    this.Count);
            }
        }

        /// <summary>
        /// Validates this individual.
        /// </summary>
        /// <param name="problem"></param>
        public override void Validate(GeneticProblem problem)
        {
            if (problem.Along.Count != this.Count)
            {
                throw new Exception("Individual is not valid!");
            }

            // TODO: check for doubles.
        }
    }
}
