using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Sparse.Routing.Graph;
using Tools.Math.Geo;
using Osm.Data.Core.Sparse.Primitives;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.Sparse.Routing
{
    /// <summary>
    /// Represents a resolved point.
    /// </summary>
    public class SparseResolvedPoint : SparseVertex, IResolvedPoint
    {
        /// <summary>
        /// The id of this resolved point.
        /// </summary>
        private long _id;

        /// <summary>
        /// The location.
        /// </summary>
        private GeoCoordinate _location;

        /// <summary>
        /// The original location.
        /// </summary>
        private GeoCoordinate _original;

        /// <summary>
        /// One of the neighbours.
        /// </summary>
        private long _neighbour1;

        /// <summary>
        /// One of the neighbours.
        /// </summary>
        private long _neighbour2;

        /// <summary>
        /// The weight to neighbour1.
        /// </summary>
        private double _weight1;

        /// <summary>
        /// The weight to neigbhbour2.
        /// </summary>
        private double _weight2;

        /// <summary>
        /// Creates resolved point.
        /// </summary>
        internal SparseResolvedPoint(long id, GeoCoordinate location, GeoCoordinate original, 
            long neighbour1, double weight1, long neighbour2, double weight2)
        {
            _id = id;
            _location = location;
            _original = original;

            _neighbour1 = neighbour1;
            _weight1 = weight1;
            _neighbour2 = neighbour2;
            _weight2 = weight2;
        }

        /// <summary>
        /// Creates a resolved point from an existing vertex.
        /// </summary>
        /// <param name="vertex"></param>
        internal SparseResolvedPoint(SparseVertex vertex)
        {
            _id = vertex.Id;
            _location = vertex.Location;
            _original = vertex.Location;

            _neighbour1 = vertex.Id;
            _weight1 = 1;
            _neighbour2 = vertex.Id;
            _weight2 = 0;
        }

        /// <summary>
        /// Returns the id.
        /// </summary>
        public long Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// The location or the resolved point.
        /// </summary>
        public GeoCoordinate Location
        {
            get 
            { 
                return _location; 
            }
        }

        /// <summary>
        /// The location or the resolved point.
        /// </summary>
        public GeoCoordinate Orginal
        {
            get
            {
                return _original;
            }
        }

        /// <summary>
        /// The first neighbour.
        /// </summary>
        public long Neighbour1
        {
            get
            {
                return _neighbour1;
            }
        }

        /// <summary>
        /// The weight to the first neighbour.
        /// </summary>
        public double Weight1
        {
            get
            {
                return _weight1;
            }
        }

        /// <summary>
        /// The second neighbour.
        /// </summary>
        public long Neighbour2
        {
            get
            {
                return _neighbour2;
            }
        }

        /// <summary>
        /// The weight to the second neighbour.
        /// </summary>
        public double Weight2
        {
            get
            {
                return _weight2;
            }
        }

        /// <summary>
        /// Returns true if the other is equal to this one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SparseVertex other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Id == other.Id;
        }
    }
}
