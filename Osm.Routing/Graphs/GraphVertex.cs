using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;
using Tools.Math.Geo;

namespace Osm.Routing.Raw.Graphs
{
    /// <summary>
    /// Vertex in this graph.
    /// </summary>
    public class GraphVertex : IEquatable<GraphVertex>
    {
        private static long _id_counter = 0;

        public GraphVertex(Node node)
        {
            this.Node = node;
            this.Type = GraphVertexEnum.Node;
            this.Id = node.Id;
        }

        internal GraphVertex(GraphResolved resolved)
        {
            _id_counter--;
            this.Id = _id_counter;
            this.Resolved = resolved;
            this.Type = GraphVertexEnum.Resolved;
        }

        internal GraphResolved Resolved { get; private set; }

        public long Id
        {
            get;
            set;
        }

        public Node Node { get; private set; }

        internal GraphVertexEnum Type { get; private set; }

        public GeoCoordinate Coordinate
        {
            get
            {
                if (this.Type == GraphVertexEnum.Node)
                {
                    return this.Node.Coordinate;
                }
                else
                {
                    return this.Resolved.Coordinate;
                }
            }
        }

        #region IEquatable<GraphVertex> Members

        public bool Equals(GraphVertex other)
        {
            if (other.Type == this.Type)
            {
                if (other.Node != null && other.Node == this.Node)
                {
                    return true;
                }
                if (other.Resolved != null)
                {
                    if (this.Resolved != null)
                    {
                        if (this.Resolved.Way == other.Resolved.Way
                            && this.Resolved.Idx == other.Resolved.Idx
                            && this.Resolved.Position == other.Resolved.Position)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Used when hashcodes are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is GraphVertex)
            {
                GraphVertex other = (obj as GraphVertex);
                return this.Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (this.Type == GraphVertexEnum.Node)
            {
                return this.Type.GetHashCode()
                    ^ this.Node.GetHashCode();
            }
            else
            {
                return this.Type.GetHashCode()
                    ^ this.Resolved.GetHashCode();
            }
        }

        public static bool operator ==(GraphVertex c1, GraphVertex c2)
        {
            if ((object)c1 == null)
            {
                if ((object)c2 == null)
                {
                    return true;
                }
                return false;
            }
            if ((object)c2 == null
                && (object)c1 != null)
            {
                return false;
            }
            return c1.Equals(c2);
        }

        public static bool operator !=(GraphVertex c1, GraphVertex c2)
        {
            return !(c1 == c2);
        }

        public override string ToString()
        {
            if (this.Type == GraphVertexEnum.Resolved)
            {
                return string.Format("{0}", this.Resolved);
            }
            else
            {
                return string.Format("{0}", this.Node);
            }
        }
    }

    /// <summary>
    /// Type of vertex.
    /// </summary>
    internal enum GraphVertexEnum
    {
        Node,
        Resolved
    }
}
