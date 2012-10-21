// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
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