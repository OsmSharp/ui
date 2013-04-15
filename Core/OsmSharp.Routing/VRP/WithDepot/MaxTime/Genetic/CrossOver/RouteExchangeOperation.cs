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
using OsmSharp.Tools.Math.AI.Genetic.Operations.Mutations;
using OsmSharp.Tools.Math.AI.Genetic.Operations.CrossOver;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.VRP.Core.BestPlacement;
using OsmSharp.Tools.Collections;

namespace OsmSharp.Routing.VRP.WithDepot.MinimaxTime.Genetic.CrossOver
{
    internal class RouteExchangeOperation :
        ICrossOverOperation<List<Genome>, Problem, Fitness>
    {
        public string Name
        {
            get
            {
                return "NotSet";
            }
        }

        public Individual<List<Genome>, Problem, Fitness> CrossOver(
            Solver<List<Genome>, Problem, Fitness> solver,
            Individual<List<Genome>, Problem, Fitness> parent1,
            Individual<List<Genome>, Problem, Fitness> parent2)
        {
            Genome route1 = parent1.Genomes[0];
            Genome route2 = parent2.Genomes[0];

            // get the minimum size of both routes.
            int size = route1.Sizes.Length;
            if (route2.Sizes.Length < size)
            {
                size = route2.Sizes.Length;
            }

            // select a random number of routes.
            HashSet<int> selected_first = new HashSet<int>();
            HashSet<int> selected_second = new HashSet<int>();
            List<IRoute> selected_routes = new List<IRoute>();
            bool first = true;
            while (selected_routes.Count < size)
            {
                // select route.
                int selected_route = -1;
                if (first)
                {
                    selected_route = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(route1.Sizes.Length);
                    while (selected_first.Contains(selected_route))
                    {
                        selected_route = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(route1.Sizes.Length);
                    }

                    selected_first.Add(selected_route);
                    selected_routes.Add(route1.Route(selected_route));
                }
                else
                {
                    selected_route = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(route2.Sizes.Length);
                    while (selected_second.Contains(selected_route))
                    {
                        selected_route = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(route2.Sizes.Length);
                    }

                    selected_second.Add(selected_route);
                    selected_routes.Add(route2.Route(selected_route));
                }

                first = !first;
            }

            // generate the new customer genome.
            HashSet<int> selected_customers = new HashSet<int>();
            List<int> customers = new List<int>();
            List<int> sizes = new List<int>();
            foreach (IRoute route in selected_routes)
            {
                int current_size = 0;
                foreach (int customer in route)
                {
                    if (!selected_customers.Contains(customer))
                    {
                        customers.Add(customer);
                        current_size++;
                        selected_customers.Add(customer);
                    }
                }
                sizes.Add(current_size);
            }
            while (sizes.Remove(0))
            {

            }

            Genome genome = new Genome();
            genome.Sizes = sizes.ToArray();
            genome.Customers = customers.ToArray();

            // insert all non-placed customers in the order of the first route.
            foreach (int customer in route1.Customers)
            {
                if (!selected_customers.Contains(customer))
                {
                    // try reinsertion.
                    CheapestInsertionResult result = new CheapestInsertionResult();
                    result.Increase = float.MaxValue;

                    int target_idx = -1;
                    for (int idx = 0; idx < genome.Sizes.Length; idx++)
                    {
                        IRoute route = genome.Route(idx);

                        if (genome.Sizes[idx] > 0)
                        {
                            CheapestInsertionResult current_result =
                                CheapestInsertionHelper.CalculateBestPlacement(solver.Problem.Weights, route, customer);
                            if (current_result.Increase < result.Increase)
                            {
                                target_idx = idx;
                                result = current_result;

                                if (result.Increase <= 0)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    // insert the customer.
                    for (int idx = 0; idx < customers.Count; idx++)
                    {
                        if (customers[idx] == result.CustomerBefore)
                        {
                            if (customers.Count - 1 == idx)
                            {
                                customers.Add(customer);
                            }
                            else
                            {
                                customers.Insert(idx + 1, customer);
                            }
                            break;
                        }
                    }

                    // set the mutated.
                    genome.Sizes[target_idx] = genome.Sizes[target_idx] + 1;
                    genome.Customers = customers.ToArray();
                }
            }

            if (!genome.IsValid())
            {
                throw new Exception();
            }
            List<Genome> genomes = new List<Genome>();
            genomes.Add(genome);
            Individual<List<Genome>, Problem, Fitness> individual = new Individual<List<Genome>, Problem, Fitness>(genomes);
            //individual.Initialize();
            return individual;
        }
    }
}
