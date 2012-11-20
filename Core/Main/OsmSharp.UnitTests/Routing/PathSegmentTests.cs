using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Routing.Core.Graph.Path;

namespace OsmSharp.UnitTests.Routing
{
    /// <summary>
    /// Does tests on the path segment class.
    /// </summary>
    [TestClass]
    public class PathSegmentTests
    {
        /// <summary>
        /// Tests path segment equality.
        /// </summary>
        [TestMethod]
        public void TestPathSegmentEqualityOperator()
        {
            PathSegment<long> segment1 = new PathSegment<long>(1);
            PathSegment<long> segment1_clone = new PathSegment<long>(1);

            Assert.IsTrue(segment1 == segment1_clone);
            Assert.IsFalse(segment1 != segment1_clone);

            PathSegment<long> segment2 = new PathSegment<long>(2, 10, segment1);
            PathSegment<long> segment2_clone = new PathSegment<long>(2, 10, segment1_clone);

            Assert.IsTrue(segment2 == segment2_clone);
            Assert.IsFalse(segment2 != segment2_clone);

            PathSegment<long> segment2_different_weight = new PathSegment<long>(2, 11, segment1_clone);

            Assert.IsFalse(segment2 == segment2_different_weight);
            Assert.IsTrue(segment2 != segment2_different_weight);

            PathSegment<long> segment2_different = new PathSegment<long>(2);

            Assert.IsFalse(segment2 == segment2_different);
            Assert.IsTrue(segment2 != segment2_different);
        }
    }
}
