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
using Tools.Math.AI.Genetic.Operations.CrossOver;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;

namespace Tools.Math.TSP.Genetic.Solver.Operations.CrossOver
{
    public class EdgeAssemblyCrossoverNaive :
        ICrossOverOperation<List<int>, GeneticProblem, Fitness>
    {
        private int _max_offspring;

        private EdgeAssemblyCrossoverSelectionStrategyEnum _strategy;

        private bool _nn;

        public EdgeAssemblyCrossoverNaive(int max_offspring,
            EdgeAssemblyCrossoverSelectionStrategyEnum strategy,
            bool nn)
        {
            _max_offspring = max_offspring;
            _strategy = strategy;
            _nn = nn;
        }

        public string Name
        {
            get
            {
                if (_strategy == EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom)
                {
                    if (_nn)
                    {
                        return string.Format("NEAX_(SR{0}_NN)", _max_offspring);
                    }
                    return string.Format("NEAX_(SR{0})", _max_offspring);
                }
                else
                {
                    if (_nn)
                    {
                        return string.Format("NEAX_(MR{0}_NN)", _max_offspring);
                    }
                    return string.Format("NEAX_(MR{0})", _max_offspring);
                }
            }
        }

        public enum EdgeAssemblyCrossoverSelectionStrategyEnum
        {
            SingleRandom, // EAX-1AB
            MultipleRandom
        }



        #region ICrossOverOperation<int,Problem> Members

        public Individual<List<int>, GeneticProblem, Fitness> CrossOver(
            Solver<List<int>, GeneticProblem, Fitness> solver,
            Individual<List<int>, GeneticProblem, Fitness> parent1,
            Individual<List<int>, GeneticProblem, Fitness> parent2)
        {
            Tools.Math.TSP.Problems.IProblem tsp_problem = solver.Problem.BaseProblem;

            // first create E_a
            int[] e_a = new int[parent1.Genomes.Count + 1];
            e_a[0] = parent1.Genomes[0];
            for (int idx = 0; idx < parent1.Genomes.Count - 1; idx++)
            {
                int from = parent1.Genomes[idx];
                int to = parent1.Genomes[idx + 1];

                e_a[from] = to;
            }
            e_a[parent1.Genomes[parent1.Genomes.Count - 1]] = 0;

            // create E_b
            int[] e_b = new int[parent2.Genomes.Count + 1];
            e_b[0] = parent2.Genomes[0];
            for (int idx = 0; idx < parent2.Genomes.Count - 1; idx++)
            {
                int from = parent2.Genomes[idx];
                int to = parent2.Genomes[idx + 1];

                e_b[from] = to;
            }
            e_b[parent2.Genomes[parent2.Genomes.Count - 1]] = 0;

            // create G_ab.
            Dictionary<int, int> g_a =
                new Dictionary<int, int>();
            Dictionary<int, int> g_b_opposite =
                new Dictionary<int, int>();
            for (int idx = 0; idx < e_b.Length; idx++)
            {
                if (e_a[idx] != e_b[idx])
                {
                    g_a.Add(idx, e_a[idx]);
                    g_b_opposite.Add(e_b[idx], idx);
                }
            }

            List<Dictionary<int, KeyValuePair<int, int>>> cycles =
                new List<Dictionary<int, KeyValuePair<int, int>>>();
            while (g_a.Count > 0)
            {
                int first = g_a.Keys.First<int>();
                KeyValuePair<int, int> next_pair;
                Dictionary<int, KeyValuePair<int, int>> cycle =
                    new Dictionary<int, KeyValuePair<int, int>>();
                while (!cycle.ContainsKey(first))
                {
                    next_pair = new KeyValuePair<int, int>(g_a[first], g_b_opposite[g_a[first]]);
                    // remove.
                    g_a.Remove(first);
                    g_b_opposite.Remove(next_pair.Key);


                    cycle.Add(first, next_pair);

                    // get all the nexts ones.
                    first = next_pair.Value;
                }
                cycles.Add(cycle);
            }

            int generated = 0;
            Individual best = null;
            while (generated < _max_offspring)
            {
                // select some random cycles.
                List<Dictionary<int, KeyValuePair<int, int>>> selected_cycles =
                    this.SelectCycles(cycles);

                // take e_a and remove all edges that are in the selected cycles and replace them by the eges
                // from e_b in the cycles.
                foreach (Dictionary<int, KeyValuePair<int, int>> cycle in selected_cycles)
                {
                    foreach (KeyValuePair<int, KeyValuePair<int, int>> pair in cycle)
                    {
                        e_a[pair.Value.Value] = pair.Value.Key;
                    }
                }

                // construct all subtours.
                List<Dictionary<int, KeyValuePair<int, double>>> subtours =
                    new List<Dictionary<int, KeyValuePair<int, double>>>();
                List<int> e_a_list = new List<int>(e_a);
                int start_idx = 0;
                while (start_idx < e_a_list.Count)
                {
                    Dictionary<int, KeyValuePair<int, double>> current_tour = new Dictionary<int, KeyValuePair<int, double>>();

                    int from = start_idx;
                    int to = e_a_list[from];
                    while (to >= 0)
                    {
                        e_a_list[from] = -1;

                        current_tour.Add(from, new KeyValuePair<int, double>(to, tsp_problem.WeightMatrix[from][to]));

                        from = to;

                        to = e_a_list[from];
                    }

                    for (start_idx = start_idx + 1; start_idx < e_a_list.Count; start_idx++)
                    {
                        if (e_a_list[start_idx] >= 0)
                        {
                            break;
                        }
                    }
                    subtours.Add(current_tour);
                }
                while (subtours.Count > 1)
                {
                    int size = e_a.Length;
                    Dictionary<int, KeyValuePair<int, double>> current_tour = null;
                    foreach (Dictionary<int, KeyValuePair<int, double>> tour in subtours)
                    {
                        if (current_tour == null ||
                            tour.Count < current_tour.Count)
                        {
                            current_tour = tour;
                        }
                    }

                    // merge two tours.
                    Dictionary<int, KeyValuePair<int, double>> target_tour = null;
                    double weight = double.MaxValue;
                    KeyValuePair<int, KeyValuePair<int, double>> from = new KeyValuePair<int, KeyValuePair<int, double>>();
                    KeyValuePair<int, KeyValuePair<int, double>> to = new KeyValuePair<int, KeyValuePair<int, double>>();

                    // first try nn approach.
                    if (_nn)
                    {
                        foreach (KeyValuePair<int, KeyValuePair<int, double>> from_pair in current_tour)
                        {
                            foreach (int nn in tsp_problem.Get10NearestNeighbours(from_pair.Key))
                            {
                                if (!current_tour.ContainsKey(nn))
                                {
                                    foreach (Dictionary<int, KeyValuePair<int, double>> target in subtours)
                                    {
                                        KeyValuePair<int, double> to_pair_value;
                                        if (target.TryGetValue(nn, out to_pair_value))
                                        {
                                            double merge_weight = (tsp_problem.WeightMatrix[from_pair.Key][to_pair_value.Key] +
                                                    tsp_problem.WeightMatrix[nn][from_pair.Value.Key]) -
                                                (from_pair.Value.Value + to_pair_value.Value);
                                            if (merge_weight < weight)
                                            {
                                                weight = merge_weight;
                                                from = from_pair;
                                                to = new KeyValuePair<int, KeyValuePair<int, double>>(nn, to_pair_value);
                                                target_tour = target;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (target_tour == null)
                    {
                        foreach (KeyValuePair<int, KeyValuePair<int, double>> from_pair in current_tour)
                        {
                            foreach (Dictionary<int, KeyValuePair<int, double>> target in subtours)
                            {
                                if (target != current_tour)
                                {
                                    foreach (KeyValuePair<int, KeyValuePair<int, double>> to_pair in target)
                                    {
                                        double merge_weight = (tsp_problem.WeightMatrix[from_pair.Key][to_pair.Value.Key] +
                                                tsp_problem.WeightMatrix[to_pair.Key][from_pair.Value.Key]) -
                                            (from_pair.Value.Value + to_pair.Value.Value);
                                        if (merge_weight < weight)
                                        {
                                            weight = merge_weight;
                                            from = from_pair;
                                            to = to_pair;
                                            target_tour = target;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // merge the tours.
                    subtours.Remove(current_tour);
                    foreach (KeyValuePair<int, KeyValuePair<int, double>> for_target in current_tour)
                    {
                        target_tour.Add(for_target.Key, for_target.Value);
                    }
                    target_tour[from.Key] = new KeyValuePair<int, double>(to.Value.Key,
                        tsp_problem.WeightMatrix[from.Key][to.Value.Key]);
                    target_tour[to.Key] = new KeyValuePair<int, double>(from.Value.Key,
                        tsp_problem.WeightMatrix[to.Key][from.Value.Key]);
                }
                List<int> new_genome = new List<int>();
                int next = subtours[0][0].Key;
                do
                {
                    new_genome.Add(next);
                    next = subtours[0][next].Key;
                }
                while (next != 0);

                Individual individual = new Individual(new_genome);
                individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
                if (best == null ||
                    best.Fitness.CompareTo(individual.Fitness) > 0)
                {
                    best = individual;
                }

                generated++;
            }
            return best;
        }

        private List<Dictionary<int, KeyValuePair<int, int>>> SelectCycles(List<Dictionary<int, KeyValuePair<int, int>>> cycles)
        {
            List<Dictionary<int, KeyValuePair<int, int>>> selected = new List<Dictionary<int, KeyValuePair<int, int>>>();
            if (_strategy == EdgeAssemblyCrossoverSelectionStrategyEnum.MultipleRandom)
            {
                foreach (Dictionary<int, KeyValuePair<int, int>> cycle in cycles)
                {
                    if (Tools.Math.Random.StaticRandomGenerator.Get().Generate(1.0) > 0.25)
                    {
                        selected.Add(cycle);
                    }
                }
                return selected;
            }
            else
            {
                if (cycles.Count > 0)
                {
                    int idx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(cycles.Count);
                    selected.Add(cycles[idx]);
                    cycles.RemoveAt(idx);
                }
            }
            return cycles;
        }

        #endregion

    }
}