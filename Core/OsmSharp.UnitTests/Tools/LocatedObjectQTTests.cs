using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Tools.Math.Structures;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math.Structures.QTree;

namespace OsmSharp.UnitTests.Tools
{
    /// <summary>
    /// Contains tests for a QuadTree index.
    /// </summary>
    [TestFixture]
    public class LocatedObjectQTTests : LocatedObjectIndexTest
    {
        /// <summary>
        /// Tests a quad tree implementation of the located QT index.
        /// </summary>
        [Test]
        public void TestLocatedObjectQTSimple()
        {
            this.DoTestSimple();
        }

        /// <summary>
        /// Tests a quad tree implementation of the located QT index.
        /// </summary>
        [Test]
        public void TestLocatedObjectQTIndex()
        {
            this.DoTestAddingRandom(1000);
        }

        /// <summary>
        /// Creates a located object index to test.
        /// </summary>
        /// <returns></returns>
        public override ILocatedObjectIndex<GeoCoordinate, LocatedObjectData> CreateIndex()
        {
            return new QuadTree<GeoCoordinate, LocatedObjectData>();
            //return new QuadTree<GeoCoordinate, LocatedObjectData>(5,
            //    new GeoCoordinateBox(new GeoCoordinate(50, 3), new GeoCoordinate(40, 2)));
        }
    }
}
