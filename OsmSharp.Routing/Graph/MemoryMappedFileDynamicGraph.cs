// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Collections.Arrays;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Math.Geo.Simple;
using System;

namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// An implementation of an in-memory dynamic graph using memory mapped files to handle huge graphs.
    /// </summary>
    /// <typeparam name="TEdgeData"></typeparam>
    public class MemoryMappedFileDynamicGraph<TEdgeData> : MemoryDynamicGraph<TEdgeData>, IDisposable
        where TEdgeData : struct, IDynamicGraphEdgeData
    {
        /// <summary>
        /// The memory mapped file for the vertices.
        /// </summary>
        private IMemoryMappedFile _verticesFile;

        /// <summary>
        /// The memory mapped file for the edges.
        /// </summary>
        private IMemoryMappedFile _edgesFile;

        /// <summary>
        /// The memory mapped file for the coordinates.
        /// </summary>
        private IMemoryMappedFile _coordinatesFile;

        /// <summary>
        /// The memory mapped file for the edge data.
        /// </summary>
        private IMemoryMappedFile _edgeDataFile;

        /// <summary>
        /// Creates a new memory mapped file dynamic graph.
        /// </summary>
        /// <param name="estimatedSize"></param>
        /// <param name="path"></param>
        public MemoryMappedFileDynamicGraph(long estimatedSize, string path)
            : this(estimatedSize,
            MemoryMappedFileDynamicGraph<TEdgeData>.CreateMemoryMappedFile(path),
            MemoryMappedFileDynamicGraph<TEdgeData>.CreateMemoryMappedFile(path),
            MemoryMappedFileDynamicGraph<TEdgeData>.CreateMemoryMappedFile(path),
            MemoryMappedFileDynamicGraph<TEdgeData>.CreateMemoryMappedFile(path))
        {

        }

        /// <summary>
        /// Creates a new memory mapped file dynamic graph based on the given memory mapped files.
        /// </summary>
        /// <param name="estimatedSize"></param>
        /// <param name="coordinatesFile"></param>
        /// <param name="verticesFile"></param>
        /// <param name="edgesFile"></param>
        /// <param name="edgeDataFile"></param>
        private MemoryMappedFileDynamicGraph(long estimatedSize,
            IMemoryMappedFile coordinatesFile,
            IMemoryMappedFile verticesFile,
            IMemoryMappedFile edgesFile,
            IMemoryMappedFile edgeDataFile)
            : base(estimatedSize,
                MemoryMappedFileDynamicGraph<TEdgeData>.CreateArray<GeoCoordinateSimple>(coordinatesFile),
                MemoryMappedFileDynamicGraph<TEdgeData>.CreateArray<uint>(verticesFile),
                MemoryMappedFileDynamicGraph<TEdgeData>.CreateArray<uint>(edgesFile),
                MemoryMappedFileDynamicGraph<TEdgeData>.CreateArray<TEdgeData>(edgeDataFile))
        {
            _coordinatesFile = coordinatesFile;
            _edgesFile = edgesFile;
            _edgeDataFile = edgeDataFile;
            _verticesFile = verticesFile;
        }

        /// <summary>
        /// Creates a random memory mapped file in the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static IMemoryMappedFile CreateMemoryMappedFile(string path)
        {
            return NativeMemoryMappedFileFactory.Create(path + System.Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Creates a memory mapped arra.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static IHugeArray<T> CreateArray<T>(IMemoryMappedFile file) where T : struct
        {
            return new MemoryMappedHugeArray<T>(file, 10);
        }

        /// <summary>
        /// Disposes of all native resources associated with this memory dynamic graph.
        /// </summary>
        public void Dispose()
        {
            _coordinatesFile.Dispose();
            _coordinatesFile = null;
            _edgeDataFile.Dispose();
            _edgeDataFile = null;
            _edgesFile.Dispose();
            _edgesFile = null;
            _verticesFile.Dispose();
            _verticesFile = null;
        }
    }
}
