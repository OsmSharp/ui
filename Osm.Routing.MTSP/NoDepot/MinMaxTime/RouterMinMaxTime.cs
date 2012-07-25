using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Units.Time;
using Osm.Routing.Core.Route;
using Tools.Math.TSP.Problems;
using Tools.Math.VRP.Core;
using Osm.Core;

namespace Osm.Routing.Core.VRP.NoDepot.MinMaxTime
{
    /// <summary>
    /// Class to solve VRP problems that have no depot but min-max time constraints on routes.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public abstract class RouterMinMaxTime<ResolvedType> : RouterNoDepot<ResolvedType>
        where ResolvedType : ILocationObject
    {
        /// <summary>
        /// Creates a new min max VRP router.
        /// </summary>
        /// <param name="router"></param>
        public RouterMinMaxTime(IRouter<ResolvedType> router, Second min, Second max)
            :base(router)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Returns the minimum time a route is allowed to take.
        /// </summary>
        public Second Min { get; private set; }

        /// <summary>
        /// Returns the maximum time a route is allow to take.
        /// </summary>
        public Second Max { get; private set; }
        
        /// <summary>
        /// Calculates the actual VRP.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public override OsmSharpRoute[] CalculateNoDepot(ResolvedType[] points)
        {        
            /// Keeps a local copy of the current calculation points.
            /// 
            /// TODO: find a better solution to make this thread-safe!
            _points = points;

            // first calculate the weights in seconds.
            float[][] weights = this.CalculateManyToManyWeigth(points);

            // create the problem for the genetic algorithm.
            List<int> customers = new List<int>();
            for (int customer = 0; customer < points.Length; customer++)
            {
                customers.Add(customer);
            }
            MatrixProblem matrix = new MatrixProblem(weights, false);
            int[][] vrp_solution = this.DoCalculation(matrix, customers, this.Min, this.Max);

            // construct and return solution.
            return this.ConstructSolution(vrp_solution, null, points);
        }
        
        #region Intermidiate Results

        /// <summary>
        /// Keeps a local copy of the current calculation points.
        /// 
        /// TODO: find a better solution to make this thread-safe!
        /// </summary>
        private ResolvedType[] _points;

        /// <summary>
        /// Called when an intermidiate result is available.
        /// </summary>
        /// <param name="vrp_solution"></param>
        protected void DoIntermidiateResult(int[][] vrp_solution)
        {
            if (this.CanRaiseIntermidiateResult())
            {
                OsmSharpRoute[] result = this.ConstructSolution(vrp_solution, null, _points);
                this.RaiseIntermidiateResult(result, null);
            }
        }

        #endregion

        /// <summary>
        /// Implements the actual logic.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="customers"></param>
        /// <param name="second"></param>
        /// <param name="second_2"></param>
        /// <returns></returns>
        protected abstract int[][] DoCalculation(IProblemWeights problem,
            ICollection<int> customers, Second min, Second max);
    }
}
