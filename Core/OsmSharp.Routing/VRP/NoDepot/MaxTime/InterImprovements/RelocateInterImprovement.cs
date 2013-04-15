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
using OsmSharp.Tools.Math.VRP.Core.BestPlacement;
using OsmSharp.Tools.Math.VRP.Core;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.InterImprovements
{
    /// <summary>
    /// Relocate heurstic.
    /// </summary>
    public class RelocateImprovement : IInterImprovement
    {
        /// <summary>
        /// Return the name of this improvement.
        /// </summary>
        public string Name
        {
            get
            {
                return "REL";
            }
        }

        /// <summary>
        /// Returns true if this operation is symmetric.
        /// </summary>
        public bool IsSymmetric
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Tries to improve the existing routes by re-inserting a customer from one route into another.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="solution"></param>
        /// <param name="route1_idx"></param>
        /// <param name="route2_idx"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public bool Improve(MaxTimeProblem problem, MaxTimeSolution solution,
            int route1_idx, int route2_idx, double max)
        {
            if (this.RelocateFromTo(problem, solution, route1_idx, route2_idx, max))
            {
                return true;
            }
            if (this.RelocateFromTo(problem, solution, route2_idx, route1_idx, max))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries a relocation of the customers in route1 to route2.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="solution"></param>
        /// <param name="route1_idx"></param>
        /// <param name="route2_idx"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private bool RelocateFromTo(MaxTimeProblem problem, MaxTimeSolution solution,
            int route1_idx, int route2_idx, double max)
        {
            int previous = -1;
            int current = -1;

            IRoute route1 = solution.Route(route1_idx);
            IRoute route2 = solution.Route(route2_idx);

            double route2_weight = solution[route2_idx];

            foreach (int next in route1)
            {
                if (previous >= 0 && current >= 0)
                { // consider the next customer.

                    int count_before1 = route1.Count;
                    int count_before2 = route2.Count;

                    string route1_string = route1.ToString();
                    string route2_string = route2.ToString();

                    if (this.ConsiderCustomer(problem, route2, previous, current, next, route2_weight, max))
                    {
                        route1.ReplaceEdgeFrom(previous, next);

                        int count_after = route1.Count + route2.Count;
                        if ((count_before1 + count_before2) != count_after)
                        {
                            throw new Exception();
                        }
                        return true;
                    }
                }

                previous = current;
                current = next;
            }

            return false;
        }

        /// <summary>
        /// Considers one customer for relocation.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="previous"></param>
        /// <param name="current"></param>
        /// <param name="next"></param>
        /// <param name="route_weight"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private bool ConsiderCustomer(IProblemWeights problem, IRoute route, int previous, int current, int next, double route_weight, double max)
        {
            // calculate the removal gain of the customer.
            double removal_gain = problem.WeightMatrix[previous][current] + problem.WeightMatrix[current][next]
                - problem.WeightMatrix[previous][next];
            if (removal_gain > 0.0001)
            {
                // try and place the customer in the next route.
                CheapestInsertionResult result =
                    CheapestInsertionHelper.CalculateBestPlacement(problem, route, current);
                if (result.Increase < removal_gain - 0.001 && route_weight + result.Increase < max)
                { // there is a gain in relocating this customer.
                    int count_before = route.Count;
                    string route_string = route.ToString();

                    // and the route is still within bounds!
                    route.ReplaceEdgeFrom(result.CustomerBefore, result.Customer);
                    route.ReplaceEdgeFrom(result.Customer, result.CustomerAfter);

                    int count_after = route.Count;
                    if (count_before + 1 != count_after)
                    {
                        throw new Exception();
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
