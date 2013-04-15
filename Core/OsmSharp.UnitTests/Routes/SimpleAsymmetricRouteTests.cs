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
    /// Contains testing code for a simple asymmetric route.
    /// </summary>
    [TestFixture]
    public class SimpleAsymmetricRouteTests : RouteTest
    {
        /// <summary>
        /// Builds a simple route initialized with an initial customer.
        /// </summary>
        /// <returns></returns>
        protected override IRoute BuildRoute(int customer, bool is_round)
        {
            List<int> customers = new List<int>();
            customers.Add(customer);
            return new SimpleAsymmetricRoute(customers, is_round);
        }

        /// <summary>
        /// Builds a dynamic route that is empty.
        /// </summary>
        /// <param name="is_round"></param>
        /// <returns></returns>
        protected override IRoute BuildRoute(bool is_round)
        {
            List<int> customers = new List<int>();
            return new SimpleAsymmetricRoute(customers, is_round);
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
