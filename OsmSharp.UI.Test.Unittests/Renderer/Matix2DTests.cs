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
using OsmSharp.UI.Renderer;

namespace OsmSharp.UI.Test.Renderer
{
    /// <summary>
    /// Contains tests for the Matrix2D graphics transformations.
    /// </summary>
    [TestFixture]
    public class Matix2DTests
    {
        /// <summary>
        /// Tests a scaling operation.
        /// </summary>
        [Test]
        public void TestMatrix2Scale()
        {
            // create a scaling matrix from (1,1)->(100,100).
            var scale = Matrix2D.Scale(100);

            double x, y;
            scale.Apply(0.25, 0.75, out x, out y);
            Assert.AreEqual(25, x);
            Assert.AreEqual(75, y);
        }

        /// <summary>
        /// Tests a rotation operation.
        /// </summary>
        [Test]
        public void TestMatrixF2DRotate()
        {
            double delta = 0.00001;
            var sqrt2 = System.Math.Sqrt(2);

            // rotate by 45°.
            var rotate = Matrix2D.Rotate(System.Math.PI / 4);

            double x, y;
            rotate.Apply(sqrt2, sqrt2, out x, out y);

            Assert.AreEqual(2, x, delta);
            Assert.AreEqual(0, y, delta);

            // rotate by 90°
            rotate = Matrix2D.Rotate(System.Math.PI / 2);

            rotate.Apply(sqrt2, sqrt2, out x, out y);

            Assert.AreEqual(sqrt2, x, delta);
            Assert.AreEqual(-sqrt2, y, delta);
        }

        /// <summary>
        /// Tests a transformation matrix containing a translation operation.
        /// </summary>
        [Test]
        public void TestMatrix2DTranslate()
        {
            var translate = Matrix2D.Translate(1, 0);

            double x, y;
            translate.Apply(1, 1, out x, out y);

            Assert.AreEqual(2, x);
            Assert.AreEqual(1, y);
        }

        /// <summary>
        /// Tests a transformation matrix that has a combination of scale and rotate.
        /// </summary>
        [Test]
        public void TestMatrix2DScaleAndRotate()
        {
            double delta = 0.00001;
            var scale = Matrix2D.Scale(2);
            var rotate = Matrix2D.Rotate(System.Math.PI / 2);

            double x, y;
            scale.Apply(1, 1, out x, out y);
            rotate.Apply(x, y, out x, out y);

            Assert.AreEqual(2, x, delta);
            Assert.AreEqual(-2, y, delta);

            var transformation = scale * rotate;
            transformation.Apply(1, 1, out x, out y);

            Assert.AreEqual(2, x, delta);
            Assert.AreEqual(-2, y, delta);
        }

        /// <summary>
        /// Tests a transformation matrix that has a combination of scale a translate.
        /// </summary>
        [Test]
        public void TestMatrix2DScaleAndTranslate()
        {
            double delta = 0.00001;
            var scale = Matrix2D.Scale(2);
            var translate = Matrix2D.Translate(1, 0);

            double x, y;
            scale.Apply(1, 1, out x, out y);
            translate.Apply(x, y, out x, out y);

            Assert.AreEqual(3, x, delta);
            Assert.AreEqual(2, y, delta);

            var transformation = scale * translate;
            transformation.Apply(1, 1, out x, out y);

            Assert.AreEqual(3, x, delta);
            Assert.AreEqual(2, y, delta);
        }

        /// <summary>
        /// Tests rotation and translation.
        /// </summary>
        [Test]
        public void TestMatrix2DRotateAndTranslate()
        {
            double delta = 0.00001;
            var rotate = Matrix2D.Rotate(System.Math.PI / 2);
            var translate = Matrix2D.Translate(1, 1);

            double x, y;
            rotate.Apply(1, 1, out x, out y);
            translate.Apply(x, y, out x, out y);

            Assert.AreEqual(2, x, delta);
            Assert.AreEqual(0, y, delta);

            var transformation = rotate * translate;
            transformation.Apply(1, 1, out x, out y);

            Assert.AreEqual(2, x, delta);
            Assert.AreEqual(0, y, delta);
        }

        /// <summary>
        /// Tests a tranformation matrix that has a combination of scale, translate and rotate.
        /// </summary>
        [Test]
        public void TestMatrix2DScaleRotateAndTranslate()
        {
            double delta = 0.00001;
            var scale = Matrix2D.Scale(2);
            var translate = Matrix2D.Translate(1, 0);
            var rotate = Matrix2D.Rotate(System.Math.PI / 2);

            double x, y;
            scale.Apply(1, 1, out x, out y);
            translate.Apply(x, y, out x, out y);
            rotate.Apply(x, y, out x, out y);

            Assert.AreEqual(2, x, delta);
            Assert.AreEqual(-3, y, delta);

            var transformation = scale * translate * rotate;
            transformation.Apply(1, 1, out x, out y);

            Assert.AreEqual(2, x, delta);
            Assert.AreEqual(-3, y, delta);
        }

        /// <summary>
        /// Tests a transformation matrix that has a mirror.
        /// </summary>
        [Test]
        public void TestMatrix2DMirror()
        {
            var mirror = Matrix2D.Scale(1, -1);

            double x, y;
            mirror.Apply(1, 1, out x, out y);

            Assert.AreEqual(1, x);
            Assert.AreEqual(-1, y);

            mirror = Matrix2D.Scale(-1, 1);
            mirror.Apply(1, 1, out x, out y);

            Assert.AreEqual(-1, x);
            Assert.AreEqual(1, y);
        }
    }
}