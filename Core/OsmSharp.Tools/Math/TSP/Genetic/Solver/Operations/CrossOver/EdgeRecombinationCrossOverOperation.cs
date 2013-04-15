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
    /// An edge recombination operation.
    /// </summary>
    public class EdgeRecombinationCrossOverOperation :
        ICrossOverOperation<List<int>, GeneticProblem, Fitness>
    {
        /// <summary>
        /// Returns the name of this operation.
        /// </summary>
        public string Name
        {
            get
            {
                return "ER";
            }
        }

        #region ICrossOverOperation<int,Problem> Members

        /// <summary>
        /// Applies this operation.
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
            HashSet<int> selected_cities = new HashSet<int>();
            List<int> non_selected_cities = new List<int>(parent1.Genomes);

            // build the edge list.
            Dictionary<int, HashSet<int>> edges_in_parents = new Dictionary<int, HashSet<int>>();
            for (int idx = 0; idx < parent1.Genomes.Count; idx++)
            {
                int city = parent1.Genomes[idx];

                HashSet<int> city_edges = null;
                if (!edges_in_parents.TryGetValue(city, out city_edges))
                {
                    city_edges = new HashSet<int>();
                    edges_in_parents.Add(city, city_edges);
                }

                int next_idx = idx + 1;
                if (next_idx >= parent1.Genomes.Count)
                {
                    next_idx = 0;
                }
                city_edges.Add(parent1.Genomes[next_idx]);
                
                city = parent2.Genomes[idx];
                if (!edges_in_parents.TryGetValue(city, out city_edges))
                {
                    city_edges = new HashSet<int>();
                    edges_in_parents.Add(city, city_edges);
                }
                next_idx = idx + 1;
                if (next_idx >= parent2.Genomes.Count)
                {
                    next_idx = 0;
                }
                city_edges.Add(parent2.Genomes[next_idx]);
            }

            // select the initial city.
            int selected_city = parent1.Genomes[0];
            if (edges_in_parents[parent2.Genomes[0]].Count < edges_in_parents[parent1.Genomes[0]].Count)
            {
                selected_city = parent2.Genomes[0];
            }
            new_individual.Add(selected_city);
            selected_cities.Add(selected_city);
            non_selected_cities.Remove(selected_city);

            while (non_selected_cities.Count > 0)
            {

                // select the next city.
                HashSet<int> edges_of_current = edges_in_parents[selected_city];
                edges_in_parents.Remove(selected_city);
                if (edges_of_current.Count > 0)
                {
                    int edge_count = parent1.Genomes.Count;
                    List<int> minimum_edges = new List<int>();

                    // build a list of cities with the minimum neighbour count.
                    foreach (int edge_of_current in edges_of_current)
                    {
                        HashSet<int> edges_of_edge = null;
                        if (edges_in_parents.TryGetValue(edge_of_current, out edges_of_edge))
                        {
                            int count = 0;
                            foreach (int edge_of_edge in edges_of_edge)
                            {
                                if (!selected_cities.Contains(edge_of_edge))
                                {
                                    count++;
                                }
                            }

                            if (count < edge_count)
                            {
                                minimum_edges.Clear();
                                minimum_edges.Add(edge_of_current);

                                edge_count = count;
                            }
                            else if (count == edge_count)
                            {
                                minimum_edges.Add(edge_of_current);
                            }
                        }
                    }

                    // randomly select one.
                    if (minimum_edges.Count != 0)
                    {
                        selected_city = minimum_edges[Tools.Math.Random.StaticRandomGenerator.Get().Generate(minimum_edges.Count)];
                    }
                    else
                    {
                        selected_city = non_selected_cities[Tools.Math.Random.StaticRandomGenerator.Get().Generate(non_selected_cities.Count)];
                    }
                }
                else
                { // randomly select a city.
                    selected_city = non_selected_cities[Tools.Math.Random.StaticRandomGenerator.Get().Generate(non_selected_cities.Count)];
                }

                new_individual.Add(selected_city);
                selected_cities.Add(selected_city);
                non_selected_cities.Remove(selected_city);
            }

            Individual individual = new Individual(new_individual);
            individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            return individual;
        }

        #endregion
    }
}
