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
using OsmSharp.Tools.Math.TSP.LK.SparseSets;
using OsmSharp.Tools.Math.TSP.Problems;
using System.Diagnostics;
using OsmSharp.Tools.Math.TSP.ArbitraryInsertion;
using OsmSharp.Tools.Math.VRP.Core.Routes.Symmetric;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;
using OsmSharp.Tools.Collections;

namespace OsmSharp.Tools.Math.TSP.LK
{
    /// <summary>
    /// A solver that use the Lin-Kernighan heuristic to solve instances of the TSP.
    /// </summary>
    public class LinKernighanSolver : SolverBase
    {
        private SparseSet _sparse_set;

        private bool _was_asym;

        //private bool _stopped;

        private IList<int> _customers;

        internal LinKernighanSolver()
        {    
        
        }

        /// <summary>
        /// Retuns the name of this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                return "LinKernighan";
            }
        }

        /// <summary>
        /// Does the actual solving.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        protected override IRoute DoSolve(IProblem problem)
        {
            // convert to a symetric problem if needed.
            IProblem _problem = problem;
            if (!_problem.Symmetric)
            {
                _problem = Convertor.ConvertToSymmetric(_problem);
                _was_asym = true;
            }

            // create the list of customers.
            _customers = new List<int>();
            for (int customer = 0; customer < _problem.Size; customer++)
            {
                _customers.Add(customer);
            }
            _sparse_set = SparseSetHelper.CreateNearestNeighourSet(_problem, _customers, _customers.Count / 10);
            //_sparse_set = SparseSetHelper.CreateNonSparseSet(_problem, _customers);     
            // construct a route from the customers.
            //FixedSymmetricRoute init_route = new FixedSymmetricRoute(_customers);

            // construct a random route using best-placement.
            ArbitraryInsertionSolver bp_solver = new ArbitraryInsertionSolver();
            IRoute bp_route = bp_solver.Solve(_problem);
            FixedSymmetricRoute init_route = new FixedSymmetricRoute(bp_route);

            double init_route_weight = LinKernighanSolver.Weight(_problem, init_route);
            RouteFound route = new RouteFound()
            {
                Route = init_route,
                RouteWeight = init_route_weight
            };
            Console.WriteLine("Route {0}:{1}",
                route.Route.ToString(),
                route.RouteWeight);

            // step 2.
            EdgeSet X = new EdgeSet();
            EdgeSet Y = new EdgeSet();
            IList<int> untried_t_1 = new List<int>(route.Route);
            while (untried_t_1.Count > 0)
            {
                // select t1.
                int t_1 = untried_t_1[0];
                untried_t_1.RemoveAt(0);

                // search route with t_1.
                RouteFound t_1_route = this.AfterSelectt1(_problem, route, X, Y, t_1);

                // select the better route.
                if (t_1_route.RouteWeight < route.RouteWeight)
                {
                    untried_t_1 = new List<int>(route.Route);
                    route = RouteFound.SelectBest(route, t_1_route);
                    X = new EdgeSet();
                    Y = new EdgeSet();
                }
            } // step 2 and step 12.

            // convert back to asym solution if needed.
            //result.RemoveAt(result.Count - 1);
            if (_was_asym)
            {
                return this.ConvertToASymRoute(new List<int>(route.Route));
            }
            return route.Route;
        }

        private IRoute ConvertToASymRoute(IList<int> route)
        {
            List<int> converted = new List<int>();
            int n = route.Count / 2;
            int original = -1;
            for (int idx = 0; idx < route.Count; idx++)
            {
                route[idx] = route[idx] % n;

                if(original != route[idx])
                {
                    converted.Add(route[idx]);
                    original = route[idx];
                }                
            }
            if (converted[0] == original)
            {
                converted.RemoveAt(converted.Count - 1);
            }
            return new SimpleAsymmetricRoute(converted, true);
        }

        private RouteFound AfterSelectt1(IProblem problem, RouteFound route, EdgeSet X, EdgeSet Y, int t_1)
        {
            // try and find a better route with t_1.
            RouteFound new_route = new RouteFound()
            {
                RouteWeight = float.MaxValue,
                Route = null
            };

            // step 3 and step 11.
            HashSet<int> t_2_exceptions = new HashSet<int>();
            EdgeList x = new EdgeList();
            int? result = this.SelectX(problem, route.Route, X, Y, x, t_1, t_2_exceptions);
            while (result != null)
            {
                // select t_2.
                int t_2 = result.Value;
                t_2_exceptions.Add(t_2);

                // add x_1 to the edge list.
                Edge x_edge = new Edge()
                {
                    From = t_1,
                    To = t_2,
                    Weight = problem.Weight(t_1, t_2)
                };
                x.Add(x_edge);
                X.Add(x_edge);

                // search route with t_2.
                new_route = this.AfterSelectt2(problem, route, X, Y, x);
                if (new_route.Route != null &&
                    new_route.RouteWeight < route.RouteWeight)
                { // trackback to t_1 if a better route is found.
                    break;
                }

                // remove x_1 again and backtrack.
                x.RemoveLast();

                result = this.SelectX(problem, route.Route, X, Y, x, t_1, t_2_exceptions);
            } // step 3 and step 11.
            return new_route;
        }

        private RouteFound AfterSelectt2(IProblem problem, RouteFound route, EdgeSet X, EdgeSet Y, EdgeList x)
        {
            int t_2 = x[0].To;

            // try and find a better route with t_1.
            RouteFound new_route = new RouteFound()
            {
                RouteWeight = float.MaxValue,
                Route = null
            };

            // step 4 and step 10.
            HashSet<int> t_3_exceptions = new HashSet<int>();
            EdgeList y = new EdgeList();
            int? result = this.SelectY(problem, route.Route, X, Y, x, y, t_3_exceptions);
            while (result != null)
            {
                // select t_3.
                int t_3 = result.Value;
                t_3_exceptions.Add(t_3);

                // add y_1 to the edge list.
                Edge y_edge = new Edge()
                {
                    From = t_2,
                    To = t_3,
                    Weight = problem.Weight(t_2, t_3)
                };
                y.Add(y_edge);
                Y.Add(y_edge);

                // search route with t_3.
                new_route = this.AfterSelectt3(problem, route, X, Y, x, y);
                if (new_route.Route != null &&
                    new_route.RouteWeight < route.RouteWeight)
                { // trackback to t_1 if a better route is found.
                    break;
                }

                // remove y_1 again and backtrack.
                y.RemoveLast();

                if (x.Count != y.Count + 1)
                {
                    OsmSharp.Tools.Output.OutputStreamHost.Write(string.Empty);
                }

                result = this.SelectY(problem, route.Route, X, Y, x, y, t_3_exceptions);
            }
            return new_route;
        }

        private RouteFound AfterSelectt3(IProblem problem, RouteFound route, EdgeSet X, EdgeSet Y, EdgeList x, EdgeList y)
        {
            int t_1 = x[0].From;
            int t_3 = y[0].To;

            // try and find a better route with t_1.
            RouteFound new_route = new RouteFound()
            {
                RouteWeight = float.MaxValue,
                Route = null
            };

            // choose t_4 for backtracking later.
            HashSet<int> t_4_exceptions = new HashSet<int>();
            int? result = this.SelectX(problem, route.Route, X, Y, x, t_3, t_4_exceptions);
            while (result != null)
            {
                // select t_4.       
                int t_4 = result.Value;
                t_4_exceptions.Add(t_4);

                // add to x and test with perliminary y.
                Edge x_edge = new Edge()
                {
                    From = t_3,
                    To = t_4,
                    Weight = problem.Weight(t_3, t_4)
                };
                x.Add(x_edge);
                X.Add(x_edge);

                // Test the route T' for feasability and weight.
                y.Add(new Edge()
                {
                    From = t_4,
                    To = t_1,
                    Weight = problem.Weight(t_4, t_1)
                });
                new_route.Route = this.Replace(route.Route, x, y);
                y.RemoveLast(); // remove the perliminary y.
                if (new_route.Route.IsValid())
                {
                    // stop the search for now if the route is already better.
                    new_route.RouteWeight = LinKernighanSolver.Weight(problem, new_route.Route);
                    if (new_route.RouteWeight < route.RouteWeight)
                    { // break.
                        Console.WriteLine("Route {0}:{1}",
                            new_route.Route.ToString(),
                            new_route.RouteWeight);
                        x.RemoveLast(); // remove the latest x here!
                        break;
                    }
                }

                // choose t_5 here.
                new_route = this.AfterSelectt4(problem, route, X, Y, x, y);
                if (new_route.Route != null &&
                    new_route.RouteWeight < route.RouteWeight)
                { // trackback to t_1 if a better route is found.
                    break;
                }

                // reset the new route.
                new_route.Route = null;
                new_route.RouteWeight = float.MaxValue;

                // remove and backtrack x_2.
                x.RemoveLast();

                if (x.Count != y.Count)
                {
                    OsmSharp.Tools.Output.OutputStreamHost.Write(string.Empty);
                }

                result = this.SelectX(problem, route.Route, X, Y, x, t_3, t_4_exceptions);
            } // choose t_4
            return new_route;
        }

        private RouteFound AfterSelectt4(IProblem problem, RouteFound route, EdgeSet X, EdgeSet Y, EdgeList x, EdgeList y)
        {
            int t_1 = x[0].From;
            int t_3 = y[0].To;
            int t_4 = x[1].To;

            // try and find a better route with t_1.
            RouteFound new_route = new RouteFound()
            {
                RouteWeight = float.MaxValue,
                Route = null
            };

            // choose t_5 for backtracking later.
            HashSet<int> t_5_exceptions = new HashSet<int>();
            int? result = this.SelectY(problem, route.Route, X, Y, x, y, t_5_exceptions);
            while (result != null)
            {
                // choose t_5.
                int t_5 = result.Value;
                t_5_exceptions.Add(t_5);
                Edge y_2 = new Edge()
                {
                    From = t_4,
                    To = t_5,
                    Weight = problem.Weight(t_4, t_5)
                };
                y.Add(y_2);
                Y.Add(y_2);


                if (x.Count != y.Count)
                {
                    OsmSharp.Tools.Output.OutputStreamHost.Write(string.Empty);
                }

                // try and find a better route by selecting more customer.
                new_route = this.AfterSelectt5(problem, route, X, Y, x, y);
                if (new_route.Route != null &&
                    new_route.RouteWeight < route.RouteWeight)
                { // trackback to t_1 if a better route is found.
                    break;
                }

                // remove and backtrack y_2.
                y.RemoveLast();

                if (x.Count != y.Count + 1)
                {
                    OsmSharp.Tools.Output.OutputStreamHost.Write(string.Empty);
                }

                result = this.SelectY(problem, route.Route, X, Y, x, y, t_5_exceptions);
            } // choose t_5
            return new_route;
        }

        private RouteFound AfterSelectt5(IProblem problem, RouteFound route, EdgeSet X, EdgeSet Y, EdgeList x, EdgeList y)
        {
            if (x.Count != y.Count)
            {
                OsmSharp.Tools.Output.OutputStreamHost.Write(string.Empty);
            }
            int t_1 = x[0].From;
            int t_2i_min_1 = y[y.Count - 1].To;

            // try and find a better route with t_1.
            RouteFound new_route = new RouteFound()
            {
                RouteWeight = float.MaxValue,
                Route = null
            };

            HashSet<int> exceptions = new HashSet<int>();
            int? result = this.SelectX(problem, route.Route, X, Y, x, t_2i_min_1, exceptions);
            while (result != null) // do backtacking over x: TODO: improve this and immidiately find the correct tour.
            {
                int t_2i = result.Value;
                exceptions.Add(t_2i);

                // step 6.
                t_2i = result.Value;
                Edge x_edge = new Edge()
                {
                    From = t_2i_min_1,
                    To = t_2i,
                    Weight = problem.Weight(t_2i_min_1, t_2i)
                };
                x.Add(x_edge);
                X.Add(x_edge);

                // Test the route T' for feasability and weight.
                y.Add(new Edge()
                {
                    From = t_2i,
                    To = t_1,
                    Weight = problem.Weight(t_2i, t_2i)
                });
                new_route.Route = this.Replace(route.Route, x, y);
                y.RemoveLast(); // remove the perliminary y.

                if (x.Count != y.Count + 1)
                {
                    OsmSharp.Tools.Output.OutputStreamHost.Write(string.Empty);
                }

                if (new_route.Route.IsValid())
                {
                    // stop the search for now if the route is already better.
                    new_route.RouteWeight = LinKernighanSolver.Weight(problem, new_route.Route);
                    if (new_route.RouteWeight < route.RouteWeight)
                    { // break.
                        Console.WriteLine("Route {0}:{1}",
                            new_route.Route.ToString(),
                            new_route.RouteWeight);
                        return new_route;
                    }

                    // choose next y.
                    result = this.SelectY(problem, route.Route, X, Y, x, y, null);
                    if (result != null)
                    {
                        int t_2_plus_1 = result.Value;
                        Edge y_edge = new Edge()
                        {
                            From = t_2i,
                            To = t_2_plus_1,
                            Weight = problem.Weight(t_2i, t_2_plus_1)
                        };
                        y.Add(y_edge);
                        Y.Add(y_edge);

                        // choose next.
                        new_route = this.AfterSelectt5(problem, route, X, Y, x, y);
                        if (new_route.RouteWeight < route.RouteWeight)
                        { // break.
                            Console.WriteLine("Route {0}:{1}",
                                new_route.Route.ToString(),
                                new_route.RouteWeight);
                            return new_route;
                        }

                        // remove y again.
                        y.RemoveLast();
                    }
                    x.RemoveLast();
                    break; // the other x cannot be valid!
                }

                // backtrack over x.
                x.RemoveLast();
                result = this.SelectX(problem, route.Route, X, Y, x, t_2i_min_1, exceptions);
            }
            if (x.Count != y.Count)
            {
                OsmSharp.Tools.Output.OutputStreamHost.Write(string.Empty);
            }
            return new_route;
        }

        private struct RouteFound
        {
            public FixedSymmetricRoute Route { get; set; }
            public double RouteWeight { get; set; }

            public static RouteFound SelectBest(RouteFound route1, RouteFound route2)
            {
                if (route1.RouteWeight <= route2.RouteWeight)
                {
                    return route1;
                }
                else
                {
                    return route2;
                }
            }
        }

        private static double Weight(IProblem problem, ISymmetricRoute route)
        {
            int previous = -1;
            double weight = 0;
            foreach (int customer in route)
            {
                if (previous >= 0)
                {
                    weight = weight +
                        problem.Weight(previous, customer);
                }
                previous = customer;
            }
            weight = weight +
                problem.Weight(previous, 0);
            return weight;
        }

        /// <summary>
        /// Replaces some edges with a list of other edges.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private FixedSymmetricRoute Replace(FixedSymmetricRoute route, EdgeList x, EdgeList y)
        {
            if (x.Count != y.Count)
            {
                OsmSharp.Tools.Output.OutputStreamHost.Write(string.Empty);
            }
            FixedSymmetricRoute route_new = route.Clone() as FixedSymmetricRoute;
            for (int idx = 0; idx < x.Count; idx++)
            {
                Edge x_edge = x[idx];
                route_new.Remove(x_edge.From, x_edge.To);
            }
            for (int idx = 0; idx < x.Count; idx++)
            {
                Edge y_edge = y[idx];
                route_new.Add(y_edge.From, y_edge.To);
            }
            //if (!route_new.IsValidNew() && route_new.IsValid())
            //{
            //    Debug.Write(string.Empty);
            //}
            //else
            //{
            //    Debug.Write(string.Empty);
            //}
            return route_new;
        }

        ///// <summary>
        ///// A naive selection method.
        ///// </summary>
        ///// <param name="problem"></param>
        ///// <param name="route"></param>
        ///// <param name="X"></param>
        ///// <param name="Y"></param>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <param name="exceptions"></param>
        ///// <returns></returns>
        //private int? SelectY(
        //    IProblem problem, FixedSymmetricRoute route, EdgeSet X, EdgeSet Y, EdgeList x, EdgeList y, HashSet<int> exceptions)
        //{
        //    int from = x.Last.To;

        //    // search an edge with weight smaller than g.
        //    // VERY NAIVE METHOD!
        //    List<int> test_list = new List<int>();
        //    float best_extra = float.MaxValue;
        //    int? best_customer = null;
        //    for (int other_customer = 0; other_customer < problem.Size; other_customer++)
        //    {
        //        if (other_customer != from && exceptions != null && !exceptions.Contains(other_customer))
        //        {
        //            if (!x.Contains(from, other_customer)
        //                && !route.Contains(from, other_customer))
        //            //&& !Y.Contains(from, other_customer))
        //            //&& !X.Contains(from, other_customer))
        //            {
        //                float extra = problem.Weight(from, other_customer);
        //                if (y.Weight + extra < x.Weight)
        //                {
        //                    if (x.Count < 3)
        //                    {
        //                        best_customer = other_customer;
        //                        best_extra = extra;
        //                        break;
        //                    }
        //                    test_list.Add(other_customer);
        //                    if (extra < best_extra)
        //                    {
        //                        best_customer = other_customer;
        //                        best_extra = extra;
        //                        if (x.Count < 3)
        //                        {
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return best_customer;
        //}

        /// <summary>
        /// Selects with special priority.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        private int? SelectY(
            IProblem problem, FixedSymmetricRoute route, EdgeSet X, EdgeSet Y, EdgeList x, EdgeList y, HashSet<int> exceptions)
        {
            int from = x.Last.To;

            // search an edge with weight smaller than g.
            //float best_priority = float.MinValue;
            int? best_customer = null;
            foreach (Edge edge in _sparse_set.GetFor(from))
            {
                if (exceptions == null ||
                    !exceptions.Contains(edge.To))
                {
                    if (edge.From != edge.To
                        && !x.Contains(edge)
                        && !y.Contains(edge)
                        && !route.Contains(edge.From, edge.To)
                        //&& !Y.Contains(from, other_customer))
                        && !X.Contains(from, edge.To))
                    {
                        double extra = edge.Weight;
                        if (y.Weight + extra < x.Weight)
                        {
                            //if (x.Count < 2)
                            //{
                                best_customer = edge.To;
                                break;
                            //}

                            //// calculate priority.
                            //int? x_other = this.SelectX(problem, route, X, Y, x, edge.To, null);
                            //float priority = float.MinValue;
                            //if (x_other != null)
                            //{
                            //    float x_other_weight = problem.Weight(edge.To, x_other.Value);
                            //    priority = x_other_weight - extra;
                            //}
                            //if (priority > best_priority)
                            //{
                            //    best_customer = edge.To;
                            //    best_priority = priority;
                            //}
                        }
                    }
                }
            }
            return best_customer;
        }


        private int? SelectX(
            IProblem problem, FixedSymmetricRoute route, EdgeSet X, EdgeSet Y, EdgeList x, int customer, HashSet<int> exceptions)
        {
            // select the only two edges that have the given customer in the given route.
            int[] neigbours = route.GetNeigbours(customer);
            int previous = neigbours[0];
            int next = neigbours[1];

            int? best_neighour = null;
            double best_weight = double.MinValue;
            if (previous > 0 && (exceptions == null || !exceptions.Contains(previous))
                && !x.Contains(customer, previous))
                //&& !X.Contains(customer, previous))
                //&& !Y.Contains(customer, previous))
            {
                //if (x.Count > 2)
                //{
                double weight = problem.Weight(customer, previous);
                if (weight > best_weight)
                {
                    best_neighour = previous;
                    best_weight = weight;
                }
                //}
                //else
                //{
                    return previous;
                //}
            }
            if (next > 0 && (exceptions == null || !exceptions.Contains(next))
                && !x.Contains(customer, next))
                //&& !X.Contains(customer, previous))
                //&& !Y.Contains(customer, previous))
            {
                //if (x.Count > 2)
                //{
                double weight = problem.Weight(customer, next);
                if (weight > best_weight)
                {
                    best_neighour = next;
                    best_weight = weight;
                }
                //}
                //else
                //{
                    return next;
                //}
            }
            return best_neighour;
        }

        /// <summary>
        /// Stops this solver.
        /// </summary>
        public override void Stop()
        {
            //_stopped = true;
        }
    }
}
