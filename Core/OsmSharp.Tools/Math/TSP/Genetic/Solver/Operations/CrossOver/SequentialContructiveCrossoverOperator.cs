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
using OsmSharp.Tools.Math.AI.Genetic.Operations.CrossOver;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Collections;

namespace OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.CrossOver
{
    /// <summary>
    /// Implements a version of the sequential constructive crossover operator.
    /// </summary>
    public class SequentialContructiveCrossoverOperator :
        ICrossOverOperation<List<int>, GeneticProblem, Fitness>
    {
        /// <summary>
        /// Returns the name of this operation.
        /// </summary>
        public string Name
        {
            get
            {
                return "SQC";
            }
        }

        #region ICrossOverOperation<int,Problem> Members

        /// <summary>
        /// Crosses over the two indivduals using sequantial contructive crossover.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="parent1"></param>
        /// <param name="parent2"></param>
        /// <returns></returns>
        public Individual<List<int>, GeneticProblem, Fitness> CrossOver(
            Solver<List<int>, GeneticProblem, Fitness> solver,
            Individual<List<int>, GeneticProblem, Fitness> parent1,
            Individual<List<int>, GeneticProblem, Fitness> parent2)
        {
            List<int> new_individual = new List<int>();
            HashSet<int> selected_nodes = new HashSet<int>();
            List<int> non_selected_nodes = new List<int>(parent1.Genomes);

            // build the edge list.
            int edges = 1;
            if (solver.Problem.First != solver.Problem.Last)
            {
                edges = 2;
            }
            int[] edges_in_parent1 = new int[parent1.Genomes.Count + edges];
            int[] edges_in_parent2 = new int[parent1.Genomes.Count + edges];
            edges_in_parent1[solver.Problem.First] = -1;
            edges_in_parent1[solver.Problem.Last] = -1;
            edges_in_parent2[solver.Problem.First] = -1;
            edges_in_parent2[solver.Problem.Last] = -1;
            for (int idx = 0; idx < parent1.Genomes.Count - 1; idx++)
            {
                edges_in_parent1[parent1.Genomes[idx]] = parent1.Genomes[idx + 1];
                edges_in_parent2[parent2.Genomes[idx]] = parent2.Genomes[idx + 1];
            }
            edges_in_parent1[parent1.Genomes[parent1.Genomes.Count - 1]] = -1;
            edges_in_parent2[parent2.Genomes[parent2.Genomes.Count - 1]] = -1;

            // start with the first node.
            int selected_node = parent1.Genomes[0];
            new_individual.Add(selected_node);
            selected_nodes.Add(selected_node);
            non_selected_nodes.Remove(selected_node);

            // find the next legitimate node in both.
            double total_weight = solver.Problem.Weight(solver.Problem.First, selected_node);
            while (non_selected_nodes.Count > 0)
            {
                int node_parent1 = edges_in_parent1[selected_node];
                bool node_parent1_found = false;
                int node_parent2 = edges_in_parent2[selected_node];
                bool node_parent2_found = false;

                // find a node for parent1 if no legitimate one was found.
                if (node_parent1 >= 0)
                {
                    node_parent1_found = (!selected_nodes.Contains(node_parent1));
                    edges_in_parent1[selected_node] = -1;
                }

                // find a node for parent2 if no legitimate one was found.
                if (node_parent2 >= 0)
                {
                    node_parent2_found = (!selected_nodes.Contains(node_parent2));
                    edges_in_parent2[selected_node] = -1;
                }

                // find a node for parent1 if no legitimate one was found.
                if (!node_parent1_found)
                {
                    // Select the first node just like that!
                    node_parent1 = non_selected_nodes[0];
                }

                // find a node for parent2 if no legitimate one was found.
                if (!node_parent2_found)
                {
                    // Select the first node just like that!
                    node_parent2 = non_selected_nodes[0];
                }

                // select one of two
                if (node_parent1 == node_parent2)
                {
                    total_weight = total_weight + solver.Problem.Weight(selected_node, node_parent2);

                    selected_node = node_parent1;
                }
                else
                {
                    double weight1 = solver.Problem.Weight(selected_node, node_parent1);
                    double weight2 = solver.Problem.Weight(selected_node, node_parent2);
                    if (weight1 < weight2)
                    {
                        selected_node = node_parent1;

                        total_weight = total_weight + weight1;
                    }
                    else
                    {
                        selected_node = node_parent2;

                        total_weight = total_weight + weight2;
                    }
                }
                total_weight = total_weight + solver.Problem.Weight(selected_node, solver.Problem.Last);

                // update data structures.
                new_individual.Add(selected_node);
                selected_nodes.Add(selected_node);
                non_selected_nodes.Remove(selected_node);

                //if (non_selected_nodes.Count + selected_nodes.Count != parent1.Genomes.Count)
                //{
                //    Console.WriteLine("HELP");
                //}
            }

            Individual individual = new Individual(new_individual);
            //individual.CalculateFitness(total_weight);
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            //if (individual.Count > parent1.Genomes.Count)
            //{
            //    Console.WriteLine("HELP");
            //}
            return individual;
        }

        #endregion
    }
}
