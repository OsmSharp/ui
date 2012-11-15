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
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Tools.Math.VRP.Core.BestPlacement
{
    /// <summary>
    /// Implements some generic functions for best-placement.
    /// </summary>
    public class CheapestInsertionHelper
    {
        /// <summary>
        /// The minimum difference allowed.
        /// </summary>
        private static double _epsilon = 0;

        /// <summary>
        /// Calculates the best possible best-placement result for each customer.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="customers"></param>
        /// <returns></returns>
        public static double[] CalculateBestValues(
            IProblemWeights problem,
            IEnumerable<int> customers)
        {
            double[][] weights = problem.WeightMatrix;

            double[] solutions = new double[customers.Count<int>()];
            int idx = 0;
            foreach (int customer in customers)
            {
                double solution = double.MaxValue;
                foreach (int first in customers)
                {
                    if (first != customer)
                    {
                        foreach (int second in customers)
                        {
                            if (second != customer)
                            {
                                double new_solution = weights[first][customer] + weights[customer][second];
                                if (new_solution < solution)
                                {
                                    solution = new_solution;
                                }

                                if (solution <= 0)
                                {
                                    break;
                                }
                            }
                        }

                        if (solution <= 0)
                        {
                            break;
                        }
                    }
                }

                solutions[idx] = solution;
                idx++;
            }

            return solutions;
        }

        ///// <summary>
        ///// Returns the customer that least increases the length of the given route.
        ///// </summary>
        ///// <param name="problem"></param>
        ///// <param name="route"></param>
        ///// <param name="customers"></param>
        ///// <returns></returns>
        //public static BestPlacementResult CalculateBestPlacement(
        //    IProblemWeights problem,
        //    IRoute route,
        //    IEnumerable<int> customers)
        //{
        //    return BestPlacementHelper.CalculateBestPlacement(problem, route, customers, null);
        //}

        /// <summary>
        /// Returns the customer that least increases the length of the given route.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="customers"></param>
        /// <returns></returns>
        public static CheapestInsertionResult CalculateBestPlacement(
            IProblemWeights problem,
            IRoute route,
            IEnumerable<int> customers)
            //float[] solutions)
        {  // initialize the best placement result.
            CheapestInsertionResult best = new CheapestInsertionResult();
            best.Increase = float.MaxValue;
            if (!route.IsEmpty)
            {
                foreach (int customer in customers)
                {
                    CheapestInsertionResult result = CheapestInsertionHelper.CalculateBestPlacement(
                        problem, route, customer);
                    if (result.Increase < best.Increase)
                    {
                        best = result;

                        if (result.Increase < _epsilon)
                        {
                            break;
                        }
                    }
                }
            }
            else
            { // route needs to be initialized.
                throw new ArgumentException("Route needs to be initialized with at least one customer!");
            }

            // return result.
            return best;
        }

        //public static BestPlacementResult CalculateBestPlacement(
        //    IProblemWeights problem,
        //    IRoute route,
        //    int customer)
        //{
        //    return BestPlacementHelper.CalculateBestPlacement(problem, route, customer, null);
        //}

        /// <summary>
        /// Searches for the best place to insert the given customer.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="genomes"></param>
        /// <param name="city_to_place"></param>
        /// <returns></returns>
        public static CheapestInsertionResult CalculateBestPlacement(
            IProblemWeights problem,
            IRoute route,
            int customer)
            //float[] solutions)
        {  // initialize the best placement result.
            double[][] weights = problem.WeightMatrix;

            CheapestInsertionResult result
                = new CheapestInsertionResult();
            result.Customer = customer;
            result.CustomerAfter = -1;
            result.CustomerBefore = -1;
            result.Increase = double.MaxValue;

            double difference = double.MaxValue;
            if (!route.IsEmpty)
            {
                double new_weight = double.MaxValue;
                double old_weight = 0;

                int previous = -1;
                int first = -1;
                foreach (int current in route)
                {
                    if (previous >= 0)
                    { // the previous customer exists.
                        // only if the previous is known.

                        // calculate the old weights.
                        //old_weight = problem.Weight(previous, current);
                        old_weight = weights[previous][current];

                        // calculate the new weights.
                        //new_weight = problem.Weight(previous, customer);
                        new_weight = weights[previous][customer];

                        //new_weight = new_weight +
                        //    problem.Weight(customer, current);
                        new_weight = new_weight +
                            weights[customer][current];

                        // calculate the difference.
                        difference = new_weight - old_weight;
                        if (result.Increase > difference)
                        {
                            result.CustomerAfter = current;
                            result.CustomerBefore = previous;
                            result.Increase = difference;

                            // if the difference is equal to or smaller than epsilon we have search enough.
                            if (difference <= _epsilon)
                            {
                                // result is the best we will be able to get.
                                return result;
                            }
                        }
                    }
                    else
                    { // store the first city for later.
                        first = current;
                    }

                    // go to the next loop.
                    previous = current;
                }

                // set the pervious to the last.
                //previous = route.Last;

                // test last-to-first if the route is a round.
                if (route.IsRound)
                {
                    // calculate the new weights.
                    //new_weight = problem.Weight(previous, customer)
                    //        + (problem.Weight(customer, first));
                    new_weight = weights[previous][customer]
                        + weights[customer][first];

                    // calculate the old weights.
                    //old_weight = problem.Weight(previous, first);
                    old_weight = weights[previous][first];

                    // calculate the difference.
                    difference = new_weight - old_weight;
                    if (result.Increase > difference)
                    {
                        result.CustomerAfter = first;
                        result.CustomerBefore = previous;
                        result.Increase = difference;
                    }
                }
            }
            else
            { // route needs to be initialized.
                throw new ArgumentException("Route needs to be initialized with at least one customer!");
            }

            // return result.
            return result;
        }

        /// <summary>
        /// Searches for the best place to insert the given two customers abstracting the distance between them.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="genomes"></param>
        /// <param name="city_to_place"></param>
        /// <returns></returns>
        public static CheapestInsertionResult CalculateBestPlacement(
            IProblemWeights problem,
            IRoute route,
            int from,
            int to)
        {  // initialize the best placement result.
            CheapestInsertionResult result
                = new CheapestInsertionResult();
            result.Customer = -1; // this property is useless here!
            result.CustomerAfter = -1;
            result.CustomerBefore = -1;
            result.Increase = double.MaxValue;

            if (!route.IsEmpty)
            {
                double new_weight = double.MaxValue;
                double old_weight = 0;

                int previous = -1;
                int first = -1;
                foreach (int current in route)
                {
                    if (previous >= 0)
                    { // only if the previous is known.
                        // calculate the new weights.
                        new_weight = problem.Weight(previous, from)
                                + (problem.Weight(to, current));

                        // calculate the old weights.
                        old_weight = problem.Weight(previous, current);

                        // calculate the difference.
                        double difference = new_weight - old_weight;
                        if (result.Increase > difference)
                        {
                            result.CustomerAfter = current;
                            result.CustomerBefore = previous;
                            result.Increase = difference;
                        }
                    }
                    else
                    { // store the first city for later.
                        first = current;
                    }

                    // go to the next loop.
                    previous = current;
                }

                // test last-to-first if the route is a round.
                if (route.IsRound)
                {
                    // calculate the new weights.
                    new_weight = problem.Weight(previous, from)
                            + (problem.Weight(to, first));

                    // calculate the old weights.
                    old_weight = problem.Weight(previous, first);

                    // calculate the difference.
                    double difference = new_weight - old_weight;
                    if (result.Increase > difference)
                    {
                        result.CustomerAfter = previous;
                        result.CustomerBefore = first;
                        result.Increase = difference;
                    }
                }
            }
            else
            { // route needs to be initialized.
                throw new ArgumentException("Route needs to be initialized with at least one customer!");
            }

            // return result.
            return result;
        }
    }
}
