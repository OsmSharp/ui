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
using NUnit.Framework;
using OsmSharp.Math.Primitives;

namespace OsmSharp.UnitTests.Math.Primitives
{
	/// <summary>
	/// Contains test for the RectangleF2D primitive.
	/// </summary>
	[TestFixture]
	public class RectangleF2DTests
	{
		/// <summary>
		/// Tests a RectangleF2D without a direction.
		/// </summary>
		[Test]
		public void TestRectangleF2DNoDirection()
		{
			RectangleF2D rectangle = new RectangleF2D (0, 0, 1, 1);

			double[] converted = rectangle.TransformFrom (100, 100, false, false,
			                                             new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (0.25, converted [0]);
			Assert.AreEqual (0.75, converted [1]);
			double[] convertedBack = rectangle.TransformTo (100, 100, false, false,
			                                               converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (25, convertedBack [0]);
			Assert.AreEqual (75, convertedBack [1]);

			converted = rectangle.TransformFrom (100, 100, false, true,
			                                     new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (0.25, converted [0]);
			Assert.AreEqual (0.25, converted [1]);
			convertedBack = rectangle.TransformTo (100, 100, false, false,
			                                                converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (25, convertedBack [0]);
			Assert.AreEqual (25, convertedBack [1]);

			converted = rectangle.TransformFrom (100, 100, true, false,
			                                     new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (0.75, converted [0]);
			Assert.AreEqual (0.75, converted [1]);
			convertedBack = rectangle.TransformTo (100, 100, false, false,
			                                                converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (75, convertedBack [0]);
			Assert.AreEqual (75, convertedBack [1]);

			converted = rectangle.TransformFrom (100, 100, true, true,
			                                     new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (0.75, converted [0]);
			Assert.AreEqual (0.25, converted [1]);
			convertedBack = rectangle.TransformTo (100, 100, false, false,
			                                                converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (75, convertedBack [0]);
			Assert.AreEqual (25, convertedBack [1]);
		}

		/// <summary>
		/// Tests a RectangleF2D without a direction.
		/// </summary>
		[Test]
		public void TestRectangleF2DDirection()
		{
			double delta = 0.00001;
			RectangleF2D rectangle = new RectangleF2D (1, 1, System.Math.Sqrt (2) * 2,
			                                           System.Math.Sqrt (2) * 2, 45);

			double[] converted = rectangle.TransformFrom (100, 100, false, false,
			                                              new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (3, converted [0], delta);
			Assert.AreEqual (2, converted [1], delta);
			double[] convertedBack = rectangle.TransformTo (100, 100, false, false,
			                                                converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (25, convertedBack [0], delta);
			Assert.AreEqual (75, convertedBack [1], delta);

			converted = rectangle.TransformFrom (100, 100, true, false,
			                                              new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (4, converted [0], delta);
			Assert.AreEqual (1, converted [1], delta);
			convertedBack = rectangle.TransformTo (100, 100, true, false,
			                                                converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (25, convertedBack [0], delta);
			Assert.AreEqual (75, convertedBack [1], delta);

			converted = rectangle.TransformFrom (100, 100, false, true,
			                                     new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (2, converted [0], delta);
			Assert.AreEqual (1, converted [1], delta);
			convertedBack = rectangle.TransformTo (100, 100, false, true,
			                                       converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (25, convertedBack [0], delta);
			Assert.AreEqual (75, convertedBack [1], delta);

			converted = rectangle.TransformFrom (100, 100, true, true,
			                                     new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (3, converted [0], delta);
			Assert.AreEqual (0, converted [1], delta);
			convertedBack = rectangle.TransformTo (100, 100, true, true,
			                                       converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (25, convertedBack [0], delta);
			Assert.AreEqual (75, convertedBack [1], delta);
		}
	}
}

