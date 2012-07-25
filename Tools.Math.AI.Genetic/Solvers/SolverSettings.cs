using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.AI.Genetic.Solvers
{
    /// <summary>
    /// Holds settings used by the solver class.
    /// </summary>
    public class SolverSettings
    {
        public SolverSettings(
            int stagnation_count,
            int population_size,
            int max_generations,
            double elitism_percentage,
            double cross_percentage,
            double mutation_percentage)
        {
            this.StagnationCount = stagnation_count;
            this.PopulationSize = population_size;
            this.MaxGeneration = max_generations;
            this.ElitismPercentage = elitism_percentage;
            this.CrossOverPercentage = cross_percentage;
            this.MutationPercentage = mutation_percentage;
        }

        /// <summary>
        /// Returns the stagnation count.
        /// </summary>
        public int StagnationCount { get; private set; }

        /// <summary>
        /// Returns the population size used.
        /// </summary>
        public int PopulationSize { get; private set; }

        /// <summary>
        /// Returns the maximum number of generations.
        /// </summary>
        public int MaxGeneration { get; private set; }

        /// <summary>
        /// Returns the elitism percentage used.
        /// </summary>
        public double ElitismPercentage { get; private set; }

        /// <summary>
        /// Returns the cross over percentage used.
        /// </summary>
        public double CrossOverPercentage { get; private set; }

        /// <summary>
        /// Returns the mutation percentage used.
        /// </summary>
        public double MutationPercentage { get; private set; }
    }
}