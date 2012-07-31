using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Units.Time;
using Osm.Routing.Core.Route;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.VRP.Core;
using Tools.Math.VRP.Core.Routes.ASymmetric;
using Tools.Math.AI.Genetic.Selectors;
using Osm.Routing.Core.VRP.NoDepot.MaxTime.Genetic.Mutation;
using Osm.Routing.Core.VRP.NoDepot.MaxTime.Genetic.CrossOver;
using Osm.Routing.Core.VRP.NoDepot.MaxTime.Genetic.Generation;
using Tools.Math.AI.Genetic;
using Tools.Math.VRP.Core.Routes;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.Random;
using System.IO;
using System.Globalization;
using Osm.Core;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.Genetic
{
    /// <summary>
    /// Calculates VRP's without a depot with min max time constraints per route using genetic algorithms.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public class RouterGeneticSimple<ResolvedType> : RouterMaxTime<ResolvedType>
        where ResolvedType : ILocationObject
    {
        /// <summary>
        /// Creates a new genetic min max no depot vrp router.
        /// </summary>
        /// <param name="router"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public RouterGeneticSimple(IRouter<ResolvedType> router, Second max, Second delivery_time)
            :base(router, max, delivery_time)
        {
            _population = 100;
            _stagnation = 2000;

            _elitism_percentage = 10;
            _cross_percentage = 60;
            _mutation_percentage = 30;
        }

        /// <summary>
        /// Creates a new genetic min max no depot vrp router.
        /// </summary>
        /// <param name="router"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public RouterGeneticSimple(IRouter<ResolvedType> router, Second max, Second delivery_time, int population, int stagnation,
            double elitism_percentage, double cross_percentage, double mutation_percentage)
            : base(router, max, delivery_time)
        {
            _population = population;
            _stagnation = stagnation;

            _elitism_percentage = elitism_percentage;
            _cross_percentage = cross_percentage;
            _mutation_percentage = mutation_percentage;
        }


        /// <summary>
        /// Creates a new genetic min max no depot vrp router.
        /// </summary>
        /// <param name="router"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public RouterGeneticSimple(IRouter<ResolvedType> router,
            Second max, Second delivery_time, int population, int stagnation,
            double elitism_percentage, double cross_percentage, double mutation_percentage, List<double> probabilities)
            : base(router, max, delivery_time)
        {
            _population = population;
            _stagnation = stagnation;

            _elitism_percentage = elitism_percentage;
            _cross_percentage = cross_percentage;
            _mutation_percentage = mutation_percentage;
            _probabilities = probabilities;
        }

        private double _elitism_percentage;

        private double _cross_percentage;

        private double _mutation_percentage;

        private List<double> _probabilities;

        private int _population;

        private int _stagnation;

        private StreamWriter output;

        private ICollection<int> _customers;

        private Second _max;

        private long _ticks_before;

        private int _max_generations;

        /// <summary>
        /// Does the actual calculation, in this case, using a genetic algorithm.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="customers"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public override int[][] DoCalculation(MaxTimeProblem problem, ICollection<int> customers, 
            Second max)
        {
            _customers = customers;
            _max = max;

            float[] solutions = Tools.Math.VRP.Core.BestPlacement.CheapestInsertionHelper.CalculateBestValues(
                problem, customers);

            generations = 0;

            _max_generations = 10000000;

            FileInfo output_file = new FileInfo(string.Format("test_{0}.txt",
                customers.Count));
            output = output_file.AppendText();

            //output.WriteLine(string.Format("New Test {0} {1}->{2}@{3} ({4}, {5}, {6}, {7}, {8}, {9}):", 
            //    customers.Count, min, max, DateTime.Now,
            //    stagnation, population, max_generations, elitism_percentage, cross_percentage, mutation_percentage));
            //output.WriteLine();

            long ticks_before = DateTime.Now.Ticks;
            _ticks_before = ticks_before;

            //List<IMutationOperation<List<Genome>, Problem, Fitness>> mutators =
            //    new List<IMutationOperation<List<Genome>, Problem, Fitness>>();
            //mutators.Add(new RoutePartExchangeMutation());
            //mutators.Add(new VehicleMutation());
            //mutators.Add(new RelocationMutation());
            //mutators.Add(new RedivideRouteMutation());
            //if (_probabilities == null)
            //{
            //    _probabilities = new List<double>();
            //    //_probabilities.Add(0);
            //    _probabilities.Add(0.5);
            //    _probabilities.Add(0.5);
            //    //_probabilities.Add(0);
            //}

            //CombinedMutation<List<Genome>, Problem, Fitness> mutation = new CombinedMutation<List<Genome>, Problem, Fitness>(
            //    StaticRandomGenerator.Get(),
            //    mutators,
            //    _probabilities);

            SolverSettings settings = new SolverSettings(_stagnation, _population, _max_generations,
                _elitism_percentage, _cross_percentage, _mutation_percentage);
            MaxTimeProblem genetic_problem = problem;// new MaxTimeProblem(max,  problem, solutions);
            Solver<MaxTimeSolution, MaxTimeProblem, Fitness> solver =
                new Solver<MaxTimeSolution, MaxTimeProblem, Fitness>(genetic_problem, settings,
                new TournamentBasedSelector<MaxTimeSolution, MaxTimeProblem, Fitness>(10, 0.1),
                new ThreeOptMutation(),
                new RouteExchangeOperation(), //new RouteExchangeOperation(), //new RouteExchangeAndVehicleOperation(), // Order1CrossoverOperation()
                new RandomBestPlacement(),//new RandomGeneration(), //new RandomBestPlacement(),
                new FitnessCalculator());
            //solver.NewFittest += new Solver<MaxTimeSolution, Problem, Fitness>.NewFittestDelegate(solver_NewFittest);
            //solver.NewGeneration += new Solver<MaxTimeSolution, Problem, Fitness>.NewGenerationDelegate(solver_NewGeneration);
            Individual<MaxTimeSolution, MaxTimeProblem, Fitness> solution = solver.Start(null);
            //this.solver_NewFittest(solution);

            MaxTimeSolution routes = solution.Genomes;
            
            long ticks_after = DateTime.Now.Ticks;
            TimeSpan span = new TimeSpan(ticks_after - ticks_before);

            StringBuilder sizes = new StringBuilder();
            foreach (int size in routes.Sizes)
            {
                sizes.Append(size);
                sizes.Append(" ");
            }

            StringBuilder weights = new StringBuilder();
            foreach (double weight in solution.Fitness.Weights)
            {
                weights.Append(weight.ToString(CultureInfo.InvariantCulture));
                weights.Append(" ");
            }

            StringBuilder probalities = new StringBuilder();
            //foreach (double prod in _probabilities)
            //{
            //    probalities.Append(prod.ToString(CultureInfo.InvariantCulture));
            //    probalities.Append(";");
            //}


            string settings_string = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};",
                customers.Count, max.Value,
                _stagnation, _population, _max_generations, _elitism_percentage, _cross_percentage, _mutation_percentage);
            output.Write(settings_string);
            output.Write(string.Format("{0};{1};{2};{3};{4};{5};{6};{7}", generations, solution.Fitness.ActualFitness.ToString(CultureInfo.InvariantCulture),
                solution.Fitness.Vehicles, solution.Fitness.TotalTime.ToString(CultureInfo.InvariantCulture),
                solution.Fitness.MaxWeight.ToString(CultureInfo.InvariantCulture), sizes.ToString().Trim(), weights.ToString().Trim(), probalities.ToString()));
            output.WriteLine(string.Format("{0};{1};", span.Ticks, span.ToString()));
            output.Flush();
            output.Close();
            output.Dispose();
            output_file = null;

            // TODO: convert solution.
            int[][] result = new int[routes.Count][];
            for (int idx = 0; idx < routes.Count; idx++)
            {
                IRoute route = routes.Route(idx);
                result[idx] = route.ToArray();
            }
            return result;
        }

        bool output_each = false;

        int generations = 0;

        void solver_NewGeneration(int generation, int stagnation_count, Population<MaxTimeSolution, MaxTimeProblem, Fitness> population)
        {
            generations++;

            long ticks_after = DateTime.Now.Ticks;
            TimeSpan span = new TimeSpan(ticks_after - _ticks_before);

            Individual<MaxTimeSolution, MaxTimeProblem, Fitness> solution = population[0];

            MaxTimeSolution routes = solution.Genomes;
            if (output_each)
            {
                StringBuilder sizes = new StringBuilder();
                foreach (int size in routes.Sizes)
                {
                    sizes.Append(size);
                    sizes.Append(" ");
                }

                StringBuilder weights = new StringBuilder();
                foreach (double weight in solution.Fitness.Weights)
                {
                    weights.Append(weight.ToString(CultureInfo.InvariantCulture));
                    weights.Append(" ");
                }

                StringBuilder probalities = new StringBuilder();
                foreach (double prod in _probabilities)
                {
                    probalities.Append(prod.ToString(CultureInfo.InvariantCulture));
                    probalities.Append(";");
                }


                string settings_string = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};",
                    _customers.Count, _max.Value,
                    _stagnation, _population, _max_generations, _elitism_percentage, _cross_percentage, _mutation_percentage);
                output.Write(settings_string);
                output.Write(string.Format("{0};{1};{2};{3};{4};{5};{6};{7}", generations, solution.Fitness.ActualFitness.ToString(CultureInfo.InvariantCulture),
                    solution.Fitness.Vehicles, solution.Fitness.TotalTime.ToString(CultureInfo.InvariantCulture),
                    solution.Fitness.MaxWeight.ToString(CultureInfo.InvariantCulture), sizes.ToString().Trim(), weights.ToString().Trim(), probalities.ToString()));
                output.WriteLine(string.Format("{0};{1};", span.Ticks, span.ToString()));
                output.Flush();
                //output.Close();
                //output.Dispose();
                //output_file = null;
            }
        }

        void solver_NewFittest(Individual<MaxTimeSolution, MaxTimeProblem, Fitness> solution)
        {
            long ticks_after = DateTime.Now.Ticks;
            TimeSpan span = new TimeSpan(ticks_after - _ticks_before);

            MaxTimeSolution routes = solution.Genomes;
            if (output_each)
            {
                StringBuilder sizes = new StringBuilder();
                foreach (int size in routes.Sizes)
                {
                    sizes.Append(size);
                    sizes.Append(" ");
                }

                StringBuilder weights = new StringBuilder();
                foreach (double weight in solution.Fitness.Weights)
                {
                    weights.Append(weight.ToString(CultureInfo.InvariantCulture));
                    weights.Append(" ");
                }

                StringBuilder probalities = new StringBuilder();
                foreach (double prod in _probabilities)
                {
                    probalities.Append(prod.ToString(CultureInfo.InvariantCulture));
                    probalities.Append(";");
                }


                string settings_string = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};",
                    _customers.Count, _max.Value,
                    _stagnation, _population, _max_generations, _elitism_percentage, _cross_percentage, _mutation_percentage);
                output.Write(settings_string);
                output.Write(string.Format("{0};{1};{2};{3};{4};{5};{6};{7}", generations, solution.Fitness.ActualFitness.ToString(CultureInfo.InvariantCulture),
                    solution.Fitness.Vehicles, solution.Fitness.TotalTime.ToString(CultureInfo.InvariantCulture),
                    solution.Fitness.MaxWeight.ToString(CultureInfo.InvariantCulture), sizes.ToString().Trim(), weights.ToString().Trim(), probalities.ToString()));
                output.WriteLine(string.Format("{0};{1};", span.Ticks, span.ToString()));
                output.Flush();
                //output.Close();
                //output.Dispose();
                //output_file = null;
            }

            //Genome routes = individual.Genomes[0];
            // TODO: convert solution.
            int[][] result = new int[routes.Count][];
            for (int idx = 0; idx < routes.Count; idx++)
            {
                IRoute route = routes.Route(idx);
                result[idx] = route.ToArray();
            }

            this.DoIntermidiateResult(result);
        }
    }
}
