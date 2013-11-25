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

using System.Collections.Generic;
using ProtoBuf;

namespace OsmSharp.UI.Renderer.Scene.Storage
{
    /// <summary>
    /// Serializable leaf.
    /// </summary>
    [ProtoContract]
    internal class SceneObjectBlock
    {
        [ProtoMember(1, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public List<long> PointsX { get; set; }

        [ProtoMember(2, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public List<long> PointsY { get; set; }


        [ProtoMember(3, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public List<uint> PointStyleId { get; set; }

        [ProtoMember(4, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public List<int> PointPointId { get; set; }


        [ProtoMember(5, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public List<uint> TextPointStyleId { get; set; }

        [ProtoMember(6, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public List<int> TextPointPointId { get; set; }

        [ProtoMember(7)]
        public List<string> TextPointText { get; set; }


        [ProtoMember(8, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public List<uint> LineStyleId { get; set; }

        [ProtoMember(9, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public List<int> LinePointsId { get; set; }


        [ProtoMember(10, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public List<uint> PolygonStyleId { get; set; }

        [ProtoMember(11, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public List<int> PolygonPointsId { get; set; }


        [ProtoMember(12, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public List<uint> LineTextStyleId { get; set; }

        [ProtoMember(13, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public List<int> LineTextPointsId { get; set; }

        [ProtoMember(14)]
        public List<string> LineTextText { get; set; }


        [ProtoMember(15, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public List<uint> IconImageId { get; set; }

        [ProtoMember(16, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public List<int> IconPointId { get; set; }


        [ProtoMember(17, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public long PointsXMin { get; set; }

        [ProtoMember(18, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public long PointsYMin { get; set; }
    }
}