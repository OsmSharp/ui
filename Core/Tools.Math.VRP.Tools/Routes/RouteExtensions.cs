using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.VRP.Core.Routes
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
        public static float CalculateWeight(this IRoute route, IProblemWeights weights)
        {
            float weight = 0;
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
    }
}
