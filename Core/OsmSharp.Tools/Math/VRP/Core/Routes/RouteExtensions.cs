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

namespace OsmSharp.Tools.Math.VRP.Core.Routes
{
    /// <summary>
    /// 
    /// </summary>
    public static class RouteExtensions
    {
        /// <summary>
        /// Calculates the weight of the route given the weights.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="weights"></param>
        public static double CalculateWeight(this IRoute route, IProblemWeights weights)
        {
            double weight = 0;
            int previous = -1;
            foreach(int customer in route)
            {
                if(previous >= 0)
                { // calculate the weight.
                    weight = weight + 
                        weights.WeightMatrix[previous][customer];
                }

                // set the previous and current.
                previous = customer;
            }

            // if the route is round add the last-first weight.
            if (route.IsRound)
            {
                weight = weight +
                    weights.WeightMatrix[previous][route.First];
            }
            return weight;
        }

        /// <summary>
        /// Calculates the weight of the route given the weights.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="is_round"></param>
        /// <param name="weights"></param>
        public static double CalculateWeight(this int[] route, bool is_round, IProblemWeights weights)
        {
            double weight = 0;
            int previous = -1;
            foreach (int customer in route)
            {
                if (previous >= 0)
                { // calculate the weight.
                    weight = weight +
                        weights.WeightMatrix[previous][customer];
                }

                // set the previous and current.
                previous = customer;
            }

            // if the route is round add the last-first weight.
            if (is_round)
            {
                weight = weight +
                    weights.WeightMatrix[previous][route[0]];
            }
            return weight;
        }
    }
}
