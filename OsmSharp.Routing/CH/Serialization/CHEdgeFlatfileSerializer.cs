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

using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.IO;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Serialization;
using ProtoBuf;
using ProtoBuf.Meta;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Routing.CH.Serialization
{
    /// <summary>
    /// Serializes/deserialiers a routing data source with CH edges.
    /// </summary>
    public class CHEdgeFlatfileSerializer : FlatfileSerializer<CHEdgeData>
    {
        /// <summary>
        /// Creates the graph to serialize into.
        /// </summary>
        /// <param name="tagsCollectionIndex"></param>
        /// <returns></returns>
        protected override DynamicGraphRouterDataSource<CHEdgeData> CreateGraph(ITagsCollectionIndex tagsCollectionIndex)
        {
            return new DynamicGraphRouterDataSource<CHEdgeData>(tagsCollectionIndex);
        }

        /// <summary>
        /// Serializes all edges.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="graph"></param>
        protected override void SerializeEdges(LimitedStream stream, DynamicGraphRouterDataSource<CHEdgeData> graph)
        {
            var typeModel = RuntimeTypeModel.Create();
            typeModel.Add(typeof(SerializableEdge), true);

            int blockSize = 1000;
            var arcsQueue = new List<SerializableEdge>(blockSize);

            uint vertex = 0;
            while (vertex < graph.VertexCount)
            { // keep looping and serialize all vertices.
                var arcs = graph.GetEdges(vertex).ToList();
                if (arcs != null)
                { // serialize the arcs, but serialize them only once. 
                    // choose only those arcs that start at a vertex smaller than the target.
                    for (int idx = 0; idx < arcs.Count; idx++)
                    {
                        if (arcs[idx].Neighbour > vertex)
                        {
                            arcsQueue.Add(new SerializableEdge()
                            {
                                FromId = vertex,
                                ToId = arcs[idx].Neighbour,
                                ContractedDirectionValue = arcs[idx].EdgeData.ContractedDirectionValue,
                                TagsValue = arcs[idx].EdgeData.TagsValue,
                                ForwardContractedId = arcs[idx].EdgeData.ForwardContractedId,
                                ForwardWeight = arcs[idx].EdgeData.ForwardWeight,
                                BackwardContractedId = arcs[idx].EdgeData.BackwardContractedId,
                                BackwardWeight = arcs[idx].EdgeData.BackwardWeight,
                                Coordinates = arcs[idx].Intermediates.ToSimpleArray()
                            });

                            if (arcsQueue.Count == blockSize)
                            { // execute serialization.
                                typeModel.SerializeWithSize(stream, arcsQueue.ToArray());
                                arcsQueue.Clear();
                            }
                        }
                    }

                    // serialize.
                    vertex++;
                }
            }

            if (arcsQueue.Count > 0)
            { // execute serialization.
                typeModel.SerializeWithSize(stream, arcsQueue.ToArray());
                arcsQueue.Clear();
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
                    ICoordinateCollection coordinateCollection = null;
                    if (serializableEdges[idx].Coordinates != null)
                    {
                        coordinateCollection = new CoordinateArrayCollection<GeoCoordinateSimple>(serializableEdges[idx].Coordinates);
                    }
                    graph.AddEdge(serializableEdges[idx].FromId, serializableEdges[idx].ToId,
                        new CHEdgeData()
                        {
                            ContractedDirectionValue = serializableEdges[idx].ContractedDirectionValue,
                            TagsValue = serializableEdges[idx].TagsValue,
                            ForwardContractedId = serializableEdges[idx].ForwardContractedId,
                            ForwardWeight = serializableEdges[idx].ForwardWeight,
                            BackwardContractedId = serializableEdges[idx].BackwardContractedId,
                            BackwardWeight = serializableEdges[idx].BackwardWeight
                        }, coordinateCollection);
                }
            }
        }

        /// <summary>
        /// Returns the version string.
        /// </summary>
        public override string VersionString
        {
            get { return "CHedgeFlatfile.v2.0"; }
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
            public float ForwardWeight { get; set; }

            /// <summary>
            /// The vertex contracted by this edge.
            /// </summary>
            [ProtoMember(4)]
            public uint ForwardContractedId { get; set; }

            /// <summary>
            /// Gets or sets the weight.
            /// </summary>
            [ProtoMember(5)]
            public float BackwardWeight { get; set; }

            /// <summary>
            /// The vertex contracted by this edge.
            /// </summary>
            [ProtoMember(6)]
            public uint BackwardContractedId { get; set; }

            /// <summary>
            /// Gets or sets the raw direction value.
            /// </summary>
            [ProtoMember(7)]
            public byte ContractedDirectionValue { get; set; }

            /// <summary>
            /// Gets or sets the raw tags value (including direction).
            /// </summary>
            [ProtoMember(8)]
            public uint TagsValue { get; set; }

            /// <summary>
            /// Gets or sets the coordinates.
            /// </summary>
            [ProtoMember(9)]
            public GeoCoordinateSimple[] Coordinates { get; set; }
        }
    }
}