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
using OsmSharp.Tools.Math.AI.Genetic.Operations.Mutations;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.TSP.Genetic.Solver;
using OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Helpers;

namespace OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Mutation
{
    /// <summary>
    /// A detailed best placement operation.
    /// </summary>
    public class BestDetailedPlacementMutationOperation :
        IMutationOperation<List<int>, GeneticProblem, Fitness>
    {
        /// <summary>
        /// Creates a new best placement operation.
        /// </summary>
        public BestDetailedPlacementMutationOperation()
        {

        }

        /// <summary>
        /// Returns the name of this operation.
        /// </summary>
        public string Name
        {
            get
            {
                return "CI";
            }
        }

        #region IMutationOperation<Node,Problem> Members

        /// <summary>
        /// Applies this operation.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="mutating"></param>
        /// <returns></returns>
        public Individual<List<int>, GeneticProblem, Fitness> Mutate(
            Solver<List<int>, GeneticProblem, Fitness> solver,
            Individual<List<int>, GeneticProblem, Fitness> mutating)
        {
            if (solver.Random.Next(100) > 60)
            {
                return MutateByTakingPiece(solver, mutating);
            }
            else
            {
                return MutateByRePlacement(solver, mutating);
            }
        }

        /// <summary>
        /// Take a piece of the genome and re-do best placement.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="mutating"></param>
        /// <returns></returns>
        private Individual<List<int>, GeneticProblem, Fitness> MutateByTakingPiece(
            Solver<List<int>, GeneticProblem, Fitness> solver,
            Individual<List<int>, GeneticProblem, Fitness> mutating)
        {
            // take a random piece.
            int idx1 = 0;
            int idx2 = 0;
            while (idx2 - idx1 == 0)
            {
                idx1 = solver.Random.Next(mutating.Genomes.Count - 1) + 1;
                idx2 = solver.Random.Next(mutating.Genomes.Count - 1) + 1;
                if (idx1 > idx2)
                {
                    int temp = idx1;
                    idx1 = idx2;
                    idx2 = temp;
                }
            }

            // if the genome range is big take it from the best individual.
            Individual<List<int>, GeneticProblem, Fitness> source =
                (mutating as Individual<List<int>, GeneticProblem, Fitness>);
            Individual<List<int>, GeneticProblem, Fitness> target =
                (mutating as Individual<List<int>, GeneticProblem, Fitness>);

            List<int> source_piece = source.Genomes.GetRange(idx1, idx2 - idx1);
            List<int> new_genome = target.Genomes.GetRange(0, target.Genomes.Count);

            // insert the piece into the worst individual.
            // remove nodes in the source_piece.
            foreach (int source_node in source_piece)
            {
                new_genome.Remove(source_node);
            }
            
            // apply best placement algorithm to place the selected genomes.
            List<int> genome = new List<int>();
            BestPlacementHelper helper = BestPlacementHelper.Instance();
            helper.DoFast(
                solver.Problem,
                solver.FitnessCalculator as FitnessCalculator,
                new_genome,
                source_piece);

            Individual individual = new Individual(new_genome);
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        /// <summary>
        /// Re-places all the cities again to their own best place.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="mutating"></param>
        /// <returns></returns>
        private Individual<List<int>, GeneticProblem, Fitness> MutateByRePlacement(
            Solver<List<int>, GeneticProblem, Fitness> solver,
            Individual<List<int>, GeneticProblem, Fitness> mutating)
        {
            List<int> nodes_to_re_place = mutating.Genomes.ToList<int>();
            List<int> current_placement = mutating.Genomes.ToList<int>();

            foreach (int node_to_place in nodes_to_re_place)
            {
                // take the node out.
                current_placement.Remove(node_to_place);

                // place the node back in.
                BestPlacementHelper helper = BestPlacementHelper.Instance();
                helper.Do(
                    solver.Problem,
                    solver.FitnessCalculator as FitnessCalculator,
                    current_placement, 
                    node_to_place);
            }

            Individual individual = new Individual(current_placement);
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        #endregion
    }
}
