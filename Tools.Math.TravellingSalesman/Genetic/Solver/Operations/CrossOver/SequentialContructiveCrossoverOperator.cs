using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.CrossOver;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;

namespace Tools.Math.TSP.Genetic.Solver.Operations.CrossOver
{
    /// <summary>
    /// Implements a version of the sequential constructive crossover operator.
    /// </summary>
    public class SequentialContructiveCrossoverOperator :
        ICrossOverOperation<int, GeneticProblem, Fitness>
    {
        public string Name
        {
            get
            {
                return "SQC";
            }
        }

        #region ICrossOverOperation<int,Problem> Members

        public Individual<int, GeneticProblem, Fitness> CrossOver(
            Solver<int, GeneticProblem, Fitness> solver,
            Individual<int, GeneticProblem, Fitness> parent1,
            Individual<int, GeneticProblem, Fitness> parent2)
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
            float total_weight = solver.Problem.Weight(solver.Problem.First, selected_node);
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
                    float weight1 = solver.Problem.Weight(selected_node, node_parent1);
                    float weight2 = solver.Problem.Weight(selected_node, node_parent2);
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

            Individual individual = new Individual();
            individual.Initialize(new_individual);
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
