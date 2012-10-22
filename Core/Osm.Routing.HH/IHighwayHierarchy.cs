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
using Osm.Routing.HH.Primitives;

namespace Osm.Routing.HH
{
    /// <summary>
    /// An interface with all the functions of a datastructure holding HH data.
    /// </summary>
    public interface IHighwayHierarchy
    {
        /// <summary>
        /// Clears all the data from the target.
        /// </summary>
        void ClearTarget();

        /// <summary>
        /// Returns vertices at a given level.
        /// </summary>
        /// <returns></returns>
        IEnumerable<HighwayVertex> GetVertices(int level);

        /// <summary>
        /// Returns the vertex with the given id.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        HighwayVertex GetVertex(long vertex_id);

        /// <summary>
        /// Persists the given vertex to a given level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="edges"></param>
        void PersistVertex(HighwayVertex vertex);

    }
}
