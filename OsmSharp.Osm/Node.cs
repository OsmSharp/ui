// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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
using OsmSharp.Geo.Geometries;
using OsmSharp.Osm.Data;
using OsmSharp.Osm.Interpreter;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Represents a simple node.
    /// </summary>
    public class Node : OsmGeo
    {
        /// <summary>
        /// Creates a new simple node.
        /// </summary>
        public Node()
        {
            this.Type = OsmGeoType.Node;
        }

        /// <summary>
        /// The latitude.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// The longitude.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!this.Id.HasValue)
            {
                return "Node[null]";
            }
            return string.Format("Node[{0}]", this.Id.Value);
        }

        #region Construction Methods

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static Node Create(long id, double latitude, double longitude)
        {
            Node node = new Node();
            node.Id = id;
            node.Latitude = latitude;
            node.Longitude = longitude;
            return node;
        }

        #endregion
    }
}
