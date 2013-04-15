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
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;
using OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.CrossOver.Helpers;

namespace OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.CrossOver
{
    /// <summary>
    /// An edge assembly crossover.
    /// </summary>
    public class EdgeAssemblyCrossover :
        ICrossOverOperation<List<int>, GeneticProblem, Fitness>
    {
        private int _max_offspring;

        private EdgeAssemblyCrossoverSelectionStrategyEnum _strategy;

        private bool _nn;

        /// <summary>
        /// Creates a new edge assembly crossover.
        /// </summary>
        /// <param name="max_offspring"></param>
        /// <param name="strategy"></param>
        /// <param name="nn"></param>
        public EdgeAssemblyCrossover(int max_offspring,
            EdgeAssemblyCrossoverSelectionStrategyEnum strategy,
            bool nn)
        {
            _max_offspring = max_offspring;
            _strategy = strategy;
            _nn = nn;
        }

        /// <summary>
        /// Returns the name of this operator.
        /// </summary>
        public string Name
        {
            get
            {
                if (_strategy == EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom)
                {
                    if (_nn)
                    {
                        return string.Format("EAX_(SR{0}_NN)", _max_offspring);
                    }
                    return string.Format("EAX_(SR{0})", _max_offspring);
                }
                else
                {
                    if (_nn)
                    {
                        return string.Format("EAX_(MR{0}_NN)", _max_offspring);
                    }
                    return string.Format("EAX_(MR{0})", _max_offspring);
                }
            }
        }

        /// <summary>
        /// An enumeration of the crossover types.
        /// </summary>
        public enum EdgeAssemblyCrossoverSelectionStrategyEnum
        {
            /// <summary>
            /// SingleRandom.
            /// </summary>
            SingleRandom, // EAX-1AB
            /// <summary>
            /// MultipleRandom.
            /// </summary>
            MultipleRandom
        }



        #region ICrossOverOperation<int,Problem> Members

        /// <summary>
        /// Applies this crossover operation.
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
            OsmSharp.Tools.Math.TSP.Problems.IProblem tsp_problem = solver.Problem.BaseProblem;
            double[][] weights = tsp_problem.WeightMatrix;

            // first create E_a
            AsymmetricCycles e_a = new AsymmetricCycles(parent1.Genomes.Count + 1);
            e_a.AddEdge(0, parent1.Genomes[0]);
            for (int idx = 0; idx < parent1.Genomes.Count - 1; idx++)
            {
                int from = parent1.Genomes[idx];
                int to = parent1.Genomes[idx + 1];

                e_a.AddEdge(from, to);
            }
            e_a.AddEdge(parent1.Genomes[parent1.Genomes.Count - 1], 0);

            // create E_b
            int[] e_b = new int[parent2.Genomes.Count + 1];
            e_b[parent2.Genomes[0]] = 0;
            for (int idx = 0; idx < parent2.Genomes.Count - 1; idx++)
            {
                int from = parent2.Genomes[idx];
                int to = parent2.Genomes[idx + 1];

                e_b[to] = from;
            }
            e_b[0] = parent2.Genomes[parent2.Genomes.Count - 1];

            // create cycles.
            AsymmetricAlternatingCycles cycles = new AsymmetricAlternatingCycles(
                parent2.Genomes.Count + 1);
            for (int idx = 0; idx < e_b.Length; idx++)
            {
                int a = e_a[idx];
                int b = e_b[a];
                if (idx != b)
                {
                    cycles.AddEdge(idx, a, b);
                }
            }

            // the cycles that can be selected.
            List<int> selectable_cycles = new List<int>(cycles.Cycles.Keys);

            int generated = 0;
            Individual best = null;
            while (generated < _max_offspring 
                && selectable_cycles.Count > 0)
            {
                // select some random cycles.
                List<int> cycle_starts = this.SelectCycles(selectable_cycles);

                // copy if needed.
                AsymmetricCycles a = null;
                if (_max_offspring > 1)
                {
                    a = e_a.Clone();
                }
                else
                {
                    a = e_a;
                }

                // take e_a and remove all edges that are in the selected cycles and replace them by the eges
                int[] next_array_a = a.NextArray;
                foreach(int start in cycle_starts)
                {
                    int current = start;
                    KeyValuePair<int, int> current_next = cycles.Next(current);
                    do
                    {
                        a.AddEdge(current_next.Value, current_next.Key);

                        current = current_next.Value;
                        current_next = cycles.Next(current);
                    } while(current != start);
                }

                // connect all subtoures.
                int cycle_count = a.Cycles.Count;
                while (cycle_count > 1)
                {
                    // get the smallest tour.
                    KeyValuePair<int, int> current_tour = 
                        new KeyValuePair<int,int>(-1, int.MaxValue);
                    foreach (KeyValuePair<int, int> tour in a.Cycles)
                    {
                        if (tour.Value < current_tour.Value)
                        {
                            current_tour = tour;
                        }
                    }

                    // first try nn approach.
                    double weight = double.MaxValue;
                    int selected_from1 = -1;
                    int selected_from2 = -1;
                    int selected_to1 = -1;
                    int selected_to2 = -1;

                    bool[] ignore_list = new bool[a.Length];
                    int from;
                    int to;
                    from = current_tour.Key;
                    ignore_list[from] = true;
                    //ignore_list.Add(from);
                    to = next_array_a[from];
                    do
                    {
                        // step to the next ones.
                        from = to;
                        to = next_array_a[from];

                        //ignore_list.Add(from);
                        ignore_list[from] = true;
                    } while (from != current_tour.Key);

                    if (_nn)
                    { // only try tours containing nn.

                        from = current_tour.Key;
                        to = next_array_a[from];
                        double weight_from_to = weights[from][to];
                        do
                        {
                            // check the nearest neighbours of from
                            foreach (int nn in tsp_problem.Get10NearestNeighbours(from))
                            {
                                int nn_to = next_array_a[nn];

                                //if (!ignore_list.Contains(nn) &&
                                //    !ignore_list.Contains(nn_to))
                                if (!ignore_list[nn] &&
                                    !ignore_list[nn_to])
                                {
                                    double merge_weight =
                                        (weights[from][nn_to] + weights[nn][to]) -
                                        (weight_from_to + weights[nn][nn_to]);
                                    if (weight > merge_weight)
                                    {
                                        weight = merge_weight;
                                        selected_from1 = from;
                                        selected_from2 = nn;
                                        selected_to1 = to;
                                        selected_to2 = nn_to;
                                    }
                                }
                            }

                            // step to the next ones.
                            from = to;
                            to = next_array_a[from];
                        } while (from != current_tour.Key);
                    }
                    if (selected_from2 < 0)
                    {
                        // check the nearest neighbours of from
                        foreach (int customer in parent1.Genomes)
                        {
                            int customer_to = next_array_a[customer];

                            //if (!ignore_list.Contains(customer) &&
                            //    !ignore_list.Contains(customer_to))
                            if (!ignore_list[customer] &&
                                !ignore_list[customer_to])
                            {
                                double merge_weight =
                                    (weights[from][customer_to] + weights[customer][to]) -
                                    (weights[from][to] + weights[customer][customer_to]);
                                if (weight > merge_weight)
                                {
                                    weight = merge_weight;
                                    selected_from1 = from;
                                    selected_from2 = customer;
                                    selected_to1 = to;
                                    selected_to2 = customer_to;
                                }
                            }
                        }
                    }

                    //if (selected_from1 >= 0)
                    //{
                        a.AddEdge(selected_from1, selected_to2);
                        a.AddEdge(selected_from2, selected_to1);
                    //}
                    //else
                    //{
                    //    throw new Exception();
                    //}
                        cycle_count--;
                }

                //if (a.Cycles.Values.First<int>() != a.Length)
                //{
                //    throw new Exception();
                //}
                List<int> new_genome = new List<int>(a.Length);
                int next = next_array_a[0];
                do
                {
                    new_genome.Add(next);
                    next = next_array_a[next];
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

            if (best == null)
            {
                List<int> new_genome = new List<int>();
                int next = e_a[0];
                do
                {
                    new_genome.Add(next);
                    next = e_a[next];
                }
                while (next != 0);

                Individual individual = new Individual(new_genome);
                individual.CalculateFitness(solver.Problem, solver.FitnessCalculator);
                if (best == null ||
                    best.Fitness.CompareTo(individual.Fitness) > 0)
                {
                    best = individual;
                }
            }
            return best;
        }

        private List<int> SelectCycles(
            List<int> cycles)
        {
            List<int> starts = new List<int>();
            if (_strategy == EdgeAssemblyCrossoverSelectionStrategyEnum.MultipleRandom)
            {
                foreach (int cycle in cycles)
                {
                    if (OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(1.0) > 0.25)
                    {
                        starts.Add(cycle);
                    }
                }
                return starts;
            }
            else
            {
                if (cycles.Count > 0)
                {
                    int idx = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(cycles.Count);
                    starts.Add(cycles[idx]);
                    cycles.RemoveAt(idx);
                }
            }
            return starts;
        }

        #endregion

    }
}
