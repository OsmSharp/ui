// OsmSharp - OpenStreetMap tools & library.
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

namespace OsmSharp.UI.Unittests.UI
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
            View2D view = View2D.CreateFrom(0, 0, 200, 200, 1, true, true);

            // test result.
            Assert.AreEqual(view.LeftTop[0], -100);
            Assert.AreEqual(view.RightTop[0], 100);
            Assert.AreEqual(view.RightBottom[1], -100);
            Assert.AreEqual(view.RightTop[1], 100);
            Assert.IsTrue(view.Contains(0, 0));
            double[] topLeft = view.FromViewPort(1000, 1000, 0, 0);
            Assert.AreEqual(topLeft[0], -100);
            Assert.AreEqual(topLeft[1], 100);
            double[] bottomRight = view.FromViewPort(1000, 1000, 1000, 1000);
            Assert.AreEqual(bottomRight[0], 100);
            Assert.AreEqual(bottomRight[1], -100);
            Assert.AreEqual(bottomRight[0], view.FromViewPortX(1000, 1000));
            Assert.AreEqual(bottomRight[1], view.FromViewPortY(1000, 1000));
            Assert.AreEqual(topLeft[0], view.FromViewPortX(1000, 0));
            Assert.AreEqual(topLeft[1], view.FromViewPortY(1000, 0));
            double[] viewTopLeft = view.ToViewPort(1000, 1000, view.LeftTop[0], view.RightTop[1]);
            Assert.AreEqual(0, viewTopLeft[0]);
            Assert.AreEqual(0, viewTopLeft[1]);
            Assert.AreEqual(viewTopLeft[0], view.ToViewPortX(1000, view.LeftTop[0]));
            Assert.AreEqual(viewTopLeft[1], view.ToViewPortY(1000, view.RightTop[1]));
            double[] viewBottomRight = view.ToViewPort(1000, 1000, view.RightTop[0], view.RightBottom[1]);
            Assert.AreEqual(1000, viewBottomRight[0]);
            Assert.AreEqual(1000, viewBottomRight[1]);
            Assert.AreEqual(viewBottomRight[0], view.ToViewPortX(1000, view.RightTop[0]));
            Assert.AreEqual(viewBottomRight[1], view.ToViewPortY(1000, view.RightBottom[1]));

            view = View2D.CreateFrom(0, 0, 200, 200, 1, false, true);

            // test result.
            Assert.AreEqual(view.LeftTop[0], 100);
            Assert.AreEqual(view.RightTop[0], -100);
            Assert.AreEqual(view.RightBottom[1], -100);
            Assert.AreEqual(view.RightTop[1], 100);
            Assert.IsTrue(view.Contains(0, 0));
            topLeft = view.FromViewPort(1000, 1000, 0, 0);
            Assert.AreEqual(topLeft[0], 100);
            Assert.AreEqual(topLeft[1], 100);
            bottomRight = view.FromViewPort(1000, 1000, 1000, 1000);
            Assert.AreEqual(bottomRight[0], -100);
            Assert.AreEqual(bottomRight[1], -100);
            Assert.AreEqual(bottomRight[0], view.FromViewPortX(1000, 1000));
            Assert.AreEqual(bottomRight[1], view.FromViewPortY(1000, 1000));
            Assert.AreEqual(topLeft[0], view.FromViewPortX(1000, 0));
            Assert.AreEqual(topLeft[1], view.FromViewPortY(1000, 0));
            viewTopLeft = view.ToViewPort(1000, 1000, view.LeftTop[0], view.RightTop[1]);
            Assert.AreEqual(0, viewTopLeft[0]);
            Assert.AreEqual(0, viewTopLeft[1]);
            Assert.AreEqual(viewTopLeft[0], view.ToViewPortX(1000, view.LeftTop[0]));
            Assert.AreEqual(viewTopLeft[1], view.ToViewPortY(1000, view.RightTop[1]));
            viewBottomRight = view.ToViewPort(1000, 1000, view.RightTop[0], view.RightBottom[1]);
            Assert.AreEqual(1000, viewBottomRight[0]);
            Assert.AreEqual(1000, viewBottomRight[1]);
            Assert.AreEqual(viewBottomRight[0], view.ToViewPortX(1000, view.RightTop[0]));
            Assert.AreEqual(viewBottomRight[1], view.ToViewPortY(1000, view.RightBottom[1]));

            view = View2D.CreateFrom(0, 0, 200, 200, 1, true, false);

            // test result.
            Assert.AreEqual(view.LeftTop[0], -100);
            Assert.AreEqual(view.RightTop[0], 100);
            Assert.AreEqual(view.RightBottom[1], 100);
            Assert.AreEqual(view.RightTop[1], -100);
            Assert.IsTrue(view.Contains(0, 0));
            topLeft = view.FromViewPort(1000, 1000, 0, 0);
            Assert.AreEqual(topLeft[0], -100);
            Assert.AreEqual(topLeft[1], -100);
            bottomRight = view.FromViewPort(1000, 1000, 1000, 1000);
            Assert.AreEqual(bottomRight[0], 100);
            Assert.AreEqual(bottomRight[1], 100);
            Assert.AreEqual(bottomRight[0], view.FromViewPortX(1000, 1000));
            Assert.AreEqual(bottomRight[1], view.FromViewPortY(1000, 1000));
            Assert.AreEqual(topLeft[0], view.FromViewPortX(1000, 0));
            Assert.AreEqual(topLeft[1], view.FromViewPortY(1000, 0));
            viewTopLeft = view.ToViewPort(1000, 1000, view.LeftTop[0], view.RightTop[1]);
            Assert.AreEqual(0, viewTopLeft[0]);
            Assert.AreEqual(0, viewTopLeft[1]);
            Assert.AreEqual(viewTopLeft[0], view.ToViewPortX(1000, view.LeftTop[0]));
            Assert.AreEqual(viewTopLeft[1], view.ToViewPortY(1000, view.RightTop[1]));
            viewBottomRight = view.ToViewPort(1000, 1000, view.RightTop[0], view.RightBottom[1]);
            Assert.AreEqual(1000, viewBottomRight[0]);
            Assert.AreEqual(1000, viewBottomRight[1]);
            Assert.AreEqual(viewBottomRight[0], view.ToViewPortX(1000, view.RightTop[0]));
            Assert.AreEqual(viewBottomRight[1], view.ToViewPortY(1000, view.RightBottom[1]));

            view = View2D.CreateFrom(0, 0, 200, 200, 1, false, false);

            // test result.
            Assert.AreEqual(view.LeftTop[0], 100);
            Assert.AreEqual(view.RightTop[0], -100);
            Assert.AreEqual(view.RightBottom[1], 100);
            Assert.AreEqual(view.RightTop[1], -100);
            Assert.IsTrue(view.Contains(0, 0));
            topLeft = view.FromViewPort(1000, 1000, 0, 0);
            Assert.AreEqual(topLeft[0], 100);
            Assert.AreEqual(topLeft[1], -100);
            bottomRight = view.FromViewPort(1000, 1000, 1000, 1000);
            Assert.AreEqual(bottomRight[0], -100);
            Assert.AreEqual(bottomRight[1], 100);
            Assert.AreEqual(bottomRight[0], view.FromViewPortX(1000, 1000));
            Assert.AreEqual(bottomRight[1], view.FromViewPortY(1000, 1000));
            Assert.AreEqual(topLeft[0], view.FromViewPortX(1000, 0));
            Assert.AreEqual(topLeft[1], view.FromViewPortY(1000, 0));
            viewTopLeft = view.ToViewPort(1000, 1000, view.LeftTop[0], view.RightTop[1]);
            Assert.AreEqual(0, viewTopLeft[0]);
            Assert.AreEqual(0, viewTopLeft[1]);
            Assert.AreEqual(viewTopLeft[0], view.ToViewPortX(1000, view.LeftTop[0]));
            Assert.AreEqual(viewTopLeft[1], view.ToViewPortY(1000, view.RightTop[1]));
            viewBottomRight = view.ToViewPort(1000, 1000, view.RightTop[0], view.RightBottom[1]);
            Assert.AreEqual(1000, viewBottomRight[0]);
            Assert.AreEqual(1000, viewBottomRight[1]);
            Assert.AreEqual(viewBottomRight[0], view.ToViewPortX(1000, view.RightTop[0]));
            Assert.AreEqual(viewBottomRight[1], view.ToViewPortY(1000, view.RightBottom[1]));

            view = View2D.CreateFromBounds(100, -100, -100, 100);

            // test result.
            Assert.AreEqual(view.LeftTop[0], -100);
            Assert.AreEqual(view.RightTop[0], 100);
            Assert.AreEqual(view.RightBottom[1], -100);
            Assert.AreEqual(view.RightTop[1], 100);
            Assert.IsTrue(view.Contains(0, 0));
            topLeft = view.FromViewPort(1000, 1000, 0, 0);
            Assert.AreEqual(topLeft[0], -100);
            Assert.AreEqual(topLeft[1], 100);
            bottomRight = view.FromViewPort(1000, 1000, 1000, 1000);
            Assert.AreEqual(bottomRight[0], 100);
            Assert.AreEqual(bottomRight[1], -100);
            Assert.AreEqual(bottomRight[0], view.FromViewPortX(1000, 1000));
            Assert.AreEqual(bottomRight[1], view.FromViewPortY(1000, 1000));
            Assert.AreEqual(topLeft[0], view.FromViewPortX(1000, 0));
            Assert.AreEqual(topLeft[1], view.FromViewPortY(1000, 0));
            viewTopLeft = view.ToViewPort(1000, 1000, view.LeftTop[0], view.RightTop[1]);
            Assert.AreEqual(0, viewTopLeft[0]);
            Assert.AreEqual(0, viewTopLeft[1]);
            Assert.AreEqual(viewTopLeft[0], view.ToViewPortX(1000, view.LeftTop[0]));
            Assert.AreEqual(viewTopLeft[1], view.ToViewPortY(1000, view.RightTop[1]));
            viewBottomRight = view.ToViewPort(1000, 1000, view.RightTop[0], view.RightBottom[1]);
            Assert.AreEqual(1000, viewBottomRight[0]);
            Assert.AreEqual(1000, viewBottomRight[1]);
            Assert.AreEqual(viewBottomRight[0], view.ToViewPortX(1000, view.RightTop[0]));
            Assert.AreEqual(viewBottomRight[1], view.ToViewPortY(1000, view.RightBottom[1]));

            view = View2D.CreateFromBounds(-100, 100, 100, -100);

            // test result.
            Assert.AreEqual(view.LeftTop[0], 100);
            Assert.AreEqual(view.RightTop[0], -100);
            Assert.AreEqual(view.RightBottom[1], 100);
            Assert.AreEqual(view.RightTop[1], -100);
            Assert.IsTrue(view.Contains(0, 0));
            topLeft = view.FromViewPort(1000, 1000, 0, 0);
            Assert.AreEqual(topLeft[0], 100);
            Assert.AreEqual(topLeft[1], -100);
            bottomRight = view.FromViewPort(1000, 1000, 1000, 1000);
            Assert.AreEqual(bottomRight[0], -100);
            Assert.AreEqual(bottomRight[1], 100);
            Assert.AreEqual(bottomRight[0], view.FromViewPortX(1000, 1000));
            Assert.AreEqual(bottomRight[1], view.FromViewPortY(1000, 1000));
            Assert.AreEqual(topLeft[0], view.FromViewPortX(1000, 0));
            Assert.AreEqual(topLeft[1], view.FromViewPortY(1000, 0));
            viewTopLeft = view.ToViewPort(1000, 1000, view.LeftTop[0], view.RightTop[1]);
            Assert.AreEqual(0, viewTopLeft[0]);
            Assert.AreEqual(0, viewTopLeft[1]);
            Assert.AreEqual(viewTopLeft[0], view.ToViewPortX(1000, view.LeftTop[0]));
            Assert.AreEqual(viewTopLeft[1], view.ToViewPortY(1000, view.RightTop[1]));
            viewBottomRight = view.ToViewPort(1000, 1000, view.RightTop[0], view.RightBottom[1]);
            Assert.AreEqual(1000, viewBottomRight[0]);
            Assert.AreEqual(1000, viewBottomRight[1]);
            Assert.AreEqual(viewBottomRight[0], view.ToViewPortX(1000, view.RightTop[0]));
            Assert.AreEqual(viewBottomRight[1], view.ToViewPortY(1000, view.RightBottom[1]));
        }
    }
}