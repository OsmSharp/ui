// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
//using System.IO.Compression;

namespace OsmSharp.Tools.Collections
{
    /// <summary>
    /// Proxy class to permit XML Serialization of generic dictionaries
    /// </summary>
    /// <typeparam name="K">The type of the key</typeparam>
    /// <typeparam name="V">The type of the value</typeparam>
    [XmlRoot("Dic")]
    public class DictionaryProxy<K, V>
    {
        #region Construction and Initialization

        /// <summary>
        /// Creates a new dictionary proxy.
        /// </summary>
        /// <param name="original"></param>
        public DictionaryProxy(IDictionary<K, V> original)
        {
            Original = original;
        }

        /// <summary>
        /// Default constructor so deserialization works
        /// </summary>
        public DictionaryProxy()
        {
        }

        /// <summary>
        /// Use to set the dictionary if necessary, but don't serialize
        /// </summary>
        [XmlIgnore]
        public IDictionary<K, V> Original { get; set; }
        #endregion

        #region The Proxy List
        /// <summary>
        /// Holds the keys and values
        /// </summary>
        [XmlRoot("Pair")]
        public class KeyAndValue
        {
            /// <summary>
            /// The key.
            /// </summary>
            [XmlElement("k")]
            public K Key { get; set; }
            /// <summary>
            /// The value.
            /// </summary>
            [XmlElement("v")]
            public V Value { get; set; }
        }

        // This field will store the deserialized list
        private Collection<KeyAndValue> _list;

        /// <remarks>
        /// XmlElementAttribute is used to prevent extra nesting level. It's
        /// not necessary.
        /// </remarks>
        [XmlElement("Pairs")]
        public Collection<KeyAndValue> KeysAndValues
        {
            get
            {
                if (_list == null)
                {
                    _list = new Collection<KeyAndValue>();
                }

                // On deserialization, Original will be null, just return what we have
                if (Original == null)
                {
                    return _list;
                }

                // If Original was present, add each of its elements to the list
                _list.Clear();
                foreach (var pair in Original)
                {
                    _list.Add(new KeyAndValue { Key = pair.Key, Value = pair.Value });
                }

                return _list;
            }
        }
        #endregion

        /// <summary>
        /// Convenience method to return a dictionary from this proxy instance
        /// </summary>
        /// <returns></returns>
        public Dictionary<K, V> ToDictionary()
        {
            return KeysAndValues.ToDictionary(key => key.Key, value => value.Value);
        }

        #region Serialize/Deserialize

        /// <summary>
        /// Converts this to bytearray.
        /// </summary>
        /// <returns></returns>
        public byte[] ConvertToByteArray()
        {
            //Create our own namespaces for the output
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

            //Add an empty namespace and empty value
            ns.Add("", "");

            XmlSerializer serializer = new XmlSerializer(typeof(DictionaryProxy<K,V>), string.Empty);
            using (MemoryStream mem_stream = new MemoryStream())
            {
                //using (DeflateStream compres = new DeflateStream(mem_stream, CompressionMode.Compress))
                //{
                //    serializer.Serialize(compres, this);
                //    compres.Flush();
                //}
                serializer.Serialize(mem_stream, this, ns);
                mem_stream.Flush();
                return mem_stream.ToArray();
            }
        }

        /// <summary>
        /// Reads a dictionary from a byte array.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static DictionaryProxy<K, V> FromByteArray(byte[] bytes)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DictionaryProxy<K, V>), string.Empty);
            using (MemoryStream mem_stream = new MemoryStream(bytes))
            {
                //using (DeflateStream decompres = new DeflateStream(mem_stream, CompressionMode.Decompress))
                //{
                //    return (serializer.Deserialize(decompres) as DictionaryProxy<K, V>);
                //}
                return (serializer.Deserialize(mem_stream) as DictionaryProxy<K, V>);
            }
        }

        #endregion
    }
}