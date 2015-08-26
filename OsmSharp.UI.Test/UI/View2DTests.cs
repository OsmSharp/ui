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
using OsmSharp.Math.Geo;
using OsmSharp.UI.Renderer;

namespace OsmSharp.UI.Test.Unittests.UI
{
    /// <summary>
    /// Contains tests for the View2D class and all conversion functionalities.
    /// </summary>
    [TestFixture]
    public class View2DTests
    {
        /// <summary>
        /// Tests CreateFrom
        /// </summary>
        [Test]
        public void View2DTestCreateFrom()
        {
            double x, y;
            var view = View2D.CreateFrom(0, 0, 200, 200, 1, false, false);

            // test result.
            Assert.AreEqual(view.LeftTop[0], -100);
            Assert.AreEqual(view.RightTop[0], 100);
            Assert.AreEqual(view.RightBottom[1], -100);
            Assert.AreEqual(view.RightTop[1], 100);
            Assert.IsTrue(view.Contains(0, 0));
            var fromMatrix = view.CreateFromViewPort(1000, 1000);
            fromMatrix.Apply(0, 0, out x, out y);
            // var topLeft = view.FromViewPort(1000, 1000, 0, 0);
            Assert.AreEqual(x, -100);
            Assert.AreEqual(y, -100);
            fromMatrix.Apply(1000, 1000, out x, out y);
            // var bottomRight = view.FromViewPort(1000, 1000, 1000, 1000);
            Assert.AreEqual(x, 100);
            Assert.AreEqual(y, 100);

            var toMatrix = view.CreateToViewPort(1000, 1000);
            //var viewTopLeft = view.ToViewPort(1000, 1000, view.LeftTop[0], view.RightTop[1]);
            toMatrix.Apply(view.LeftTop[0], view.RightTop[1], out x, out y);
            Assert.AreEqual(0, x);
            Assert.AreEqual(1000, y);
            // var viewBottomRight = view.ToViewPort(1000, 1000, view.RightBottom[0], view.RightBottom[1]);
            toMatrix.Apply(view.RightBottom[0], view.RightBottom[1], out x, out y);
            Assert.AreEqual(1000, x);
            Assert.AreEqual(0, y);

            view = View2D.CreateFrom(0, 0, 200, 200, 1, true, false);

            // test result.
            Assert.AreEqual(view.LeftTop[0], 100);
            Assert.AreEqual(view.RightTop[0], -100);
            Assert.AreEqual(view.RightBottom[1], -100);
            Assert.AreEqual(view.RightTop[1], 100);
            Assert.IsTrue(view.Contains(0, 0));
            fromMatrix = view.CreateFromViewPort(1000, 1000);
            // topLeft = view.FromViewPort(1000, 1000, 0, 0);
            fromMatrix.Apply(0, 0, out x, out y);
            Assert.AreEqual(x, 100);
            Assert.AreEqual(y, -100);
            // bottomRight = view.FromViewPort(1000, 1000, 1000, 1000);
            fromMatrix.Apply(1000, 1000, out x, out y);
            Assert.AreEqual(x, -100);
            Assert.AreEqual(y, 100);

            toMatrix = view.CreateToViewPort(1000, 1000);
            // viewTopLeft = view.ToViewPort(1000, 1000, view.LeftTop[0], view.LeftTop[1]);
            toMatrix.Apply(view.LeftTop[0], view.LeftTop[1], out x, out y);
            Assert.AreEqual(0, x);
            Assert.AreEqual(1000, y);
            // viewBottomRight = view.ToViewPort(1000, 1000, view.RightBottom[0], view.RightBottom[1]);
            toMatrix.Apply(view.RightBottom[0], view.RightBottom[1], out x, out y);
            Assert.AreEqual(1000, x);
            Assert.AreEqual(0, y);

            view = View2D.CreateFrom(0, 0, 200, 200, 1, false, true);

            // test result.
            Assert.AreEqual(view.LeftTop[0], -100);
            Assert.AreEqual(view.RightTop[0], 100);
            Assert.AreEqual(view.RightBottom[1], 100);
            Assert.AreEqual(view.RightTop[1], -100);
            Assert.IsTrue(view.Contains(0, 0));
            fromMatrix = view.CreateFromViewPort(1000, 1000);
            // topLeft = view.FromViewPort(1000, 1000, 0, 0);
            fromMatrix.Apply(0, 0, out x, out y);
            Assert.AreEqual(x, -100);
            Assert.AreEqual(y, 100);
            // bottomRight = view.FromViewPort(1000, 1000, 1000, 1000);
            fromMatrix.Apply(1000, 1000, out x, out y);
            Assert.AreEqual(x, 100);
            Assert.AreEqual(y, -100);

            toMatrix = view.CreateToViewPort(1000, 1000);
            // viewTopLeft = view.ToViewPort(1000, 1000, view.LeftTop[0], view.LeftTop[1]);
            toMatrix.Apply(view.LeftTop[0], view.LeftTop[1], out x, out y);
            Assert.AreEqual(0, x);
            Assert.AreEqual(1000, y);
            // viewBottomRight = view.ToViewPort(1000, 1000, view.RightBottom[0], view.RightBottom[1]);
            toMatrix.Apply(view.RightBottom[0], view.RightBottom[1], out x, out y);
            Assert.AreEqual(1000, x);
            Assert.AreEqual(0, y);

            view = View2D.CreateFrom(0, 0, 200, 200, 1, true, true);

            // test result.
            Assert.AreEqual(view.LeftTop[0], 100);
            Assert.AreEqual(view.RightTop[0], -100);
            Assert.AreEqual(view.RightBottom[1], 100);
            Assert.AreEqual(view.RightTop[1], -100);
            Assert.IsTrue(view.Contains(0, 0));
            fromMatrix = view.CreateFromViewPort(1000, 1000);
            // topLeft = view.FromViewPort(1000, 1000, 0, 0);
            fromMatrix.Apply(0, 0, out x, out y);
            Assert.AreEqual(x, 100);
            Assert.AreEqual(y, 100);
            // bottomRight = view.FromViewPort(1000, 1000, 1000, 1000);
            fromMatrix.Apply(1000, 1000, out x, out y);
            Assert.AreEqual(x, -100);
            Assert.AreEqual(y, -100);

            toMatrix = view.CreateToViewPort(1000, 1000);
            // viewTopLeft = view.ToViewPort(1000, 1000, view.LeftTop[0], view.LeftTop[1]);
            toMatrix.Apply(view.LeftTop[0], view.LeftTop[1], out x, out y);
            Assert.AreEqual(0, x);
            Assert.AreEqual(1000, y);
            // viewBottomRight = view.ToViewPort(1000, 1000, view.RightBottom[0], view.RightBottom[1]);
            toMatrix.Apply(view.RightBottom[0], view.RightBottom[1], out x, out y);
            Assert.AreEqual(1000, x);
            Assert.AreEqual(0, y);

            view = View2D.CreateFromBounds(100, -100, -100, 100);

            // test result.
            Assert.AreEqual(view.LeftTop[0], -100);
            Assert.AreEqual(view.RightTop[0], 100);
            Assert.AreEqual(view.RightBottom[1], -100);
            Assert.AreEqual(view.RightTop[1], 100);
            Assert.IsTrue(view.Contains(0, 0));

            view = View2D.CreateFromBounds(-100, 100, 100, -100);

            // test result.
            Assert.AreEqual(view.LeftTop[0], 100);
            Assert.AreEqual(view.RightTop[0], -100);
            Assert.AreEqual(view.RightBottom[1], 100);
            Assert.AreEqual(view.RightTop[1], -100);
            Assert.IsTrue(view.Contains(0, 0));
        }

        /// <summary>
        /// Tests CreateFrom with a direction.
        /// </summary>
        [Test]
        public void View2DTestCreateFromWithDirection()
        {
            double delta = 0.000001;
            double x, y, xFrom, yFrom;

            // create a new view that is tilted.
            var view = View2D.CreateFrom(0, 0, System.Math.Sqrt(2), System.Math.Sqrt(2), 1, false, false, 45);
            var fromMatrix = view.CreateFromViewPort(1000, 1000);
            var toMatrix = view.CreateToViewPort(1000, 1000);

            Assert.AreEqual(-1, view.LeftBottom[0], delta);
            Assert.AreEqual(0, view.LeftBottom[1], delta);
            Assert.AreEqual(0, view.RightBottom[0], delta);
            Assert.AreEqual(-1, view.RightBottom[1], delta);
            Assert.AreEqual(0, view.LeftTop[0], delta);
            Assert.AreEqual(1, view.LeftTop[1], delta);
            Assert.AreEqual(1, view.RightTop[0], delta);
            Assert.AreEqual(0, view.RightTop[1], delta);

            Assert.IsTrue(view.Contains(0, 0));
            // double[] bottomLeft = view.FromViewPort(1000, 1000, 0, 0);
            fromMatrix.Apply(0, 0, out x, out y);
            Assert.AreEqual(x, view.LeftBottom[0], delta);
            Assert.AreEqual(y, view.LeftBottom[1], delta);

            // double[] topRight = view.FromViewPort(1000, 1000, 1000, 1000);
            fromMatrix.Apply(1000, 1000, out x, out y);
            Assert.AreEqual(x, view.RightTop[0], delta);
            Assert.AreEqual(y, view.RightTop[1], delta);

            // double[] topLeft = view.FromViewPort(1000, 1000, 0, 1000);
            fromMatrix.Apply(0, 1000, out x, out y);
            Assert.AreEqual(x, view.LeftTop[0], delta);
            Assert.AreEqual(y, view.LeftTop[1], delta);

            // double[] bottomRight = view.FromViewPort(1000, 1000, 1000, 0);
            fromMatrix.Apply(1000, 0, out x, out y);
            Assert.AreEqual(x, view.RightBottom[0], delta);
            Assert.AreEqual(y, view.RightBottom[1], delta);

            // double[] viewBottomLeft = view.ToViewPort(1000, 1000, view.LeftBottom[0], view.LeftBottom[1]);
            toMatrix.Apply(view.LeftBottom[0], view.LeftBottom[1], out x, out y);
            fromMatrix.Apply(x, y, out xFrom, out yFrom);
            Assert.AreEqual(view.LeftBottom[0], xFrom, delta);
            Assert.AreEqual(view.LeftBottom[1], yFrom, delta);

            // double[] viewBottomRight = view.ToViewPort(1000, 1000, view.RightBottom[0], view.RightBottom[1]);
            toMatrix.Apply(view.RightBottom[0], view.RightBottom[1], out x, out y);
            fromMatrix.Apply(x, y, out xFrom, out yFrom);
            Assert.AreEqual(view.RightBottom[0], xFrom, delta);
            Assert.AreEqual(view.RightBottom[1], yFrom, delta);

            // double[] viewTopLeft = view.ToViewPort(1000, 1000, view.LeftTop[0], view.LeftTop[1]);
            toMatrix.Apply(view.LeftTop[0], view.LeftTop[1], out x, out y);
            fromMatrix.Apply(x, y, out xFrom, out yFrom);
            Assert.AreEqual(view.LeftTop[0], xFrom, delta);
            Assert.AreEqual(view.LeftTop[1], yFrom, delta);

            // double[] viewTopRight = view.ToViewPort(1000, 1000, view.RightTop[0], view.RightTop[1]);
            toMatrix.Apply(view.RightTop[0], view.RightTop[1], out x, out y);
            fromMatrix.Apply(x, y, out xFrom, out yFrom);
            Assert.AreEqual(view.RightTop[0], xFrom, delta);
            Assert.AreEqual(view.RightTop[1], yFrom, delta);
        }

        /// <summary>
        /// Tests CreateFrom with a x-y size difference.
        /// </summary>
        [Test]
        public void View2DTestCreateFromXYDifference()
        {
            double x, y;
            var view = View2D.CreateFrom(0, 0, 400, 200, 1, false, false);

            // test result.
            Assert.AreEqual(view.LeftTop[0], -200);
            Assert.AreEqual(view.RightTop[0], 200);
            Assert.AreEqual(view.RightBottom[1], -100);
            Assert.AreEqual(view.RightTop[1], 100);
            Assert.IsTrue(view.Contains(0, 0));
            var fromMatrix = view.CreateFromViewPort(2000, 1000);
            // var topLeft = view.FromViewPort(2000, 1000, 0, 0);
            fromMatrix.Apply(0, 0, out x, out y);
            Assert.AreEqual(x, -200);
            Assert.AreEqual(y, -100);
            // var bottomRight = view.FromViewPort(2000, 1000, 2000, 1000);
            fromMatrix.Apply(2000, 1000, out x, out y);
            Assert.AreEqual(x, 200);
            Assert.AreEqual(y, 100);

            var toMatrix = view.CreateToViewPort(2000, 1000);
            // var viewTopLeft = view.ToViewPort(2000, 1000, view.LeftTop[0], view.RightTop[1]);
            toMatrix.Apply(view.LeftTop[0], view.RightTop[1], out x, out y);
            Assert.AreEqual(0, x);
            Assert.AreEqual(1000, y);
            // var viewBottomRight = view.ToViewPort(2000, 1000, view.RightBottom[0], view.RightBottom[1]);
            toMatrix.Apply(view.RightBottom[0], view.RightBottom[1], out x, out y);
            Assert.AreEqual(2000, x);
            Assert.AreEqual(0, y);
        }
    }
}