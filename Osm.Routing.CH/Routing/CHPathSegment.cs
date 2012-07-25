using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.CH.Routing
{
    /// <summary>
    /// Linked list of routed vertices.
    /// </summary>
    public class CHPathSegment
    {
          /// <summary>
        /// Creates a vertex not linked to any others.
        /// </summary>
        /// <param name="vertex_id"></param>
        public CHPathSegment(long vertex_id)
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
        public CHPathSegment(long vertex_id, float weight, CHPathSegment from)
        {
            this.VertexId = vertex_id;
            this.Weight = weight;
            this.From = from;
        }

        /// <summary>
        /// The id of this vertex.
        /// </summary>
        public long VertexId { get; private set; }

        /// <summary>
        /// The weight from the source vertex.
        /// </summary>
        public float Weight { get; private set; }

        /// <summary>
        /// The vertex that came before this one.
        /// </summary>
        public CHPathSegment From { get; private set; }

        /// <summary>
        /// Returns the reverse of this path segment.
        /// </summary>
        /// <returns></returns>
        public CHPathSegment Reverse()
        {
            CHPathSegment route = new CHPathSegment(this.VertexId);
            CHPathSegment next = this;
            while (next.From != null)
            {
                route = new CHPathSegment(next.From.VertexId,
                    next.Weight + route.Weight, route);
                next = next.From;
            }
            return route;
        }

        /// <summary>
        /// Returns the first vertex.
        /// </summary>
        /// <returns></returns>
        public CHPathSegment First()
        {
            CHPathSegment next = this;
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
            CHPathSegment next = this;
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
        public void ConcatenateAfter(CHPathSegment path)
        {
            CHPathSegment first = this.First();

            if (first.VertexId == path.VertexId)
            {
                first.Weight = path.Weight;
                first.From = path.From;
                return;
            }
            throw new ArgumentException("Paths must share beginning and end vertices to concatenate!");
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            CHPathSegment next = this;
            while (next.From != null)
            {
                builder.Insert(0, string.Format("-> {0}", next.VertexId.ToString()));
                next = next.From;
            }
            builder.Insert(0, string.Format("{0}", next.VertexId));
            return builder.ToString();
        }
    }
}
