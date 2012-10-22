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
using Osm.Routing.Core.Roads.Tags;
using Osm.Routing.Core.Interpreter;

namespace Osm.Routing.Core.Resolving
{
    /// <summary>
    /// Interface used to match a coordinate to a configurable routable position.
    /// </summary>
    public interface IResolveMatcher
    {
        /// <summary>
        /// Returns true if the point is a suitable candidate as a point being resolved on the given way.
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        bool Match(RoutingWayInterperterBase way_interpreter);
    }
}