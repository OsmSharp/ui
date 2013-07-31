// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2013 Abelshausen Ben
//                    Scheinpflug Tommy
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
using NUnit.Framework;
using OsmSharp.Osm;
using OsmSharp.Collections.Tags;

namespace OsmSharp.UnitTests.Osm
{
    /// <summary>
    /// Contains test methods for parsing functions.
    /// </summary>
    [TestFixture]
    public class TagParsingTests
    {
        /// <summary>
        /// Tests weight parsing.
        /// </summary>
        [Test]
        public void TestWeightParsing()
        {
            double result;

            // Official Valid Values
            Assert.AreEqual(true, TagExtensions.TryParseWeight("5", out result));
            Assert.AreEqual(5, result);

            Assert.AreEqual(true, TagExtensions.TryParseWeight("5 t", out result));
            Assert.AreEqual(5, result);

            Assert.AreEqual(true, TagExtensions.TryParseWeight("5.5 t", out result));
            Assert.AreEqual(5.5, result);

            // Additional Valid Values
            Assert.AreEqual(true, TagExtensions.TryParseWeight("5.5 to", out result));
            Assert.AreEqual(5.5, result);

            Assert.AreEqual(true, TagExtensions.TryParseWeight("5.5  tonnes  ", out result));
            Assert.AreEqual(5.5, result);

            Assert.AreEqual(true, TagExtensions.TryParseWeight("   5.5    Tonnen   ", out result));
            Assert.AreEqual(5.5, result);

            // Invalid Values
            Assert.AreEqual(false, TagExtensions.TryParseWeight("3 persons", out result));
            Assert.AreEqual(false, TagExtensions.TryParseWeight("0,6", out result));
        }

        /// <summary>
        /// Tests length parsing.
        /// </summary>
        [Test]
        public void TestLengthParsing()
        {
            double result;

            // Official Valid Values
            Assert.AreEqual(true, TagExtensions.TryParseLength("3", out result));
            Assert.AreEqual(3, result);

            Assert.AreEqual(true, TagExtensions.TryParseLength("3 m", out result));
            Assert.AreEqual(3, result);

            Assert.AreEqual(true, TagExtensions.TryParseLength("3.8 m", out result));
            Assert.AreEqual(3.8, result);

            Assert.AreEqual(true, TagExtensions.TryParseLength("6'7\"", out result));
            Assert.AreEqual(6 * 0.3048 + 7 * 0.0254, result);

            // Additional Valid Values
            Assert.AreEqual(true, TagExtensions.TryParseLength("3.8 meters", out result));
            Assert.AreEqual(3.8, result);

            Assert.AreEqual(true, TagExtensions.TryParseLength("3.8  metres  ", out result));
            Assert.AreEqual(3.8, result);

            Assert.AreEqual(true, TagExtensions.TryParseLength("   3.8    Meter   ", out result));
            Assert.AreEqual(3.8, result);

            // Invalid Values
            Assert.AreEqual(false, TagExtensions.TryParseLength("2.3; 7'9\"", out result));
            Assert.AreEqual(false, TagExtensions.TryParseLength("2,3", out result));
            Assert.AreEqual(false, TagExtensions.TryParseLength("6'", out result));
            Assert.AreEqual(false, TagExtensions.TryParseLength("6 ft", out result));
        }

        /// <summary>
        /// Tests boolean parsing.
        /// </summary>
        [Test]
        public void TestBooleanParsing()
        {
            TagsCollection tags = new SimpleTagsCollection();
            tags.Add("area", "yes");
            Assert.IsTrue(tags.IsTrue("area"));

            tags = new SimpleTagsCollection();
            tags.Add("area", "1");
            Assert.IsTrue(tags.IsTrue("area"));

            tags = new SimpleTagsCollection();
            tags.Add("area", "true");
            Assert.IsTrue(tags.IsTrue("area"));

            tags = new SimpleTagsCollection();
            tags.Add("area", "false");
            Assert.IsFalse(tags.IsTrue("area"));

            tags = new SimpleTagsCollection();
            tags.Add("area", "0");
            Assert.IsFalse(tags.IsTrue("area"));

            tags = new SimpleTagsCollection();
            tags.Add("area", "no");
            Assert.IsFalse(tags.IsTrue("area"));
        }
    }
}
