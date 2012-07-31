using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;
using Osm.Routing.Core.Route;
using Tools.Math.TSP.Problems;
using Tools.Math.Units.Time;
using Tools.Math.VRP.Core;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime
{
    /// <summary>
    /// Class to solve VRP problems that have no depot but min-max time constraints on routes.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public abstract class RouterMaxTime<ResolvedType> : RouterNoDepot<ResolvedType>
        where ResolvedType : ILocationObject
    {
        /// <summary>
        /// Creates a new min max VRP router.
        /// </summary>
        /// <param name="router"></param>
        public RouterMaxTime(IRouter<ResolvedType> router, Second max, Second delivery_time)
            :base(router)
        {
            this.Max = max;
            this.DeliveryTime = delivery_time;
        }

        /// <summary>
        /// Returns the maximum time a route is allow to take.
        /// </summary>
        public Second Max { get; private set; }

        /// <summary>
        /// The average time a delivery taks.
        /// </summary>
        public Second DeliveryTime { get; private set; }
        
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
            MaxTimeProblem problem = new MaxTimeProblem(this.Max, this.DeliveryTime, matrix);
            int[][] vrp_solution = this.DoCalculation(problem, customers, this.Max);

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
        public abstract int[][] DoCalculation(MaxTimeProblem problem,
            ICollection<int> customers, Second max);
    }
}
