using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core.Routes.ASymmetric;
using Tools.Math.AI.Genetic.Fitness;
using Tools.Math.AI.Genetic;
using Tools.Math.VRP.Core.Routes;

namespace Osm.Routing.Core.VRP.NoDepot.MinMaxTime.Genetic
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
            fitness.MaxWeight = problem.Max.Value;
            fitness.Weights = new List<double>();
            List<double> above_max = new List<double>();
            double total = 0;
            double total_above_max = 0;
            int total_count_above_max = 0;
            double max = -1;
            for (int route_idx = 0; route_idx < multi_route.Sizes.Length; route_idx++)
            {
                double weight = this.Fitness(problem, multi_route.Route(
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

            double above_max_factor = 3;
            // multiply with the maximum.
            //fitness.ActualFitness = (vehicles * ((total_above_max) + total));
            //fitness.ActualFitness = (vehicles * ((total_above_max * max) + total));
            //fitness.ActualFitness = (vehicles * ((System.Math.Pow(total_above_max, 1.28)) + total + max));
            fitness.ActualFitness = (vehicles * ((System.Math.Pow(total_above_max, 4)) + total));
            //fitness.ActualFitness = (vehicles * (total + ((total_count_above_max * above_max_factor) * max)));
            //fitness.ActualFitness = (total + ((total_count_above_max * above_max_factor) * max));
            fitness.Vehicles = vehicles;
            fitness.TotalTime = total;
            fitness.TotalAboveMax = total_above_max;
            return fitness;
        }

        public double Fitness(Problem problem, IRoute route)
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
                average.ActualFitness = average.ActualFitness + individual.Fitness.ActualFitness;
                average.Vehicles = average.Vehicles + individual.Fitness.Vehicles;
                average.TotalTime = average.TotalTime + individual.Fitness.TotalTime;
                average.TotalAboveMax = average.TotalAboveMax + individual.Fitness.TotalAboveMax;
                average.MaxWeight = average.MaxWeight + individual.Fitness.MaxWeight;

                count++;
            }

            average.ActualFitness = average.ActualFitness / count;
            average.Vehicles = (int)((double)average.Vehicles / count);
            average.TotalTime = average.TotalTime / count;
            average.TotalAboveMax = average.TotalAboveMax / count;
            average.MaxWeight = average.MaxWeight / count;

            return average;
        }
    }
}
