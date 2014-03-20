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

using OsmSharp.Collections.Tags.Index;
using OsmSharp.Collections.Tags.Serializer;
using OsmSharp.IO;
using OsmSharp.Routing.Graph.Router;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Routing.Graph.Serialization
{
    /// <summary>
    /// An abstract serializer to serialize/deserialize a routing data source to a flat-file.
    /// </summary>
    public abstract class FlatfileSerializer<TEdgeData> : RoutingDataSourceSerializer<TEdgeData>
        where TEdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Does the v1 serialization.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="graph"></param>
        /// <returns></returns>
        protected override void DoSerialize(LimitedStream stream,
            DynamicGraphRouterDataSource<TEdgeData> graph)
        {
            // LAYOUT:
            // [SIZE_OF_VERTICES(8bytes)][VERTICES][SIZE_OF_EDGES(8bytes)][EDGES][SIZE_OF_TAGS(8bytes][TAGS]

            // serialize coordinates.
            stream.Seek(8, System.IO.SeekOrigin.Current);
            long position = stream.Position;
            this.SerializeVertices(stream, graph);
            long size = stream.Position - position;
            stream.Seek(position - 8, System.IO.SeekOrigin.Begin);
            var sizeBytes = BitConverter.GetBytes(size);
            stream.Write(sizeBytes, 0, 8);
            stream.Seek(size, System.IO.SeekOrigin.Current);

            // serialize edges.
            stream.Seek(8, System.IO.SeekOrigin.Current);
            position = stream.Position;
            this.SerializeEdges(stream, graph);
            size = stream.Position - position;
            stream.Seek(position - 8, System.IO.SeekOrigin.Begin);
            sizeBytes = BitConverter.GetBytes(size);
            stream.Write(sizeBytes, 0, 8);
            stream.Seek(size, System.IO.SeekOrigin.Current);

            // serialize tags.
            stream.Seek(8, System.IO.SeekOrigin.Current);
            position = stream.Position;
            this.SerializeTags(stream, graph.TagsIndex);
            size = stream.Position - position;
            stream.Seek(position - 8, System.IO.SeekOrigin.Begin);
            sizeBytes = BitConverter.GetBytes(size);
            stream.Write(sizeBytes, 0, 8);
            stream.Seek(size, System.IO.SeekOrigin.Current);
        }

        /// <summary>
        /// Does the v1 deserialization.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lazy"></param>
        /// <param name="vehicles"></param>
        /// <returns></returns>
        protected override IBasicRouterDataSource<TEdgeData> DoDeserialize(
            LimitedStream stream, bool lazy, IEnumerable<string> vehicles)
        {
            ITagsCollectionIndex tagsCollectionIndex = this.CreateTagsCollectionIndex();
            DynamicGraphRouterDataSource<TEdgeData> graph = this.CreateGraph(tagsCollectionIndex);

            // deserialize vertices.
            var sizeBytes = new byte[8];
            stream.Read(sizeBytes, 0, 8);
            var position = stream.Position;
            var size = BitConverter.ToInt32(sizeBytes, 0);
            this.DeserializeVertices(stream, size, graph);
            stream.Seek(position + size, System.IO.SeekOrigin.Begin);

            // deserialize edges.
            stream.Read(sizeBytes, 0, 8);
            position = stream.Position;
            size = BitConverter.ToInt32(sizeBytes, 0);
            this.DeserializeEdges(stream, size, graph);
            stream.Seek(position + size, System.IO.SeekOrigin.Begin);

            // deserialize tags.
            stream.Read(sizeBytes, 0, 8);
            position = stream.Position;
            size = BitConverter.ToInt32(sizeBytes, 0);
            this.DeserializeTags(stream, size, tagsCollectionIndex);
            stream.Seek(position + size, System.IO.SeekOrigin.Begin);

            return graph;
        }

        /// <summary>
        /// Creates the graph.
        /// </summary>
        /// <returns></returns>
        protected abstract DynamicGraphRouterDataSource<TEdgeData> CreateGraph(ITagsCollectionIndex tagsCollectionIndex);

        /// <summary>
        /// Creates the tags collection index.
        /// </summary>
        /// <returns></returns>
        protected virtual ITagsCollectionIndex CreateTagsCollectionIndex()
        {
            return new TagsTableCollectionIndex();
        }

        /// <summary>
        /// Serializes the vertices
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="graph"></param>
        protected virtual void SerializeVertices(LimitedStream stream, DynamicGraphRouterDataSource<TEdgeData> graph)
        {
            RuntimeTypeModel typeModel = RuntimeTypeModel.Create();
            typeModel.Add(typeof(SerializableVertex), true);

            int blockSize = 10;
            var vertices = new SerializableVertex[blockSize];
            uint vertex = 1;
            float latitude, longitude;
            while(vertex <= graph.VertexCount)
            {
                // adjust array size if needed.
                if (vertices.Length > graph.VertexCount - vertex)
                { // shrink array.
                    vertices = new SerializableVertex[graph.VertexCount - vertex + 1];
                }

                // build block.
                for (uint idx = 0; idx < vertices.Length; idx++)
                {
                    uint current = vertex + idx;
                    if (vertex <= graph.VertexCount && graph.GetVertex(current, out latitude, out longitude))
                    { // vertex in the graph.
                        if(vertices[idx] == null)
                        { // make sure there is a vertex.
                            vertices[idx] = new SerializableVertex();
                        }
                        vertices[idx].Latitude = latitude;
                        vertices[idx].Longitude = longitude;
                    }
                    else
                    { // vertex not in the graph.
                        throw new Exception("Cannot serialize non-existing vertices!");
                    }
                }

                // serialize.
                typeModel.SerializeWithSize(stream, vertices);

                // move to the next vertex.
                vertex = (uint)(vertex + blockSize);
            }
        }

        /// <summary>
        /// Serializes the edges.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="graph"></param>
        protected abstract void SerializeEdges(LimitedStream stream, DynamicGraphRouterDataSource<TEdgeData> graph);

        /// <summary>
        /// Serializes the meta-data.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="tagsCollectionIndex"></param>
        protected virtual void SerializeTags(LimitedStream stream, ITagsCollectionIndexReadonly tagsCollectionIndex)
        {
            // write tags collection-count.
            var countBytes = BitConverter.GetBytes(tagsCollectionIndex.Max);
            stream.Write(countBytes, 0, 4);

            // serialize tags collections one-by-one.
            var serializer = new TagsCollectionSerializer();
            for (uint idx = 0; idx < tagsCollectionIndex.Max; idx++)
            { // serialize objects one-by-one.
                serializer.SerializeWithSize(tagsCollectionIndex.Get(idx), stream);
            }
        }

        /// <summary>
        /// Deserializes the vertices
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="graph"></param>
        /// <param name="size"></param>
        protected virtual void DeserializeVertices(LimitedStream stream, long size, DynamicGraphRouterDataSource<TEdgeData> graph)
        {
            RuntimeTypeModel typeModel = RuntimeTypeModel.Create();
            typeModel.Add(typeof(SerializableVertex), true);

            long position = stream.Position;
            uint vertex = 0;
            while (stream.Position - position < size)
            { // keep reading vertices until the appriated number of bytes have been read.
                var vertices = typeModel.DeserializeWithSize(stream, null, typeof(SerializableVertex[])) as SerializableVertex[];
                if (vertices != null)
                { // there are a vertices.
                    for (int idx = 0; idx < vertices.Length; idx++)
                    {
                        if(vertices[idx] != null)
                        { // there is a vertex.
                            graph.AddVertex(vertices[idx].Latitude, vertices[idx].Longitude);
                        }
                        vertex++;
                    }
                }
            }
        }

        /// <summary>
        /// Deserializes the edges.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="size"></param>
        /// <param name="graph"></param>
        protected abstract void DeserializeEdges(LimitedStream stream, long size, DynamicGraphRouterDataSource<TEdgeData> graph);

        /// <summary>
        /// Deserializes the meta-data.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="tagsCollectionIndex"></param>
        /// <param name="size"></param>
        protected virtual void DeserializeTags(LimitedStream stream, long size, ITagsCollectionIndex tagsCollectionIndex)
        {           
            // read tags collection-count.
            var countBytes = new byte[4];
            stream.Read(countBytes, 0, 4);
            int max = BitConverter.ToInt32(countBytes, 0);

            // serialize tags collections one-by-one.
            var serializer = new TagsCollectionSerializer();
            for (uint idx = 0; idx < max; idx++)
            { // serialize objects one-by-one.
                tagsCollectionIndex.Add(serializer.DeserializeWithSize(stream));
            }
        }

        #region Serializable Classes

        /// <summary>
        /// Serializable coordinate.
        /// </summary>
        [ProtoContract]
        internal class SerializableVertex
        {
            /// <summary>
            /// Gets/sets the latitude.
            /// </summary>
            [ProtoMember(1)]
            public float Latitude { get; set; }

            /// <summary>
            /// Gets/sets the longitude.
            /// </summary>
            [ProtoMember(2)]
            public float Longitude { get; set; }
        }

        #endregion
    }
}