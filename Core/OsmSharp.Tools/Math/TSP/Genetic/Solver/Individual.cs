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
using OsmSharp.Tools.Math.AI.Genetic;

namespace OsmSharp.Tools.Math.TSP.Genetic.Solver
{
    /// <summary>
    /// Represents an individual.
    /// </summary>
    public class Individual : Individual<List<int>, GeneticProblem, Fitness>
    {
        /// <summary>
        /// Creates an individual.
        /// </summary>
        /// <param name="genomes"></param>
        public Individual(List<int> genomes)
            :base(genomes)
        {

        }
        
        /// <summary>
        /// Creates an individual.
        /// </summary>
        /// <param name="fitness_calculated"></param>
        /// <param name="fitness"></param>
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
