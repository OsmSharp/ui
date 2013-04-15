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
    /// Contains test function for the dynamic asymmetric multi route.
    /// </summary>
    [TestFixture]
    public class DynamicAsymmetricMultiRouteTests : MultiRouteTest
    {
        /// <summary>
        /// Creates a dynamic asymmetric multi route instance to test.
        /// </summary>
        /// <param name="is_round"></param>
        /// <returns></returns>
        protected override IMultiRoute BuildRoute(bool is_round)
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
