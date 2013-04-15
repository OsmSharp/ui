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
using OsmSharp.Tools.Math.Units.Time;
using OsmSharp.Tools.Math.Random;
using OsmSharp.Tools.Progress;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Problems;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Solver.Operations.Mutation;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic;
using OsmSharp.Tools.Math.AI.Genetic.Selectors;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic.Operations.CrossOver;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Solver.Operations.Generation;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic.Helpers;

namespace OsmSharp.Tools.Math.VRP.MultiSalesman
{
    /// <summary>
    /// Facade.
    /// </summary>
    public static class Facade
    {
        private class LocalProblem : Problem
        {
            private OsmSharp.Tools.Math.VRP.Core.IProblemWeights _problem;

            public LocalProblem(OsmSharp.Tools.Math.VRP.Core.IProblemWeights problem,
                Second min, Second max)
                :base(problem.Size, min, max)
            {
                _problem = problem;
            }

            public override double Weight(int city1, int city2)
            {
                return _problem.Weight(city1, city2);
            }
        }

        /// <summary>
        /// Calculates solution to the given problem.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int[][] Calculate(OsmSharp.Tools.Math.VRP.Core.IProblemWeights problem,
            Second min, Second max)
        {
            Problem local_problem = new LocalProblem(problem, min, max);
            SolverSettings settings = new SolverSettings(1000, 10, 0, 10, 20, 30);
            return Facade.Calculate(local_problem, settings, 50);
        }

        internal static int[][] Calculate(Problem problem,
            SolverSettings settings,
            int categories)
        {
            Solver<List<Genome>, Problem, Fitness> solver = new Solver<List<Genome>, Problem, Fitness>(
                problem,
                settings,
                new TournamentBasedSelector<List<Genome>, Problem, Fitness>(10, 0.5),
                new RedivideFromLargeMutationOperation(),
                new RandomRandomSelectionCrossOverOperation(),
                new BestFastPlacementGenerationOperation(),
                new FitnessCalculator(categories));
            solver.ProgressReporter = _registered_progress_reporter;
            solver.NewFittest += new Solver<List<Genome>, Problem, Fitness>.NewFittestDelegate(solver_NewFittest);

            Individual fittest = solver.Start(null) as Individual;
            int[][] solution = new int[fittest.Genomes.Count][];
            for (int route_idx = 0; route_idx < fittest.Genomes.Count; route_idx++)
            {
                solution[route_idx] = new int[fittest.Genomes[route_idx].Count];
                for (int idx = 0; idx < fittest.Genomes[route_idx].Count; idx++)
                {
                    solution[route_idx][idx] = fittest.Genomes[route_idx][idx];
                }
            }
            return solution;
        }

        #region Progress Reporting

        private static IProgressReporter _registered_progress_reporter;

        internal static void RegisterProgressReporter(IProgressReporter reporter)
        {
            _registered_progress_reporter = reporter;
        }

        internal static void UnregisterProgressReporter()
        {
            _registered_progress_reporter = null;
        }

        #endregion

        static void solver_NewFittest(Individual<List<Genome>, Problem, Fitness> individual)
        {
            Facade.RaiseNewSolution(individual);
        }

        #region NewFittest

        /// <summary>
        /// New fittest delegate for the new fittest event.
        /// </summary>
        /// <param name="individual"></param>
        internal delegate void FittestDelegate(Individual<List<Genome>, Problem, Fitness> individual);

        /// <summary>
        /// New fittest event.
        /// </summary>
        internal static event FittestDelegate NewSolution;

        /// <summary>
        /// Raises the new fittest event.
        /// </summary>
        /// <param name="individual"></param>
        private static void RaiseNewSolution(Individual<List<Genome>, Problem, Fitness> individual)
        {
            if (NewSolution != null)
            {
                NewSolution(individual);
            }
        }

        #endregion

        internal static Population<List<Genome>, Problem, Fitness> InitializePopulation(
            Problem problem, Second target, int population_size, int round_count)
        {
            IRandomGenerator random = new RandomGenerator();

            // generate a list of cities to place.
            List<int> cities = new List<int>();
            for (int city_to_place = 0; city_to_place < problem.Cities; city_to_place++)
            {
                cities.Add(city_to_place);
            }

            // create the population
            Population<List<Genome>, Problem, Fitness> population = new Population<List<Genome>, Problem, Fitness>(
                null, false);

            // create the fitness calculator.
            FitnessCalculator fitness_calculator = new FitnessCalculator(5);

            while (population.Count < population_size)
            {
                OsmSharp.Tools.Output.OutputStreamHost.Write(
                    "Initializing population individual {0}/{1}...", population.Count + 1, population_size);

                // create copy of cities
                List<int> cities_list = new List<int>(cities);

                // create new individuals.
                Individual individual =
                    new Individual(new List<Genome>());

                // place one random city in each round.
                for (int round_idx = 0; round_idx < round_count; round_idx++)
                {
                    // select a random city to place.
                    int city_idx = random.Generate(cities_list.Count);
                    int city = cities_list[city_idx];
                    cities_list.RemoveAt(city_idx);

                    // create new genome.
                    Genome genome = new Genome();
                    genome.Add(city);
                    individual.Genomes.Add(genome);
                }

                individual = BestPlacementHelper.Do(
                    problem,
                    fitness_calculator,
                    individual,
                    cities_list);

                // add inidividual to the population.
                population.Add(individual);

                OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Done!");
            }

            return population;
        }

        internal static List<Genome> EstimateVehicles<EdgeType, VertexType>(
            Problem problem,
            Second min, Second max)
            where EdgeType : class
            where VertexType : class, IEquatable<VertexType>
        {
            // create the fitness calculator.
            FitnessCalculator fitness_calculator = new FitnessCalculator(5);

            IRandomGenerator random = new RandomGenerator();

            double average_time = (min.Value + max.Value) / 2.0;
            double previous_time = average_time;

            // generate a list of cities to place.
            List<int> cities = new List<int>();
            for (int city_to_place = 0; city_to_place < problem.Cities; city_to_place++)
            {
                cities.Add(city_to_place);
            }

            // first optimize the number of vehicles; this means generate rounds that are as close to the max as possible.
            List<Genome> generated_rounds = new List<Genome>();
            bool new_round = true;
            Genome current_round = null;
            while (cities.Count > 0)
            {
                OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Placing cities {0}/{1}",
                    cities.Count,
                    problem.Cities);
                if (_registered_progress_reporter != null)
                {
                    ProgressStatus status = new ProgressStatus();
                    status.TotalNumber = problem.Cities;
                    status.CurrentNumber = problem.Cities - cities.Count;
                    status.Message = "Placing cities...";
                    _registered_progress_reporter.Report(status);
                }

                // create a new round if needed.
                int city;
                int city_idx;
                if (new_round)
                {
                    new_round = false;
                    current_round = new Genome();

                    // select a random city to place.
                    city_idx = random.Generate(cities.Count);
                    city = cities[city_idx];
                    cities.RemoveAt(city_idx);
                    current_round.Add(city);

                    previous_time = average_time;
                }

                if (cities.Count > 0)
                {
                    // find the best city to place next.
                    // calculate the best position to place the next city.
                    BestPlacementHelper.BestPlacementResult new_position_to_place =
                        BestPlacementHelper.CalculateBestPlacementInGenome(
                            problem,
                            fitness_calculator,
                            current_round,
                            cities);

                    city = new_position_to_place.City;

                    // remove the node from the source list.
                    cities.Remove(city);

                    // place the node.
                    current_round.Insert(new_position_to_place.CityIdx, new_position_to_place.City);

                    // calculate the time.
                    double time = fitness_calculator.CalculateTime(
                        problem, current_round);

                    if (average_time < time)
                    { // time limit has been reached.
                        double diff_average = time - average_time;
                        double diff_previous = average_time - previous_time;

                        if (diff_average > diff_previous)
                        { // remove the last added city.
                            current_round.Remove(city);
                            cities.Add(city);
                        }
                        else
                        { // keep the last city.

                        }

                        // keep the generated round.
                        generated_rounds.Add(current_round);
                        new_round = true;
                    }
                    previous_time = time;
                }
            }

            return generated_rounds;
        }

        //        public static List<List<int>> GenerateForVehicles<EdgeType, VertexType>(
        //            ITravellingSalesmanProblem<WeightType> problem,
        //            Second min, Second max,
        //            IGraphInterpreter<EdgeType, VertexType, WeightType> interpreter,
        //            int count)
        //            where EdgeType : class
        //            where VertexType : class, IEquatable<VertexType>
        //        {
        //            int population_size = 10;

        //            int current_population_size = 0;

        //            bool success = false;
        //            List<List<int>> generated_rounds =null;
        //            while (!success)
        //            {
        //                // generate rounds as close to the average as possible.
        //                IRandomGenerator random = new RandomGenerator();

        //                double average_time = (min.Value + max.Value) / 2.0;
        //                double previous_time = average_time;

        //                // generate a list of cities to place.
        //                List<int> cities = new List<int>();
        //                for (int city_to_place = 0; city_to_place < problem.Count; city_to_place++)
        //                {
        //                    cities.Add(city_to_place);
        //                }

        //                // place one random city in a new round.
        //                generated_rounds = new List<List<int>>();
        //                int city_idx;
        //                int city;

        //                for (int idx = 0; idx < count; idx++)
        //                {
        //                    generated_rounds.Add(new List<int>());
        //                    city_idx = random.Generate(cities.Count);
        //                    city = cities[city_idx];
        //                    cities.RemoveAt(city_idx);
        //                    generated_rounds[idx].Add(city);
        //                }
        //                List<int> tried_and_failed_cities = new List<int>();

        //                OsmSharp.Tools.Output.OutputTextStreamHost.WriteLine("");
        //                OsmSharp.Tools.Output.OutputTextStreamHost.Write("Placing cities");

        //                while (cities.Count > 0)
        //                {
        //                    OsmSharp.Tools.Output.OutputTextStreamHost.Write(".",
        //                        cities.Count,
        //                        problem.Count);

        //                    // selected a random city to place next.
        //                    city_idx = random.Generate(cities.Count);
        //                    city = cities[city_idx];
        //                    cities.RemoveAt(city_idx);

        //                    // place the city at the best position
        //                    double added_weight = double.MaxValue;
        //                    int best_round_idx = -1;
        //                    BestPlacementHelper<WeightType>.NodeIndex best_placement = null;

        //                    // CRITERIA: 1: no times above the limits.
        //                    // CRITERIA: 2: the smallest increase in weight.
        //                    for (int round_idx = 0; round_idx < generated_rounds.Count; round_idx++)
        //                    {
        //                        // take a local copy of the current round.
        //                        List<int> current_round = new List<int>(generated_rounds[round_idx]);

        //                        // calculate the best position to place the next city.
        //                        BestPlacementHelper<WeightType>.NodeIndex new_position_to_place =
        //                            BestPlacementHelper<WeightType>.CalculateCheapestPlacementPosition(
        //                            current_round,
        //                            problem,
        //                            new List<int>(new int[] { city }));

        //                        // place the node.
        //                        current_round.Insert(new_position_to_place.Idx, new_position_to_place.Node);

        //                        // calculate the time.
        //                        WeightType weight = BestPlacementHelper<WeightType>.CalculateHardDistance(
        //                                problem.Factory, problem, current_round);

        //                        if (weight.Time > max.Value)
        //                        { // this round would become too big; don't select.

        //                        }
        //                        else
        //                        {
        //                            if (new_position_to_place.Weight < added_weight)
        //                            { // this round adds a smaller weight in placement.
        //                                best_round_idx = round_idx;
        //                                best_placement = new_position_to_place;

        //                                added_weight = new_position_to_place.Weight;
        //                            }
        //                        }
        //                    }

        //                    if (best_round_idx > 0)
        //                    {
        //                        // take a local copy of the current round.
        //                        List<int> best_round =
        //                            generated_rounds[best_round_idx];

        //                        // place the node.
        //                        best_round.Insert(best_placement.Idx, best_placement.Node);

        //                        // add the tried cities again.
        //                        cities.AddRange(tried_and_failed_cities);
        //                        tried_and_failed_cities.Clear();
        //                    }
        //                    else
        //                    { // add the city again and try again later.
        //                        tried_and_failed_cities.Add(city);
        //                    }
        //                }

        //                if (tried_and_failed_cities.Count == 0)
        //                {
        //                    success = true;

        //                    OsmSharp.Tools.Output.OutputTextStreamHost.WriteLine("");
        //                    OsmSharp.Tools.Output.OutputTextStreamHost.WriteLine("Done for {0} rounds!", count);
        //                }

        //                if (!success && current_population_size >= population_size)
        //                {
        //                    count++;
        //                    OsmSharp.Tools.Output.OutputTextStreamHost.WriteLine("");
        //                    OsmSharp.Tools.Output.OutputTextStreamHost.WriteLine("Increasing to {0} rounds!", count);

        //                    current_population_size = 0;
        //                }
        //                current_population_size++;
        //            }

        //            return generated_rounds;
        //        }
    }
}
