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
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;

namespace Osm.Core
{
    /// <summary>
    /// Node class.
    /// </summary>
    [Serializable]
    public class Node : OsmGeo, IEquatable<Node>
    {
        internal protected Node(long id)
            :base(id)
        {

        }

        /// <summary>
        /// Returns the node type.
        /// </summary>
        public override OsmType Type
        {
            get { return OsmType.Node; }
        }

        /// <summary>
        /// The coordinates of this node.
        /// </summary>
        public GeoCoordinate Coordinate { get; set; }

        #region IEquatable<Node> Members

        public bool Equals(Node other)
        {
            if (other != null)
            {
                return other.Id == this.Id;
            }
            return false;
        }

        #endregion

        public override string ToString()
        {
            if (this.Coordinate != null)
            {
                return string.Format("http://www.openstreetmap.org/?node={0}:[{1};{2}]",
                    this.Id,
                    this.Coordinate.Longitude,
                    this.Coordinate.Latitude);
            }
            return string.Format("http://www.openstreetmap.org/?node={0}",
                this.Id);
        }

        /// <summary>
        /// Copies all properties of this node onto the given node without the id.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public void CopyTo(Node n)
        {
            foreach (KeyValuePair<string, string> tag in this.Tags)
            {
                n.Tags.Add(tag.Key, tag.Value);
            }

            n.TimeStamp = this.TimeStamp;
            n.User = this.User;
            n.UserId = this.UserId;
            n.Version = this.Version;
            n.Visible = this.Visible;
        }

        /// <summary>
        /// Returns an exact copy of this way.
        /// 
        /// WARNING: even the id is copied!
        /// </summary>
        /// <returns></returns>
        public Node Copy()
        {
            Node n = new Node(this.Id);
            this.CopyTo(n);
            return n;
        }
    }
}
