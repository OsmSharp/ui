// OsmSharp - OpenStreetMap (OSM) SDK
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

using NUnit.Framework;
using OsmSharp.Math.VRP.Core.Routes;
using OsmSharp.Math.VRP.Core.Routes.ASymmetric;

namespace OsmSharp.UnitTests.Routes
{
    /// <summary>
    /// Contains testing code for a dynamic asymmetric route.
    /// </summary>
    [TestFixture]
    public class DynamicAsymmetricRouteTests : RouteTest
    {
        /// <summary>
        /// Builds a dynamic route initialized with an initial customer.
        /// </summary>
        /// <returns></returns>
        protected override IRoute BuildRoute(int customer, bool isRound)
        {
            return new DynamicAsymmetricRoute(1, customer, isRound);
        }

        /// <summary>
        /// Builds a dynamic route that is empty.
        /// </summary>
        /// <param name="isRound"></param>
        /// <returns></returns>
        protected override IRoute BuildRoute(bool isRound)
        {
            return new DynamicAsymmetricRoute(1, isRound);
        }

        /// <summary>
        /// Tests a dynamic asymmetric route by adding customers.
        /// </summary>
        [Test]
        public void TestDynamicAsymmetricRouteInsertAfter()
        {
            this.DoTestAdd();
        }

        /// <summary>
        /// Tests a dynamic asymmetric route by removing customers.
        /// </summary>
        [Test]
        public void TestDynamicAsymmetricRouteRemove()
        {
            this.DoTestRemove();
        }

        /// <summary>
        /// Tests a dynamic asymmetric route by removing customers.
        /// </summary>
        [Test]
        public void TestDynamicAsymmetricRouteContains()
        {
            this.DoTestContains();
        }

        /// <summary>
        /// Test a dynamic asymetric route by removing and adding every customer at every position.
        /// </summary>
        [Test]
        public void TestDynamicAsymmetricRouteInserAfterRemoveComplete()
        {
            this.DoTestAddRemoveComplete();
        }

        /// <summary>
        /// Tests a dynamic asymetric route's enumeration functionality.
        /// </summary>
        [Test]
        public void TestDynamicAsymmetricRouteEnumerateBetween()
        {
            this.DoTestEnumerateBetween();
        }

        /// <summary>
        /// Tests a dynamic asymetric route's enumeration functionality.
        /// </summary>
        [Test]
        public void TestDynamicAsymmetricRouteGetNeighbours()
        {
            this.DoTestGetNeighbours();
        }

        /// <summary>
        /// Tests a dynamic asymetric route's enumeration functionality.
        /// </summary>
        [Test]
        public void TestDynamicAsymmetricEnumeratePairs()
        {
            this.DoTestEnumeratePairs();
        }
    }
}