using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Routing.Graph.Path;

namespace OsmSharp.UnitTests.Routing
{
    /// <summary>
    /// Does tests on the path segment class.
    /// </summary>
    [TestFixture]
    public class PathSegmentTests
    {
        /// <summary>
        /// Tests path segment equality.
        /// </summary>
        [Test]
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

        /// <summary>
        /// Tests concatenation of a path segment.
        /// 
        /// Regression Test: Concatenation returns weight of the second route only not their sum.
        /// </summary>
        [Test]
        public void TestPathSegmentConcatenation()
        {
            PathSegment<long> segment12 = new PathSegment<long>(2, 10, new PathSegment<long>(1)); // 1 -> 2
            PathSegment<long> segment23 = new PathSegment<long>(3, 20, new PathSegment<long>(2)); // 2 -> 3

            PathSegment<long> segment_123 = segment23.ConcatenateAfter(segment12);
            Assert.AreEqual(3, segment_123.VertexId);
            Assert.AreEqual(2, segment_123.From.VertexId);
            Assert.AreEqual(1, segment_123.From.From.VertexId);
            Assert.AreEqual(30, segment_123.Weight);

            PathSegment<long> segment123 = new PathSegment<long>(3, 22, new PathSegment<long>(2, 10, new PathSegment<long>(1))); // 1 -> 2 -> 3
            PathSegment<long> segment345 = new PathSegment<long>(5, 23, new PathSegment<long>(4, 20, new PathSegment<long>(3))); // 3 -> 4 -> 5

            PathSegment<long> segment_12345 = segment345.ConcatenateAfter(segment123);
            Assert.AreEqual(5, segment_12345.VertexId);
            Assert.AreEqual(4, segment_12345.From.VertexId);
            Assert.AreEqual(3, segment_12345.From.From.VertexId);
            Assert.AreEqual(2, segment_12345.From.From.From.VertexId);
            Assert.AreEqual(1, segment_12345.From.From.From.From.VertexId);
            Assert.AreEqual(45, segment_12345.Weight);
        }
    }
}