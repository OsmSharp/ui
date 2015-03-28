// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using NUnit.Framework;
using OsmSharp.Collections.Indexes.MemoryMapped;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Math.Random;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Test.Unittests.Collections.Indexes.MemoryMapped
{
    /// <summary>
    /// Contains test for a memory-mapped index.
    /// </summary>
    [TestFixture]
    public class MemoryMappedIndexTests
    {
        /// <summary>
        /// Test structure.
        /// </summary>
        private struct TestStruct
        {
            /// <summary>
            /// Gets or sets field1.
            /// </summary>
            public string Field1 { get; set; }
        }

        /// <summary>
        /// Tests using a fixed-size structure.
        /// </summary>
        [Test]
        public void TestFixed()
        {
            var randomGenerator = new RandomGenerator(66707770); // make this deterministic 

            var buffer = new byte[255];
            var readFrom = new MemoryMappedFile.ReadFromDelegate<TestStruct>((stream, position) =>
            {
                stream.Seek(position, System.IO.SeekOrigin.Begin);
                var size = stream.ReadByte();
                int pos = 0;
                stream.Read(buffer, pos, size);
                while (size == 255)
                {
                    pos = pos + size;
                    size = stream.ReadByte();
                    if (buffer.Length < size + pos)
                    {
                        Array.Resize(ref buffer, size + pos);
                    }
                    stream.Read(buffer, pos, size);
                }
                pos = pos + size;
                return new TestStruct()
                {
                    Field1 = System.Text.Encoding.Unicode.GetString(buffer, 0, pos)
                };
            });
            var writeTo = new MemoryMappedFile.WriteToDelegate<TestStruct>((stream, position, structure) =>
            {
                stream.Seek(position, System.IO.SeekOrigin.Begin);
                var bytes = System.Text.Encoding.Unicode.GetBytes(structure.Field1);
                var length = bytes.Length;
                for(int idx = 0; idx < bytes.Length; idx = idx + 255)
                {
                    var size = bytes.Length - idx;
                    if (size > 255)
                    {
                        size = 255;
                    }

                    stream.WriteByte((byte)size);
                    stream.Write(bytes, idx, size);
                    length++;
                }
                return length;
            });

            var index = new MemoryMappedIndex<TestStruct>(new MemoryMappedStream(new MemoryStream()), 
                readFrom, writeTo);
            var indexRef = new Dictionary<long, string>();

            // add the data.
            var testCount = 10;
            while(testCount > 0)
            {
                var data = randomGenerator.GenerateString(
                    randomGenerator.Generate(256) + 32);
                indexRef.Add(index.Add(new TestStruct()
                {
                    Field1 = data
                }), data);
                testCount--;
            }

            // get the data and check.
            foreach(var entry in indexRef)
            {
                var data = index.Get(entry.Key);
                Assert.AreEqual(indexRef[entry.Key], data.Field1);
            }
        }
    }
}