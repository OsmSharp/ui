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
using System.Text;

namespace OsmSharp.Routing.Graph.Path
{
    /// <summary>
    /// Linked list of routed vertices.
    /// </summary>
    public class PathSegment<TIdType>
    {
        /// <summary>
        /// Creates a vertex not linked to any others.
        /// </summary>
        /// <param name="vertexId"></param>
        public PathSegment(TIdType vertexId)
        {
            this.VertexId = vertexId;
            this.Weight = 0;
            this.From = null;
        }

        /// <summary>
        /// Creates a new linked vertex.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="weight"></param>
        /// <param name="from"></param>
        public PathSegment(TIdType vertexId, double weight, PathSegment<TIdType> from)
        {
            this.VertexId = vertexId;
            this.Weight = weight;
            this.From = from;
        }

        /// <summary>
        /// The id of this vertex.
        /// </summary>
        public TIdType VertexId { get; private set; }

        /// <summary>
        /// The weight from the source vertex.
        /// </summary>
        public double Weight { get; private set; }

        /// <summary>
        /// The vertex that came before this one.
        /// </summary>
        public PathSegment<TIdType> From { get; private set; }

        /// <summary>
        /// Returns the reverse of this path segment.
        /// </summary>
        /// <returns></returns>
        public PathSegment<TIdType> Reverse()
        {
            var route = new PathSegment<TIdType>(this.VertexId);
            PathSegment<TIdType> next = this;
            while (next.From != null)
            {
                route = new PathSegment<TIdType>(next.From.VertexId,
                    next.Weight + route.Weight, route);
                next = next.From;
            }
            return route;
        }

        /// <summary>
        /// Returns the first vertex.
        /// </summary>
        /// <returns></returns>
        public PathSegment<TIdType> First()
        {
            PathSegment<TIdType> next = this;
            while (next.From != null)
            {
                next = next.From;
            }
            return next;
        }

        /// <summary>
        /// Returns the length.
        /// </summary>
        /// <returns></returns>
        public int Length()
        {
            int length = 1;
            PathSegment<TIdType> next = this;
            while (next.From != null)
            {
                length++;
                next = next.From;
            }
            return length;
        }

        /// <summary>
        /// Concatenates this path after the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public PathSegment<TIdType> ConcatenateAfter(PathSegment<TIdType> path)
        {
            PathSegment<TIdType> clone = this.Clone();
            PathSegment<TIdType> first = clone.First();
            PathSegment<TIdType> pathClone = path.Clone();

            PathSegment<TIdType> current = clone;
            current.Weight = path.Weight + current.Weight;
            while (current.From != null)
            {
                current.From.Weight = path.Weight + current.From.Weight;
                current = current.From;
            }

            if (first.VertexId.Equals(path.VertexId))
            {
                first.Weight = pathClone.Weight;
                first.From = pathClone.From;
                return clone;
            }
            throw new ArgumentException("Paths must share beginning and end vertices to concatenate!");
        }

        /// <summary>
        /// Returns an exact copy of this path segment.
        /// </summary>
        /// <returns></returns>
        public PathSegment<TIdType> Clone()
        {
            if (this.From == null)
            { // cloning this case is easy!
                return new PathSegment<TIdType>(this.VertexId);
            }
            else
            { // recursively clone the from segments.
                PathSegment<TIdType> from = this.From.Clone();
                return new PathSegment<TIdType>(this.VertexId, this.Weight, from);
            }
        }

        /// <summary>
        /// Returns a description of this path.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            PathSegment<TIdType> next = this;
            while (next.From != null)
            {
                builder.Insert(0, string.Format("-> {0}", next.VertexId));
                next = next.From;
            }
            builder.Insert(0, string.Format("{0}", next.VertexId));
            return builder.ToString();
        }

        /// <summary>
        /// Returns all the vertices in an array.
        /// </summary>
        /// <returns></returns>
        public TIdType[] ToArray()
        {
            var vertices = new List<TIdType>();
            PathSegment<TIdType> next = this;
            while (next.From != null)
            {
                vertices.Add(next.VertexId);
                next = next.From;
            }
            vertices.Add(next.VertexId);
            vertices.Reverse();
            return vertices.ToArray();
        }

        /// <summary>
        /// Returns true if the path is the samen.
        /// </summary>
        /// <param name="segment1"></param>
        /// <param name="segment2"></param>
        /// <returns></returns>
        public static bool operator ==(PathSegment<TIdType> segment1, PathSegment<TIdType> segment2)
        {
            if ((object)segment1 != null && (object)segment2 != null)
            {
                if (segment1.VertexId.Equals(segment2.VertexId) &&
                    segment1.Weight == segment2.Weight)
                {
                    if ((object)segment1.From != null && (object)segment2.From != null)
                    {
                        return segment1.From == segment2.From;
                    }
                    else
                    {
                        return (object)segment1.From == null && (object)segment2.From == null;
                    }
                }
                return false;
            }
            return (object)segment1 == null && (object)segment2 == null;
        }


        /// <summary>
        /// Returns true if the path is the samen.
        /// </summary>
        /// <param name="segment1"></param>
        /// <param name="segment2"></param>
        /// <returns></returns>
        public static bool operator !=(PathSegment<TIdType> segment1, PathSegment<TIdType> segment2)
        {
            return !(segment2 == segment1);
        }

        /// <summary>
        /// Returns true if the given object equals this one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool Equals(PathSegment<TIdType> other)
        {
            return EqualityComparer<TIdType>.Default.Equals(VertexId, other.VertexId) && Weight.Equals(other.Weight) && Equals(From, other.From);
        }

        /// <summary>
        /// Returns true if the given object equals this one.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PathSegment<TIdType>)obj);
        }

        /// <summary>
        /// Returns the hashcode for this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = EqualityComparer<TIdType>.Default.GetHashCode(VertexId);
                hashCode = (hashCode * 397) ^ Weight.GetHashCode();
                hashCode = (hashCode * 397) ^ (From != null ? From.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
