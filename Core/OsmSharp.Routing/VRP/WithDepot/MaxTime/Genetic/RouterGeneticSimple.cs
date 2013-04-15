//// OsmSharp - OpenStreetMap tools & library.
//// Copyright (C) 2012 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Tools.Math.Units.Time;
//using OsmSharp.Tools.Math.AI.Genetic.Solvers;
//using OsmSharp.Tools.Math.VRP.Core;
//using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;
//using OsmSharp.Tools.Math.AI.Genetic.Selectors;
//using OsmSharp.Tools.Math.AI.Genetic;
//using OsmSharp.Tools.Math.VRP.Core.Routes;
//using OsmSharp.Tools.Math.AI.Genetic.Operations.Mutations;
//using OsmSharp.Tools.Math.Random;
//using System.IO;
//using System.Globalization;
//using OsmSharp.Routing;
//using OsmSharp.Routing.VRP.WithDepot.MinimaxTime.Genetic.CrossOver;
//using OsmSharp.Routing.VRP.WithDepot.MinimaxTime.Genetic.Generation;
//using OsmSharp.Routing.VRP.WithDepot.MinimaxTime.Genetic.Mutation;

//namespace OsmSharp.Routing.VRP.WithDepot.MinimaxTime.Genetic
//{
//    /// <summary>
//    /// Calculates VRP's without a depot with min max time constraints per route using genetic algorithms.
//    /// </summary>
//    /// <typeparam name="ResolvedType"></typeparam>
//    public class RouterGeneticSimple<ResolvedType> : RouterMaxTime<ResolvedType>
//        where ResolvedType : IRouterPoint
//    {
//        /// <summary>
//        /// Creates a new genetic min max no depot vrp router.
//        /// </summary>
//        /// <param name="router"></param>
//        /// <param name="min"></param>
//        /// <param name="max"></param>
//        public RouterGeneticSimple(IRouter<ResolvedType> router
//            )
//            : base(router)
//        {
//            _population = 100;
//            _stagnation = 400;

//            _elitism_percentage = 10;
//            _cross_percentage = 60;
//            _mutation_percentage = 30;
//        }

//        /// <summary>
//        /// Creates a new genetic min max no depot vrp router.
//        /// </summary>
//        /// <param name="router"></param>
//        /// <param name="min"></param>
//        /// <param name="max"></param>
//        public RouterGeneticSimple(IRouter<ResolvedType> router,
//            Second min, Second max, int population, int stagnation,
//            double elitism_percentage, double cross_percentage, double mutation_percentage)
//            : base(router)
//        {
//            _population = population;
//            _stagnation = stagnation;

//            _elitism_percentage = elitism_percentage;
//            _cross_percentage = cross_percentage;
//            _mutation_percentage = mutation_percentage;
//        }

//        private double _elitism_percentage;

//        private double _cross_percentage;

//        private double _mutation_percentage;

//        private int _population;

//        private int _stagnation;

//        private StreamWriter output;

//        /// <summary>
//        /// Does the actual calculation, in this case, using a genetic algorithm.
//        /// </summary>
//        /// <param name="problem"></param>
//        /// <param name="customers"></param>
//        /// <param name="min"></param>
//        /// <param name="max"></param>
//        /// <returns></returns>
//        protected override int[][] DoCalculation(IProblemWeights problem, ICollection<int> depots, ICollection<int> customers)
//        {
//            generations = 0;

//            int max_generations = 10000000;

//            FileInfo output_file = new FileInfo(string.Format("Test_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}.txt",
//                customers.Count, depots.Count, 0,
//                _stagnation, _population, max_generations, _elitism_percentage, _cross_percentage, _mutation_percentage));
//            output = output_file.AppendText();

//            //output.WriteLine(string.Format("New Test {0} {1}->{2}@{3} ({4}, {5}, {6}, {7}, {8}, {9}):", 
//            //    customers.Count, min, max, DateTime.Now,
//            //    stagnation, population, max_generations, elitism_percentage, cross_percentage, mutation_percentage));
//            //output.WriteLine();

//            long ticks_before = DateTime.Now.Ticks;

//            List<IMutationOperation<List<Genome>, Problem, Fitness>> mutators =
//                new List<IMutationOperation<List<Genome>, Problem, Fitness>>();
//            mutators.Add(new RoutePartExchangeMutation());
//            //mutators.Add(new VehicleMutation());
//            //mutators.Add(new RelocationMutation());
//            //mutators.Add(new RedivideRouteMutation());
//            List<double> probabilities = new List<double>();
//            probabilities.Add(1);
//            //probabilities.Add(0.5);
//            //probabilities.Add(0.4);
//            //probabilities.Add(0.2);

//            CombinedMutation<List<Genome>, Problem, Fitness> mutation = new CombinedMutation<List<Genome>, Problem, Fitness>(
//                StaticRandomGenerator.Get(),
//                mutators,
//                probabilities);

//            SolverSettings settings = new SolverSettings(_stagnation, _population, max_generations, _elitism_percentage, _cross_percentage, _mutation_percentage);
//            Problem genetic_problem = new Problem(depots, customers, problem);

//            Solver<List<Genome>, Problem, Fitness> solver =
//                new Solver<List<Genome>, Problem, Fitness>(genetic_problem, settings,
//                new TournamentBasedSelector<List<Genome>, Problem, Fitness>(10, 0.1),
//                mutation, //new RoutePartExchangeMutation(),
//                new RouteExchangeOperation(),//new RouteExchangeOperationSimple(), //new RouteExchangeOperation(), //new RouteExchangeAndVehicleOperation(),
//                new RandomBestPlacement(),//new RandomGeneration(), //new RandomBestPlacement(),
//                new FitnessCalculator());

//            solver.NewFittest += new Solver<List<Genome>, Problem, Fitness>.NewFittestDelegate(solver_NewFittest);
//            solver.NewGeneration += new Solver<List<Genome>, Problem, Fitness>.NewGenerationDelegate(solver_NewGeneration);
            
            
//            Individual<List<Genome>, Problem, Fitness> solution = solver.Start(null);
//            //this.solver_NewFittest(solution);

//            Genome routes = solution.Genomes[0];

//            long ticks_after = DateTime.Now.Ticks;
//            TimeSpan span = new TimeSpan(ticks_after - ticks_before);

//            StringBuilder sizes = new StringBuilder();
//            foreach (int size in routes.Sizes)
//            {
//                sizes.Append(size);
//                sizes.Append(" ");
//            }

//            StringBuilder weights = new StringBuilder();
//            foreach (double weight in solution.Fitness.Weights)
//            {
//                weights.Append(weight.ToString(CultureInfo.InvariantCulture));
//                weights.Append(" ");
//            }

            
//            output.Write(string.Format("{0};{1};{2};{3};{4};{5};{6};", generations, solution.Fitness.ActualFitness.ToString(CultureInfo.InvariantCulture),
//                solution.Fitness.Vehicles, solution.Fitness.TotalTime.ToString(CultureInfo.InvariantCulture),
//                solution.Fitness.MaxWeight.ToString(CultureInfo.InvariantCulture), sizes.ToString().Trim(), weights.ToString().Trim()));
//            output.WriteLine(string.Format("{0};{1};", span.Ticks, span.ToString()));
//            output.Flush();
//            output.Close();
//            output.Dispose();
//            output_file = null;

//            // TODO: convert solution.
//            int[][] result = new int[routes.Sizes.Length][];
//            for (int idx = 0; idx < routes.Sizes.Length; idx++)
//            {
//                IRoute route = routes.Route(idx);
//                result[idx] = route.ToArray();
//            }
//            return result;
//        }

//        int generations = 0;

//        void solver_NewGeneration(int generation, int stagnation_count, Population<List<Genome>, Problem, Fitness> population)
//        {
//            generations++;
//        }

//        void solver_NewFittest(Individual<List<Genome>, Problem, Fitness> individual)
//        {
//            //Genome genome = individual.Genomes[0];

//            //StringBuilder sizes = new StringBuilder();
//            //foreach (int size in genome.Sizes)
//            //{
//            //    sizes.Append(size);
//            //    sizes.Append(" ");
//            //}

//            //StringBuilder weights = new StringBuilder();
//            //foreach (double weight in individual.Fitness.Weights)
//            //{
//            //    weights.Append(weight);
//            //    weights.Append(" ");
//            //}

//            //output.Write(string.Format("{0};{1};{2};{3};{4};{5};{6};", generations, individual.Fitness.ActualFitness, individual.Fitness.Vehicles, individual.Fitness.TotalTime,
//            //    individual.Fitness.MaxWeight, sizes.ToString().Trim(), weights.ToString().Trim()));

//            Genome routes = individual.Genomes[0];
//            // TODO: convert solution.
//            int[][] result = new int[routes.Sizes.Length][];
//            for (int idx = 0; idx < routes.Sizes.Length; idx++)
//            {
//                IRoute route = routes.Route(idx);
//                result[idx] = route.ToArray();
//            }

//            this.DoIntermidiateResult(result);
//        }
//    }
//}
