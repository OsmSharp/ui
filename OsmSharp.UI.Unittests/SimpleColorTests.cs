using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace OsmSharp.UI.Unittests
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
    }
}
