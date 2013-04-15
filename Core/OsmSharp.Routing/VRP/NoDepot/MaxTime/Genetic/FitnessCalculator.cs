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
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;
using OsmSharp.Tools.Math.AI.Genetic.Fitness;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.Genetic
{
    internal class FitnessCalculator : IFitnessCalculator<MaxTimeSolution, MaxTimeProblem, Fitness>
    {
        public double Epsilon
        {
            get
            {
                return 0.00001;
            }
        }

        public Fitness Fitness(MaxTimeProblem problem, Individual<MaxTimeSolution, MaxTimeProblem, Fitness> individual)
        {
            return this.Fitness(problem, individual, false);
        }

        public Fitness Fitness(MaxTimeProblem problem, Individual<MaxTimeSolution, MaxTimeProblem, Fitness> individual, bool validate)
        {
            return this.Fitness(problem, individual.Genomes);
        }

        public Fitness Fitness(MaxTimeProblem problem, MaxTimeSolution multi_route)
        {
            //MaxTimeSolution multi_route = genomes[0];

            int vehicles = multi_route.Count;

            Fitness fitness = new Fitness();
            fitness.MaxWeight = problem.Max.Value;
            fitness.Weights = new List<double>();
            List<double> above_max = new List<double>();
            double total = 0;
            double total_above_max = 0;
            int total_count_above_max = 0;
            double max = -1;
            for (int route_idx = 0; route_idx < multi_route.Count; route_idx++)
            {
                double weight = problem.Time(multi_route.Route(
                    route_idx));
                fitness.Weights.Add(weight);
                total = total + weight;
                if (weight > problem.Max.Value)
                {
                    total_above_max = total_above_max +
                        (weight - problem.Max.Value);
                    total_count_above_max++;
                }
                if (max < weight)
                {
                    max = weight;
                }
            }

            double total_weight = problem.Weight(multi_route);

            //double above_max_factor = 3;
            // multiply with the maximum.
            //fitness.ActualFitness = (vehicles * ((total_above_max) + total));
            //fitness.ActualFitness = (vehicles * ((total_above_max * max) + total));
            //fitness.ActualFitness = (vehicles * ((System.Math.Pow(total_above_max, 1.28)) + total + max));
            fitness.ActualFitness = total_weight;
            //fitness.ActualFitness = (vehicles * (total + ((total_count_above_max * above_max_factor) * max)));
            //fitness.ActualFitness = (total + ((total_count_above_max * above_max_factor) * max));
            fitness.Vehicles = vehicles;
            fitness.TotalTime = total;
            fitness.TotalAboveMax = total_above_max;
            return fitness;
        }

        public double Fitness(MaxTimeProblem problem, IRoute route)
        {
            double current_weight = 0;
            int previous = -1;
            int first = -1;
            int count = 0;
            foreach (int customer in route)
            {
                count++;
                if (previous >= 0)
                {
                    current_weight = current_weight + problem.Weight(
                        previous, customer);
                }
                else
                {
                    first = customer;
                }

                previous = customer;
            }
            current_weight = current_weight + problem.Weight(
                previous, first) + (count * 20);
            return current_weight;
        }


        public Fitness AverageFitness(MaxTimeProblem problem,
            IEnumerable<Individual<MaxTimeSolution, MaxTimeProblem, Fitness>> population)
        {
            //Fitness average = new Fitness();

            //double count = 0;
            //foreach (Individual<MaxTimeProblem, MaxTimeProblem, Fitness> individual in population)
            //{
            //    if (!individual.FitnessCalculated)
            //    {
            //        individual.CalculateFitness(problem, this);
            //    }
            //    average.ActualFitness = average.ActualFitness + individual.Fitness.ActualFitness;
            //    average.Vehicles = average.Vehicles + individual.Fitness.Vehicles;
            //    average.TotalTime = average.TotalTime + individual.Fitness.TotalTime;
            //    average.TotalAboveMax = average.TotalAboveMax + individual.Fitness.TotalAboveMax;
            //    average.MaxWeight = average.MaxWeight + individual.Fitness.MaxWeight;

            //    count++;
            //}

            //average.ActualFitness = average.ActualFitness / count;
            //average.Vehicles = (int)((double)average.Vehicles / count);
            //average.TotalTime = average.TotalTime / count;
            //average.TotalAboveMax = average.TotalAboveMax / count;
            //average.MaxWeight = average.MaxWeight / count;

            //return average;
            throw new NotImplementedException();
        }
    }
}
