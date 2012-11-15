using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math;
using OsmSharp.Routing.Core.Graph;

namespace OsmSharp.Osm.Routing.Data
{
    /// <summary>
    /// Basic OSM edge data.
    /// </summary>
    public class OsmEdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the weight of this edge.
        /// </summary>
        private float _weight;

        /// <summary>
        /// Holds the tags of this edge.
        /// </summary>
        private uint _tags;

        /// <summary>
        /// Holds the forward flag.
        /// </summary>
        private bool _forward;

        /// <summary>
        /// Holds the backward flag.
        /// </summary>
        private bool _backward;

        /// <summary>
        /// Creates a new osm edge data object.
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="forward"></param>
        /// <param name="backward"></param>
        /// <param name="tags"></param>
        public OsmEdgeData(float weight, bool forward, bool backward, uint tags)
        {
            _weight = weight;
            _tags = tags;
            _forward = forward;
            _backward = backward;
        }

        /// <summary>
        /// Returns true if the edge is traversable forward.
        /// </summary>
        public bool Forward
        {
            get { return _forward; }
        }

        /// <summary>
        /// Returns true if the edge is traversable backward.
        /// </summary>
        public bool Backward
        {
            get { return _backward; }
        }

        /// <summary>
        /// Returns the weight of this edge.
        /// </summary>
        public double Weight
        {
            get { return _weight; }
        }

        /// <summary>
        /// Returns the tags of this edge.
        /// </summary>
        public uint Tags
        {
            get { return _tags;  }
        }

        /// <summary>
        /// These edge can always be resolved on.
        /// </summary>
        public bool IsVirtual
        {
            get
            {
                return false;
            }
        }
    }
}
