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
using OsmSharp.Tools.Math.AI.Genetic.Fitness;
using OsmSharp.Tools.Math.AI.Genetic;

namespace OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic
{
    /// <summary>
    /// Calculates fitness.
    /// </summary>
    internal class FitnessCalculator :
        IFitnessCalculator<List<Genome>, Problem, Fitness>
    {
        /// <summary>
        /// The amount of categories to devide the times in.
        /// </summary>
        private int _category_count;

        /// <summary>
        /// Creates a new fitness calculator.
        /// </summary>
        /// <param name="category_count"></param>
        public FitnessCalculator(int category_count)
        {
            _category_count = category_count;
        }

        public double Epsilon
        {
            get
            {
                return double.MinValue;
            }
        }

        public Fitness Fitness(
            Problem problem,
            Individual<List<Genome>, Problem, Fitness> individual)
        {
            return this.Fitness(problem, individual, true);
        }

        /// <summary>
        /// Calculates the fitness of one individual.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="individual"></param>
        /// <param name="validate"></param>
        /// <returns></returns>
        public Fitness Fitness(
            Problem problem,
            Individual<List<Genome>, Problem, Fitness> individual, bool validate)
        {
            if (validate)
            {
                individual.Validate(problem);
            }

            return this.Fitness(problem, individual.Genomes);
        }

        /// <summary>
        /// Calculates the fitness of one individual based on it's genomes.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="genomes"></param>
        /// <returns></returns>
        public Fitness Fitness(
            Problem problem,
            List<Genome> genomes)
        {
            // calculate the times.
            List<double> times = this.CalculateTimes(problem, genomes);

            // calculate the rest.           
            return this.Calculate(problem, times);
        }

        /// <summary>
        /// Calculates the increase of fitness when one round is increased/descreased in weights.
        /// </summary>
        /// <param name="fitness"></param>
        /// <param name="round"></param>
        /// <param name="increase"></param>
        /// <param name="problem"></param>
        /// <returns></returns>
        public Fitness Adjust(
            Problem problem,
            Fitness fitness,
            int round,
            double increase)
        {
            // re-calculate times.
            List<double> times = new List<double>(fitness.Times);
            times[round] = times[round] + increase;

            // re-calculate the rest.
            return this.Calculate(problem, times);
        }

        #region Calculations

        /// <summary>
        /// Calculates the times 
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="genomes"></param>
        /// <returns></returns>
        private List<double> CalculateTimes(
            Problem problem,
            IList<Genome> genomes)
        {
            List<double> times = new List<double>();
            for (int idx = 0; idx < genomes.Count; idx++)
            {
                // initialize the time to zero.
                double time = 0;
                Genome current = genomes[idx];

                // calculate the genomes time.
                time = this.CalculateTime(problem, current);

                times.Add(time);
            }
            return times;
        }

        public double CalculateTime(
            Problem problem,
            Genome genome)
        {
            double time = 0;

            // genomes with zero or one city have a zero time.
            if (genome.Count > 1)
            {
                double weight = problem.Weight(
                    genome[0], genome[1]);
                for (int city_idx = 1; city_idx < genome.Count - 1; city_idx++)
                {
                    weight = weight + (
                        problem.Weight(
                            genome[city_idx], genome[city_idx + 1]));
                }

                // add the last to first distance.
                weight = weight + (
                    problem.Weight(
                        genome[genome.Count - 1], genome[0]));

                // get the time.
                time = weight;
            }

            return time;
        }

        /// <summary>
        /// Calculates the rest of the fitness indicators using the times per round.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        private Fitness Calculate(
            Problem problem,
            List<double> times)
        {
            // TODO: optimize this entire calculation!!!!
            Fitness result = new Fitness();
            result.SmallestRoundCategories = new List<int>();
            result.LargestRoundCategories = new List<int>();
            result.Vehicles = times.Count;

            // the latest round is taken.
            int longest_round = 0;
            int smallest_round = 0;

            double smallest_round_time = double.MaxValue;
            double longest_round_time = double.MinValue;
            double total_time = 0;

            // calculate the categories.
            // TODO: save a calculation by calculating this in the contructor.
            double category_size =
                problem.TargetTime.Value / (double)_category_count;

            // find the smallest/largest round and calculate total time.
            for (int round_idx = 0; round_idx < times.Count; round_idx++)
            {
                double round_time = times[round_idx];
                // calculate total time.
                total_time = total_time + round_time;

                // calculate categories
                // calculate the smallest category.
                if (round_time < problem.TargetTime.Value)
                {
                    double smallest_diff =
                        problem.TargetTime.Value - round_time;

                    int temp_smallest_category = (int)System.Math.Floor(smallest_diff / category_size);
                    if (temp_smallest_category < problem.Tolerance)
                    {
                        temp_smallest_category = 0;
                    }
                    result.SmallestRoundCategories
                        .Add(temp_smallest_category);
                }
                else
                {
                    result.SmallestRoundCategories.Add(0);
                }

                // calculate the largest category.
                if (round_time > problem.TargetTime.Value)
                {
                    double largest_diff =
                        round_time - problem.TargetTime.Value;

                    //largest_category = (int)System.Math.Min(System.Math.Floor(largest_diff / category_size),
                    //    _category_count);
                    int temp_largest_category = (int)System.Math.Floor(largest_diff / category_size);
                    if (temp_largest_category < problem.Tolerance)
                    {
                        temp_largest_category = 0;
                    }
                    result.LargestRoundCategories
                        .Add(temp_largest_category);

                }
                else
                {
                    result.LargestRoundCategories.Add(0);
                }

                // find smallest/largest round.
                if (smallest_round_time > round_time)
                {
                    smallest_round = round_idx;
                    smallest_round_time = round_time;
                }
                if (longest_round_time < round_time)
                {
                    longest_round = round_idx;
                    longest_round_time = round_time;
                }
            }

            // set longset/shortest time.
            result.MinimumTime = smallest_round_time;
            result.MaximumTime = longest_round_time;

            // calculate the smallest category.
            int smallest_category = 0;
            if (smallest_round_time < problem.TargetTime.Value)
            {
                double smallest_diff =
                    problem.TargetTime.Value - smallest_round_time;

                smallest_category = (int)System.Math.Floor(smallest_diff / category_size);
                if (smallest_category < problem.Tolerance)
                {
                    smallest_category = 0;
                }
            }

            // calculate the largest category.
            int largest_category = 0;
            if (longest_round_time > problem.TargetTime.Value)
            {
                double largest_diff =
                    longest_round_time - problem.TargetTime.Value;

                //largest_category = (int)System.Math.Min(System.Math.Floor(largest_diff / category_size),
                //    _category_count);
                largest_category = (int)System.Math.Floor(largest_diff / category_size);
                if (largest_category < problem.Tolerance)
                {
                    largest_category = 0;
                }
            }

            // create and return the fitness object.
            result.LargestRoundCategory = largest_category;
            result.SmallestRoundCategory = smallest_category;
            result.TotalTime = total_time;
            result.Times = times;

            // calculate feasability            
            result.Feasable = result.MaximumTime < problem.MaximumTime.Value
                    && result.MinimumTime > problem.MinimumTime.Value;

            return result;
        }

        #endregion

        #region IFitnessCalculator<List<Genome>, Problem, Fitness> Members

        //public Fitness FitnessPart(Problem problem, Genome first, Genome second)
        //{
        //    throw new NotSupportedException();
        //}

        //public Fitness FitnessFirstPart(Problem problem, IList<Genome> genome_part)
        //{
        //    throw new NotSupportedException();
        //}

        //public Fitness FitnessLastPart(Problem problem, IList<Genome> genome_part)
        //{
        //    throw new NotSupportedException();
        //}

        //public Fitness FitnessPart(Problem problem, IList<Genome> genome_part)
        //{
        //    throw new NotSupportedException();
        //}

        #endregion


        public Fitness AverageFitness(Problem problem, IEnumerable<Individual<List<Genome>, Problem, Fitness>> population)
        {
            throw new NotImplementedException();
        }
    }
}
