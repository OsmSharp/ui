using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;

namespace OsmSharp.UnitTests.Routes
{
    /// <summary>
    /// Contains testing code for a dynamic asymmetric route.
    /// </summary>
    [TestClass]
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
        [TestMethod]
        public void TestDynamicAsymmetricRouteAdd()
        {
            this.DoTestAdd();
        }

        /// <summary>
        /// Tests a dynamic asymmetric route by removing customers.
        /// </summary>
        [TestMethod]
        public void TestDynamicAsymmetricRouteRemove()
        {
            this.DoTestRemove();
        }

        /// <summary>
        /// Tests a dynamic asymmetric route by removing customers.
        /// </summary>
        [TestMethod]
        public void TestDynamicAsymmetricRouteContains()
        {
            this.DoTestContains();
        }
    }
}
