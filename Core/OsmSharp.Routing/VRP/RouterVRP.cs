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
using OsmSharp.Routing;
using OsmSharp.Routing.Route;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Routing.VRP
{
    /// <summary>
    /// Base class for all the VRP solvers.
    /// </summary>
    public abstract class RouterVRP
    {
        /// <summary>
        /// Creates a new VRP router.
        /// </summary>
        public RouterVRP()
        {

        }

        /// <summary>
        /// Returns the name of this router.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        #region Intermidiate Results

        /// <summary>
        /// Delegate to pass on an array of routes.
        /// </summary>
        /// <param name="result"></param>
        public delegate void SolutionDelegate(IEnumerable<int[]> result);

        /// <summary>
        /// Raised when an intermidiate result is available.
        /// </summary>
        public event SolutionDelegate IntermidiateResult;

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
        protected void RaiseIntermidiateResult(IEnumerable<int[]> result)
        {
            if (IntermidiateResult != null)
            {
                this.IntermidiateResult(result);
            }
        }

        #endregion
    }
}
