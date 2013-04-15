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
using System.Threading;
using OsmSharp.Tools.Math.AI.Genetic.Operations;
using OsmSharp.Tools.Math.AI.Genetic.Operations.CrossOver;
using OsmSharp.Tools.Math.AI.Genetic.Operations.Mutations;
using OsmSharp.Tools.Math.AI.Genetic.Selectors;
using OsmSharp.Tools.Math.AI.Genetic.Operations.Generation;
using OsmSharp.Tools.Math.AI.Genetic.Fitness;
using OsmSharp.Tools.Progress;
using OsmSharp.Tools.Collections;

namespace OsmSharp.Tools.Math.AI.Genetic.Solvers
{
    /// <summary>
    /// Class implementing the main flow of any genetic algorithm.
    ///     - selection
    ///     - cross-over
    ///     - mutation
    /// by using operators for each of these steps.
    /// </summary>
    public class Solver<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : class
        where WeightType : IComparable
    {
        /// <summary>
        /// Settings used in this solver.
        /// </summary>
        private SolverSettings _settings;

        /// <summary>
        /// Selector used to select individuals for cross-over.
        /// </summary>
        private ISelector<GenomeType, ProblemType, WeightType> _selector;

        /// <summary>
        /// Operation used to cross over individuals.
        /// </summary>
        private ICrossOverOperation<GenomeType, ProblemType, WeightType> _cross_over_operation;
        
        /// <summary>
        /// Operation used to cross over individuals.
        /// </summary>
        private IMutationOperation<GenomeType, ProblemType, WeightType> _mutation_operation;

        /// <summary>
        /// Operation used to generate individuals.
        /// </summary>
        private IGenerationOperation<GenomeType, ProblemType, WeightType> _generation_operation;

        /// <summary>
        /// Holds the fitness calculator.
        /// </summary>
        private IFitnessCalculator<GenomeType, ProblemType, WeightType> _fitness_calculator;
                
        /// <summary>
        /// The random number generator.
        /// </summary>
        private System.Random _random;

        /// <summary>
        /// The problem being solved.
        /// </summary>
        private ProblemType _problem;

        /// <summary>
        /// Flag indicating what individuals to accept.
        /// </summary>
        private bool _accept_only_better_on_mutation = false;

        /// <summary>
        /// Flag indicating what individuals to accept.
        /// </summary>
        private bool _accept_only_better_on_cross_over = false;

        /// <summary>
        /// The fittest solution found so far.
        /// </summary>
        private Individual<GenomeType, ProblemType, WeightType> fittest;

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="settings"></param>
        /// <param name="selector"></param>
        /// <param name="mutation"></param>
        /// <param name="cross_over"></param>
        /// <param name="generation"></param>
        /// <param name="fitness_calculator"></param>
        public Solver(
            ProblemType problem,
            SolverSettings settings,
            ISelector<GenomeType, ProblemType, WeightType> selector,
            IMutationOperation<GenomeType, ProblemType, WeightType> mutation,
            ICrossOverOperation<GenomeType, ProblemType, WeightType> cross_over,
            IGenerationOperation<GenomeType, ProblemType, WeightType> generation,
            IFitnessCalculator<GenomeType, ProblemType, WeightType> fitness_calculator)
        {
            _problem = problem;
            _settings = settings;
            _selector = selector;
            _generation_operation = generation;
            _mutation_operation = mutation;
            _cross_over_operation = cross_over;
            _fitness_calculator = fitness_calculator;
        }

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="settings"></param>
        /// <param name="selector"></param>
        /// <param name="mutation"></param>
        /// <param name="cross_over"></param>
        /// <param name="generation"></param>
        /// <param name="fitness_calculator"></param>
        /// <param name="accept_only_better_cross_over"></param>
        /// <param name="accept_only_better_mutation"></param>
        public Solver(
            ProblemType problem,
            SolverSettings settings,
            ISelector<GenomeType, ProblemType, WeightType> selector,
            IMutationOperation<GenomeType, ProblemType, WeightType> mutation,
            ICrossOverOperation<GenomeType, ProblemType, WeightType> cross_over,
            IGenerationOperation<GenomeType, ProblemType, WeightType> generation,
            IFitnessCalculator<GenomeType, ProblemType, WeightType> fitness_calculator,
            bool accept_only_better_cross_over, bool accept_only_better_mutation)
        {
            _problem = problem;
            _settings = settings;
            _selector = selector;
            _generation_operation = generation;
            _mutation_operation = mutation;
            _cross_over_operation = cross_over;
            _fitness_calculator = fitness_calculator;
            _accept_only_better_on_cross_over = accept_only_better_cross_over;
            _accept_only_better_on_mutation = accept_only_better_mutation;
        }

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="selector"></param>
        /// <param name="combined_operation"></param>
        /// <param name="problem"></param>
        /// <param name="fitness_calculator"></param>
        public Solver(
            ProblemType problem,
            SolverSettings settings,
            ISelector<GenomeType, ProblemType, WeightType> selector,
            IOperation<GenomeType, ProblemType, WeightType> combined_operation,
            IFitnessCalculator<GenomeType, ProblemType, WeightType> fitness_calculator)
        {
            _problem = problem;
            _settings = settings;
            _selector = selector;
            _mutation_operation = combined_operation;
            _cross_over_operation = combined_operation;
            _generation_operation = combined_operation;
            _fitness_calculator = fitness_calculator;
        }

        /// <summary>
        /// The random number generator.
        /// </summary>
        public System.Random Random 
        {
            get
            {
                if (_random == null)
                {
                    _random = new System.Random();
                }
                return _random;
            }
        }

        /// <summary>
        /// Returns the problem being solved.
        /// </summary>
        public ProblemType Problem
        {
            get
            {
                return _problem;
            }
        }

        /// <summary>
        /// Returns the problem being solved.
        /// </summary>
        public Individual<GenomeType, ProblemType, WeightType> Fittest
        {
            get
            {
                return fittest;
            }
        }

        /// <summary>
        /// Gets/Sets the cancelled boolean.
        /// </summary>
        public bool Cancelled { get; set; }

        #region Properties

        /// <summary>
        /// Returns the settings this solver is using.
        /// </summary>
        public SolverSettings Setting
        {
            get
            {
                return _settings;
            }
        }

        /// <summary>
        /// Returns the selector this solver is using.
        /// </summary>
        public ISelector<GenomeType, ProblemType, WeightType> Selector
        {
            get
            {
                return _selector;
            }
        }

        /// <summary>
        /// Returns the mutation operator.
        /// </summary>
        public IMutationOperation<GenomeType, ProblemType, WeightType> MutationOperator
        {
            get
            {
                return _mutation_operation;
            }
        }

        /// <summary>
        /// Returns the cross over operator.
        /// </summary>
        public ICrossOverOperation<GenomeType, ProblemType, WeightType> CrossOverOperator
        {
            get
            {
                return _cross_over_operation;
            }
        }

        /// <summary>
        /// Returns the generation operator.
        /// </summary>
        public IGenerationOperation<GenomeType, ProblemType, WeightType> GenerationOperator
        {
            get
            {
                return _generation_operation;
            }
        }

        /// <summary>
        /// Returns the fitness calculator.
        /// </summary>
        public IFitnessCalculator<GenomeType, ProblemType, WeightType> FitnessCalculator
        {
            get
            {
                return _fitness_calculator;
            }
        }

        #endregion

        private bool _parallel = false;

        /// <summary>
        /// Start the solver with an initial population.
        /// </summary>
        /// <param name="initial"></param>
        public Individual<GenomeType, ProblemType, WeightType> Start(Population<GenomeType, ProblemType, WeightType> initial)
        {
            WeightType fitness = default(WeightType);

            Population<GenomeType, ProblemType, WeightType> population = new Population<GenomeType, ProblemType, WeightType>(initial, true);
            OsmSharp.Tools.Output.OutputStreamHost.Write("Generating population...");

            // use parallelism to generate population.
#if !WINDOWS_PHONE
            if (_parallel)
            {
                System.Threading.Tasks.Parallel.For(population.Count, _settings.PopulationSize, delegate(int i)
                {
                    // generate new.
                    IGenerationOperation<GenomeType, ProblemType, WeightType> generation_operation = this.CreateGenerationOperation();
                    Individual<GenomeType, ProblemType, WeightType> new_individual =
                        generation_operation.Generate(this);

                    // add to population.
                    lock (population)
                    {
                        population.Add(new_individual);
                    }

                    // report population generation.
                    OsmSharp.Tools.Output.OutputStreamHost.Write(string.Format("Generating population{0}/{1}..."), population.Count, _settings.PopulationSize);
                    this.ReportNew(string.Format("Generating population..."), population.Count, _settings.PopulationSize);
                });
            }
#endif

            while (population.Count < _settings.PopulationSize)
            {
                // generate new.
                IGenerationOperation<GenomeType, ProblemType, WeightType> generation_operation = this.CreateGenerationOperation();
                Individual<GenomeType, ProblemType, WeightType> new_individual =
                    generation_operation.Generate(this);

                // add to population.
                population.Add(new_individual);

                // report population generation.
                OsmSharp.Tools.Output.OutputStreamHost.WriteLine(string.Format("Generating population {0}/{1}...", population.Count, _settings.PopulationSize));
                this.ReportNew(string.Format("Generating population..."), population.Count, _settings.PopulationSize);
            }

            OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Done!");

            // sort the initial population.
            population.Sort(this, _fitness_calculator);

            //Tools.Core.Output.OutputTextStreamHost.WriteLine("Average {0}",
            //    _fitness_calculator.AverageFitness(_problem, population));

            int stagnation = 0;

            // start next generation or stop.
            WeightType new_fitness =
                population[0].Fitness;

            // is there improvement?
            if (new_fitness.CompareTo(fitness) > 0)
            { // a new fitness is found!
                // raise a new event here.
                this.RaiseNewFittest(population[0]);


                // set new fitness.
                fitness = new_fitness;
                fittest = population[0];

                // reset the stagnation count.
                stagnation = 0;

                OsmSharp.Tools.Output.OutputStreamHost.WriteLine("New Fittest {0}",
                    fittest.ToString());
            }

            // start getting genetic!
            int generation_count = 0;
            while (generation_count < _settings.MaxGeneration
                && stagnation < _settings.StagnationCount)
            {
                // check if the solving has been cancelled.
                if (this.Cancelled)
                {
                    this.ReportNew(string.Format("Solver cancelled at generation {0}!", generation_count), 1, 1);
                    return null;
                }
                if (this.Stopped)
                {
                    this.ReportNew(string.Format("Solver stopped at generation {0}!", generation_count), 1, 1);
                    return fittest;
                }

                // calculate the next generation.
                population =
                    this.NextGeneration(population);

                // get the population fitness.
                population.Sort(this, _fitness_calculator);
                OsmSharp.Tools.Output.OutputStreamHost.WriteLine("{0}->{1}",
                    population[0].Fitness,
                    population[population.Count - 1].Fitness);

                // start next generation or stop.
                new_fitness =
                    population[0].Fitness;

                // is there improvement?
                if (new_fitness.CompareTo(fitness) < 0)
                { // a new fitness is found!
                    // raise a new event here.
                    this.RaiseNewFittest(population[0]);

                    // set new fitness.
                    fitness = new_fitness;
                    fittest = population[0];
                    
                    OsmSharp.Tools.Output.OutputStreamHost.WriteLine("New Fittest {0}-{1} {2}",
                        generation_count,
                        stagnation,
                        fittest.ToString());

                    // reset the stagnation count.
                    stagnation = 0;

                    // report the new fittest.
                    this.ReportNew(string.Format("New Fittest {0}",
                        fittest.ToString()), 0, 1);
                }
                else
                {
                    // increase the stagnation count.
                    stagnation++;
                }

                // raise the generation count.
                generation_count++;

                // raise a new generation event.
                this.RaiseNewGeneration(generation_count, stagnation, population);

                // report the new generation.
                this.ReportNew(string.Format("Generation {0}-{1} {2}",
                    generation_count,
                    stagnation,
                    fittest.ToString()), stagnation, _settings.StagnationCount);
                if (stagnation != 0 && stagnation % 1 == 0)
                {
                    OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Generation {0}-{1} {2}",
                        generation_count,
                        stagnation,
                        fittest.ToString());
                }
            }

            OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Result [{0}]:",
                fitness, fittest.ToString());

            // report the new generation.
            this.ReportNew(string.Format("Evolution finished @ generation {0}: {1}",
                generation_count,
                fittest.ToString()), generation_count, _settings.StagnationCount);
            OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Evolution finished @ generation {0}: {1}",
                generation_count,
                fittest.ToString());

            return fittest;
        }

        /// <summary>
        /// Creates a generation operation.
        /// </summary>
        /// <returns></returns>
        protected virtual IGenerationOperation<GenomeType, ProblemType, WeightType> CreateGenerationOperation()
        {
            return _generation_operation;
        }

        /// <summary>
        /// Creates a mutation operation.
        /// </summary>
        /// <returns></returns>
        protected virtual IMutationOperation<GenomeType, ProblemType, WeightType> CreateMutationOperation()
        {
            return _mutation_operation;
        }

        /// <summary>
        /// Creates a crossover operation.
        /// </summary>
        /// <returns></returns>
        protected virtual ICrossOverOperation<GenomeType, ProblemType, WeightType> CreateCrossoverOperation()
        {
            return _cross_over_operation;
        }

        /// <summary>
        /// Calculates/generates a next generation based on the given one.
        /// </summary>
        /// <param name="population"></param>
        /// <returns></returns>
        private Population<GenomeType, ProblemType, WeightType> NextGeneration(Population<GenomeType, ProblemType, WeightType> population)
        {
            Population<GenomeType, ProblemType, WeightType> next_population =
                new Population<GenomeType, ProblemType, WeightType>(true);

            // do elitims selection.
            int elitism_count = (int)(population.Count * (_settings.ElitismPercentage / 100f));
            next_population.AddRange(
                population.GetRange(
                    0,
                    elitism_count));

            // do selection/cross-over.
            //Tools.Core.Output.OutputTextStreamHost.Write(" C:");
            int cross_over_count = (int)(population.Count * 
                (_settings.CrossOverPercentage / 100f)) + elitism_count;
            while (next_population.Count < cross_over_count 
                && next_population.Count < population.Count)
            {
                // select two individuals.
                Individual<GenomeType, ProblemType, WeightType> individual1 = _selector.Select(
                    this, 
                    population,
                    null);
                Individual<GenomeType, ProblemType, WeightType> individual2 = _selector.Select(
                    this, 
                    population,
                    new HashSet<Individual<GenomeType, ProblemType, WeightType>>(
                        new Individual<GenomeType, ProblemType, WeightType>[] { individual1 }));

                // cross-over.
                Individual<GenomeType, ProblemType, WeightType> new_individual =
                    _cross_over_operation.CrossOver(
                    this,
                        individual1,
                        individual2);

                new_individual.CalculateFitness(_problem, _fitness_calculator);
                if(_accept_only_better_on_cross_over)
                {
                    if (new_individual.Fitness.CompareTo(individual1.Fitness) < 0 &&
                        new_individual.Fitness.CompareTo(individual2.Fitness) < 0)
                    {
                        // add to the new population.
                        next_population.Add(new_individual);
                    }
                    else
                    {
                        //next_population.Add(individual1);
                        if (individual2.Fitness.CompareTo(individual1.Fitness) < 0)
                        {
                            // add to the new population.
                            next_population.Add(individual2);
                        }
                        else
                        {
                            next_population.Add(individual1);
                        }
                    }
                }
                else
                {
                    // add to the new population.
                    next_population.Add(new_individual);
                }

                //this.ReportNew("Crossing over population!", next_population.Count, population.Count);
                //Tools.Core.Output.OutputTextStreamHost.Write(".");
            }

            // do mutation to generate the rest of the population.
            int mutation_count =
                (int)(population.Count * (_settings.MutationPercentage / 100f)) + cross_over_count;
            int mutation_count_max_tries = mutation_count * 2; // try mutation 10 times more.
            int mutation_idx_absolute = 0;
            //Tools.Core.Output.OutputTextStreamHost.Write(" m:");
            while (next_population.Count < mutation_count
                && next_population.Count < population.Count)
            {
                // get random individual.
                int individual_to_mutate_idx = this.Random.Next(population.Count);
                Individual<GenomeType, ProblemType, WeightType> individual_to_mutate =
                    population[individual_to_mutate_idx];

                // mutate.
                Individual<GenomeType, ProblemType, WeightType> mutation = _mutation_operation.Mutate(
                    this,
                    individual_to_mutate);
                //if (!mutation.Equals(individual_to_mutate))
                //{ // do not add indentical ones.
                    // add to population.
                    if (_accept_only_better_on_mutation)
                    {
                        mutation.CalculateFitness(_problem, _fitness_calculator);
                        if (mutation.Fitness.CompareTo(individual_to_mutate.Fitness) < 0)
                        {
                            next_population.Add(mutation);                            
                        }
                    }
                    else
                    {
                        next_population.Add(mutation);                        
                    }

                //    //Tools.Core.Output.OutputTextStreamHost.Write(".");
                //    this.ReportNew("Mutating population!", next_population.Count, population.Count);
                //}

                // increase the absolute count.
                mutation_idx_absolute++;
                if (mutation_idx_absolute > mutation_count_max_tries)
                { // stop trying to mutate not new individuals could be found.
                    break;
                }
            }
            

            while (next_population.Count < population.Count)
            {
                // get random individual.
                int individual_to_mutate_idx = this.Random.Next(population.Count);
                Individual<GenomeType, ProblemType, WeightType> individual =
                    population[individual_to_mutate_idx];

                // place in the new population.
                next_population.Add(individual);
                
                this.ReportNew("Filling population!", next_population.Count, population.Count);
            }

            return next_population;
        }

        #region Events

        #region NewFittest

        /// <summary>
        /// New fittest delegate for the new fittest event.
        /// </summary>
        /// <param name="individual"></param>
        public delegate void NewFittestDelegate(Individual<GenomeType, ProblemType, WeightType> individual);

        /// <summary>
        /// New fittest event.
        /// </summary>
        public event NewFittestDelegate NewFittest;

        /// <summary>
        /// Raises the new fittest event.
        /// </summary>
        /// <param name="individual"></param>
        private void RaiseNewFittest(Individual<GenomeType, ProblemType, WeightType> individual)
        {
            if (NewFittest != null)
            {
                NewFittest(individual);
            }
        }
        
        #endregion

        #region NewGeneration

        /// <summary>
        /// New generation delegate for the new generation event.
        /// </summary>
        /// <param name="generation"></param>
        /// <param name="stagnation_count"></param>
        /// <param name="population"></param>
        public delegate void NewGenerationDelegate(int generation,int stagnation_count, 
            Population<GenomeType, ProblemType, WeightType> population);
        
        /// <summary>
        /// New generation event.
        /// </summary>
        public event NewGenerationDelegate NewGeneration;

        /// <summary>
        /// Raises the new generation event.
        /// </summary>
        /// <param name="generation"></param>
        /// <param name="stagnation_count"></param>
        /// <param name="population"></param>
        private void RaiseNewGeneration(int generation, int stagnation_count, 
            Population<GenomeType, ProblemType, WeightType> population)
        {
            if (NewGeneration != null)
            {
                NewGeneration(generation,stagnation_count, population);
            }
        }

        #endregion
    
    
        #endregion

        #region Progress Reporting


        /// <summary>
        /// Gets/Sets a progress reporter acception updates from this solver.
        /// </summary>
        public IProgressReporter ProgressReporter { get; set; }

        /// <summary>
        /// Reports a new message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="current"></param>
        /// <param name="max"></param>
        private void ReportNew(string message,int current, int max)
        {
            if(this.ProgressReporter != null)
            {
                ProgressStatus status = new ProgressStatus();
                status.CurrentNumber = current;
                status.Message = message;
                status.Status = ProgressStatus.ProgressStatusEnum.Busy;
                status.TotalNumber = max;
                this.ProgressReporter.Report(status);
            }
        }

        #endregion

        /// <summary>
        /// Stops the solver.
        /// </summary>
        public void Stop()
        {
            this.Stopped = true;
        }

        /// <summary>
        /// Returns true if the solver has stopped.
        /// </summary>
        public bool Stopped { get; private set; }
    }
}