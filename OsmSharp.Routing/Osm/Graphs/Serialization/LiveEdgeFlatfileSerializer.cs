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
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Serialization;
using ProtoBuf;
using ProtoBuf.Meta;
using System.Collections.Generic;

namespace OsmSharp.Routing.Osm.Graphs.Serialization
{
    /// <summary>
    /// Serializes/deserializes edges.
    /// </summary>
    public class LiveEdgeFlatfileSerializer : FlatfileSerializer<LiveEdge>
    {
        /// <summary>
        /// Creates the graph to deserialize into.
        /// </summary>
        /// <param name="tagsCollectionIndex"></param>
        /// <returns></returns>
        protected override DynamicGraphRouterDataSource<LiveEdge> CreateGraph(ITagsCollectionIndex tagsCollectionIndex)
        {
            return new DynamicGraphRouterDataSource<LiveEdge>(tagsCollectionIndex);
        }

        /// <summary>
        /// Serializes all edges.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="graph"></param>
        protected override void SerializeEdges(LimitedStream stream, DynamicGraphRouterDataSource<LiveEdge> graph)
        {
            var typeModel = RuntimeTypeModel.Create();
            typeModel.Add(typeof(SerializableEdge), true);
            typeModel.Add(typeof(GeoCoordinateSimple), true);

            int blockSize = 10000;
            var arcsQueue = new List<SerializableEdge>(blockSize);

            uint vertex = 0;
            while (vertex < graph.VertexCount)
            { // keep looping and serialize all vertices.
                var edges = graph.GetEdges(vertex);
                // serialize the arcs, but serialize them only once. 
                // choose only those arcs that start at a vertex smaller than the target.
                foreach(var edge in edges)
                {
                    if (edge.Neighbour > vertex)
                    {
                        arcsQueue.Add(new SerializableEdge()
                        {
                            Distance = edge.EdgeData.Distance,
                            FromId = vertex,
                            ToId = edge.Neighbour,
                            Value = edge.EdgeData.Value,
                            Coordinates = edge.Intermediates.ToSimpleArray()
                        });

                        if (arcsQueue.Count == blockSize)
                        { // execute serialization.
                            typeModel.SerializeWithSize(stream, arcsQueue.ToArray());
                            arcsQueue.Clear();
                        }
                    }
                }

                // move to next vertex.
                vertex++;
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
        protected override void DeserializeEdges(LimitedStream stream, long size, DynamicGraphRouterDataSource<LiveEdge> graph)
        {
            var typeModel = RuntimeTypeModel.Create();
            typeModel.Add(typeof(SerializableEdge), true);
            typeModel.Add(typeof(GeoCoordinateSimple), true);

            long position = stream.Position;
            while(stream.Position < position + size)
            { // keep looping until the appropriate number of bytes have been read.
                var serializableEdges = typeModel.DeserializeWithSize(stream, null, typeof(SerializableEdge[])) as SerializableEdge[];
                for (int idx = 0; idx < serializableEdges.Length; idx++)
                {
                    ICoordinateCollection coordinateCollection = null;
                    if(serializableEdges[idx].Coordinates != null)
                    {
                        coordinateCollection = new CoordinateArrayCollection<GeoCoordinateSimple>(serializableEdges[idx].Coordinates);
                    }
                    graph.AddEdge(serializableEdges[idx].FromId, serializableEdges[idx].ToId,
                        new LiveEdge()
                        {
                            Distance = serializableEdges[idx].Distance,
                            Value = serializableEdges[idx].Value
                        }, coordinateCollection, null);
                }
            }
        }

        /// <summary>
        /// Returns the version string.
        /// </summary>
        public override string VersionString
        {
            get { return "LiveEdgeFlatfile.v1.0"; }
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
            /// Gets or sets the value.
            /// </summary>
            [ProtoMember(3)]
            public uint Value { get; set; }

            /// <summary>
            /// Gets or sets the distance.
            /// </summary>
            [ProtoMember(4)]
            public float Distance { get; set; }

            /// <summary>
            /// Gets or sets the coordinates.
            /// </summary>
            [ProtoMember(5)]
            public GeoCoordinateSimple[] Coordinates { get; set; }
        }
    }
}
