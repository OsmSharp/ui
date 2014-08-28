//// OsmSharp - OpenStreetMap (OSM) SDK
//// Copyright (C) 2013 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

//using NUnit.Framework;
//using OsmSharp.Routing.CH.PreProcessing;

//namespace OsmSharp.Test.Unittests.Routing.CH
//{
//    /// <summary>
//    /// Contains tests for the CHEdgeData class.
//    /// </summary>
//    [TestFixture]
//    public class CHEdgeDataTests
//    {
//        /// <summary>
//        /// Tests the set direction functionality.
//        /// </summary>
//        [Test]
//        public void TestCHEdgeDataSetDirection()
//        {
//            var edge = new CHEdgeData();
//            this.DoTestSetDirection(edge, false, false, false);
//            this.DoTestSetDirection(edge, true, false, false);
//            this.DoTestSetDirection(edge, false, true, false);
//            this.DoTestSetDirection(edge, false, false, true);
//            this.DoTestSetDirection(edge, true, true, false);
//            this.DoTestSetDirection(edge, true, false, true);
//            this.DoTestSetDirection(edge, false, true, true);
//            this.DoTestSetDirection(edge, true, true, true);

//            this.DoTestSetDirection(edge, false, false);
//            this.DoTestSetDirection(edge, true, false);
//            this.DoTestSetDirection(edge, false, true);
//            this.DoTestSetDirection(edge, true, true);
//        }

//        /// <summary>
//        /// Tests the set direction functionality for the given parameters.
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <param name="forward"></param>
//        /// <param name="backward"></param>
//        private void DoTestSetDirection(CHEdgeData edge, bool forward, bool backward)
//        {
//            edge.SetDirection(forward, backward);

//            if (forward) { Assert.IsTrue(edge.Forward); }
//            else { Assert.IsFalse(edge.Forward); }
//            if (backward) { Assert.IsTrue(edge.Backward); }
//            else { Assert.IsFalse(edge.Backward); }
//            Assert.IsFalse(edge.ToLower);
//            Assert.IsFalse(edge.ToHigher);
//        }

//        /// <summary>
//        /// Tests the set direction functionality for the given parameters.
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <param name="forward"></param>
//        /// <param name="backward"></param>
//        /// <param name="toHigher"></param>
//        private void DoTestSetDirection(CHEdgeData edge, bool forward, bool backward, bool toHigher)
//        {
//            edge.SetDirection(forward, backward, toHigher);

//            if (forward) { Assert.IsTrue(edge.Forward); }
//            else { Assert.IsFalse(edge.Forward); }
//            if (backward) { Assert.IsTrue(edge.Backward); }
//            else { Assert.IsFalse(edge.Backward); }
//            if (toHigher) { Assert.IsTrue(edge.ToHigher); }
//            else { Assert.IsFalse(edge.ToHigher); }
//        }

//        /// <summary>
//        /// Tests the ch edge comparison.
//        /// </summary>
//        [Test]
//        public void TestCHEdgeDataComparer()
//        {
//            for (float weight1 = 0; weight1 < 3; weight1++)
//            {
//                for (float weight2 = 0; weight2 < 3; weight2++)
//                {
//                    bool overlapsWeight = weight1 <= weight2;

//                    // tests all the weight differences.
//                    this.DoTestCompare(new CHEdgeData(weight1, true, true),
//                        new CHEdgeData(weight2, true, true), overlapsWeight);
//                    this.DoTestCompare(new CHEdgeData(weight1, true, true, false),
//                        new CHEdgeData(weight2, true, true, false), overlapsWeight);
//                    this.DoTestCompare(new CHEdgeData(weight1, true, true, true),
//                        new CHEdgeData(weight2, true, true, true), overlapsWeight);
//                    this.DoTestCompare(new CHEdgeData(weight1, true, true, false, 0, 1),
//                        new CHEdgeData(weight2, true, true, false, 0, 1), overlapsWeight);
//                    this.DoTestCompare(new CHEdgeData(weight1, true, true, true, 0, 1),
//                        new CHEdgeData(weight2, true, true, true, 0, 1), overlapsWeight);
//                }
//            }
//        }

//        /// <summary>
//        /// Tests if the two given edges are compared correctly by the CHEdgeDataComparer.
//        /// </summary>
//        /// <param name="overapping"></param>
//        /// <param name="overlappee"></param>
//        /// <param name="result"></param>
//        private void DoTestCompare(CHEdgeData overapping, CHEdgeData overlappee, bool result)
//        {
//            CHEdgeDataComparer comparer = new CHEdgeDataComparer();
//            if (result)
//            {
//                Assert.IsTrue(comparer.Overlaps(overapping, overlappee));
//            }
//            else
//            {
//                Assert.IsFalse(comparer.Overlaps(overapping, overlappee));
//            }
//        }
//    }
//}
