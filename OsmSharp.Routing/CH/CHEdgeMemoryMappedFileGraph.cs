using OsmSharp.Collections.Arrays;
using OsmSharp.Collections.Arrays.MemoryMapped;
using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Routing.CH
{
    /// <summary>
    /// A memory mapped graph to store CHEdgeData edges.
    /// </summary>
    public class CHEdgeMemoryMappedGraph : MemoryDirectedGraph<CHEdgeData>
    {       
        /// <summary>
        /// Creates a new memory mapped graph.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="estimatedSize">The initial size.</param>
        public CHEdgeMemoryMappedGraph(MemoryMappedFile file, long estimatedSize)
            : base(estimatedSize,
            new MappedHugeArray<GeoCoordinateSimple, float>(new MemoryMappedHugeArraySingle(file, estimatedSize * 2), 2, 
            (array, idx, value) => 
            {
                array[idx] = value.Latitude;
                array[idx + 1] = value.Longitude;
            },
            (array, idx) => 
            {
                return new GeoCoordinateSimple()
                {
                    Latitude = array[idx],
                    Longitude = array[idx + 1]
                };
            }),
            new MemoryMappedHugeArrayUInt32(file, estimatedSize),
            new MemoryMappedHugeArrayUInt32(file, estimatedSize),
            new MappedHugeArray<CHEdgeData, uint>(new MemoryMappedHugeArrayUInt32(file, estimatedSize * 2), 2,
            (array, idx, value) =>
            {
                array[idx] = value.Value;
                array[idx + 1] = value.Tags;
                array[idx + 2] = value.Meta;
            },
            (array, idx) =>
            {
                return new CHEdgeData(
                    array[idx],
                    array[idx + 1],
                    (byte)array[idx + 2]);
            }),
            new HugeCoordinateCollectionIndex(file, estimatedSize))
        {

        }
    }
}
