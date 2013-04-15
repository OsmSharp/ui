using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;

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
        protected override IRoute BuildRoute(int customer, bool is_round)
        {
            return new DynamicAsymmetricRoute(1, customer, is_round);
        }

        /// <summary>
        /// Builds a dynamic route that is empty.
        /// </summary>
        /// <param name="is_round"></param>
        /// <returns></returns>
        protected override IRoute BuildRoute(bool is_round)
        {
            return new DynamicAsymmetricRoute(1, is_round);
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
