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

namespace OsmSharp.Osm.Simple
{
    /// <summary>
    /// Represents a simple way.
    /// </summary>
    public class SimpleWay : SimpleOsmGeo
    {
        /// <summary>
        /// Creates a new simple way.
        /// </summary>
        public SimpleWay()
        {
            this.Type = SimpleOsmGeoType.Way;
        }

        /// <summary>
        /// Holds the list of nodes.
        /// </summary>
        public List<long>  Nodes { get; set; }

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Way[{0}]", this.Id.Value);
        }
    }
}
