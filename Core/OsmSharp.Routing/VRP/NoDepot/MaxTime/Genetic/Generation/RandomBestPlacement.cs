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
using OsmSharp.Tools.Math.AI.Genetic.Operations.Generation;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.VRP.Core.BestPlacement;
using OsmSharp.Tools.Math.VRP.Core;
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.Genetic.Generation
{
    /// <summary>
    /// Best-placement generator based on a random first customer for each route.
    /// </summary>
    internal class RandomBestPlacement :
        IGenerationOperation<MaxTimeSolution, MaxTimeProblem, Fitness>
    {
        public string Name
        {
            get
            {
                return "NotSet";
            }
        }

        /// <summary>
        /// Generates individuals based on a random first customer for each route.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        public Individual<MaxTimeSolution, MaxTimeProblem, Fitness> Generate(
            Solver<MaxTimeSolution, MaxTimeProblem, Fitness> solver)
        {
            MaxTimeProblem problem = solver.Problem;

            // create the solution.
            MaxTimeSolution solution = new MaxTimeSolution(problem.Size, true);

            CheapestInsertionHelper helper = new CheapestInsertionHelper();

            // keep placing customer until none are left.
            List<int> customers = new List<int>(problem.Customers);
            while (customers.Count > 0)
            {
                // select a random customer.
                double weight = 0;
                int customer_idx = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(customers.Count);
                int customer = customers[customer_idx];
                customers.RemoveAt(customer_idx);

                // use best placement to generate a route.
                IRoute current_route = solution.Add(customer);
                //Console.WriteLine("Starting new route with {0}", customer);
                while (customers.Count > 0)
                {
                    // calculate the best placement.
                    CheapestInsertionResult result =
                        CheapestInsertionHelper.CalculateBestPlacement(problem, current_route, customers);

                    // calculate the new weight.
                    double potential_weight = result.Increase + weight + 20;
                    // cram as many customers into one route as possible.
                    if (potential_weight < problem.Max.Value)
                    {
                        customers.Remove(result.Customer);
                        //current_route.InsertAfterAndRemove(result.CustomerBefore, result.Customer, result.CustomerAfter);
                        current_route.InsertAfter(result.CustomerBefore, result.Customer);
                        weight = potential_weight;
                    }
                    else
                    {// ok we are done!
                        break;
                    }
                }
            }

            return new Individual<MaxTimeSolution, MaxTimeProblem, Fitness>(solution);
        }
    }
}
