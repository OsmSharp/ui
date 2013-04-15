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
using OsmSharp.Tools.Math.TSP.Problems;
using OsmSharp.Tools.Math.Units.Time;
using OsmSharp.Tools.Math.VRP.Core;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Routing;
using OsmSharp.Routing.Route;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Collections;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime
{
    /// <summary>
    /// Class to solve VRP problems that have no depot but min-max time constraints on routes.
    /// </summary>
    public abstract class RouterMaxTime : RouterNoDepot, IMaxTimeSolver
    {
        /// <summary>
        /// Creates a new min max VRP router.
        /// </summary>
        public RouterMaxTime(Second max, Second delivery_time)
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
        /// Calculates the No-depot VRP solution.
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="locations"></param>
        /// <returns></returns>
        public override int[][] CalculateNoDepot(double[][] weights, GeoCoordinate[] locations)
        {        
            // Keeps a local copy of the current calculation points.
            // 
            // TODO: find a better solution to make this thread-safe!
            _locations = locations;

            // convert to ints.
            for (int x = 0; x < weights.Length; x++)
            {
                double[] weights_x = weights[x];
                for (int y = 0; y < weights_x.Length; y++)
                {
                    weights_x[y] = weights_x[y];
                }
            }

            // create the problem for the genetic algorithm.
            MatrixProblem matrix = MatrixProblem.CreateATSP(weights);
            MaxTimeProblem problem = new MaxTimeProblem(matrix, this.Max, this.DeliveryTime, 10, 1000);
            List<int> customers = new List<int>();
            for (int customer = 0; customer < _locations.Length; customer++)
            {
                customers.Add(customer);
                problem.CustomerPositions.Add(
                    _locations[customer]);
            }

            MaxTimeSolution routes = this.DoCalculation(problem, customers, this.Max);

            // convert output.
            int[][] vrp_solution = new int[routes.Count][];
            double[] vrp_solution_weights = new double[routes.Count];
            for (int idx = 0; idx < routes.Count; idx++)
            {
                // get the route.
                IRoute current = routes.Route(idx);

                // calculate the weight.
                vrp_solution_weights[idx] = problem.Time(current);

                OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Route {0}: {1}s",
                    idx, vrp_solution_weights[idx]);

                // convert the route.
                List<int> route = new List<int>(current);
                if (current.IsRound)
                {
                    route.Add(route[0]);
                }
                vrp_solution[idx] = route.ToArray();
            }

            // construct and return solution.
            return vrp_solution;
        }

        /// <summary>
        /// Calculates a bounding box.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        protected GeoCoordinateBox CalculateBox(MaxTimeProblem problem, IRoute route)
        {
            HashSet<GeoCoordinate> coordinates = new HashSet<GeoCoordinate>();
            foreach (int customer in route)
            {
                coordinates.Add(problem.CustomerPositions[customer]);
            }
            if (coordinates.Count == 0)
            {
                return null;
            }
            return new GeoCoordinateBox(coordinates.ToArray());
        }

        /// <summary>
        /// Returns true if the routes overlap.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route1"></param>
        /// <param name="route2"></param>
        /// <returns></returns>
        protected bool Overlaps(MaxTimeProblem problem, IRoute route1, IRoute route2)
        {
            GeoCoordinateBox route1_box = this.CalculateBox(problem, route1);
            GeoCoordinateBox route2_box = this.CalculateBox(problem, route2);
            if (route1_box != null && route2_box != null)
            {
                return route1_box.Overlaps(route2_box);
            }
            return false;
        }

        #region Intermidiate Results

        /// <summary>
        /// Keeps a local copy of the current calculation points.
        /// </summary>
        private GeoCoordinate[] _locations;

        #endregion


        /// <summary>
        /// Implements the actual logic.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="customers"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public MaxTimeSolution DoCalculation(MaxTimeProblem problem,
            ICollection<int> customers, Second max)
        {
            MaxTimeSolution routes = this.Solve(problem);

            return routes;
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
