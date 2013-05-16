using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Math.Primitives;

namespace OsmSharp.UnitTests.Math.Primitives
{
    /// <summary>
    /// Holds common tests for the Rectangle2DF class.
    /// </summary>
    [TestFixture]
    public class RectangleF2DTests
    {
        /// <summary>
        /// Tests the union operation.
        /// </summary>
        [Test]
        public void RectangleF2DUnionTest()
        {
            var testDataList = new List<RectangleF2D>();
            for (int idx = 0; idx < 10000; idx++)
            {
                double x1 = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(1.0);
                double x2 = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(1.0);
                double y1 = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(1.0);
                double y2 = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(1.0);

                testDataList.Add(new RectangleF2D(x1, y1, x2, y2));
            }

            RectangleF2D box = testDataList[0];
            foreach (RectangleF2D rectangleF2D in testDataList)
            {
                box = box.Union(rectangleF2D);
            }
            
            foreach (RectangleF2D rectangleF2D in testDataList)
            {
                box.IsInside(rectangleF2D);
            }
        }

        /// <summary>
        /// Tests the overlaps function.
        /// </summary>
        [Test]
        public void RectangleF2DOverlapsTest()
        {
            var rect1 = new RectangleF2D(0, 0, 2, 2);
            var rect2 = new RectangleF2D(3, 2, 5, 4);

            Assert.IsFalse(rect1.Overlaps(rect2));
            Assert.AreEqual(rect1.Overlaps(rect2), rect2.Overlaps(rect1));

            rect1 = new RectangleF2D(0, 0, 2, 2);
            rect2 = new RectangleF2D(2, 0, 4, 2);
            
            Assert.IsTrue(rect1.Overlaps(rect2));
            Assert.AreEqual(rect1.Overlaps(rect2), rect2.Overlaps(rect1));

            rect1 = new RectangleF2D(0, 0, 2, 2);
            rect2 = new RectangleF2D(1, 1, 3, 3);

            Assert.IsTrue(rect1.Overlaps(rect2));
            Assert.AreEqual(rect1.Overlaps(rect2), rect2.Overlaps(rect1));

            rect1 = new RectangleF2D(1, 0, 2, 3);
            rect2 = new RectangleF2D(0, 1, 3, 2);

            Assert.IsTrue(rect1.Overlaps(rect2));
            Assert.AreEqual(rect1.Overlaps(rect2), rect2.Overlaps(rect1));

            rect1 = new RectangleF2D(0, 0, 3, 3);
            rect2 = new RectangleF2D(1, 1, 2, 2);

            Assert.IsTrue(rect1.Overlaps(rect2));
            Assert.AreEqual(rect1.Overlaps(rect2), rect2.Overlaps(rect1));
        }
    }
}
