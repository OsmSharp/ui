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
using NUnit.Framework;

namespace OsmSharp.UI.Test.Unittests
{
    /// <summary>
    /// Contains tests for the simple color class.
    /// </summary>
    [TestFixture]
    public class SimpleColorTests
    {
        /// <summary>
        /// Tests the ARGB properties.
        /// </summary>
        [Test]
        public void TestSimpleColorArgbProperties()
        {
            SimpleColor simpleColor = SimpleColor.FromArgb(10, 20, 30, 40);

            Assert.AreEqual(10, simpleColor.A);
            Assert.AreEqual(20, simpleColor.R);
            Assert.AreEqual(30, simpleColor.G);
            Assert.AreEqual(40, simpleColor.B);
            
            simpleColor = SimpleColor.FromArgb(255, 255, 255, 255);

            Assert.AreEqual(255, simpleColor.A);
            Assert.AreEqual(255, simpleColor.R);
            Assert.AreEqual(255, simpleColor.G);
            Assert.AreEqual(255, simpleColor.B);

            simpleColor = SimpleColor.FromArgb(0, 0, 0, 0);

            Assert.AreEqual(0, simpleColor.A);
            Assert.AreEqual(0, simpleColor.R);
            Assert.AreEqual(0, simpleColor.G);
            Assert.AreEqual(0, simpleColor.B);
        }
        
        /// <summary>
        /// Tests the KnownColors.
        /// </summary>
        [Test]
        public void TestSimpleColorKnownColor()
        {
            SimpleColor simpleColor = SimpleColor.FromKnownColor(KnownColor.White);

            Assert.AreEqual(255, simpleColor.A);
            Assert.AreEqual(255, simpleColor.R);
            Assert.AreEqual(255, simpleColor.G);
            Assert.AreEqual(255, simpleColor.B);

            simpleColor = SimpleColor.FromKnownColor(KnownColor.Black);

            Assert.AreEqual(255, simpleColor.A);
            Assert.AreEqual(0, simpleColor.R);
            Assert.AreEqual(0, simpleColor.G);
            Assert.AreEqual(0, simpleColor.B);
        }

		/// <summary>
		/// Tests the simple color from hex.
		/// </summary>
		[Test]
		public void TestSimpleColorFromHex() {
			SimpleColor color = SimpleColor.FromKnownColor (KnownColor.Red);
			Assert.AreEqual (color.Value, SimpleColor.FromHex (color.HexRgb).Value);

			color = SimpleColor.FromKnownColor (KnownColor.Blue);
			Assert.AreEqual (color.Value, SimpleColor.FromHex (color.HexRgb).Value);

			color = SimpleColor.FromKnownColor (KnownColor.Yellow);
			Assert.AreEqual (color.Value, SimpleColor.FromHex (color.HexRgb).Value);

			color = SimpleColor.FromKnownColor (KnownColor.Green);
			Assert.AreEqual (color.Value, SimpleColor.FromHex (color.HexRgb).Value);

			color = SimpleColor.FromKnownColor (KnownColor.White);
			Assert.AreEqual (color.Value, SimpleColor.FromHex (color.HexRgb).Value);
		}
    }
}
