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

namespace OsmSharp.Routing.Osm.Graphs
{
    /// <summary>
    /// A memory mapped graph to store liveedge edges.
    /// </summary>
    public class LiveEdgeMemoryMappedGraph : MemoryGraph<LiveEdge>
    {       
        /// <summary>
        /// Creates a new memory mapped file dynamic graph.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="estimatedSize">The estimated size.</param>
        public LiveEdgeMemoryMappedGraph(MemoryMappedFile file, long estimatedSize)
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
            new MappedHugeArray<LiveEdge, uint>(new MemoryMappedHugeArrayUInt32(file, estimatedSize * 2), 2,
            (array, idx, value) =>
            {
                array[idx] = value.Value;
                array[idx + 1] = value.Tags;
                array[idx + 2] = BitConverter.ToUInt32(BitConverter.GetBytes(value.Distance), 0);
            },
            (array, idx) =>
            {
                return new LiveEdge()
                {
                    Value = array[idx],
                    Tags = array[idx + 1],
                    Distance = BitConverter.ToSingle(BitConverter.GetBytes(array[idx + 2]), 0)
                };
            }),
            new HugeCoordinateCollectionIndex(file, estimatedSize))
        {

        }
    }
}
