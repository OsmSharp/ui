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
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.VRP.Core;
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;
using OsmSharp.Tools.Math.AI.Genetic.Selectors;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.AI.Genetic.Operations.Mutations;
using OsmSharp.Tools.Math.Random;
using System.IO;
using System.Globalization;
using OsmSharp.Tools.Math.AI.Genetic.Operations.Generation;
using OsmSharp.Tools.Math.AI.Genetic.Operations.CrossOver;
using OsmSharp.Routing;
using OsmSharp.Routing.VRP.NoDepot.MaxTime.Genetic.Mutation;
using OsmSharp.Routing.VRP.NoDepot.MaxTime.Genetic.Generation;
using OsmSharp.Routing.VRP.NoDepot.MaxTime.CheapestInsertion;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.Genetic
{
    /// <summary>
    /// Calculates VRP's without a depot with min max time constraints per route using genetic algorithms.
    /// </summary>
    public class RouterGeneticSimple : RouterMaxTime
    {
        /// <summary>
        /// Creates a new genetic min max no depot vrp router.
        /// </summary>
        /// <param name="max"></param>
        /// <param name="delivery_time"></param>
        public RouterGeneticSimple(Second max, Second delivery_time)
            :base(max, delivery_time)
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
        /// <param name="max"></param>
        /// <param name="delivery_time"></param>
        /// <param name="population"></param>
        /// <param name="stagnation"></param>
        /// <param name="elitism_percentage"></param>
        /// <param name="cross_percentage"></param>
        /// <param name="mutation_percentage"></param>
        public RouterGeneticSimple(Second max, Second delivery_time, int population, int stagnation,
            double elitism_percentage, double cross_percentage, double mutation_percentage)
            : base(max, delivery_time)
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
        /// <param name="max"></param>
        /// <param name="delivery_time"></param>
        /// <param name="population"></param>
        /// <param name="stagnation"></param>
        /// <param name="elitism_percentage"></param>
        /// <param name="cross_percentage"></param>
        /// <param name="mutation_percentage"></param>
        /// <param name="probabilities"></param>
        public RouterGeneticSimple(
            Second max, Second delivery_time, int population, int stagnation,
            double elitism_percentage, double cross_percentage, double mutation_percentage, 
            List<double> probabilities)
            : base(max, delivery_time)
        {
            _population = population;
            _stagnation = stagnation;

            _elitism_percentage = elitism_percentage;
            _cross_percentage = cross_percentage;
            _mutation_percentage = mutation_percentage;
            _probabilities = probabilities;
        }

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                return "GA";
            }
        }

        private double _elitism_percentage;

        private double _cross_percentage;

        private double _mutation_percentage;

        private List<double> _probabilities;

        private int _population;

        private int _stagnation;

        private ICollection<int> _customers;

        private int _max_generations;


        /// <summary>
        /// Executes a solver procedure.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        internal override MaxTimeSolution Solve(MaxTimeProblem problem)
        {
            _customers = problem.Customers;

            //float[] solutions = OsmSharp.Tools.Math.VRP.Core.BestPlacement.CheapestInsertionHelper.CalculateBestValues(
            //    problem, _customers);

            generations = 0;

            _max_generations = 10000000;

            // calculate one tsp solution.
            //Tools.Math.TSP.ISolver tsp_solver = new OsmSharp.Tools.Math.TSP.EdgeAssemblyGenetic.EdgeAssemblyCrossOverSolver(_population, _stagnation,
            //         new OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Generation._3OptGenerationOperation(),
            //          new OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.CrossOver.EdgeAssemblyCrossover(30,
            //                 OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.CrossOver.EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom,
            //                 true));
            //IRoute tsp_solution = tsp_solver.Solve(new OsmSharp.Routing.VRP.NoDepot.MaxTime.TSPPlacement.TSPProblem(
            //    problem));
            // initialize the generation.
            IGenerationOperation<MaxTimeSolution, MaxTimeProblem, Fitness> generation =
                //new SolverGenerationOperation(new TSPPlacement.TSPPlacementSolver<ResolvedType>(
                //    this.Router, this.Max, this.DeliveryTime, tsp_solution));
                new OsmSharp.Routing.VRP.NoDepot.MaxTime.Genetic.Generation.RandomBestPlacement();
            //new SolverGenerationOperation(new CheapestInsertionSolverWithImprovements<ResolvedType>(
            //    this.Router, this.Max, this.DeliveryTime, 5, 0.1f, true, 0.1f, false, 1f, null, null));
            //new SolverGenerationOperation(new CheapestInsertionSolverWithImprovements<ResolvedType>(
            //    this.Router, this.Max, this.DeliveryTime, 5, 0.1f, true, 0.1f, true, 1f));

            // initialize the crossover.
            ICrossOverOperation<MaxTimeSolution, MaxTimeProblem, Fitness> cross_over =
                new OsmSharp.Routing.VRP.NoDepot.MaxTime.Genetic.CrossOver.RouteExchangeOperation();

            // initialize the mutation.
            //IMutationOperation<MaxTimeSolution, MaxTimeProblem, Fitness> mutation =
            //    new VehicleMutation();

            List<IMutationOperation<MaxTimeSolution, MaxTimeProblem, Fitness>> mutators =
                new List<IMutationOperation<MaxTimeSolution, MaxTimeProblem, Fitness>>();
            mutators.Add(new VehicleMutation());
            //mutators.Add(new ThreeOptMutation());
            //mutators.Add(new RedivideRouteMutation());
            mutators.Add(new RoutePartExchangeMutation());
            if (_probabilities == null)
            {
                _probabilities = new List<double>();
                _probabilities.Add(0.2);
                _probabilities.Add(0.6);
                _probabilities.Add(0.2);
            }

            CombinedMutation<MaxTimeSolution, MaxTimeProblem, Fitness> mutation = new CombinedMutation<MaxTimeSolution, MaxTimeProblem, Fitness>(
                StaticRandomGenerator.Get(),
                mutators,
                _probabilities);


            SolverSettings settings = new SolverSettings(_stagnation, _population, _max_generations,
                _elitism_percentage, _cross_percentage, _mutation_percentage);
            MaxTimeProblem genetic_problem = problem;// new MaxTimeProblem(max,  problem, solutions);
            Solver<MaxTimeSolution, MaxTimeProblem, Fitness> solver =
                new Solver<MaxTimeSolution, MaxTimeProblem, Fitness>(genetic_problem, settings,
                new TournamentBasedSelector<MaxTimeSolution, MaxTimeProblem, Fitness>(5, 0.5),
                mutation, //new ThreeOptMutation(),
                cross_over, // new RouteExchangeOperation(), //new RouteExchangeOperation(), //new RouteExchangeAndVehicleOperation(), // Order1CrossoverOperation()
                generation, //new RandomBestPlacement(),//new RandomGeneration(), //new RandomBestPlacement(),
                new FitnessCalculator());
            solver.NewFittest += new Solver<MaxTimeSolution, MaxTimeProblem, Fitness>.NewFittestDelegate(solver_NewFittest);
            //solver.NewGeneration += new Solver<MaxTimeSolution, Problem, Fitness>.NewGenerationDelegate(solver_NewGeneration);
            Individual<MaxTimeSolution, MaxTimeProblem, Fitness> solution = solver.Start(null);
            //this.solver_NewFittest(solution);

            MaxTimeSolution routes = solution.Genomes;

            long ticks_after = DateTime.Now.Ticks;

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

            return routes;
        }

        bool output_each = false;

        int generations = 0;

        void solver_NewGeneration(int generation, int stagnation_count, Population<MaxTimeSolution, MaxTimeProblem, Fitness> population)
        {
            generations++;

            long ticks_after = DateTime.Now.Ticks;

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
                    _customers.Count, this.Max.Value,
                    _stagnation, _population, _max_generations, _elitism_percentage, _cross_percentage, _mutation_percentage);
                //output.Close();
                //output.Dispose();
                //output_file = null;
            }
        }

        void solver_NewFittest(Individual<MaxTimeSolution, MaxTimeProblem, Fitness> solution)
        {
            long ticks_after = DateTime.Now.Ticks;

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
                    _customers.Count, this.Max.Value,
                    _stagnation, _population, _max_generations, _elitism_percentage, _cross_percentage, _mutation_percentage);

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

            this.RaiseIntermidiateResult(result);
        }
    }
}
