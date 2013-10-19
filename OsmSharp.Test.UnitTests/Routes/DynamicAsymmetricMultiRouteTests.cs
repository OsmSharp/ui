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
using OsmSharp.Math.VRP.Core.Routes;
using OsmSharp.Math.VRP.Core.Routes.ASymmetric;

namespace OsmSharp.Test.Unittests.Routes
{
    /// <summary>
    /// Contains test function for the dynamic asymmetric multi route.
    /// </summary>
    [TestFixture]
    public class DynamicAsymmetricMultiRouteTests : MultiRouteTest
    {
        /// <summary>
        /// Creates a dynamic asymmetric multi route instance to test.
        /// </summary>
        /// <param name="isRound"></param>
        /// <returns></returns>
        protected override IMultiRoute BuildRoute(bool isRound)
        {
            return new DynamicAsymmetricMultiRoute(1, true);
        }

        /// <summary>
        /// Tests adding customers to a multi route.
        /// </summary>
        [Test]
        public void TestDynamicAsymmetricMultiRouteInsertAfter()
        {
            this.DoTestAdd();
        }

        /// <summary>
        /// Tests adding customers to a multi route.
        /// </summary>
        [Test]
        public void TestDynamicAsymmetricMultiRouteRemove()
        {
            this.DoTestRemove();
        }

        /// <summary>
        /// Tests adding/removing customers to a multi route for each route, customer and position.
        /// </summary>
        [Test]
        public void TestDynamicAsymmetricMultiRouteInsertAfterRemoveComplete()
        {
            this.DoTestAddRemoveComplete();
        }

        /// <summary>
        /// Tests adding/removing customers to a multi route for each route, customer and position.
        /// </summary>
        [Test]
        public void TestDynamicAsymmetricMultiRouteEnumerateBetween()
        {
            this.DoTestEnumerateBetween();
        }

        /// <summary>
        /// Tests 
        /// </summary>
        [Test]
        public void TestDynamicAsymmetricMultiRouteGetNeighbours()
        {
            this.DoTestGetNeighbours();
        }
    }
}