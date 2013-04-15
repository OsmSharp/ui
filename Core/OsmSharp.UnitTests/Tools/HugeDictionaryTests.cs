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
using OsmSharp.Tools.Collections.Huge;
using NUnit.Framework;

namespace OsmSharp.UnitTests.Tools
{
    /// <summary>
    /// Testclass with facilities to test the huge dictionary.
    /// </summary>
    [TestFixture]
    public class HugeDictionaryTest
    {
        /// <summary>
        /// Tests a huge dictionary.
        /// </summary>
        [Test]
        public void TestHugeDictionary()
        {
            // create the huge dictionary.
            HugeDictionary<long, long> huge_dictionary = new HugeDictionary<long, long>();

            for (long idx = 0; idx < 10000; idx++)
            {
                huge_dictionary.Add(idx, idx);
            }

            Assert.AreEqual(10000, huge_dictionary.Count);
            Assert.AreEqual(1, huge_dictionary.CountDictionaries);

            for (long idx = 0; idx < 10000; idx++)
            {
                huge_dictionary.Remove(idx);
            }

            Assert.AreEqual(0, huge_dictionary.Count);
            Assert.AreEqual(1, huge_dictionary.CountDictionaries);

            huge_dictionary = new HugeDictionary<long, long>();

            for (long idx = 0; idx < 10000000; idx++)
            {
                huge_dictionary.Add(idx, idx);
            }

            Assert.AreEqual(10000000, huge_dictionary.Count);
            Assert.AreEqual(10, huge_dictionary.CountDictionaries);

            for (long idx = 0; idx < 10000000; idx++)
            {
                huge_dictionary.Remove(idx);
            }

            Assert.AreEqual(0, huge_dictionary.Count);
            Assert.AreEqual(1, huge_dictionary.CountDictionaries);
        }
    }
}