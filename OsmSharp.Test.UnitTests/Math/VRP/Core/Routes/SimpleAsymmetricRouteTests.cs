// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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
using System.Collections.Generic;

namespace OsmSharp.Test.Unittests.Math.VRP.Core.Routes
{
    /// <summary>
    /// Contains testing code for a simple asymmetric route.
    /// </summary>
    [TestFixture]
    public class SimpleAsymmetricRouteTests : RouteTest
    {
        /// <summary>
        /// Builds a simple route initialized with an initial customer.
        /// </summary>
        /// <returns></returns>
        protected override IRoute BuildRoute(int customer, bool isRound)
        {
            var customers = new List<int>();
            customers.Add(customer);
            return new SimpleAsymmetricRoute(customers, isRound);
        }

        /// <summary>
        /// Builds a dynamic route that is empty.
        /// </summary>
        /// <param name="isRound"></param>
        /// <returns></returns>
        protected override IRoute BuildRoute(bool isRound)
        {
            var customers = new List<int>();
            return new SimpleAsymmetricRoute(customers, isRound);
        }

        /// <summary>
        /// Tests a dynamic asymmetric route by adding customers.
        /// </summary>
        [Test]
        public void TestSimpleAsymmetricRouteAdd()
        {
            this.DoTestAdd();
        }

        /// <summary>
        /// Tests a dynamic asymmetric route by removing customers.
        /// </summary>
        [Test]
        public void TestSimpleAsymmetricRouteRemove()
        {
            this.DoTestRemove();
        }
    }
}