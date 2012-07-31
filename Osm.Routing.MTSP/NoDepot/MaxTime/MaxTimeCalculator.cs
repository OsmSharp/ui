using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Fitness;
using Tools.Math.AI.Genetic;
using Tools.Math.VRP.Core.Routes;
using Tools.Math.VRP.Core;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime
{
    public class MaxTimeCalculator
    {
        private MaxTimeProblem _problem;

        public MaxTimeCalculator(MaxTimeProblem problem)
        {
            _problem = problem;
        }

        public float DeliveryTime 
        {
            get
            {
                return (float)_problem.DeliveryTime.Value;
            }
        }

        #region Per Route

        /// <summary>
        /// Calculates the new weight when adding one customer to one route.
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="difference"></param>
        /// <returns></returns>
        public float CalculateOneRouteIncrease(float weight, float difference)
        {
            return weight + difference + this.DeliveryTime;
        }

        /// <summary>
        /// Calculates the weight of one route.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public float CalculateOneRoute(IRoute route)
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
                    current_weight = current_weight + _problem.Weight(
                        previous, customer);
                }
                else
                {
                    first = customer;
                }

                previous = customer;
            }
            current_weight = current_weight + _problem.Weight(
                previous, first) + (count * this.DeliveryTime);
            return (float)current_weight;
        }

        #endregion

        /// <summary>
        /// Calculates the tot weight of one solution.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="solution"></param>
        /// <returns></returns>
        public float Calculate(MaxTimeSolution solution)
        {
            int vehicles = solution.Count;

            List<double> above_max = new List<double>();
            double total = 0;
            double total_above_max = 0;
            int total_count_above_max = 0;
            double max = -1;
            List<double> weights = new List<double>();
            for (int route_idx = 0; route_idx < solution.Count; route_idx++)
            {
                double weight = this.CalculateOneRoute(solution.Route(
                    route_idx));
                weights.Add(weight);
                total = total + weight;
                if (weight > _problem.Max.Value)
                {
                    total_above_max = total_above_max +
                        (weight - _problem.Max.Value);
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
            return (float)(vehicles * ((System.Math.Pow(total_above_max, 4)) + total));
            ////fitness.ActualFitness = (vehicles * (total + ((total_count_above_max * above_max_factor) * max)));
            ////fitness.ActualFitness = (total + ((total_count_above_max * above_max_factor) * max));
            //fitness.Vehicles = vehicles;
            //fitness.TotalTime = total;
            //fitness.TotalAboveMax = total_above_max;
            //return fitness;
        }
    }
}
