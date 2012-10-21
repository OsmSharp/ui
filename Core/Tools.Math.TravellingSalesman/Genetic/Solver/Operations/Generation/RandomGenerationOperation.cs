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
using Tools.Math.AI.Genetic.Operations.Generation;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.TSP.Genetic.Solver;
using Tools.Math.Graph;

namespace Tools.Math.TSP.Genetic.Solver.Operations.Generation
{
    /// <summary>
    /// Generates a random individual.
    /// </summary>
    /// <typeparam name="GenomeType"></typeparam>
    public class RandomGenerationOperation :
        IGenerationOperation<List<int>, GeneticProblem, Fitness>
    {
        public string Name
        {
            get
            {
                return "RAN";
            }
        }

        #region IGenerationOperation<GenomeType> Members

        /// <summary>
        /// Generates a random individual.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        public Individual<List<int>, GeneticProblem, Fitness> Generate(
            Solver<List<int>, GeneticProblem, Fitness> solver)
        {
            List<int> genome = new List<int>();
            List<int> node_indexes = new List<int>();
            for (int i = 0; i < solver.Problem.Along.Count; i++)
            {
                node_indexes.Add(i);
            }
            for (int i = 0; i < solver.Problem.Along.Count; i++)
            {
                // get the random idx.
                int idx = solver.Random.Next(node_indexes.Count);

                // get the node at the given idx.
                genome.Add(solver.Problem.Along[node_indexes[idx]]);
                node_indexes.RemoveAt(idx); // remove the idx.
            }

            Individual individual = new Individual(genome);
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        #endregion
    }
}