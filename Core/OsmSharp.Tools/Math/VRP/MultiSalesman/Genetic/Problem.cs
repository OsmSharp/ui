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
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.Units.Time;

namespace OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic
{
    /// <summary>
    /// Class representing a problem.
    /// </summary>
    internal abstract class Problem : IProblem
    {
        /// <summary>
        /// Creates a new problem.
        /// </summary>
        /// <param name="cities"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        public Problem(int cities,
            Second minimum,
            Second maximum)
        {
            this.InitialVehicles = 3;
            this.Cities = cities;
            this.TargetTime = (minimum.Value + maximum.Value) / 2.0;
            this.Tolerance = 0;
            this.MaximumTime = maximum;
            this.MinimumTime = minimum;
        }

        public int Tolerance { get; set; }
        public int TotalTolerance { get; set; }

        ///// <summary>
        ///// Gets the number of vehicles.
        ///// </summary>
        internal int InitialVehicles { get; set; }

        /// <summary>
        /// Gets the number of cities.
        /// </summary>
        public int Cities { get; set; }

        /// <summary>
        /// The ideal time a round should take.
        /// </summary>
        public Second TargetTime { get; set; }

        /// <summary>
        /// The maximum time.
        /// </summary>
        public Second MaximumTime { get; set; }

        /// <summary>
        /// The minimum time.
        /// </summary>
        public Second MinimumTime { get; set; }

        /// <summary>
        /// Returns the weight between city1 and city2.
        /// </summary>
        /// <param name="city1"></param>
        /// <param name="city2"></param>
        /// <returns></returns>
        public abstract double Weight(int city1, int city2);
    }
}
