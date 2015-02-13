// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph;

namespace OsmSharp.Routing.CH
{
    /// <summary>
    /// A memory mapped graph to store CHEdgeData edges.
    /// </summary>
    public class CHEdgeDataGraph : MemoryDirectedGraph<CHEdgeData>
    {
        /// <summary>
        /// Creates a new contracted edge graph.
        /// </summary>
        public CHEdgeDataGraph()
            : base()
        {

        }
        /// <summary>
        /// Creates a new contracted edge graph.
        /// </summary>
        /// <param name="sizeEstimate">The estimated size.</param>
        public CHEdgeDataGraph(long sizeEstimate)
            : base(sizeEstimate)
        {

        }

        /// <summary>
        /// Creates a new memory mapped graph.
        /// </summary>
        /// <param name="file">The file to store the data at.</param>
        /// <param name="estimatedSize">The estimated size.</param>
        public CHEdgeDataGraph(MemoryMappedFile file, long estimatedSize)
            : base(file, estimatedSize,
            (array, idx) =>
            {
                return new CHEdgeData(
                    array[idx],
                    array[idx + 1],
                    (byte)array[idx + 2]);
            }, 
            (array, idx, value) =>
            {
                array[idx] = value.Value;
                array[idx + 1] = value.Tags;
                array[idx + 2] = value.Meta;
            }, 3)
        {

        }

        /// <summary>
        /// Creates a new memory mapped graph based on existing data.
        /// </summary>
        /// <param name="vertexCount"></param>
        /// <param name="edgesCount"></param>
        /// <param name="verticesFile"></param>
        /// <param name="verticesCoordinatesFile"></param>
        /// <param name="edgesFile"></param>
        /// <param name="edgeDataFile"></param>
        /// <param name="shapesIndexFile"></param>
        /// <param name="shapesIndexLength"></param>
        /// <param name="shapesCoordinateFile"></param>
        /// <param name="shapesCoordinateLength"></param>
        public CHEdgeDataGraph(long vertexCount, long edgesCount, 
            MemoryMappedFile verticesFile,
            MemoryMappedFile verticesCoordinatesFile,
            MemoryMappedFile edgesFile,
            MemoryMappedFile edgeDataFile,
            MemoryMappedFile shapesIndexFile, long shapesIndexLength,
            MemoryMappedFile shapesCoordinateFile, long shapesCoordinateLength)
            : base(vertexCount, edgesCount, verticesFile, verticesCoordinatesFile, edgesFile, edgeDataFile, shapesIndexFile, shapesIndexLength,
                shapesCoordinateFile, shapesCoordinateLength,
            (array, idx) =>
            {
                return new CHEdgeData(
                    array[idx],
                    array[idx + 1],
                    (byte)array[idx + 2]);
            }, 
            (array, idx, value) =>
            {
                array[idx] = value.Value;
                array[idx + 1] = value.Tags;
                array[idx + 2] = value.Meta;
            }, 3)
        {

        }
    }
}