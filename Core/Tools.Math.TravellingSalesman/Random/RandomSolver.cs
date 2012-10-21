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
using Tools.Core;
using Tools.Math.TSP;
using Tools.Math.VRP.Core.Routes;
using Tools.Math.TSP.Problems;
using Tools.Math.VRP.Core.Routes.ASymmetric;

namespace Tools.Math.TravellingSalesman.Random
{
    /// <summary>
    /// Just generates random routes.
    /// </summary>
    public class RandomSolver : ISolver
    {
        /// <summary>
        /// Boolean to stop execution.
        /// </summary>
        private bool _stopped = false;

        /// <summary>
        /// The route this solver was initialized with.
        /// </summary>
        private IRoute _route;

        /// <summary>
        /// Retuns the name of this solver.
        /// </summary>
        public string Name
        {
            get
            {
                return "Random";
            }
        }

        /// <summary>
        /// Generates a random route.
        /// </summary>
        /// <returns></returns>
        public IRoute Solve(IProblem problem)
        {
            return RandomSolver.DoSolve(problem);
        }

        /// <summary>
        /// Generates a random route.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        public static IRoute DoSolve(IProblem problem)
        {
            List<int> customers = new List<int>();
            for (int customer = 0; customer < problem.Size; customer++)
            {
                customers.Add(customer);
            }
            customers.Shuffle<int>();
            return DynamicAsymmetricRoute.CreateFrom(customers);
        }

        /// <summary>
        /// Stops execution.
        /// </summary>
        public void Stop()
        {

        }

        public event SolverDelegates.IntermidiateDelegate IntermidiateResult;
    }
}
