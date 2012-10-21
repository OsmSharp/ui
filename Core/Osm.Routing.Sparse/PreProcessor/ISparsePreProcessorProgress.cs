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
using Tools.Math.Geo;
using Osm.Data.Core.Sparse.Primitives;

namespace Osm.Routing.Sparse.PreProcessor
{
    /// <summary>
    /// Interface used to report progress about pre-processing.
    /// </summary>
    public interface ISparsePreProcessorProgress
    {
        /// <summary>
        /// Started processing the given vertex.
        /// </summary>
        /// <param name="vertex_id"></param>
        void StartVertex(long vertex_id);

        /// <summary>
        /// Vertex was processed.
        /// </summary>
        /// <param name="vertex"></param>
        void ProcessedVertex(SparseVertex vertex, bool deleted);

        /// <summary>
        /// Bypassed vertex was processed.
        /// </summary>
        /// <param name="vertex_id"></param>
        /// <param name="coordinate"></param>
        /// <param name="neighbour1"></param>
        /// <param name="neighbour2"></param>
        void PersistedBypassed(long vertex_id, GeoCoordinate coordinate, long neighbour1, long neighbour2);
    }
}
