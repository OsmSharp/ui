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

namespace OsmSharp.Routing.CH
{
    /// <summary>
    /// A memory mapped graph to store CHEdgeData edges.
    /// </summary>
    public class CHEdgeMemoryMappedGraph : MemoryMappedGraph<CHEdgeData>
    {       
        /// <summary>
        /// Creates a new memory mapped file dynamic graph.
        /// </summary>
        /// <param name="estimatedSize"></param>
        public CHEdgeMemoryMappedGraph(long estimatedSize)
            : this(estimatedSize, null)
        {

        }

        /// <summary>
        /// Creates a new memory mapped file dynamic graph.
        /// </summary>
        /// <param name="estimatedSize"></param>
        /// <param name="arraySize"></param>
        public CHEdgeMemoryMappedGraph(long estimatedSize, int arraySize)
            : this(estimatedSize, arraySize, null)
        {

        }

        /// <summary>
        /// Creates a new memory mapped file dynamic graph.
        /// </summary>
        /// <param name="estimatedSize"></param>
        /// <param name="parameters"></param>
        public CHEdgeMemoryMappedGraph(long estimatedSize, MemoryMappedFileParameters parameters)
            : base(estimatedSize,
            new MemoryMappedHugeArray<GeoCoordinateSimple>(MemoryMappedFileFactories.GeoCoordinateSimpleFile(parameters), estimatedSize),
            new MemoryMappedHugeArray<uint>(MemoryMappedFileFactories.UInt32File(parameters), estimatedSize),
            new MemoryMappedHugeArray<uint>(MemoryMappedFileFactories.UInt32File(parameters), estimatedSize),
            new MemoryMappedHugeArray<CHEdgeData>(CHEdgeMemoryMappedGraph.CHEdgeData(parameters), estimatedSize),
            new HugeCoordinateCollectionIndex(parameters, estimatedSize))
        {

        }

        /// <summary>
        /// Creates a new memory mapped file dynamic graph.
        /// </summary>
        /// <param name="estimatedSize"></param>
        /// <param name="parameters"></param>
        /// <param name="arraySize"></param>
        public CHEdgeMemoryMappedGraph(long estimatedSize, int arraySize, MemoryMappedFileParameters parameters)
            : base(estimatedSize,
            new MemoryMappedHugeArray<GeoCoordinateSimple>(MemoryMappedFileFactories.GeoCoordinateSimpleFile(parameters), estimatedSize, arraySize),
            new MemoryMappedHugeArray<uint>(MemoryMappedFileFactories.UInt32File(parameters), estimatedSize, arraySize),
            new MemoryMappedHugeArray<uint>(MemoryMappedFileFactories.UInt32File(parameters), estimatedSize, arraySize),
            new MemoryMappedHugeArray<CHEdgeData>(CHEdgeMemoryMappedGraph.CHEdgeData(parameters), estimatedSize, arraySize),
            new HugeCoordinateCollectionIndex(parameters, estimatedSize))
        {

        }

        /// <summary>
        /// Creates a new memory mapped file factory.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static MemoryMappedFileFactory<CHEdgeData> CHEdgeData(MemoryMappedFileParameters parameters)
        {
            return null;
        }
    }
}
