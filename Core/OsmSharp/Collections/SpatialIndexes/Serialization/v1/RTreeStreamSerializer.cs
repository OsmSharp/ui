// OsmSharp - OpenStreetMap tools & library.
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

using System;
using System.Collections.Generic;
using System.IO;
using OsmSharp.Math.Primitives;
using ProtoBuf;
using ProtoBuf.Meta;

namespace OsmSharp.Collections.SpatialIndexes.Serialization.v1
{
    /// <summary>
    /// Serializer for an R-tree spatial index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RTreeStreamSerializer<T> : SpatialIndexSerializer<T>
    {
        /// <summary>
        /// Creates a new serializer.
        /// </summary>
        protected RTreeStreamSerializer()
        {

        }

        /// <summary>
        /// Holds the type model.
        /// </summary>
        private RuntimeTypeModel _typeModel;

        /// <summary>
        /// Returns the runtime type model.
        /// </summary>
        /// <returns></returns>
        private RuntimeTypeModel GetRuntimeTypeModel()
        {
            if (_typeModel == null)
            {
                // build the run time type model.
                _typeModel = TypeModel.Create();
                _typeModel.Add(typeof(ChildrenIndex), true); // the tile metadata.
                this.BuildRuntimeTypeModel(_typeModel);
            }
            return _typeModel;
        }

        /// <summary>
        /// Builds the type model.
        /// </summary>
        /// <param name="typeModel"></param>
        protected abstract void BuildRuntimeTypeModel(RuntimeTypeModel typeModel);

        /// <summary>
        /// Serializes the given index to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="index"></param>
        protected override void DoSerialize(SpatialIndexSerializerStream stream, 
            RTreeStreamIndex<T> index)
        {
            // build the run time type model.
            RuntimeTypeModel typeModel = this.GetRuntimeTypeModel();

            // serialize root node.
            byte[] serializedRoot = 
                this.Serialize(typeModel, index.Root);
            stream.Write(serializedRoot, 0, serializedRoot.Length);
            stream.Flush();
        }

        /// <summary>
        /// Serializes all children of the given node.
        /// </summary>
        /// <param name="typeModel"></param>
        /// <param name="nodeBase"></param>
        /// <returns></returns>
        private byte[] Serialize(RuntimeTypeModel typeModel, RTreeStreamIndex<T>.RTreeNodeBase nodeBase)
        {
            var stream = new MemoryStream();
            if (nodeBase is RTreeStreamIndex<T>.RTreeNode)
            { // the node is not a leaf.
                int position = 0;
                var node = (nodeBase as RTreeStreamIndex<T>.RTreeNode);
                var childrenIndex = new ChildrenIndex();
                childrenIndex.IsLeaf = new bool[node.Count];
                childrenIndex.MinX = new float[node.Count];
                childrenIndex.MinY = new float[node.Count];
                childrenIndex.MaxX = new float[node.Count];
                childrenIndex.MaxY = new float[node.Count];
                childrenIndex.Starts = new int[node.Count];
                var serializedChildren = new List<byte[]>();
                for (int idx = 0; idx < node.Count; idx++)
                {
                    RectangleF2D box = node.Box(idx);
                    childrenIndex.MinX[idx] = (float) box.Min[0];
                    childrenIndex.MinY[idx] = (float) box.Min[1];
                    childrenIndex.MaxX[idx] = (float) box.Max[0];
                    childrenIndex.MaxY[idx] = (float) box.Max[1];
                    childrenIndex.IsLeaf[idx] = (node.Child(idx) is RTreeStreamIndex<T>.RTreeLeafNode);

                    byte[] childSerialized = this.Serialize(typeModel, node.Child(idx));
                    serializedChildren.Add(childSerialized);

                    childrenIndex.Starts[idx] = position;
                    position = position + childSerialized.Length;
                }
                childrenIndex.End = position;

                // serialize this index object.
                var indexStream = new MemoryStream();
                typeModel.Serialize(indexStream, childrenIndex);
                byte[] indexBytes = indexStream.ToArray();

                // START WRITING THE DATA TO THE TARGET STREAM HERE!

                // 1: write the type of data.
                byte[] leafFlag = new[] { (byte)(false ? 1 : 0) };
                stream.Write(leafFlag, 0, 1);
                
                // 2: Write the length of the meta data.
                byte[] indexLength = BitConverter.GetBytes(indexBytes.Length);
                stream.Write(indexLength, 0, indexLength.Length);

                // 3: write the meta data or the node-index.
                stream.Write(indexBytes, 0, indexBytes.Length);

                // 4: write the actual children.
                for (int idx = 0; idx < serializedChildren.Count; idx++)
                {
                    stream.Write(serializedChildren[idx], 0, serializedChildren[idx].Length);
                }
            }
            else if (nodeBase is RTreeStreamIndex<T>.RTreeLeafNode)
            { // the node is a leaf node.
                // START WRITING THE DATA TO THE TARGET STREAM HERE!

                // 1: write the type of data.
                byte[] leafFlag = new[] { (byte)(true ? 1 : 0) };
                stream.Write(leafFlag, 0, 1);

                // 2: write the leaf data.
                return this.Serialize(typeModel,
                    (nodeBase as RTreeStreamIndex<T>.RTreeLeafNode).Children, 
                    (nodeBase as RTreeStreamIndex<T>.RTreeLeafNode).Boxes);
            }
            else
            {
                throw new Exception("Unknown node type!");
            }
            byte[] serialized = stream.ToArray();
            stream.Dispose();
            return serialized;
        }

        /// <summary>
        /// Serializes all data on one leaf.
        /// </summary>
        /// <param name="typeModel"></param>
        /// <param name="data"></param>
        /// <param name="boxes"></param>
        /// <returns></returns>
        protected abstract byte[] Serialize(RuntimeTypeModel typeModel, List<T> data, 
            List<RectangleF2D> boxes);

        /// <summary>
        /// Deserializes all data on one leaf.
        /// </summary>
        /// <param name="typeModel"></param>
        /// <param name="data"></param>
        /// <param name="boxes"></param>
        /// <returns></returns>
        protected abstract List<T> DeSerialize(RuntimeTypeModel typeModel, byte[] data,
            out List<RectangleF2D> boxes);

        /// <summary>
        /// Deserializes the given stream into an index.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lazy"></param>
        /// <returns></returns>
        protected override RTreeStreamIndex<T> DoDeserialize(SpatialIndexSerializerStream stream, bool lazy)
        {
            // build the run time type model.
            RuntimeTypeModel typeModel = this.GetRuntimeTypeModel();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Represents a reserializable index of children of an R-tree node.
        /// </summary>
        internal class ChildrenIndex
        {
            /// <summary>
            /// The min X of each child.
            /// </summary>
            [ProtoMember(1)]
            public float[] MinX { get; set; }

            /// <summary>
            /// The min Y of each child.
            /// </summary>
            [ProtoMember(2)]
            public float[] MinY { get; set; }

            /// <summary>
            /// The max X of each child.
            /// </summary>
            [ProtoMember(3)]
            public float[] MaxX { get; set; }

            /// <summary>
            /// The max Y of each child.
            /// </summary>
            [ProtoMember(4)]
            public float[] MaxY { get; set; }

            /// <summary>
            /// The start position of each node in the stream.
            /// </summary>
            [ProtoMember(5)]
            public int[] Starts { get; set; }

            /// <summary>
            /// The end of this node in the stream.
            /// </summary>
            [ProtoMember(6)]
            public int End { get; set; }

            /// <summary>
            /// Gets or sets the type flags.
            /// </summary>
            [ProtoMember(7)]
            public bool[] IsLeaf { get; set; }
        }

        /// <summary>
        /// Deserializes the root.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        internal RTreeStreamIndex<T>.RTreeNodeBase DeserializeNode(SpatialIndexSerializerStream stream, 
            int position, int size)
        {
            // build the run time type model.
            RuntimeTypeModel typeModel = this.GetRuntimeTypeModel();

            var leafFlag = new byte[1];
            stream.Read(leafFlag, 0, 1);
            if (leafFlag[0] == (byte)1 && size > 0)
            { // the data is a leaf and can be read.
                var dataBytes = new byte[size];
                stream.Read(dataBytes, 0, dataBytes.Length);
                List<RectangleF2D> boxes;
                List<T> data = this.DeSerialize(typeModel, dataBytes, out boxes);

                return new RTreeStreamIndex<T>.RTreeLeafNode(boxes, 
                    data);
            }
            else if (leafFlag[0] == 0 && size < 0)
            { // the data is a node, read meta data.
                var metaLengthBytes = new byte[4];
                stream.Read(metaLengthBytes, 0, metaLengthBytes.Length);
                int metaLength = BitConverter.ToInt32(metaLengthBytes, 0);

                var metaBytes = new byte[metaLength];
                stream.Read(metaBytes, 0, metaBytes.Length);

                var index =
                    typeModel.Deserialize(new MemoryStream(metaBytes), null, typeof(ChildrenIndex)) as ChildrenIndex;
                if (index != null)
                {
                    return new RTreeStreamIndex<T>.RTreeNode(index,
                                                             this, stream);
                }
            }
            throw new Exception("Cannot deserialize node!");
        }
    }
}