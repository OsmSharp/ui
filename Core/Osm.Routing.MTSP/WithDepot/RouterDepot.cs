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
using Osm.Routing.Core.Route;
using Osm.Core;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.Core.VRP.WithDepot
{
    /// <summary>
    /// Class to solve for a specific class of VRP problems: VRP problems with multi depot.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public abstract class RouterDepot<ResolvedType> : RouterVRP<ResolvedType>
        where ResolvedType : IResolvedPoint
    {
        /// <summary>
        /// Creates a VRP router without a depot.
        /// </summary>
        /// <param name="router"></param>
        public RouterDepot(IRouter<ResolvedType> router)
            :base(router)
        {

        }

        /// <summary>
        /// Calculates this VRP No Depot.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public abstract OsmSharpRoute[] CalculateDepot(ResolvedType[] depots, ResolvedType[] customers);


    }
}
