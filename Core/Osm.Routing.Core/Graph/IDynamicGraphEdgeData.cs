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

namespace Routing.Core.Graph
{
    /// <summary>
    /// Abstracts edge information.
    /// </summary>
    public interface IDynamicGraphEdgeData
    {
        /// <summary>
        /// Returns true if the edge can be followed only in the foward direction.
        /// </summary>
        bool Forward
        {
            get;
        }

        /// <summary>
        /// Returns true if the edge can be followed only in the backward direction.
        /// </summary>
        bool Backward
        {
            get;
        }

        /// <summary>
        /// Returns the weight of this edge.
        /// </summary>
        double Weight
        {
            get;
        }

        /// <summary>
        /// Returns the tags identifier.
        /// </summary>
        uint Tags
        {
            get;
        }

        /// <summary>
        /// Returns true if the edge is virtual (it does not exist in the orginal graph).
        /// </summary>
        bool IsVirtual
        {
            get;
        }
    }
}
