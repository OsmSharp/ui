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
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.TSP.Genetic.Solver.Operations.Helpers;
using Tools.Math.TSP.Genetic.Solver;
using Tools.Math.Graph;

namespace Tools.Math.TSP.Genetic.Solver.Operations.Mutation
{
    public class BestPlacementMutationOperation :
        IMutationOperation<List<int>, GeneticProblem, Fitness>
    {        
        public BestPlacementMutationOperation()
        {

        }

        public string Name
        {
            get
            {
                return "AI";
            }
        }

        #region IMutationOperation<int,Problem> Members

        public Individual<List<int>, GeneticProblem, Fitness> Mutate(
            Solver<List<int>, GeneticProblem, Fitness> solver,
            Individual<List<int>, GeneticProblem, Fitness> mutating)
        {
            // take a random piece.
            int idx = solver.Random.Next(mutating.Genomes.Count);

            List<int> new_genome = new List<int>(mutating.Genomes);
            int customer = new_genome[idx];
            new_genome.RemoveAt(idx);

            // apply best placement algorithm to place the selected genomes.
            BestPlacementHelper helper = BestPlacementHelper.Instance();
            helper.Do(
                solver.Problem,
                solver.FitnessCalculator as FitnessCalculator,
                new_genome,
                customer);

            Individual individual = new Individual(new_genome);
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        #endregion
    }
}
