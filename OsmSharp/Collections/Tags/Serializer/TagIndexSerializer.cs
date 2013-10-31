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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ProtoBuf.Meta;
using OsmSharp.IO;

namespace OsmSharp.Collections.Tags.Serializer
{
    /// <summary>
    /// Contains serialize/deserialize functionalities.
    /// </summary>
    public class TagIndexSerializer
    {
        /// <summary>
        /// Creates the type model.
        /// </summary>
        /// <returns></returns>
        private static RuntimeTypeModel CreateTypeModel()
        {
            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(List<string>), true);
            typeModel.Add(typeof(KeyValuePair<uint, uint>), true);
            typeModel.Add(typeof(List<KeyValuePair<uint, uint>>), true);
            typeModel.Add(typeof(List<KeyValuePair<uint, List<KeyValuePair<uint, uint>>>>), true);

            return typeModel;
        }

        /// <summary>
        /// Serializes the tags into different indexed blocks of given size.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="blockSize"></param>
        public static void SerializeBlocks(Stream stream, ITagsIndex tagsIndex, uint blockSize)
        {
            int begin = (int)stream.Position;

            // calculate the amount of blocks.
            uint blocks = (uint)System.Math.Ceiling((float)tagsIndex.Max / (float)blockSize);
            
            // store block count.
            stream.Write(BitConverter.GetBytes((int)blocks), 0, 4);

            // store block size.
            stream.Write(BitConverter.GetBytes((int)blockSize), 0, 4);

            // move the stream to make room for the index.
            stream.Seek((blocks + 2) * 4 + begin, SeekOrigin.Begin);
            int beginBlocks = (int)stream.Position;

            // keep looping over these blocks.
            int[] blockPositions = new int[blocks];
            for (uint blockIdx = 0; blockIdx < blocks; blockIdx++)
            {
                uint from = blockIdx * blockSize;
                uint to = from + blockSize;

                TagIndexSerializer.Serialize(stream, tagsIndex, from, to);
                blockPositions[blockIdx] = (int)stream.Position - beginBlocks;
            }

            // write the block positions.
            stream.Seek(begin + 8, SeekOrigin.Begin);
            for(int blockIdx = 0; blockIdx < blocks; blockIdx++)
            {
                stream.Write(BitConverter.GetBytes(blockPositions[blockIdx]), 0, 4);
            }
        }

        /// <summary>
        /// Deserializes a tags stream that was serialized in blocks.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ITagsIndexReadonly DeserializeBlocks(Stream stream)
        {
            int begin = (int)stream.Position;

            byte[] intBytes = new byte[4];
            stream.Read(intBytes, 0, 4);
            int blocks = BitConverter.ToInt32(intBytes, 0);

            stream.Read(intBytes, 0, 4);
            int blockSize = BitConverter.ToInt32(intBytes, 0);

            int[] blockPositions = new int[blocks];
            intBytes = new byte[4 * blocks];
            stream.Read(intBytes, 0, intBytes.Length);
            for (int blockIdx = 0; blockIdx < blocks; blockIdx++)
            {
                blockPositions[blockIdx] = BitConverter.ToInt32(intBytes, blockIdx * 4);
            }

            int beginBlocks = (int)stream.Position;

            return new TagsBlockedIndexReadonly(stream, beginBlocks, blockSize, blockPositions);
        }

        /// <summary>
        /// Serializes the tags between the given two indexes.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void Serialize(Stream stream, ITagsIndex tagsIndex, uint from, uint to)
        {
            int begin = (int)stream.Position;

            // limit to tagsIndex count.
            if (tagsIndex.Max < to)
            {
                to = tagsIndex.Max;
            }

            // build a string index.
            ObjectTable<string> stringTable = new ObjectTable<string>(false);

            // convert tag collections to simpler objects.
            List<KeyValuePair<uint, List<KeyValuePair<uint, uint>>>> tagsIndexList = new List<KeyValuePair<uint, List<KeyValuePair<uint, uint>>>>();
            for (uint tagId = from; tagId < to; tagId++)
            {
                TagsCollection tagsCollection = tagsIndex.Get(tagId);
                if (tagsCollection != null)
                { // convert the tags collection to a list and add to the tag index.
                    List<KeyValuePair<uint, uint>> tagsList = new List<KeyValuePair<uint, uint>>();
                    foreach (Tag tag in tagsCollection)
                    {
                        uint keyId = stringTable.Add(tag.Key);
                        uint valueId = stringTable.Add(tag.Value);

                        tagsList.Add(new KeyValuePair<uint, uint>(
                            keyId, valueId));
                    }
                    tagsIndexList.Add(new KeyValuePair<uint, List<KeyValuePair<uint, uint>>>(tagId, tagsList));
                }
            }

            // do the serialization.
            TagIndexSerializer.Serialize(begin, stream, tagsIndexList, stringTable);

            // clear everything.
            tagsIndexList.Clear();
        }

        /// <summary>
        /// Serializes the given tags in the given index. This serialization preserves the id's of each tag collection.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="toSerialize"></param>
        public static void Serialize(Stream stream, ITagsIndex tagsIndex, HashSet<uint> toSerialize)
        {
            int begin = (int)stream.Position;

            // build a string index.
            ObjectTable<string> stringTable = new ObjectTable<string>(false);

            // convert tag collections to simpler objects.
            List<KeyValuePair<uint, List<KeyValuePair<uint, uint>>>> tagsIndexList = new List<KeyValuePair<uint, List<KeyValuePair<uint, uint>>>>();
            foreach(uint tagId in toSerialize)
            {
                TagsCollection tagsCollection = tagsIndex.Get(tagId);
                if (tagsCollection != null)
                { // convert the tags collection to a list and add to the tag index.
                    List<KeyValuePair<uint, uint>> tagsList = new List<KeyValuePair<uint, uint>>();
                    foreach (Tag tag in tagsCollection)
                    {
                        uint keyId = stringTable.Add(tag.Key);
                        uint valueId = stringTable.Add(tag.Value);

                        tagsList.Add(new KeyValuePair<uint, uint>(
                            keyId, valueId));
                    }
                    tagsIndexList.Add(new KeyValuePair<uint, List<KeyValuePair<uint, uint>>>(tagId, tagsList));
                }
            }

            // do the serialization.
            TagIndexSerializer.Serialize(begin, stream, tagsIndexList, stringTable);

            // clear everything.
            tagsIndexList.Clear();
        }

        /// <summary>
        /// Serializes all the tags in the given index. This serialization preserves the id's of each tag collection.
        /// </summary>
        /// <param name="stream">The target stream.</param>
        /// <param name="tagsIndex">The tags index to serialize.</param>
        public static void Serialize(Stream stream, ITagsIndex tagsIndex)
        {
            int begin = (int)stream.Position;

            // build a string index.
            ObjectTable<string> stringTable = new ObjectTable<string>(false);

            // convert tag collections to simpler objects.
            List<KeyValuePair<uint, List<KeyValuePair<uint, uint>>>> tagsIndexList = new List<KeyValuePair<uint,List<KeyValuePair<uint,uint>>>>();
            for (uint tagId = 0; tagId < tagsIndex.Max; tagId++)
            {
                TagsCollection tagsCollection = tagsIndex.Get(tagId);
                if (tagsCollection != null)
                { // convert the tags collection to a list and add to the tag index.
                    List<KeyValuePair<uint, uint>> tagsList = new List<KeyValuePair<uint, uint>>();
                    foreach (Tag tag in tagsCollection)
                    {
                        uint keyId = stringTable.Add(tag.Key);
                        uint valueId = stringTable.Add(tag.Value);

                        tagsList.Add(new KeyValuePair<uint, uint>(
                            keyId, valueId));
                    }
                    tagsIndexList.Add(new KeyValuePair<uint, List<KeyValuePair<uint, uint>>>(tagId, tagsList));
                }
            }

            // do the serialization.
            TagIndexSerializer.Serialize(begin, stream, tagsIndexList, stringTable);
            
            // clear everything.
            tagsIndexList.Clear();
        }

        /// <summary>
        /// Does the actual serialization of the given data structures.
        /// </summary>
        private static void Serialize(int begin, Stream stream, List<KeyValuePair<uint, List<KeyValuePair<uint, uint>>>> tagIndex,
            ObjectTable<string> stringTable)
        {
            RuntimeTypeModel typeModel = TagIndexSerializer.CreateTypeModel();

            // move until after the index (index contains two int's, startoftagindex, endoffile).
            stream.Seek(begin + 8, SeekOrigin.Begin);

            // serialize string table.
            List<string> strings = new List<string>();
            for (uint id = 0; id < stringTable.Count; id++)
            {
                strings.Add(stringTable.Get(id));
            }
            stringTable.Clear();
            stringTable = null;
            typeModel.Serialize(stream, strings);
            long startOfTagsIndex = stream.Position - begin;

            // serialize tagindex.
            typeModel.Serialize(stream, tagIndex);
            long endOfFile = stream.Position - begin;

            // write index.
            stream.Seek(begin, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes((int)startOfTagsIndex), 0, 4); // write start position of tagindex.
            stream.Write(BitConverter.GetBytes((int)endOfFile), 0, 4); // write size of complete file.

            stream.Seek(begin + endOfFile, SeekOrigin.Begin);
        }

        /// <summary>
        /// Deserializes a tags index from the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ITagsIndexReadonly Deserialize(Stream stream)
        {
            byte[] intBytes = new byte[4];
            stream.Read(intBytes, 0, 4);
            int startOfTags = BitConverter.ToInt32(intBytes, 0);
            stream.Read(intBytes, 0, 4);
            int endOfFile = BitConverter.ToInt32(intBytes, 0);

            RuntimeTypeModel typeModel = TagIndexSerializer.CreateTypeModel();
            byte[] stringTableBytes = new byte[startOfTags - 8];
            stream.Read(stringTableBytes, 0, stringTableBytes.Length);
            MemoryStream memoryStream = new MemoryStream(stringTableBytes);
            List<string> strings = typeModel.Deserialize(memoryStream, null, typeof(List<string>)) as List<string>;
            memoryStream.Dispose();

            byte[] tagsIndexBytes = new byte[endOfFile - startOfTags];
            stream.Read(tagsIndexBytes, 0, tagsIndexBytes.Length);
            memoryStream = new MemoryStream(tagsIndexBytes);
            List<KeyValuePair<uint, List<KeyValuePair<uint, uint>>>> tagsIndexTableList = typeModel.Deserialize(memoryStream, null,
                typeof(List<KeyValuePair<uint, List<KeyValuePair<uint, uint>>>>)) as List<KeyValuePair<uint, List<KeyValuePair<uint, uint>>>>;
            memoryStream.Dispose();

            Dictionary<uint, SimpleTagsCollection> tagsIndexList = new Dictionary<uint, SimpleTagsCollection>();
            for(int idx = 0; idx < tagsIndexTableList.Count; idx++)
            {
                KeyValuePair<uint, List<KeyValuePair<uint, uint>>> serializedTagsCollection = 
                    tagsIndexTableList[idx];
                SimpleTagsCollection tagsCollection = new SimpleTagsCollection();
                if (serializedTagsCollection.Value != null)
                {
                    foreach (KeyValuePair<uint, uint> pair in serializedTagsCollection.Value)
                    {
                        tagsCollection.Add(
                            new Tag(strings[(int)pair.Key], strings[(int)pair.Value]));
                    }
                }
                tagsIndexList.Add(serializedTagsCollection.Key, tagsCollection);
            }

            return new TagsIndexReadonly(tagsIndexList);
        }

        /// <summary>
        /// Represents a tags index readonly.
        /// </summary>
        private class TagsIndexReadonly : ITagsIndexReadonly
        {
            /// <summary>
            /// Holds tags list.
            /// </summary>
            private Dictionary<uint, SimpleTagsCollection> _tags;

            /// <summary>
            /// Holds the max.
            /// </summary>
            private uint _max;

            /// <summary>
            /// Creates a new tags index readonly.
            /// </summary>
            /// <param name="tags"></param>
            public TagsIndexReadonly(Dictionary<uint, SimpleTagsCollection> tags)
            {
                if (tags == null) { throw new ArgumentNullException(); }

                _tags = tags;

                if (_tags.Count == 0)
                {
                    _max = 0;
                }
                else
                {
                    _max = _tags.Keys.Max() + 1;
                }
            }

            /// <summary>
            /// Returns the maximum amount of tags in this tags index.
            /// </summary>
            public uint Max
            {
                get { return _max; }
            }

            /// <summary>
            /// Returns the tags collection at the given tags id.
            /// </summary>
            /// <param name="tagsId"></param>
            /// <returns></returns>
            public TagsCollection Get(uint tagsId)
            {
                SimpleTagsCollection collection;
                if (_tags.TryGetValue(tagsId, out collection))
                {
                    return collection;
                }
                return null;
            }
        }

        /// <summary>
        /// Represents a tags index readonly with a blocked index.
        /// </summary>
        private class TagsBlockedIndexReadonly : ITagsIndexReadonly
        {
            /// <summary>
            /// Holds the beginning of the blocks.
            /// </summary>
            private readonly int _begin;

            /// <summary>
            /// Holds the source stream.
            /// </summary>
            private readonly Stream _stream;

            /// <summary>
            /// Holds the block size.
            /// </summary>
            private readonly int _blockSize;

            /// <summary>
            /// Holds the block positions.
            /// </summary>
            private readonly int[] _blockPositions;

            /// <summary>
            /// Creates a new reaonly index.
            /// </summary>
            /// <param name="begin"></param>
            /// <param name="blockSize"></param>
            /// <param name="blockPositions"></param>
            public TagsBlockedIndexReadonly(Stream stream, int begin, int blockSize, int[] blockPositions)
            {
                _stream = stream;
                _begin = begin;
                _blockSize = blockSize;
                _blockPositions = blockPositions;
            }

            /// <summary>
            /// Holds the current block idx.
            /// </summary>
            private int _currentBlockIdx = -1;

            /// <summary>
            /// Holds the current block min.
            /// </summary>
            private uint _currentBlockMin;

            /// <summary>
            /// Holds the current block.
            /// </summary>
            private ITagsIndexReadonly _currentBlock;

            /// <summary>
            /// Deserializes a block.
            /// </summary>
            /// <param name="blockIdx"></param>
            private void DeserializeBlock(int blockIdx)
            {
                // calculate current bounds.
                _currentBlockMin = (uint)(blockIdx * _blockSize);
               
                // move stream to correct position.
                int blockOffset = 0;
                if (blockIdx > 0)
                {
                    blockOffset = _blockPositions[blockIdx - 1];
                }

                // seek stream.
                _stream.Seek(blockOffset + _begin, SeekOrigin.Begin);

                // deserialize this block.
                _currentBlock = TagIndexSerializer.Deserialize(_stream);
            }

            /// <summary>
            /// Returns the maximum amount of tags in this index.
            /// </summary>
            public uint Max
            {
                get {
                    if (_currentBlockIdx != _blockPositions.Length - 1)
                    { // load the last block.
                        this.DeserializeBlock(_blockPositions.Length - 1);
                    }
                    return _currentBlock.Max; 
                }
            }

            /// <summary>
            /// Returns the tags collection.
            /// </summary>
            /// <param name="tagsId"></param>
            /// <returns></returns>
            public TagsCollection Get(uint tagsId)
            {
                // check bounds of current block.
                if (_currentBlock != null)
                {
                    if (tagsId >= _currentBlockMin &&
                        tagsId < (_currentBlockMin + _blockSize))
                    { // tag is in current block.
                        return _currentBlock.Get(tagsId);
                    }
                }

                // load another block.
                int blockIdx = (int)System.Math.Ceiling((float)tagsId / (float)_blockSize);

                // deserialize block.
                this.DeserializeBlock(blockIdx);

                return _currentBlock.Get(tagsId);
            }
        }
    }
}
