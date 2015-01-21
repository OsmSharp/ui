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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using OsmSharp.Math.Geo.Simple;

namespace OsmSharp.Routing.CH.Serialization.Sorted
{
    /// <summary>
    /// Represents an index of all CH blocks.
    /// </summary>
    [ProtoContract]
    public class CHBlockIndex
    {
        /// <summary>
        /// Holds all relative block locations in the file.
        /// </summary>
        [ProtoMember(1)]
        public int[] BlockLocationIndex { get; set; }
    }

    /// <summary>
    /// Represents a block containing sorted nodes and their respective arcs.
    /// </summary>
    [ProtoContract]
    public class CHBlock
    {
        /// <summary>
        /// Holds lower/higher index of arcs.
        /// </summary>
        [ProtoMember(1)]
        public CHVertex[] Vertices { get; set; }

        /// <summary>
        /// Holds the array of arcs for all nodes in this block.
        /// </summary>
        [ProtoMember(2)]
        public CHArc[] Arcs { get; set; }

        /// <summary>
        /// Calculates the block id for the given vertex id using the block size.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="blockSize"></param>
        /// <returns></returns>
        public static uint CalculateId(uint vertexId, uint blockSize)
        {
            return (((vertexId - 1) / blockSize) * (uint)blockSize) + 1;
        }
    }

    /// <summary>
    /// Represents a block containing arc shapes in the same position compared to the regular blocks.
    /// </summary>
    [ProtoContract]
    public class CHBlockCoordinates
    {
        /// <summary>
        /// Holds the array of arcs shapes for all nodes in this block.
        /// </summary>
        [ProtoMember(1)]
        public CHArcCoordinates[] Arcs { get; set; }
    }

    /// <summary>
    /// Represents a block containing arc shapes.
    /// </summary>
    [ProtoContract]
    public struct CHArcCoordinates
    {
        /// <summary>
        /// Gets or sets the coordinates.
        /// </summary>
        [ProtoMember(1)]
        public GeoCoordinateSimple[] Coordinates { get; set; }
    }

    /// <summary>
    /// Represents a CH vertex.
    /// </summary>
    [ProtoContract]
    public struct CHVertex
    {
        /// <summary>
        /// The lower index of the arcs associated with this vertex.
        /// </summary>
        [ProtoMember(1)]
        public ushort ArcIndex { get; set; }

        /// <summary>
        /// The number of arcs associated with this vertex.
        /// </summary>
        [ProtoMember(2)]
        public ushort ArcCount { get; set; }

        /// <summary>
        /// Holds the vertex latitude.
        /// </summary>
        [ProtoMember(3)]
        public float Latitude { get; set; }

        /// <summary>
        /// Holds the vertex longitude.
        /// </summary>
        [ProtoMember(4)]
        public float Longitude { get; set; }
    }

    /// <summary>
    /// Represents CH arc.
    /// </summary>
    [ProtoContract]
    public struct CHArc
    {
        /// <summary>
        /// The id of the target-vertex when not in this block.
        /// </summary>
        [ProtoMember(1)]
        public uint TargetId { get; set; }

        /// <summary>
        /// The weight of this arc.
        /// </summary>
        [ProtoMember(2)]
        public float Weight { get; set; }

        /// <summary>
        /// Gets or sets the raw value.
        /// </summary>
        [ProtoMember(3)]
        public uint Value { get; set; }

        /// <summary>
        /// Gets or sets the raw meta data.
        /// </summary>
        [ProtoMember(4)]
        public byte Meta { get; set; }
    }

    /// <summary>
    /// Represents an index of all CH regions.
    /// </summary>
    [ProtoContract]
    public class CHVertexRegionIndex
    {
        /// <summary>
        /// Holds all region ids.
        /// </summary>
        [ProtoMember(1)]
        public ulong[] RegionIds { get; set; }

        /// <summary>
        /// Holds all relative region locations in the file.
        /// </summary>
        [ProtoMember(2)]
        public int[] LocationIndex { get; set; }
    }

    /// <summary>
    /// Represents a region containing a list of vertices.
    /// </summary>
    [ProtoContract]
    public class CHVertexRegion
    {
        /// <summary>
        /// The list of vertices in this region.
        /// </summary>
        [ProtoMember(1)]
        public uint[] Vertices { get; set; }
    }
}