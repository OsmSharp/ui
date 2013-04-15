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
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.VRP.Core;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime
{
    /// <summary>
    /// Calculates fitness
    /// </summary>
    public class MaxTimeCalculator
    {
        /// <summary>
        /// The problem.
        /// </summary>
        private MaxTimeProblem _problem;

        /// <summary>
        /// Creates a new maxtime calculator.
        /// </summary>
        /// <param name="problem"></param>
        public MaxTimeCalculator(MaxTimeProblem problem)
        {
            _problem = problem;
        }

        /// <summary>
        /// Returns the delivery time.
        /// </summary>
        public float DeliveryTime 
        {
            get
            {
                return (float)_problem.DeliveryTime.Value;
            }
        }

        #region Per Route

        /// <summary>
        /// Calculates the cumulative weights of a route indexed by customer.
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public double[] CalculateCumulWeights(IRoute route)
        {
            // intialize the result array.
            double[] cumul = new double[route.Count + 1];

            int previous = -1; // the previous customer.
            double weight = 0; // the current weight.
            int idx = 0; // the current index.
            foreach (int customer1 in route)
            { // loop over all customers.
                if (previous >= 0)
                { // there is a previous customer.
                    // add one customer and the distance to the previous customer.
                    weight = _problem.MaxTimeCalculator.CalculateOneRouteIncrease(weight,
                        _problem.WeightMatrix[previous][customer1]);
                    cumul[idx] = weight;
                }
                else
                { // there is no previous customer, this is the first one.
                    cumul[idx] = _problem.MaxTimeCalculator.CalculateOneRouteIncrease(weight,
                        0);
                }

                idx++; // increase the index.
                previous = customer1; // prepare for next loop.
            }
            // handle the edge last->first.
            weight = weight +
                _problem.WeightMatrix[previous][route.First];
            cumul[idx] = weight;
            return cumul;
        }

        /// <summary>
        /// Calculates the new weight when adding one customer to one route.
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="difference"></param>
        /// <returns></returns>
        public double CalculateOneRouteIncrease(double weight, double difference)
        {
            return weight + difference + this.DeliveryTime;
        }

        /// <summary>
        /// Calculates the weight of one route.
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public double CalculateOneRoute(IRoute route)
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
            return current_weight;
        }

        #endregion

        /// <summary>
        /// Calculates the tot weight of one solution.
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        public double Calculate(MaxTimeSolution solution)
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

            //double above_max_factor = 3;
            // multiply with the maximum.
            //fitness.ActualFitness = (vehicles * ((total_above_max) + total));
            //fitness.ActualFitness = (vehicles * ((total_above_max * max) + total));
            //fitness.ActualFitness = (vehicles * ((System.Math.Pow(total_above_max, 1.28)) + total + max));
            return (vehicles * System.Math.Pow(total_above_max, 4)) + total;
            ////fitness.ActualFitness = (vehicles * (total + ((total_count_above_max * above_max_factor) * max)));
            ////fitness.ActualFitness = (total + ((total_count_above_max * above_max_factor) * max));
            //fitness.Vehicles = vehicles;
            //fitness.TotalTime = total;
            //fitness.TotalAboveMax = total_above_max;
            //return fitness;
        }
    }
}
