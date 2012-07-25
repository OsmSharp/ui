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
            return string.Format("http://www.openstreetmap.org/?node={0}:[{1};{2}]",
                this.Id,
                this.Coordinate.Longitude,
                this.Coordinate.Latitude);
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
