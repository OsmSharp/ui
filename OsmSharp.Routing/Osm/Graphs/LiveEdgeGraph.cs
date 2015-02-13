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

using OsmSharp.Collections.Arrays;
using OsmSharp.Collections.Arrays.MemoryMapped;
using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Graph;
using System;

namespace OsmSharp.Routing.Osm.Graphs
{
    /// <summary>
    /// A memory mapped graph to store liveedge edges.
    /// </summary>
    public class LiveEdgeGraph : MemoryGraph<LiveEdge>
    {   
        /// <summary>
        /// Creates a new graph.
        /// </summary>
        public LiveEdgeGraph()
            : base()
        {

        }

        /// <summary>
        /// Creates a new graph.
        /// </summary>
        /// <param name="sizeEstimate"></param>
        public LiveEdgeGraph(long sizeEstimate)
            : base(sizeEstimate)
        {

        }

        /// <summary>
        /// Creates a new memory mapped graph;
        /// </summary>
        /// <param name="file"></param>
        /// <param name="estimatedSize"></param>
        public LiveEdgeGraph(MemoryMappedFile file, long estimatedSize)
            : base(file, estimatedSize, LiveEdge.MapFromDelegate, LiveEdge.MapToDelegate, LiveEdge.SizeUints)
        {

        }

        /// <summary>
        /// Creates a memory mapped graph based on existing data.
        /// </summary>
        public LiveEdgeGraph(long vertexCount, long edgesCount, 
            MemoryMappedFile verticesFile,
            MemoryMappedFile verticesCoordinatesFile,
            MemoryMappedFile edgesFile,
            MemoryMappedFile edgeDataFile,
            MemoryMappedFile shapesIndexFile, long shapesIndexLength,
            MemoryMappedFile shapesCoordinateFile, long shapesCoordinateLength)
            : base(vertexCount, edgesCount, verticesFile, verticesCoordinatesFile, edgeDataFile, edgeDataFile, shapesIndexFile, shapesIndexLength, shapesCoordinateFile, shapesCoordinateLength,
                LiveEdge.MapFromDelegate, LiveEdge.MapToDelegate, LiveEdge.SizeUints)
        {

        }
    }
}