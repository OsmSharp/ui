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
using OsmSharp.Routing.Core;
using OsmSharp.Routing.Core.Route;

namespace OsmSharp.Routing.Core.VRP.NoDepot
{
    /// <summary>
    /// Class to solve for a specific class of VRP problems: VRP problems with any depot.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public abstract class RouterNoDepot<ResolvedType> : RouterVRP<ResolvedType>
        where ResolvedType : IRouterPoint
    {
        /// <summary>
        /// Creates a VRP router without a depot.
        /// </summary>
        /// <param name="router"></param>
        public RouterNoDepot(IRouter<ResolvedType> router)
            :base(router)
        {

        }

        /// <summary>
        /// Calculates this VRP No Depot.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public abstract OsmSharpRoute[] CalculateNoDepot(VehicleEnum vehicle, ResolvedType[] points);
    }
}
