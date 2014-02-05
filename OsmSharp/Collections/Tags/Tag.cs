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
using System.IO;
namespace OsmSharp.Collections.Tags
{
    /// <summary>
    /// Represents a tag (a key-value pair).
    /// </summary>
    public struct Tag
    {
        /// <summary>
        /// Holds all the tag-data.
        /// </summary>
        private byte[] _data;

        /// <summary>
        /// Holds the value index.
        /// </summary>
        private short _valueIdx;

        /// <summary>
        /// Creates a new tag.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Tag(string key, string value)
            :this()
        {
            var keyData = Tag.Encode(key);
            var valueData = Tag.Encode(value);

            _data = new byte[keyData.Length + valueData.Length];
            _valueIdx = (short)keyData.Length;
            keyData.CopyTo(_data, 0);
            valueData.CopyTo(_data, _valueIdx);
        }

        /// <summary>
        /// The key (or the actual tag name).
        /// </summary>
        public string Key
        {
            get
            {
                return Tag.Decode(_data, 0, _valueIdx);
            }
        }

        /// <summary>
        /// The value of the tag.
        /// </summary>
        public string Value
        {
            get
            {
                return Tag.Decode(_data, _valueIdx, _data.Length - _valueIdx);
            }
        }

        /// <summary>
        /// Creates a new tag.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Tag Create(string key, string value)
        {
            return new Tag(key, value);
        }

        /// <summary>
        /// Returns a description of this tag.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}={1}", this.Key, this.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Tag)
            {
                Tag other = (Tag)obj;
                if(other._data.Length == this._data.Length)
                { // array are same size.
                    for(int idx = 0; idx < other._data.Length; idx++)
                    {
                        if(other._data[idx] != this._data[idx])
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hash = 140011346;
            for (int idx = 0; idx < this._data.Length; idx++)
            {
                hash = hash ^ this._data[idx].GetHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Holds the protobuf typemodel.
        /// </summary>
        private static TypeModel _typeModel = TypeModel.Create();

        /// <summary>
        /// Holds the memory stream use to decode/encode tags.
        /// </summary>
        private static MemoryStream _stream = new MemoryStream();

        /// <summary>
        /// Encodes and compresses a string to a byte array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static byte[] Encode(string value)
        {
            _stream.SetLength(0);

            _typeModel.Serialize(_stream, value);
            return _stream.ToArray();
        }

        /// <summary>
        /// Decodes and decompresses a string from a byte array.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static string Decode(byte[] data, int index, int count)
        {
            _stream.SetLength(0);
            _stream.Write(data, index, count);
            _stream.Seek(0, SeekOrigin.Begin);

            return _typeModel.Deserialize(_stream, null, typeof(string)) as string;
        }
    }
}