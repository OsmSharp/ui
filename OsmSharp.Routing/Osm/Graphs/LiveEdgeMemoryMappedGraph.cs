using OsmSharp.Collections.Arrays;
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
    /// A memory mapped graph to store CHEdgeData edges.
    /// </summary>
    public class LiveEdgeMemoryMappedGraph : MemoryMappedGraph<LiveEdge>
    {       
        /// <summary>
        /// Creates a new memory mapped file dynamic graph.
        /// </summary>
        /// <param name="estimatedSize"></param>
        public LiveEdgeMemoryMappedGraph(long estimatedSize)
            : this(estimatedSize, null)
        {

        }

        /// <summary>
        /// Creates a new memory mapped file dynamic graph.
        /// </summary>
        /// <param name="estimatedSize"></param>
        /// <param name="arraySize"></param>
        public LiveEdgeMemoryMappedGraph(long estimatedSize, int arraySize)
            : this(estimatedSize, arraySize, null)
        {

        }

        /// <summary>
        /// Creates a new memory mapped file dynamic graph.
        /// </summary>
        /// <param name="estimatedSize"></param>
        /// <param name="parameters"></param>
        public LiveEdgeMemoryMappedGraph(long estimatedSize, MemoryMappedFileParameters parameters)
            : base(estimatedSize,
            new MemoryMappedHugeArray<GeoCoordinateSimple>(MemoryMappedFileFactories.GeoCoordinateSimpleFile(parameters), estimatedSize),
            new MemoryMappedHugeArray<uint>(MemoryMappedFileFactories.UInt32File(parameters), estimatedSize),
            new MemoryMappedHugeArray<uint>(MemoryMappedFileFactories.UInt32File(parameters), estimatedSize),
            new MemoryMappedHugeArray<LiveEdge>(LiveEdgeMemoryMappedGraph.LiveEdgeFile(parameters), estimatedSize),
            new HugeCoordinateCollectionIndex(parameters, estimatedSize))
        {

        }

        /// <summary>
        /// Creates a new memory mapped file dynamic graph.
        /// </summary>
        /// <param name="estimatedSize"></param>
        /// <param name="parameters"></param>
        /// <param name="arraySize"></param>
        public LiveEdgeMemoryMappedGraph(long estimatedSize, int arraySize, MemoryMappedFileParameters parameters)
            : base(estimatedSize,
            new MemoryMappedHugeArray<GeoCoordinateSimple>(MemoryMappedFileFactories.GeoCoordinateSimpleFile(parameters), estimatedSize, arraySize),
            new MemoryMappedHugeArray<uint>(MemoryMappedFileFactories.UInt32File(parameters), estimatedSize, arraySize),
            new MemoryMappedHugeArray<uint>(MemoryMappedFileFactories.UInt32File(parameters), estimatedSize, arraySize),
            new MemoryMappedHugeArray<LiveEdge>(LiveEdgeMemoryMappedGraph.LiveEdgeFile(parameters), estimatedSize, arraySize),
            new HugeCoordinateCollectionIndex(parameters, estimatedSize))
        {

        }

        /// <summary>
        /// Creates a new memory mapped file factory.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static MemoryMappedFileFactory<LiveEdge> LiveEdgeFile(MemoryMappedFileParameters parameters)
        {
            return null;
        }
    }
}
