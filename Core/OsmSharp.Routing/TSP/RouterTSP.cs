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
using OsmSharp.Routing;
using OsmSharp.Routing.Route;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Routing.TSP
{
    /// <summary>
    /// Router that calculates TSP solutions.
    /// </summary>
    public abstract class RouterTSP
    {
        /// <summary>
        /// Creates a new TSP router.
        /// </summary>
        public RouterTSP()
        {

        }

        /// <summary>
        /// Calculates a solution to the ATSP.
        /// </summary>
        /// <param name="weights">The weights between all the customers.</param>
        /// <param name="locations">The locations of all customers.</param>
        /// <param name="first">The first customer.</param>
        /// <param name="is_round">Return to the first customer or not.</param>
        /// <returns></returns>
        public IRoute CalculateTSP(double[][] weights, GeoCoordinate[] locations, int first, bool is_round)
        {
            // create solver.
            ISolver solver = this.DoCreateSolver(locations.Length);
            solver.IntermidiateResult += new SolverDelegates.IntermidiateDelegate(solver_IntermidiateResult);

            // calculate the TSP.
            return solver.Solve(this.GenerateProblem(weights, first, null, is_round));
        }

        /// <summary>
        /// Calculates a solution to the ATSP.
        /// </summary>
        /// <param name="weights">The weights between all the customers.</param>
        /// <param name="locations">The locations of all customers.</param>
        /// <param name="first">The index of the point to start from.</param>
        /// <param name="last">The index of the point to end at.</param>
        /// <returns></returns>
        public IRoute CalculateTSP(double[][] weights, GeoCoordinate[] locations, int first, int last)
        {
            // create solver.
            ISolver solver = this.DoCreateSolver(locations.Length);
            solver.IntermidiateResult += new SolverDelegates.IntermidiateDelegate(solver_IntermidiateResult);

            // calculate the TSP.
            return solver.Solve(this.GenerateProblem(weights, first, last, false));
        }

        /// <summary>
        /// Calculates a solution to the ATSP.
        /// </summary>
        /// <param name="weights">The weights between all the customers.</param>
        /// <param name="locations">The locations of all customers.</param>
        /// <param name="is_round">Make the route return to the start-point or not.</param>
        /// <returns></returns>
        public IRoute CalculateTSP(double[][] weights, GeoCoordinate[] locations, bool is_round)
        {
            // create solver.
            ISolver solver = null;
            if (locations.Length < 6)
            { // creates a bute force solver.
                solver = new OsmSharp.Tools.Math.TSP.BruteForce.BruteForceSolver();
            }
            else
            { // creates a solver for the larger problems.
                solver = this.DoCreateSolver(locations.Length);
            }
            solver.IntermidiateResult += new SolverDelegates.IntermidiateDelegate(solver_IntermidiateResult);

            // calculate the TSP.
            return solver.Solve(this.GenerateProblem(weights, null, null, is_round));
        }

        /// <summary>
        /// Calculates a solution to the ATSP.
        /// </summary>
        /// <param name="weights">The weights between all the customers.</param>
        /// <param name="locations">The locations of all customers.</param>
        /// <returns></returns>
        public IRoute CalculateTSP(double[][] weights, GeoCoordinate[] locations)
        {
            return this.CalculateTSP(weights, locations, true);
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
                    return MatrixProblem.CreateATSPOpen(weights, first.Value, last.Value);
                    //return new MatrixProblem(weights, false, first.Value, last.Value);
                }
                else
                {
                    if (is_round)
                    {
                        return MatrixProblem.CreateATSP(weights, first.Value);
                        //return new MatrixProblem(weights, false, first.Value, first.Value);
                    }
                    else
                    {
                        return MatrixProblem.CreateATSPOpen(weights, first.Value);
                        //return new MatrixProblem(weights, false, first.Value, null);
                    }
                }
            }
            if (is_round)
            {
                return MatrixProblem.CreateATSP(weights);
                //return new MatrixProblem(weights, false, 0, 0);
            }
            else
            {
                return MatrixProblem.CreateATSPOpen(weights);
                //return new MatrixProblem(weights, false);
            }
        }

        /// <summary>
        /// Generates a solver in function of the size of the problem.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        internal abstract ISolver DoCreateSolver(int size);

        #region Intermidiate Results

        /// <summary>
        /// Raise intermidiate result event.
        /// </summary>
        /// <param name="result"></param>
        void solver_IntermidiateResult(int[] result)
        {
            if (this.CanRaiseIntermidiateResult())
            {
                this.RaiseIntermidiateResult(result);
            }
        }

        /// <summary>
        /// Delegate to pass on an intermidiate solution.
        /// </summary>
        /// <param name="result"></param>
        public delegate void IntermidiateDelegate(int[] result);

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
        protected void RaiseIntermidiateResult(int[] result)
        {
            if (IntermidiateResult != null)
            {
                this.IntermidiateResult(result);
            }
        }

        #endregion
    }
}