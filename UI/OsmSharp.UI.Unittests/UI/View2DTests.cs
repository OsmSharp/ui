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
            Assert.AreEqual(view.Left, -100);
            Assert.AreEqual(view.Right, 100);
            Assert.AreEqual(view.Bottom, -100);
            Assert.AreEqual(view.Top, 100);
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
            double[] viewTopLeft = view.ToViewPort(1000, 1000, view.Left, view.Top);
            Assert.AreEqual(0, viewTopLeft[0]);
            Assert.AreEqual(0, viewTopLeft[1]);
            Assert.AreEqual(viewTopLeft[0], view.ToViewPortX(1000, view.Left));
            Assert.AreEqual(viewTopLeft[1], view.ToViewPortY(1000, view.Top));
            double[] viewBottomRight = view.ToViewPort(1000, 1000, view.Right, view.Bottom);
            Assert.AreEqual(1000, viewBottomRight[0]);
            Assert.AreEqual(1000, viewBottomRight[1]);
            Assert.AreEqual(viewBottomRight[0], view.ToViewPortX(1000, view.Right));
            Assert.AreEqual(viewBottomRight[1], view.ToViewPortY(1000, view.Bottom));

            view = View2D.CreateFrom(0, 0, 200, 200, 1, false, true);

            // test result.
            Assert.AreEqual(view.Left, 100);
            Assert.AreEqual(view.Right, -100);
            Assert.AreEqual(view.Bottom, -100);
            Assert.AreEqual(view.Top, 100);
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
            viewTopLeft = view.ToViewPort(1000, 1000, view.Left, view.Top);
            Assert.AreEqual(0, viewTopLeft[0]);
            Assert.AreEqual(0, viewTopLeft[1]);
            Assert.AreEqual(viewTopLeft[0], view.ToViewPortX(1000, view.Left));
            Assert.AreEqual(viewTopLeft[1], view.ToViewPortY(1000, view.Top));
            viewBottomRight = view.ToViewPort(1000, 1000, view.Right, view.Bottom);
            Assert.AreEqual(1000, viewBottomRight[0]);
            Assert.AreEqual(1000, viewBottomRight[1]);
            Assert.AreEqual(viewBottomRight[0], view.ToViewPortX(1000, view.Right));
            Assert.AreEqual(viewBottomRight[1], view.ToViewPortY(1000, view.Bottom));

            view = View2D.CreateFrom(0, 0, 200, 200, 1, true, false);

            // test result.
            Assert.AreEqual(view.Left, -100);
            Assert.AreEqual(view.Right, 100);
            Assert.AreEqual(view.Bottom, 100);
            Assert.AreEqual(view.Top, -100);
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
            viewTopLeft = view.ToViewPort(1000, 1000, view.Left, view.Top);
            Assert.AreEqual(0, viewTopLeft[0]);
            Assert.AreEqual(0, viewTopLeft[1]);
            Assert.AreEqual(viewTopLeft[0], view.ToViewPortX(1000, view.Left));
            Assert.AreEqual(viewTopLeft[1], view.ToViewPortY(1000, view.Top));
            viewBottomRight = view.ToViewPort(1000, 1000, view.Right, view.Bottom);
            Assert.AreEqual(1000, viewBottomRight[0]);
            Assert.AreEqual(1000, viewBottomRight[1]);
            Assert.AreEqual(viewBottomRight[0], view.ToViewPortX(1000, view.Right));
            Assert.AreEqual(viewBottomRight[1], view.ToViewPortY(1000, view.Bottom));

            view = View2D.CreateFrom(0, 0, 200, 200, 1, false, false);

            // test result.
            Assert.AreEqual(view.Left, 100);
            Assert.AreEqual(view.Right, -100);
            Assert.AreEqual(view.Bottom, 100);
            Assert.AreEqual(view.Top, -100);
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
            viewTopLeft = view.ToViewPort(1000, 1000, view.Left, view.Top);
            Assert.AreEqual(0, viewTopLeft[0]);
            Assert.AreEqual(0, viewTopLeft[1]);
            Assert.AreEqual(viewTopLeft[0], view.ToViewPortX(1000, view.Left));
            Assert.AreEqual(viewTopLeft[1], view.ToViewPortY(1000, view.Top));
            viewBottomRight = view.ToViewPort(1000, 1000, view.Right, view.Bottom);
            Assert.AreEqual(1000, viewBottomRight[0]);
            Assert.AreEqual(1000, viewBottomRight[1]);
            Assert.AreEqual(viewBottomRight[0], view.ToViewPortX(1000, view.Right));
            Assert.AreEqual(viewBottomRight[1], view.ToViewPortY(1000, view.Bottom));

            view = View2D.CreateFromBounds(100, -100, -100, 100);

            // test result.
            Assert.AreEqual(view.Left, -100);
            Assert.AreEqual(view.Right, 100);
            Assert.AreEqual(view.Bottom, -100);
            Assert.AreEqual(view.Top, 100);
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
            viewTopLeft = view.ToViewPort(1000, 1000, view.Left, view.Top);
            Assert.AreEqual(0, viewTopLeft[0]);
            Assert.AreEqual(0, viewTopLeft[1]);
            Assert.AreEqual(viewTopLeft[0], view.ToViewPortX(1000, view.Left));
            Assert.AreEqual(viewTopLeft[1], view.ToViewPortY(1000, view.Top));
            viewBottomRight = view.ToViewPort(1000, 1000, view.Right, view.Bottom);
            Assert.AreEqual(1000, viewBottomRight[0]);
            Assert.AreEqual(1000, viewBottomRight[1]);
            Assert.AreEqual(viewBottomRight[0], view.ToViewPortX(1000, view.Right));
            Assert.AreEqual(viewBottomRight[1], view.ToViewPortY(1000, view.Bottom));

            view = View2D.CreateFromBounds(-100, 100, 100, -100);

            // test result.
            Assert.AreEqual(view.Left, 100);
            Assert.AreEqual(view.Right, -100);
            Assert.AreEqual(view.Bottom, 100);
            Assert.AreEqual(view.Top, -100);
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
            viewTopLeft = view.ToViewPort(1000, 1000, view.Left, view.Top);
            Assert.AreEqual(0, viewTopLeft[0]);
            Assert.AreEqual(0, viewTopLeft[1]);
            Assert.AreEqual(viewTopLeft[0], view.ToViewPortX(1000, view.Left));
            Assert.AreEqual(viewTopLeft[1], view.ToViewPortY(1000, view.Top));
            viewBottomRight = view.ToViewPort(1000, 1000, view.Right, view.Bottom);
            Assert.AreEqual(1000, viewBottomRight[0]);
            Assert.AreEqual(1000, viewBottomRight[1]);
            Assert.AreEqual(viewBottomRight[0], view.ToViewPortX(1000, view.Right));
            Assert.AreEqual(viewBottomRight[1], view.ToViewPortY(1000, view.Bottom));
        }
    }
}