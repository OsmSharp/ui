using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic;
using Tools.Math.Units.Time;

namespace Tools.Math.VRP.MultiSalesman.Genetic
{
    /// <summary>
    /// Class representing a problem.
    /// </summary>
    /// <typeparam name="Fitness"></typeparam>
    internal abstract class Problem : IProblem
    {
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
        /// <param name="city1"></param>
        /// <returns></returns>
        public abstract float Weight(int city1, int city2);
    }
}
