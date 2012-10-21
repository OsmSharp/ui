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

namespace Osm.Data.Core.Sparse.Primitives
{
    /// <summary>
    /// Represents a simple vertex that is part of the road network.
    /// </summary>
    public class SparseSimpleVertex
    {
        public long Id { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public long Neighbour1 { get; set; }

        public long Neighbour2 { get; set; }


        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Id.Equals((obj as SparseSimpleVertex).Id);
        }
    }
}
