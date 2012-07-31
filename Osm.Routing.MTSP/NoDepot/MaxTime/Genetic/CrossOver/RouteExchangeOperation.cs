using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core.Routes.ASymmetric;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic.Operations.CrossOver;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.VRP.Core.Routes;
using Tools.Math.VRP.Core.BestPlacement;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.Genetic.CrossOver
{
    internal class RouteExchangeOperation :
        ICrossOverOperation<MaxTimeSolution, MaxTimeProblem, Fitness>
    {
        public Individual<MaxTimeSolution, MaxTimeProblem, Fitness> CrossOver(
            Solver<MaxTimeSolution, MaxTimeProblem, Fitness> solver,
            Individual<MaxTimeSolution, MaxTimeProblem, Fitness> parent1,
            Individual<MaxTimeSolution, MaxTimeProblem, Fitness> parent2)
        {
            MaxTimeSolution route1 = parent1.Genomes;
            MaxTimeSolution route2 = parent2.Genomes;

            // get the minimum size of both routes.
            int size = route1.Count;
            if (route2.Count < size)
            {
                size = route2.Count;
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
                    selected_route = Tools.Math.Random.StaticRandomGenerator.Get().Generate(route1.Count);
                    while (selected_first.Contains(selected_route))
                    {
                        selected_route = Tools.Math.Random.StaticRandomGenerator.Get().Generate(route1.Count);
                    }

                    selected_first.Add(selected_route);
                    selected_routes.Add(route1.Route(selected_route));
                }
                else
                {
                    selected_route = Tools.Math.Random.StaticRandomGenerator.Get().Generate(route2.Count);
                    while (selected_second.Contains(selected_route))
                    {
                        selected_route = Tools.Math.Random.StaticRandomGenerator.Get().Generate(route2.Count);
                    }

                    selected_second.Add(selected_route);
                    selected_routes.Add(route2.Route(selected_route));
                }

                first = !first;
            }

            // generate the new customer genome.
            MaxTimeSolution solution = new MaxTimeSolution(route1.Size, true);

            int previous = -1;
            foreach (IRoute route in selected_routes)
            {
                IRoute current_route = null;
                foreach (int customer in route)
                {
                    //MaxTimeSolution copy = (solution.Clone() as MaxTimeSolution);
                    if (!solution.Contains(customer))
                    {
                        if (current_route == null)
                        { // add the route.
                            current_route = solution.Add(customer);

                            // safe the previous customer.
                            previous = customer;
                        }
                        else
                        { // add the customer.
                            current_route.Insert(previous, customer, current_route.First);

                            // safe the previous customer.
                            previous = customer;
                        }
                    }

                    //solution.ToString();
                    if (!solution.IsValid())
                    {
                        throw new Exception();
                    }
                }
            }

            // insert all non-placed customers in the order of the first route.
            for (int route_idx = 0; route_idx < route1.Count; route_idx++)
            {
                IRoute route1_route = route1.Route(route_idx);
                foreach (int customer in route1_route)
                {
                    if (!solution.Contains(customer))
                    {
                        //MaxTimeSolution copy = (solution.Clone() as MaxTimeSolution);

                        // try reinsertion.
                        CheapestInsertionResult result = new CheapestInsertionResult();
                        result.Increase = float.MaxValue;
                        int target_idx = -1;
                        for (int idx = 0; idx < solution.Count; idx++)
                        {
                            IRoute route = solution.Route(idx);

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

                        // get the target route and insert.
                        IRoute target_route = solution.Route(target_idx);
                        target_route.Insert(result.CustomerBefore, result.Customer, result.CustomerAfter);

                        //solution.ToString();
                        if (!solution.IsValid())
                        {
                            throw new Exception();
                        }
                    }
                }
            }

            if (!solution.IsValid())
            {
                throw new Exception();
            }

            return new Individual<MaxTimeSolution, MaxTimeProblem, Fitness>(solution);
        }

        public string Name
        {
            get
            {
                return "NotSet";
            }
        }
    }
}
