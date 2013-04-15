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
using OsmSharp.Tools.Math.AI.Genetic.Operations.Mutations;
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.VRP.Core.BestPlacement;

namespace OsmSharp.Routing.VRP.WithDepot.MinimaxTime.Genetic.Mutation
{
    /// <summary>
    /// Mutation operation exchanging a part of some route to a part of another route.
    /// </summary>
    internal class RoutePartExchangeMutation :
        IMutationOperation<List<Genome>, Problem, Fitness>
    {
        //private float _max_selection = 0.5f;

        public string Name
        {
            get
            {
                return "NotSet";
            }
        }

        /// <summary>
        /// Does the mutation.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="mutating"></param>
        /// <returns></returns>
        public Individual<List<Genome>, Problem, Fitness> Mutate(
            Solver<List<Genome>, Problem, Fitness> solver,
            Individual<List<Genome>, Problem, Fitness> mutating)
        {
            // get the route information.
            Genome multi_route = mutating.Genomes[0];
            int[] size_new = null;
            int[] customers_new = null;

            if (multi_route.Sizes.Length > 1)
            { // TODO: Maybe add some extra code for the the route-count is very low and chances of collisions are big.
                // TODO: investigate what is more expensive, and extra random generation or creating a list and remove items.
                // http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
                // select a route.
                int source_route_idx = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(multi_route.Sizes.Length);
                int target_route_idx = -1;
                do
                {
                    target_route_idx = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(multi_route.Sizes.Length);
                } while (source_route_idx == target_route_idx);
                int source_count = multi_route.Sizes[source_route_idx];
                int target_count = multi_route.Sizes[target_route_idx];
                if (target_count > 0 && source_count > 3)
                {
                    // take a part out of the orginal.
                    int size = (int)OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(source_count);
                    int source_location = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(source_count);

                    int first = source_location - (size / 2);
                    if (first < 0)
                    {
                        first = 0;
                    }
                    first = multi_route.StartOf(source_route_idx) + first;
                    int last = source_location + (size / 2);
                    if (last > source_count - 1)
                    {
                        last = source_count - 1;
                    }
                    last = multi_route.StartOf(source_route_idx) + last;
                    int idx = -1;
                    List<int> part_of_orginal = new List<int>();
                    foreach (int customer in multi_route.Customers)
                    {
                        idx++;
                        if(idx >= first && idx <= last)
                        {
                            part_of_orginal.Add(customer);
                        }
                    }
                    
                    // place it somewhere in the target.
                    if (part_of_orginal.Count > 0)
                    {
                        IRoute target = multi_route.Route(target_route_idx);

                        CheapestInsertionResult route_placement = CheapestInsertionHelper.CalculateBestPlacement(
                            solver.Problem, target, part_of_orginal[0], part_of_orginal[part_of_orginal.Count - 1]);                        

                        // create a new array and start copying.
                        customers_new = new int[multi_route.Customers.Length];
                        int new_idx = 0;
                        for (idx = 0; idx < multi_route.Customers.Length; idx++)
                        {
                            if (idx >= first && idx <= last)
                            {
                                // do nothing.
                            }
                            else
                            {
                                customers_new[new_idx] = multi_route.Customers[idx];                               
                                new_idx++;

                                if (multi_route.Customers[idx] == route_placement.CustomerBefore)
                                { // insert the entire between part here.
                                    for (int between_idx = 0; between_idx < part_of_orginal.Count; between_idx++)
                                    {
                                        customers_new[new_idx] = part_of_orginal[between_idx];         
                                        new_idx++;
                                    }
                                }
                            }
                        }

                        // adjust sizes array.
                        size_new = new int[multi_route.Sizes.Length];
                        for (idx = 0; idx < multi_route.Sizes.Length; idx++)
                        {
                            if (idx == source_route_idx)
                            {
                                size_new[idx] = multi_route.Sizes[idx] - part_of_orginal.Count;
                            }
                            else if (idx == target_route_idx)
                            {
                                size_new[idx] = multi_route.Sizes[idx] + part_of_orginal.Count;
                            }
                            else
                            {
                                size_new[idx] = multi_route.Sizes[idx];
                            }
                        }
                    }
                }
            }

            if (size_new == null || customers_new == null)
            {
                size_new = multi_route.Sizes.Clone() as int[];
                customers_new = multi_route.Customers.Clone() as int[];
            }

            // remove all zero's.
            List<int> sizes = new List<int>(size_new);
            while (sizes.Remove(0))
            {

            }
            size_new = sizes.ToArray<int>();

            List<Genome> genomes = new List<Genome>();
            Genome genome = new Genome();
            genome.Sizes = size_new;
            genome.Customers = customers_new;

            if (!genome.IsValid())
            {
                throw new Exception();
            }
            genomes.Add(genome);
            Individual<List<Genome>, Problem, Fitness> individual = new Individual<List<Genome>, Problem, Fitness>(genomes);
            //individual.Initialize();
            return individual;
        }
    }
}
