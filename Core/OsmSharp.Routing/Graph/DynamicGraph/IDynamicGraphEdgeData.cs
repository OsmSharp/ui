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

namespace OsmSharp.Routing.Graph.DynamicGraph
{
    /// <summary>
    /// Abstracts edge information.
    /// </summary>
    public interface IDynamicGraphEdgeData
    {
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
    }

    /// <summary>
    /// Abstract a comparer for edges.
    /// </summary>
    /// <typeparam name="EdgeData"></typeparam>
    public interface IDynamicGraphEdgeComparer<EdgeData>
        where EdgeData: IDynamicGraphEdgeData
    {
        /// <summary>
        /// Returns true if the data in the edge2 is useless if the data in edge1 is present.
        /// </summary>
        /// <param name="edge1"></param>
        /// <param name="edge2"></param>
        /// <returns></returns>
        bool Overlaps(EdgeData edge1, EdgeData edge2);
    }
}