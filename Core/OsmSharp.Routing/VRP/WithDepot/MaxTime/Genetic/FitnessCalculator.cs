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

namespace OsmSharp.Routing.VRP.WithDepot.MinimaxTime.Genetic
{
    internal class FitnessCalculator : IFitnessCalculator<List<Genome>, Problem, Fitness>
    {
        public double Epsilon
        {
            get
            {
                return 0.00001;
            }
        }

        public Fitness Fitness(Problem problem, Individual<List<Genome>, Problem, Fitness> individual)
        {
            return this.Fitness(problem, individual, false);
        }

        public Fitness Fitness(Problem problem, Individual<List<Genome>, Problem, Fitness> individual, bool validate)
        {
            return this.Fitness(problem, individual.Genomes);
        }

        public Fitness Fitness(Problem problem, List<Genome> genomes)
        {
            Genome multi_route = genomes[0];

            int vehicles = multi_route.Sizes.Length;

            Fitness fitness = new Fitness();
            fitness.Weights = new List<double>();

            double max = -1;
            double min = double.MaxValue;

            double total = 0;

            for (int route_idx = 0; route_idx < multi_route.Sizes.Length; route_idx++)
            {
                double weight = this.Fitness(problem, multi_route.Route(
                route_idx), route_idx);
                fitness.Weights.Add(weight);
                total += weight + 15 * 60;

                if (weight > max)
                    max = weight;

                if (weight < min)
                    min = weight;

            }

            fitness.ActualFitness = max + total / 10.0;
            fitness.Vehicles = vehicles;
            fitness.TotalTime = total;
            fitness.Range = max - min;


            return fitness;

        }

        public double Fitness(Problem problem, IRoute route, int idx)
        {
            double current_weight = 0;
            int previous = -1;
            int first = -1;
            int count = 0;

            current_weight = problem.Weight(idx, route.First) + problem.Weight(route.Last, idx);

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


        public Fitness AverageFitness(Problem problem, IEnumerable<Individual<List<Genome>, Problem, Fitness>> population)
        {
            Fitness average = new Fitness();

            double count = 0;
            foreach (Individual<List<Genome>, Problem, Fitness> individual in population)
            {
                if (!individual.FitnessCalculated)
                {
                    individual.CalculateFitness(problem, this);
                }
                //average.ActualFitness = average.ActualFitness + individual.Fitness.ActualFitness;
                //average.Vehicles = average.Vehicles + individual.Fitness.Vehicles;
                //average.TotalTime = average.TotalTime + individual.Fitness.TotalTime;
                //average.TotalAboveMax = average.TotalAboveMax + individual.Fitness.TotalAboveMax;
                //average.MaxWeight = average.MaxWeight + individual.Fitness.MaxWeight;

                count++;
            }

            //average.ActualFitness = average.ActualFitness / count;
            //average.Vehicles = (int)((double)average.Vehicles / count);
            //average.TotalTime = average.TotalTime / count;
            //average.TotalAboveMax = average.TotalAboveMax / count;
            //average.MaxWeight = average.MaxWeight / count;

            return average;
        }
    }
}
