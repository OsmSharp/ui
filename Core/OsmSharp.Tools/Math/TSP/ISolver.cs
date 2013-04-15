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
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Tools.Math.TSP
{
    /// <summary>
    /// Interface representing a solver for the TSP.
    /// </summary>
    public interface ISolver
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Solves the TSP.
        /// </summary>
        /// <returns></returns>
        IRoute Solve(IProblem problem);

        /// <summary>
        /// Stops the executing of the solving process.
        /// </summary>
        void Stop();
        
        /// <summary>
        /// Raised when an intermidiate result is available.
        /// </summary>
        event SolverDelegates.IntermidiateDelegate IntermidiateResult;
    }

    /// <summary>
    /// Contains solver delegates.
    /// </summary>
    public static class SolverDelegates
    {
        /// <summary>
        /// Delegate to pass on an intermidiate solution.
        /// </summary>
        /// <param name="result"></param>
        public delegate void IntermidiateDelegate(int[] result);
    }
}
