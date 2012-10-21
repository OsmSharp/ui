// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
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
