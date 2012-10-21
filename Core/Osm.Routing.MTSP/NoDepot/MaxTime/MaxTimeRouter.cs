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
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;
using Osm.Routing.Core.Route;
using Tools.Math.TSP.Problems;
using Tools.Math.Units.Time;
using Tools.Math.VRP.Core;
using Tools.Math.VRP.Core.Routes;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime
{
    /// <summary>
    /// Class to solve VRP problems that have no depot but min-max time constraints on routes.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public abstract class RouterMaxTime<ResolvedType> : RouterNoDepot<ResolvedType>, IMaxTimeSolver
        where ResolvedType : IResolvedPoint
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
        /// Returns the name of this solver.
        /// </summary>
        public abstract string Name
        {
            get;
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

            // convert to ints.
            for (int x = 0; x < weights.Length; x++)
            {
                float[] weights_x = weights[x];
                for (int y = 0; y < weights_x.Length; y++)
                {
                    weights_x[y] = (int)weights_x[y];
                }
            }

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
        public int[][] DoCalculation(MaxTimeProblem problem,
            ICollection<int> customers, Second max)
        {
            MaxTimeSolution routes = this.Solve(problem);

            // convert output.
            int[][] solution = new int[routes.Count][];
            for (int idx = 0; idx < routes.Count; idx++)
            {
                IRoute current = routes.Route(idx);
                //IRoute current = routes[idx];
                List<int> route = new List<int>(current);
                if (current.IsRound)
                {
                    route.Add(route[0]);
                }
                solution[idx] = route.ToArray();
            }
            return solution;
        }

        #region IMaxTimeSolver Implementation

        /// <summary>
        /// Executing a solver function.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        internal abstract MaxTimeSolution Solve(MaxTimeProblem problem);

        /// <summary>
        /// Implements the solve function of the IMaxTimeSolver interface
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        MaxTimeSolution IMaxTimeSolver.Solve(MaxTimeProblem problem)
        {
            return this.Solve(problem);
        }

        #endregion
    }
}
