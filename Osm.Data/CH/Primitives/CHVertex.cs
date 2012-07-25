using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Osm.Core;

namespace Osm.Data.Core.CH.Primitives
{
    /// <summary>
    /// Represents a CH vertex.
    /// </summary>
    public class CHVertex : ILocationObject, IEquatable<CHVertex>
    {
        /// <summary>
        /// Creates a new CH vertex.
        /// </summary>
        public CHVertex()
        {
            this.Level = int.MaxValue;
            _forward_neighbours = new List<CHVertexNeighbour>();
            _backward_neighbours = new List<CHVertexNeighbour>();
        }

        /// <summary>
        /// Holds the id of the vertex.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Holds the level of this vertex.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Holds the latitude of this vertex.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Holds the longitude of this vertex.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Returns the location of this object.
        /// </summary>
        public GeoCoordinate Location
        {
            get 
            { 
                return new GeoCoordinate(this.Latitude, this.Longitude); 
            }
        }

        /// <summary>
        /// Returns true if the given vertex equals the given vertex.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CHVertex other)
        {
            if (other != null)
            {
                return other.Id.Equals(this.Id);
            }
            return false;
        }

        #region Neighbours

        /// <summary>
        /// Holds all the forward neighbours.
        /// </summary>
        private List<CHVertexNeighbour> _forward_neighbours;

        /// <summary>
        /// Returns a list of the forward neighbours.
        /// </summary>
        public List<CHVertexNeighbour> ForwardNeighbours
        {
            get
            {
                return _forward_neighbours;
            }
        }

        /// <summary>
        /// Holds all the backward neighbours.
        /// </summary>
        private List<CHVertexNeighbour> _backward_neighbours;

        /// <summary>
        /// Returns a list of the backward neighbours.
        /// </summary>
        public List<CHVertexNeighbour> BackwardNeighbours
        {
            get
            {
                return _backward_neighbours;
            }
        }

        #endregion
    }

    /// <summary>
    /// A CH neighbour.
    /// </summary>
    public class CHVertexNeighbour
    {
        /// <summary>
        /// The id of the neighbour.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The weight to the neighbour.
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// The vertex that was contracted into this arc.
        /// </summary>
        public long ContractedVertexId { get; set; }

        /// <summary>
        /// The actual tags.
        /// </summary>
        public IDictionary<string, string> Tags { get; set; }

    }
}
