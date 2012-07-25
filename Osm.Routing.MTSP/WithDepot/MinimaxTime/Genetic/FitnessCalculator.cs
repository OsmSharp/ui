using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core.Routes.ASymmetric;
using Tools.Math.AI.Genetic.Fitness;
using Tools.Math.AI.Genetic;
using Tools.Math.VRP.Core.Routes;

namespace Osm.Routing.Core.VRP.WithDepot.MinimaxTime.Genetic
{
    internal class FitnessCalculator : IFitnessCalculator<Genome, Problem, Fitness>
    {
        public double Epsilon
        {
            get
            {
                return 0.00001;
            }
        }

        public Fitness Fitness(Problem problem, Individual<Genome, Problem, Fitness> individual)
        {
            return this.Fitness(problem, individual, false);
        }

        public Fitness Fitness(Problem problem, Individual<Genome, Problem, Fitness> individual, bool validate)
        {
            return this.Fitness(problem, individual.Genomes);
        }

        public Fitness Fitness(Problem problem, IList<Genome> genomes)
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


        public Fitness AverageFitness(Problem problem, IEnumerable<Individual<Genome, Problem, Fitness>> population)
        {
            Fitness average = new Fitness();

            double count = 0;
            foreach (Individual<Genome, Problem, Fitness> individual in population)
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
