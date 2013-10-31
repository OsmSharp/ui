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
        /// Serializes the tags between the given two indexes.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void Serialize(Stream stream, ITagsIndex tagsIndex, uint from, uint to)
        {
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
            TagIndexSerializer.Serialize(stream, tagsIndexList, stringTable);

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
            TagIndexSerializer.Serialize(stream, tagsIndexList, stringTable);

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
            // build a string index.
            ObjectTable<string> stringTable = new ObjectTable<string>(false);

            // convert tag collections to simpler objects.
            List<KeyValuePair<uint, List<KeyValuePair<uint, uint>>>> tagsIndexList = new List<KeyValuePair<uint,List<KeyValuePair<uint,uint>>>>();
            for (uint tagId = 0; tagId < tagsIndex.Count; tagId++)
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
            TagIndexSerializer.Serialize(stream, tagsIndexList, stringTable);
            
            // clear everything.
            tagsIndexList.Clear();
        }

        /// <summary>
        /// Does the actual serialization of the given data structures.
        /// </summary>
        private static void Serialize(Stream stream, List<KeyValuePair<uint, List<KeyValuePair<uint, uint>>>> tagIndex,
            ObjectTable<string> stringTable)
        {
            RuntimeTypeModel typeModel = TagIndexSerializer.CreateTypeModel();

            // move until after the index (index contains two int's, startoftagindex, endoffile).
            stream.Seek(8, SeekOrigin.Begin);

            // serialize string table.
            List<string> strings = new List<string>();
            for (uint id = 0; id < stringTable.Count; id++)
            {
                strings.Add(stringTable.Get(id));
            }
            stringTable.Clear();
            stringTable = null;
            typeModel.Serialize(stream, strings);
            long startOfTagsIndex = stream.Position;

            // serialize tagindex.
            typeModel.Serialize(stream, tagIndex);
            long endOfFile = stream.Position;

            // write index.
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes((int)startOfTagsIndex), 0, 4); // write start position of tagindex.
            stream.Write(BitConverter.GetBytes((int)endOfFile), 0, 4); // write size of complete file.
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

            List<SimpleTagsCollection> tagsIndexList = new List<SimpleTagsCollection>();
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
                tagsIndexList.Add(tagsCollection);
            }

            return new TagsIndexReadonly(tagsIndexList);
        }

        /// <summary>
        /// Creates a tags index readonly.
        /// </summary>
        private class TagsIndexReadonly : ITagsIndexReadonly
        {
            /// <summary>
            /// Holds tags list.
            /// </summary>
            private List<SimpleTagsCollection> _tags;

            /// <summary>
            /// Creates a new tags index readonly.
            /// </summary>
            /// <param name="tags"></param>
            public TagsIndexReadonly(List<SimpleTagsCollection> tags)
            {
                _tags = tags;
            }

            /// <summary>
            /// Returns the maximum amount of tags in this tags index.
            /// </summary>
            public uint Count
            {
                get { return (uint)_tags.Count; }
            }

            /// <summary>
            /// Returns the tags collection at the given tags id.
            /// </summary>
            /// <param name="tagsId"></param>
            /// <returns></returns>
            public TagsCollection Get(uint tagsId)
            {
                return _tags[(int)tagsId];
            }
        }
    }
}
