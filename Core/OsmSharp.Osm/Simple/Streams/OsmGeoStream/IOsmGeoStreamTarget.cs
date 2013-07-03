using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Osm.Data.Streams.OsmGeoStream
{
    /// <summary>
    /// Interface to a streamed source of OsmGeo-objects.
    /// </summary>
    public interface IOsmGeoStreamTarget
    {
        /// <summary>
        /// A node was detected in the source-stream.
        /// </summary>
        /// <param name="node"></param>
        void AddNode(Node node);

        /// <summary>
        /// A complete was was detected in the source-stream.
        /// </summary>
        /// <param name="way"></param>
        void AddWay(Way way);

        /// <summary>
        /// A complete relation was detected in the source-stream.
        /// </summary>
        /// <param name="relation"></param>
        void AddRelation(Relation relation);
    }
}
