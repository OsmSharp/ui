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

using OsmSharp.IO;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Serialization;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Routing.CH.Serialization
{
    /// <summary>
    /// Serializes/deserialiers a routing data source with CH edges.
    /// </summary>
    public class CHEdgeFlatfileSerializer : FlatfileSerializer<CHEdgeData>
    {
        /// <summary>
        /// Serializes all edges.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="graph"></param>
        protected override void SerializeEdges(LimitedStream stream, DynamicGraphRouterDataSource<CHEdgeData> graph)
        {
            var typeModel = RuntimeTypeModel.Create();
            typeModel.Add(typeof(SerializableEdge), true);

            uint vertex = 0;
            while (vertex < graph.VertexCount)
            { // keep looping and serialize all vertices.
                var arcs = graph.GetArcs(vertex);
                if (arcs != null)
                { // serialize the arcs.
                    var serializableArcs = new SerializableEdge[arcs.Length];
                    for (int idx = 0; idx < arcs.Length; idx++)
                    {
                        serializableArcs[idx] = new SerializableEdge()
                        {
                            FromId = vertex,
                            ToId = arcs[idx].Key,
                            ContractedVertexId = arcs[idx].Value.ContractedVertexId,
                            Direction = arcs[idx].Value.Direction,
                            Tags = arcs[idx].Value.Tags,
                            Weight = arcs[idx].Value.Weight
                        };
                    }

                    // serialize.
                    typeModel.SerializeWithSize(stream, serializableArcs);
                    vertex++;
                }
            }
        }

        /// <summary>
        /// Deserializes all edges.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="size"></param>
        /// <param name="graph"></param>
        protected override void DeserializeEdges(LimitedStream stream, long size, DynamicGraphRouterDataSource<CHEdgeData> graph)
        {
            var typeModel = RuntimeTypeModel.Create();
            typeModel.Add(typeof(SerializableEdge), true);

            long position = stream.Position;
            while (stream.Position < position + size)
            { // keep looping until the appropriate number of bytes have been read.
                var serializableEdges = typeModel.DeserializeWithSize(stream, null, typeof(SerializableEdge[])) as SerializableEdge[];
                for (int idx = 0; idx < serializableEdges.Length; idx++)
                {
                    graph.AddArc(serializableEdges[idx].FromId, serializableEdges[idx].ToId,
                        new CHEdgeData()
                        {
                            ContractedVertexId = serializableEdges[idx].ContractedVertexId,
                            Direction = serializableEdges[idx].Direction,
                            Tags = serializableEdges[idx].Tags,
                            Weight = serializableEdges[idx].Weight
                        }, null);
                }
            }
        }

        /// <summary>
        /// Returns the version string.
        /// </summary>
        public override string VersionString
        {
            get { return "CHedgeFlatfile.v1.0"; }
        }

        /// <summary>
        /// A serializable edge.
        /// </summary>
        [ProtoContract]
        private class SerializableEdge
        {
            /// <summary>
            /// Gets or sets the from id.
            /// </summary>
            [ProtoMember(1)]
            public uint FromId { get; set; }

            /// <summary>
            /// Gets or sets the to id.
            /// </summary>
            [ProtoMember(2)]
            public uint ToId { get; set; }

            /// <summary>
            /// Gets or sets the weight.
            /// </summary>
            [ProtoMember(3)]
            public float Weight { get; set; }

            /// <summary>
            /// Gets or sets the direction.
            /// </summary>
            [ProtoMember(4)]
            public byte Direction { get; set; }

            /// <summary>
            /// The vertex contracted by this edge.
            /// </summary>
            [ProtoMember(5)]
            public uint ContractedVertexId { get; set; }

            /// <summary>
            /// Returns the tags (0 means no tags). 
            /// </summary>
            [ProtoMember(6)]
            public uint Tags { get; set; }
        }
    }
}