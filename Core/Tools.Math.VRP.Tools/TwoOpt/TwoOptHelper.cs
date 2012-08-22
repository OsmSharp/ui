using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core.Routes;

namespace Tools.Math.VRP.Core.TwoOpt
{
    /// <summary>
    /// Implements some generic 2 Opt functions.
    /// </summary>
    public class TwoOptHelper
    {
        /// <summary>
        /// Returns the customer that least increases the length of the given route.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="customers"></param>
        /// <returns></returns>
        public static TwoOptResult CalculateBestPlacement(
            IProblemWeights problem,
            IRoute route)
        {
            //int previous1 = -1;
            //foreach (int customer1 in route)
            //{
            //    if (previous1 >= 0)
            //    {
            //        int previous2 = -1;
            //        foreach (int customer2 in route)
            //        {
            //            if (previous1 != previous2)
            //            {
            //                // test the two opt move.

            //            }

            //            previous2 = customer2;
            //        }
            //    }

            //    previous1 = customer1;
            //}
            throw new NotImplementedException();
        }
    }
}
