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
using OsmSharp.Tools.Math.TSP.Genetic.Solver;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.AI.Genetic.Selectors;
using OsmSharp.Tools.Math.AI.Genetic.Operations.Mutations;
using OsmSharp.Tools.Math.AI.Genetic.Operations.Generation;
using OsmSharp.Tools.Math.AI.Genetic.Operations.CrossOver;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Generation;
using OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Helpers;
using OsmSharp.Tools.Math.Random;
using OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Mutation;
using OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.CrossOver;
using OsmSharp.Tools.Progress;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;

namespace OsmSharp.Tools.Math.TSP.Genetic
{
    /// <summary>
    /// Solver that uses a Genetic Algorithm to solve instances of the TSP.
    /// </summary>
    public class GeneticSolver : SolverBase
    {
        private Solver<List<int>, GeneticProblem, Fitness> solver;

        private IMutationOperation<List<int>, GeneticProblem, Fitness> _mutation_operation;

        private ICrossOverOperation<List<int>, GeneticProblem, Fitness> _cross_over_operation;

        private IGenerationOperation<List<int>, GeneticProblem, Fitness> _generation_operation;

        private int _eltism = 10;

        private int _cross = 10;

        private int _mutation = 20;

        private int _population = -1;

        private int _stagnation_count = 100;

        /// <summary>
        /// Creates a genetic solver.
        /// </summary>
        public GeneticSolver()
        {
            _mutation_operation = new BestDetailedPlacementMutationOperation();
            _cross_over_operation = new SequentialContructiveCrossoverOperator();
            _generation_operation = new BestPlacementGenerationOperation();

            _eltism = 10;
            _mutation = 30;
            _cross = 20;

            _population = 20;
            _stagnation_count = 100;
        }

        /// <summary>
        /// Creates a genetic solver.
        /// </summary>
        /// <param name="population"></param>
        /// <param name="stagnation"></param>
        /// <param name="elitism"></param>
        /// <param name="mutation_operation"></param>
        /// <param name="mutation"></param>
        /// <param name="cross_over_operation"></param>
        /// <param name="cross_over"></param>
        /// <param name="generation_operation"></param>
        public GeneticSolver(int population, int stagnation, int elitism,
            IMutationOperation<List<int>, GeneticProblem, Fitness> mutation_operation, int mutation,
            ICrossOverOperation<List<int>, GeneticProblem, Fitness> cross_over_operation, int cross_over,
            IGenerationOperation<List<int>, GeneticProblem, Fitness> generation_operation)
        {
            _mutation_operation = mutation_operation;
            _cross_over_operation = cross_over_operation;
            _generation_operation = generation_operation;

            _eltism = elitism;
            _mutation = mutation;
            _cross = cross_over;

            _population = population;
            _stagnation_count = stagnation;
        }

        /// <summary>
        /// Retuns the name of this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                return string.Format("GA{0}-({1})-({2})-({3})",
                    _population,
                    _generation_operation.Name,
                    string.Format("{0}{1}", _cross_over_operation.Name, _cross),
                    string.Format("{0}{1}", _mutation_operation.Name, _mutation));
            }
        }

        /// <summary>
        /// Solves the problem using a GA.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        protected override IRoute DoSolve(OsmSharp.Tools.Math.TSP.Problems.IProblem problem)
        {
            //int population_size = 10;
            //if (problem.Size < 100)
            //{
            //    population_size = System.Math.Max(problem.Size * 3, 10);
            //    if (problem.Size < 10)
            //    {
            //        population_size = 1;
            //    }
            //}
            //if (problem.Size < 1000)
            //{
            //    population_size = problem.Size / 4;
            //}

            // create the settings.
            SolverSettings settings = new SolverSettings(
                _stagnation_count,
                _population,
                1000000000,
                _eltism,
                _cross,
                _mutation);
        
            //List<IMutationOperation<List<int>, GeneticProblem, Fitness>> mutators = new List<IMutationOperation<int,GeneticProblem,Fitness>>();
            ////mutators.Add(new DefaultMutationOperation());
            ////mutators.Add(new BestPlacementMutationOperation());
            //mutators.Add(new BestDetailedPlacementMutationOperation());
            //List<double> probabilities = new List<double>();
            //probabilities.Add(1);
            ////probabilities.Add(0.5);
            ////probabilities.Add(0.3);

            //CombinedMutation<List<int>, GeneticProblem, Fitness> mutation = new CombinedMutation<int,GeneticProblem,Fitness>(
            //    StaticRandomGenerator.Get(),
            //    mutators,
            //    probabilities);

            ////SequentialContructiveCrossoverOperator cross_over = new SequentialContructiveCrossoverOperator();
            //BestDetailedPlacementCrossOverOperation cross_over = new BestDetailedPlacementCrossOverOperation();
            ////BestPlacementCrossOverOperation cross_over = new BestPlacementCrossOverOperation();
            ////EdgeRecombinationCrossOverOperation cross_over = new EdgeRecombinationCrossOverOperation();

            //BestPlacementGenerationOperation generation = new BestPlacementGenerationOperation();
            ////RandomGenerationOperation generation = new RandomGenerationOperation();
            ISelector<List<int>, GeneticProblem, Fitness> selector = new RandomSelector<List<int>, GeneticProblem, Fitness>();
            //ISelector<List<int>, GeneticProblem, Fitness> selector = new TournamentBasedSelector<List<int>, GeneticProblem, Fitness>(75, 0.01);
            solver =
                new Solver<List<int>, GeneticProblem, Fitness>(
                    new GeneticProblem(problem),
                    settings,
                    selector,
                    _mutation_operation,
                    _cross_over_operation,
                    _generation_operation,
                    new FitnessCalculator(),
                    true, false);

            solver.NewFittest += new Solver<List<int>, GeneticProblem, Fitness>.NewFittestDelegate(solver_NewFittest);
            solver.NewGeneration += new Solver<List<int>, GeneticProblem, Fitness>.NewGenerationDelegate(solver_NewGeneration);
            List<int> result = new List<int>(solver.Start(null).Genomes);
            result.Insert(0, 0);
            return new SimpleAsymmetricRoute(result, true);
        }

        private double _latest_fitness = double.MaxValue;
        void solver_NewGeneration(int generation, int stagnation_count, Population<List<int>, GeneticProblem, Fitness> population)
        {
            ProgressStatus status = new ProgressStatus();
            if(population[0].Fitness.Weight < _latest_fitness)
            {
                _latest_fitness = population[0].Fitness.Weight;

                status.CurrentNumber = stagnation_count + 1;
                status.TotalNumber = _stagnation_count;
                status.Message = string.Format("Found new {0}!", population[0].Fitness.Weight);
                status.Status = ProgressStatus.ProgressStatusEnum.Busy;
            }
            else
            {
                status.CurrentNumber = stagnation_count + 1;
                status.TotalNumber = _stagnation_count;
                status.Message = string.Format("Searching...", generation, stagnation_count + 1);
                status.Status = ProgressStatus.ProgressStatusEnum.Busy;
            }

            this.ReportProgress(status);
        }

        void solver_NewFittest(Individual<List<int>, GeneticProblem, Fitness> individual)
        {
            if (this.CanRaiseIntermidiateResult())
            {
                this.RaiseIntermidiateResult(individual.Genomes.ToArray());
            }
        }

        #region Progress Reporting

        private IProgressReporter _registered_progress_reporter;

        /// <summary>
        /// Registers a given progress reporter.
        /// </summary>
        /// <param name="reporter"></param>
        public void RegisterProgressReporter(IProgressReporter reporter)
        {
            _registered_progress_reporter = reporter;
        }

        /// <summary>
        /// Unregisters progress reporter.
        /// </summary>
        public void UnregisterProgressReporter()
        {
            _registered_progress_reporter = null;
        }

        private void ReportProgress(ProgressStatus status)
        {
            if (_registered_progress_reporter != null)
            {
                _registered_progress_reporter.Report(status);
            }
        }

        #endregion

        /// <summary>
        /// Stops the solver.
        /// </summary>
        public override void Stop()
        {
            if (solver != null)
            {
                solver.Stop();
            }
        }
    }
}
