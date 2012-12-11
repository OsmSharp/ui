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

namespace OsmSharp.Routing.Core.VRP.NoDepot.MaxTime.InterRoute
{
    /// <summary>
    /// Relocate heurstic.
    /// </summary>
    public class RelocateImprovement : IInterRouteImprovement
    {
        /// <summary>
        /// Tries to improve the existing routes by re-inserting a customer from one route into another.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route1"></param>
        /// <param name="route2"></param>
        /// <param name="difference"></param>
        /// <returns></returns>
        public bool Improve(MaxTimeProblem problem, IRoute route1, IRoute route2, out double difference)
        {
            if (this.RelocateFromTo(problem, route1, route2, out difference))
            {
                return true;
            }
            if (this.RelocateFromTo(problem, route2, route1, out difference))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries a relocation of the customers in route1 to route2.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route1"></param>
        /// <param name="route2"></param>
        /// <param name="difference"></param>
        /// <returns></returns>
        private bool RelocateFromTo(MaxTimeProblem problem, IRoute route1, IRoute route2, out double difference)
        {
            int previous = -1;
            int current = -1;
            foreach (int next in route1)
            {
                if (previous >= 0 && current >= 0)
                { // consider the next customer.
                    if (this.ConsiderCustomer(problem, route2, previous, current, next, out difference))
                    {
                        route1.Remove(current);
                        break;
                    }
                }

                previous = current;
                current = next;
            }
            difference = 0;
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
        /// <returns></returns>
        private bool ConsiderCustomer(MaxTimeProblem problem, IRoute route, int previous, int current, int next, out double difference)
        {
            // calculate the removal gain of the customer.
            double removal_gain = problem.WeightMatrix[previous][current] + problem.WeightMatrix[current][next]
                - problem.WeightMatrix[previous][next];
            if (removal_gain > 0)
            {
                // try and place the customer in the next route.
                CheapestInsertionResult result = 
                    CheapestInsertionHelper.CalculateBestPlacement(problem, route, current);
                if (result.Increase < removal_gain)
                { // there is a gain in relocating this customer.
                    difference = result.Increase - removal_gain;

                    //route.InsertAfterAndRemove(result.CustomerBefore, result.Customer, result.CustomerAfter);
                    route.InsertAfter(result.CustomerBefore, result.Customer);
                }
            }
            difference = 0;
            return false;
        }
    }
}
