// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OsmSharp.Tools.Math;

namespace OsmSharp.UnitTests
{
    /// <summary>
    /// Summary description for MathTest
    /// </summary>
    [TestFixture]
    public class MathTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// Tests if the line position test is correct.
        /// </summary>
        [Test]
        public void LinePosition2DTest()
        {
            PointF2D a = new PointF2D(0, 0);
            PointF2D b = new PointF2D(1, 1);

            LineF2D line = new LineF2D(a, b);

            // test where the position lie.
            Assert.AreEqual(line.PositionOfPoint(new PointF2D(0, 0.5f)), LinePointPosition.Left, "Point position should be right!");
            Assert.AreEqual(line.PositionOfPoint(new PointF2D(0.5f, 0.5f)), LinePointPosition.On, "Point position should be on!");
            Assert.AreEqual(line.PositionOfPoint(new PointF2D(0.5f, 0)), LinePointPosition.Right, "Point position should be left!");
        }

        /// <summary>
        /// Tests if the line-point distance is correct.
        /// </summary>
        [Test]
        public void LineDistance2DTest()
        {
            double delta = 0.000000000000001;

            // create the line to test.
            PointF2D a = new PointF2D(0, 0);
            PointF2D b = new PointF2D(1, 1);
            LineF2D line = new LineF2D(a, b);

            // calculate the results
            double sqrt_2 = (double)System.Math.Sqrt(2);
            double sqrt_2_div_2 = (double)System.Math.Sqrt(2) / 2.0f;

            // the point to test to.
            PointF2D c = new PointF2D(1, 0);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(0, 1);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(2, 2);
            Assert.AreEqual(line.Distance(c), 0.0f, delta, "Point distance should be 0.0f!");

            // the point to test to.
            c = new PointF2D(2, 3);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(3, 2);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // Segments tests.
            line = new LineF2D(a, b, true, true);

            // the point to test to.
            c = new PointF2D(1, 0);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(0, 1);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(2, 2);
            Assert.AreEqual(line.Distance(c), sqrt_2, delta, string.Format("Point distance should be {0}!", sqrt_2));

            // the point to test to.
            c = new PointF2D(2, 1);
            Assert.AreEqual(line.Distance(c), 1, delta, string.Format("Point distance should be {0}f!", 1));

            // the point to test to.
            c = new PointF2D(1, 2);
            Assert.AreEqual(line.Distance(c), 1, delta, string.Format("Point distance should be {0}f!", 1));
        }

        /// <summary>
        /// Tests if the line-point distance is correct.
        /// </summary>
        [Test]
        public void LineDistance2DSegmentTest()
        {
            double delta = 0.000000000000001;

            // create the line to test.
            PointF2D a = new PointF2D(0, 0);
            PointF2D b = new PointF2D(1, 1);
            LineF2D line = new LineF2D(a, b,true,true);

            // calculate the results
            double sqrt_2 = (double)System.Math.Sqrt(2);
            double sqrt_2_div_2 = (double)System.Math.Sqrt(2) / 2.0f;

            // the point to test to.
            PointF2D c = new PointF2D(1, 0);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(0, 1);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(2, 2);
            Assert.AreEqual(line.Distance(c), sqrt_2, delta);

            // the point to test to.
            c = new PointF2D(2, 3);
            Assert.AreEqual(line.Distance(c), 2.23606797749979, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(3, 2);
            Assert.AreEqual(line.Distance(c), 2.23606797749979, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(1, 0);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(0, 1);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(2, 2);
            Assert.AreEqual(line.Distance(c), sqrt_2, delta, string.Format("Point distance should be {0}!", sqrt_2));

            // the point to test to.
            c = new PointF2D(2, 1);
            Assert.AreEqual(line.Distance(c), 1, delta, string.Format("Point distance should be {0}f!", 1));

            // the point to test to.
            c = new PointF2D(1, 2);
            Assert.AreEqual(line.Distance(c), 1, delta, string.Format("Point distance should be {0}f!", 1));
            
            // the point to test to.
            c = new PointF2D(-1, -1);
            Assert.AreEqual(line.Distance(c), sqrt_2, delta, string.Format("Point distance should be {0}f!", 1));

            // the point to test to.
            c = new PointF2D(1, -1);
            Assert.AreEqual(line.Distance(c), sqrt_2, delta, string.Format("Point distance should be {0}f!", 1));
        }


        /// <summary>
        /// Tests the functionalities of the point object.
        /// </summary>
        [Test]
        public void Point2DTest()
        {
            // create the test cases.
            PointF2D a = new PointF2D(0, 0);
            PointF2D b = new PointF2D(1, 1);

            // calculate the results
            double sqrt_2 = (double)System.Math.Sqrt(2);
            double sqrt_2_div_2 = (double)System.Math.Sqrt(2) / 2.0f;

            // test distance.
            Assert.AreEqual(a.Distance(b), sqrt_2, string.Format("Distance should be {0}!", sqrt_2));

            // test substraction into vector.
            VectorF2D ab = b - a;
            Assert.AreEqual(ab[0], 1, "Vector should be 1 at index 0!");
            Assert.AreEqual(ab[1], 1, "Vector should be 1 at index 1!");
            VectorF2D ba = a - b;
            Assert.AreEqual(ba[0], -1, "Vector should be -1 at index 0!");
            Assert.AreEqual(ba[1], -1, "Vector should be -1 at index 1!");
        }

        /// <summary>
        /// Tests the functionalities of the vector object.
        /// </summary>
        [Test]
        public void Vector2DTest()
        {
            // create the test cases.
            VectorF2D a_b = new VectorF2D(1 , 1);
            VectorF2D b_a = new VectorF2D(-1,-1);
            VectorF2D a_c = new VectorF2D(-1, 1);
            VectorF2D a_d = new VectorF2D(0, 1);
            VectorF2D a_e = new VectorF2D(-1, 0);
            VectorF2D a_f = new VectorF2D(0, 1);
            VectorF2D a_g = new VectorF2D(0, -1);

            // calculate the results
            double sqrt_2 = (double)System.Math.Sqrt(2);

            // check the sizes.
            Assert.AreEqual(a_b.Size, sqrt_2, string.Format("Size should be {0}!", sqrt_2));
            Assert.AreEqual(b_a.Size, sqrt_2, string.Format("Size should be {0}!", sqrt_2));

            // check the equality.
            Assert.IsTrue(a_b.Inverse == b_a, "The inverse of ab should be ba!");

            // check the cross product.
            Assert.AreEqual(VectorF2D.Cross(a_b, b_a), 0, "Cross product of two parallel vectors should be 0!");
            Assert.AreEqual(VectorF2D.Cross(a_b, a_c), 2, string.Format("Cross product of two perpendicular vectors should be maximized; in this case {0}!", 2));
            Assert.AreEqual(VectorF2D.Cross(b_a, a_b), 0, "Cross product of two parallel vectors should be 0!");
            Assert.AreEqual(VectorF2D.Cross(a_c, a_b), -2, string.Format("Cross product of two perpendicular vectors should be maximized; in this case {0}!", -2));

            // check the dot product.
            Assert.AreEqual(VectorF2D.Dot(a_b, b_a), -2, string.Format("Cross product of two parallel vectors should be maximized (absolute value); in this case {0}!", -2));
            Assert.AreEqual(VectorF2D.Dot(a_b, a_c), 0, string.Format("Cross product of two perpendicular vectors should be {0}!", 0));
            Assert.AreEqual(VectorF2D.Dot(a_b, a_d), 1);
            Assert.AreEqual(VectorF2D.Dot(a_b, a_e), -1);
            Assert.AreEqual(VectorF2D.Dot(a_b, a_f), 1);
            Assert.AreEqual(VectorF2D.Dot(a_b, a_g), -1);
            Assert.AreEqual(VectorF2D.Dot(b_a, a_b), -2, string.Format("Cross product of two parallel vectors should be maximized; in this case {0}!", -2));
            Assert.AreEqual(VectorF2D.Dot(a_c, a_b), 0, string.Format("Cross product of two perpendicular vectors should be {0}!", 0));
        }
    }
}
