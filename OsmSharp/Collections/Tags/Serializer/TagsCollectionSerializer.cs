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

using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.Collections.Tags.Serializer
{
    /// <summary>
    /// Serializer/deserializer for tag collections.
    /// </summary>
    public class TagsCollectionSerializer
    {
        /// <summary>
        /// Serializes a tags collection to a byte array.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public byte[] Serialize(TagsCollectionBase collection)
        {
            if (collection.Count > 0)
            {
                RuntimeTypeModel typeModel = TypeModel.Create();
                typeModel.Add(typeof(List<KeyValuePair<string, string>>), true);

                List<KeyValuePair<string, string>> tagsList = new List<KeyValuePair<string, string>>();
                foreach (var tag in collection)
                {
                    tagsList.Add(new KeyValuePair<string, string>(tag.Key, tag.Value));
                }

                byte[] tagsBytes = null;
                using (MemoryStream stream = new MemoryStream())
                {
                    typeModel.Serialize(stream, tagsList);
                    tagsBytes = stream.ToArray();
                }
                return tagsBytes;
            }
            return new byte[0];
        }

        /// <summary>
        /// Deserializes a tags collection from a byte array.
        /// </summary>
        /// <param name="tagsBytes"></param>
        /// <returns></returns>
        public TagsCollectionBase Deserialize(byte[] tagsBytes)
        {
            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(List<KeyValuePair<string, string>>), true);

            List<KeyValuePair<string, string>> tagsList = null;
            using (MemoryStream stream = new MemoryStream(tagsBytes))
            {
                tagsList = typeModel.Deserialize(stream, null, typeof(List<KeyValuePair<string, string>>)) as List<KeyValuePair<string, string>>;
            }

            TagsCollection tagsCollection = new TagsCollection();
            foreach(KeyValuePair<string, string> tag in tagsList)
            {
                tagsCollection.Add(tag.Key, tag.Value);
            }
            return tagsCollection;
        }
    }
}