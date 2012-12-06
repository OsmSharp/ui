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
using OsmSharp.Tools.Math.TSP;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Routing.Core;
using OsmSharp.Routing.Core.Route;

namespace OsmSharp.Osm.Routing.Core.TSP
{
    /// <summary>
    /// Router that calculates TSP solutions.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public abstract class RouterTSP<ResolvedType>
        where ResolvedType : IRouterPoint
    {
        /// <summary>
        /// Holds the basic router.
        /// </summary>
        private IRouter<ResolvedType> _router;

        /// <summary>
        /// Creates a new TSP router.
        /// </summary>
        /// <param name="router"></param>
        public RouterTSP(IRouter<ResolvedType> router)
        {
            _router = router;
        }

        /// <summary>
        /// Calculates a weight matrix for the given array of points.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        protected double[][] CalculateManyToManyWeight(VehicleEnum vehicle, ResolvedType[] points)
        {
            return _router.CalculateManyToManyWeight(vehicle, points, points);
        }

        /// <summary>
        /// Calculates the shortest route along all given points starting and ending at the given points.
        /// </summary>
        /// <param name="points">The points to travel along.</param>
        /// <param name="first">The point to start from.</param>
        /// <param name="is_round">Return back to the first point or not.</param>
        /// <returns></returns>
        public OsmSharpRoute CalculateTSP(VehicleEnum vehicle, ResolvedType[] points, int first, bool is_round)
        {
            // calculate the weights.
            double[][] weights = this.CalculateManyToManyWeight(vehicle, points);

            // create solver.
            ISolver solver = this.DoCreateSolver(points.Length);
            solver.IntermidiateResult += new SolverDelegates.IntermidiateDelegate(solver_IntermidiateResult);

            // calculate the TSP.
            IRoute tsp_solution = solver.Solve(this.GenerateProblem(weights, first, null, is_round));

            // concatenate the route(s).
            return this.BuildRoute(points, tsp_solution);
        }

        /// <summary>
        /// Calculates the shortest route along all given points starting and ending at the given points.
        /// </summary>
        /// <param name="points">The points to travel along.</param>
        /// <param name="first">The point to start from.</param>
        /// <param name="last">The point to end at.</param>
        /// <returns></returns>
        public OsmSharpRoute CalculateTSP(VehicleEnum vehicle, ResolvedType[] points, int first, int last)
        {
            // calculate the weights.
            double[][] weights = this.CalculateManyToManyWeight(vehicle, points);

            // create solver.
            ISolver solver = this.DoCreateSolver(points.Length);
            solver.IntermidiateResult += new SolverDelegates.IntermidiateDelegate(solver_IntermidiateResult);

            // calculate the TSP.
            IRoute tsp_solution = solver.Solve(this.GenerateProblem(weights, first, last, false));

            // concatenate the route(s).
            return this.BuildRoute(points, tsp_solution);
        }

        /// <summary>
        /// Calcculates the shortest route along all given points.
        /// </summary>
        /// <param name="points">The points to travel along.</param>
        /// <param name="is_round">Make the route a round or not.</param>
        /// <returns></returns>
        public OsmSharpRoute CalculateTSP(VehicleEnum vehicle, ResolvedType[] points, bool is_round)
        {
            // calculate the weights.
            double[][] weights = this.CalculateManyToManyWeight(vehicle, points);

            // create solver.
            ISolver solver = null;
            if (points.Length < 6)
            { // creates a bute force solver.
                solver = new OsmSharp.Tools.Math.TSP.BruteForce.BruteForceSolver();
            }
            else
            { // creates a solver for the larger problems.
                solver = this.DoCreateSolver(points.Length);
            }
            solver.IntermidiateResult += new SolverDelegates.IntermidiateDelegate(solver_IntermidiateResult);

            // calculate the TSP.
            IRoute tsp_solution = solver.Solve(this.GenerateProblem(weights, null, null, is_round));

            // concatenate the route(s).
            return this.BuildRoute(points, tsp_solution);
        }

        /// <summary>
        /// Calculates the shortest route along all given points returning back to the first.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public OsmSharpRoute CalculateTSP(VehicleEnum vehicle, ResolvedType[] points)
        {
            return this.CalculateTSP(vehicle, points, true);
        }

        /// <summary>
        /// Raise intermidiate result event.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="weight"></param>
        void solver_IntermidiateResult(int[] result, double weight)
        {
            if (this.CanRaiseIntermidiateResult())
            {
                this.RaiseIntermidiateResult(result, weight);
            }
        }

        /// <summary>
        /// Builds an OsmSharRoute.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="found_solution"></param>
        /// <returns></returns>
        public OsmSharpRoute BuildRoute(ResolvedType[] points, IRoute found_solution)
        {
            List<int> solution = new List<int>(found_solution); // TODO: improve this whole part to loop over the orginal route!

            OsmSharpRoute tsp = null;
            OsmSharpRoute route;
            for (int idx = 0; idx < solution.Count - 1; idx++)
            {
                route = _router.Calculate(VehicleEnum.Car, points[solution[idx]],
                    points[solution[idx + 1]]);
                if (tsp == null)
                { // first route = start
                    tsp = route;
                }
                else
                { // concatenate.
                    tsp = OsmSharpRoute.Concatenate(tsp, route);
                }
            }
            if (found_solution.IsRound)
            {
                // concatenate the route from the last to the first point again.
                route = _router.Calculate(VehicleEnum.Car, points[solution[solution.Count - 1]],
                            points[solution[0]]);
                return OsmSharpRoute.Concatenate(tsp, route);
            }
            return tsp;
        }

        /// <summary>
        /// Generates a problem definition.
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <param name="is_round"></param>
        /// <returns></returns>
        protected IProblem GenerateProblem(double[][] weights, int? first, int? last, bool is_round)
        {
            if (first.HasValue)
            {
                if (last.HasValue)
                {
                    return new MatrixProblem(weights, false, first.Value, last.Value);
                }
                else
                {
                    if (is_round)
                    {
                        return new MatrixProblem(weights, false, first.Value, first.Value);
                    }
                    else
                    {
                        return new MatrixProblem(weights, false, first.Value, null);
                    }
                }
            }
            if (is_round)
            {
                return new MatrixProblem(weights, false, 0, 0);
            }
            else
            {
                return new MatrixProblem(weights, false);
            }
        }

        /// <summary>
        /// Generates a solver in function of the size of the problem.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        internal abstract ISolver DoCreateSolver(int size);

        #region Intermidiate Results

        /// <summary>
        /// Delegate to pass on an intermidiate solution.
        /// </summary>
        /// <param name="result"></param>
        public delegate void IntermidiateDelegate(int[] result, double weight);

        /// <summary>
        /// Raised when an intermidiate result is available.
        /// </summary>
        public event IntermidiateDelegate IntermidiateResult;

        /// <summary>
        /// Returns true when the event has to be raised.
        /// </summary>
        /// <returns></returns>
        protected bool CanRaiseIntermidiateResult()
        {
            return this.IntermidiateResult != null;
        }

        /// <summary>
        /// Raises the intermidiate results event.
        /// </summary>
        /// <param name="result"></param>
        protected void RaiseIntermidiateResult(int[] result, double weight)
        {
            if (IntermidiateResult != null)
            {
                this.IntermidiateResult(result, weight);
            }
        }

        #endregion
    }
}
