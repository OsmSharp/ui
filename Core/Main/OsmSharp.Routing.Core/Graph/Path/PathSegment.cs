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

namespace OsmSharp.Routing.Core.Graph.Path
{
    /// <summary>
    /// Linked list of routed vertices.
    /// </summary>
    public class PathSegment<IdType>
    {
        /// <summary>
        /// Creates a vertex not linked to any others.
        /// </summary>
        /// <param name="vertex_id"></param>
        public PathSegment(IdType vertex_id)
        {
            this.VertexId = vertex_id;
            this.Weight = 0;
            this.From = null;
        }

        /// <summary>
        /// Creates a new linked vertex.
        /// </summary>
        /// <param name="vertex_id"></param>
        /// <param name="weight"></param>
        /// <param name="from"></param>
        public PathSegment(IdType vertex_id, double weight, PathSegment<IdType> from)
        {
            this.VertexId = vertex_id;
            this.Weight = weight;
            this.From = from;
        }

        /// <summary>
        /// The id of this vertex.
        /// </summary>
        public IdType VertexId { get; private set; }

        /// <summary>
        /// The weight from the source vertex.
        /// </summary>
        public double Weight { get; private set; }

        /// <summary>
        /// The vertex that came before this one.
        /// </summary>
        public PathSegment<IdType> From { get; private set; }

        /// <summary>
        /// Returns the reverse of this path segment.
        /// </summary>
        /// <returns></returns>
        public PathSegment<IdType> Reverse()
        {
            PathSegment<IdType> route = new PathSegment<IdType>(this.VertexId);
            PathSegment<IdType> next = this;
            while (next.From != null)
            {
                route = new PathSegment<IdType>(next.From.VertexId,
                    next.Weight + route.Weight, route);
                next = next.From;
            }
            return route;
        }

        /// <summary>
        /// Returns the first vertex.
        /// </summary>
        /// <returns></returns>
        public PathSegment<IdType> First()
        {
            PathSegment<IdType> next = this;
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
            PathSegment<IdType> next = this;
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
        public PathSegment<IdType> ConcatenateAfter(PathSegment<IdType> path)
        {
            PathSegment<IdType> clone = this.Clone();
            PathSegment<IdType> first = clone.First();

            if (first.VertexId.Equals(path.VertexId))
            {
                first.Weight = path.Weight;
                first.From = path.From;
                return clone;
            }
            throw new ArgumentException("Paths must share beginning and end vertices to concatenate!");
        }

        /// <summary>
        /// Returns an exact copy of this path segment.
        /// </summary>
        /// <returns></returns>
        public PathSegment<IdType> Clone()
        {
            if (this.From == null)
            { // cloning this case is easy!
                return new PathSegment<IdType>(this.VertexId);
            }
            else
            { // recursively clone the from segments.
                PathSegment<IdType> from = this.From.Clone();
                return new PathSegment<IdType>(this.VertexId, this.Weight, from);
            }
        }

        /// <summary>
        /// Returns a description of this path.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            PathSegment<IdType> next = this;
            while (next.From != null)
            {
                builder.Insert(0, string.Format("-> {0}", next.VertexId.ToString()));
                next = next.From;
            }
            builder.Insert(0, string.Format("{0}", next.VertexId));
            return builder.ToString();
        }

        /// <summary>
        /// Returns all the vertices in an array.
        /// </summary>
        /// <returns></returns>
        public IdType[] ToArray()
        {
            List<IdType> vertices = new List<IdType>();
            PathSegment<IdType> next = this;
            while (next.From != null)
            {
                vertices.Add(next.VertexId);
                next = next.From;
            }
            vertices.Add(next.VertexId);
            vertices.Reverse();
            return vertices.ToArray();
        }
    }
}
