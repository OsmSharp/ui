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
using System.Linq;
using System.Text;
using ProtoBuf;

namespace OsmSharp.Collections.SpatialIndexes.Serialization.v1
{
    /// <summary>
    /// Serializer for an R-tree spatial index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RTreeStreamSerializer<T> : SpatialIndexSerializer<T>
    {
        /// <summary>
        /// Serializes the given index to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="index"></param>
        protected override void DoSerialize(SpatialIndexSerializerStream stream, 
            RTreeStreamIndex<T> index)
        {
            //// serialize root node.
            //var root = new ChildrenIndex();
            //bool[] isLeafs;
            //List<byte[]> serializedChildren =
            //    this.SerializeChildren(index.Root, isLeafs);

            //root.IsLeaf = new bool[index.Root.Count];
            //root.MaxX = new float[index.Root.Count];
            //root.MaxY = new float[index.Root.Count];
            //root.MinX = new float[index.Root.Count];
            //root.MinY = new float[index.Root.Count];
            //root.Starts = new int[index.Root.Count];
            //for (int idx = 0; idx < index.Root.Count; idx++)
            //{
                
            //}
        }

        /// <summary>
        /// Serializes all children of the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="isLeafs"></param>
        /// <returns></returns>
        private List<byte[]> SerializeChildren(RTreeStreamIndex<T>.RTreeNode node, bool[] isLeafs)
        {
            //for (int idx = 0; idx < node.Count; idx++)
            //{
                
            //}
            return null;
        }

        /// <summary>
        /// Deserializes the given stream into an index.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lazy"></param>
        /// <returns></returns>
        protected override RTreeStreamIndex<T> DoDeserialize(SpatialIndexSerializerStream stream, bool lazy)
        {
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
    }
}