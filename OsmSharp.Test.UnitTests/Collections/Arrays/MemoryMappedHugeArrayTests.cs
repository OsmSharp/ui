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

using NUnit.Framework;
using OsmSharp.Collections.Arrays;
using OsmSharp.Math.Random;
using OsmSharp.WinForms.UI;
using System;
namespace OsmSharp.Test.Unittests.Collections.Arrays
{  
    /// <summary>
    /// Contains tests for the huge array.
    /// </summary>
    [TestFixture]
    public class MemoryMappedMemoryMappedHugeArrayTests
    {
        /// <summary>
        /// A simple test for the huge array.
        /// </summary>
        [Test]
        public void MemoryMappedHugeArraySimpleTest()
        {
            // make sure to initialize the native hooks to create a memory mapping.
            Native.Initialize();

            using (var memoryMappedFile = OsmSharp.IO.MemoryMappedFiles.NativeMemoryMappedFileFactory.Create("testmap", 32 * 1024 * 1024))
            {
                var intArrayRef = new int[1000];
                var intArray = new MemoryMappedHugeArray<int>(memoryMappedFile, 1000);

                var randomGenerator = new RandomGenerator(66707770); // make this deterministic 
                for (int idx = 0; idx < 1000; idx++)
                {
                    if (randomGenerator.Generate(2.0) > 1)
                    { // add data.
                        intArrayRef[idx] = idx;
                        intArray[idx] = idx;
                    }
                    else
                    {
                        intArrayRef[idx] = int.MaxValue;
                        intArray[idx] = int.MaxValue;
                    }
                }

                for (int idx = 0; idx < 1000; idx++)
                {
                    Assert.AreEqual(intArrayRef[idx], intArray[idx]);
                }
            }
        }

        /// <summary>
        /// A simple test resizing the huge array 
        /// </summary>
        [Test]
        public void MemoryMappedHugeArrayResizeTests()
        {
            // make sure to initialize the native hooks to create a memory mapping.
            Native.Initialize();

            var randomGenerator = new RandomGenerator(66707770); // make this deterministic 

            using (var memoryMappedFile = OsmSharp.IO.MemoryMappedFiles.NativeMemoryMappedFileFactory.Create("testmap", 32 * 1024 * 1024))
            {
                var intArrayRef = new int[1000];
                var intArray = new MemoryMappedHugeArray<int>(
                    memoryMappedFile, 1000, 300);

                for (int idx = 0; idx < 1000; idx++)
                {
                    if (randomGenerator.Generate(2.0) > 1)
                    { // add data.
                        intArrayRef[idx] = idx;
                        intArray[idx] = idx;
                    }
                    else
                    {
                        intArrayRef[idx] = int.MaxValue;
                        intArray[idx] = int.MaxValue;
                    }
                }

                Array.Resize<int>(ref intArrayRef, 335);
                intArray.Resize(335);

                Assert.AreEqual(intArrayRef.Length, intArray.Length);
                for (int idx = 0; idx < intArrayRef.Length; idx++)
                {
                    Assert.AreEqual(intArrayRef[idx], intArray[idx]);
                }
            }

            using (var memoryMappedFile = OsmSharp.IO.MemoryMappedFiles.NativeMemoryMappedFileFactory.Create("testmap", 32 * 1024 * 1024))
            {
                var intArrayRef = new int[1000];
                var intArray = new MemoryMappedHugeArray<int>(memoryMappedFile, 1000, 300);

                for (int idx = 0; idx < 1000; idx++)
                {
                    if (randomGenerator.Generate(2.0) > 1)
                    { // add data.
                        intArrayRef[idx] = idx;
                        intArray[idx] = idx;
                    }
                    else
                    {
                        intArrayRef[idx] = int.MaxValue;
                        intArray[idx] = int.MaxValue;
                    }
                }

                Array.Resize<int>(ref intArrayRef, 1235);
                intArray.Resize(1235);

                Assert.AreEqual(intArrayRef.Length, intArray.Length);
                for (int idx = 0; idx < intArrayRef.Length; idx++)
                {
                    Assert.AreEqual(intArrayRef[idx], intArray[idx]);
                }
            }
        }
    }
}