using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Osm.Data.Core.Sparse.Primitives;

namespace Osm.Routing.Sparse.PreProcessor
{
    /// <summary>
    /// Interface used to report progress about pre-processing.
    /// </summary>
    public interface ISparsePreProcessorProgress
    {
        /// <summary>
        /// Started processing the given vertex.
        /// </summary>
        /// <param name="vertex_id"></param>
        void StartVertex(long vertex_id);

        /// <summary>
        /// Vertex was processed.
        /// </summary>
        /// <param name="vertex"></param>
        void ProcessedVertex(SparseVertex vertex, bool deleted);

        /// <summary>
        /// Bypassed vertex was processed.
        /// </summary>
        /// <param name="vertex_id"></param>
        /// <param name="coordinate"></param>
        /// <param name="neighbour1"></param>
        /// <param name="neighbour2"></param>
        void PersistedBypassed(long vertex_id, GeoCoordinate coordinate, long neighbour1, long neighbour2);
    }
}
