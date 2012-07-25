using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;
using Osm.Routing.Raw.Graphs;

namespace Osm.Routing.Graphs
{
    /// <summary>
    /// Class representing a vertex that can be reached along an edge.
    /// </summary>
    /// <typeparam name="EdgeType"></typeparam>
    /// <typeparam name="VertexType"></typeparam>
    public class VertexAlongEdge
    {
        /// <summary>
        /// Creates a new VertexAlongEdge
        /// </summary>
        /// <param name="point"></param>
        public VertexAlongEdge(
            Way way,
            GraphVertex point,
            float weight)
        {
            _vertex = point;
            _edge = way;
            _weight = weight;
        }

        /// <summary>
        /// Holds the vertex.
        /// </summary>
        private GraphVertex _vertex;

        /// <summary>
        /// Returns the vertex.
        /// </summary>
        public GraphVertex Vertex
        {
            get
            {
                return _vertex;
            }
        }

        /// <summary>
        /// Holds the edge.
        /// </summary>
        private Way _edge;

        /// <summary>
        /// Returns the way.
        /// </summary>
        public Way Edge
        {
            get
            {
                return _edge;
            }
        }

        /// <summary>
        /// Holds the weight.
        /// </summary>
        private float _weight;

        /// <summary>
        /// Returns the weight.
        /// </summary>
        public float Weight
        {
            get
            {
                return _weight;
            }
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the objects
        ///     being compared. The return value has the following meanings: Value Meaning
        ///     Less than zero This object is less than the other parameter.  Zero This object
        ///     is equal to other. Greater than zero This object is greater than other.</returns>
        public int CompareTo(VertexAlongEdge other)
        {
            return this.Weight.CompareTo(other.Weight);
        }
    }
}
