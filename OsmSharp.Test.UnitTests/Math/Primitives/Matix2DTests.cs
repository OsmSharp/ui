using NUnit.Framework;
using OsmSharp.Math.Primitives;
using OsmSharp.Units.Angle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Test.Unittests.Math.Primitives
{
    [TestFixture]
    public class Matix2DTests
    {
        /// <summary>
        /// Tests a RectangleF2D without a direction.
        /// </summary>
        [Test]
        public void TestMatrix2Scale()
        {
            // create a scaling matrix from (1,1)->(100,100).
            var scale = Matrix2D.Scale(100);
            var invertedScale = scale.Inverse();

            double x, y;
            invertedScale.Multiply2D(25, 75, out x, out y);
            Assert.AreEqual(0.25, x);
            Assert.AreEqual(0.75, y);

            scale.Multiply2D(x, y, out x, out y);
            Assert.AreEqual(25, x);
            Assert.AreEqual(75, y);
        }
        /// <summary>
        /// Tests a RectangleF2D without a direction.
        /// </summary>
        [Test]
        public void TestMatrixF2DRotate()
        {
            double delta = 0.00001;
            var sqrt2 = System.Math.Sqrt(2);

            // do not scale, just rotate but multiply with identity matrix.
            //var scale = Matrix2D.Scale(1);
            var rotate = Matrix2D.Rotate((Degree)45);
            //var rotateAndScale = rotate * scale;

            double x, y;
            rotate.Multiply2D(sqrt2, sqrt2, out x, out y);

            Assert.AreEqual(2, x, delta);
            Assert.AreEqual(0, y, delta);

            // scale by a factor of two and rotate.
            rotate = Matrix2D.Rotate((Degree)90);

            rotate.Multiply2D(sqrt2, sqrt2, out x, out y);

            Assert.AreEqual(sqrt2, x, delta);
            Assert.AreEqual(-sqrt2, y, delta);
        }
    }
}