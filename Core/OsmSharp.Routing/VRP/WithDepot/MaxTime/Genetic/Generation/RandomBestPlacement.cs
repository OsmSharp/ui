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

namespace OsmSharp.Routing.VRP.WithDepot.MinimaxTime.Genetic.Generation
{
    /// <summary>
    /// Best-placement generator based on a random first customer for each route.
    /// </summary>
    internal class RandomBestPlacement :
        IGenerationOperation<List<Genome>, Problem, Fitness>
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
        public Individual<List<Genome>, Problem, Fitness> Generate(
            Solver<List<Genome>, Problem, Fitness> solver)
        {
            Problem problem = solver.Problem;

            DynamicAsymmetricMultiRoute multi_route = new DynamicAsymmetricMultiRoute(problem.Size, true);

            // create the problem for the genetic algorithm.
            List<int> customers = new List<int>();
            for (int customer = problem.Depots.Count; customer < problem.Size; customer++)
            {
                customers.Add(customer);
            }
            CheapestInsertionHelper helper = new CheapestInsertionHelper();

            List<double> weights = new List<double>();
            for (int i = 0; i < problem.Depots.Count; i++)
            {
                multi_route.Add(i);
                weights.Add(0);
            }
            int k = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(problem.Depots.Count);

            // keep placing customer until none are left.
            while (customers.Count > 0)
            {
                k = (k + 1) % problem.Depots.Count;

                // use best placement to generate a route.
                IRoute current_route = multi_route.Route(k);


                //Console.WriteLine("Starting new route with {0}", customer);
                while (customers.Count > 0)
                {
                    // calculate the best placement.
                    CheapestInsertionResult result = CheapestInsertionHelper.CalculateBestPlacement(problem, current_route, customers);

                    if (result.CustomerAfter == -1 || result.CustomerBefore == -1)
                    {
                        customers.Remove(result.Customer);
                        continue;
                    }
                    // calculate the new weight.
                    customers.Remove(result.Customer);
                    //current_route.InsertAfterAndRemove(result.CustomerBefore, result.Customer, result.CustomerAfter);
                    current_route.InsertAfter(result.CustomerBefore, result.Customer);
                    weights[k] += result.Increase + 15 * 60;

                    if (weights[k] == weights.Max())
                        break;
                }
            }

            for (int i = 0; i < problem.Depots.Count; i++)
                multi_route.RemoveCustomer(i);
            

            List<Genome> genomes = new List<Genome>();
            genomes.Add(Genome.CreateFrom(multi_route));
            Individual<List<Genome>, Problem, Fitness> individual = new Individual<List<Genome>, Problem, Fitness>(genomes);
            //individual.Initialize();
            return individual;
        }
    }
}
