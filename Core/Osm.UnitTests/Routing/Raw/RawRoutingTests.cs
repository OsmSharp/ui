using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Raw;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osm.Routing.Core;
using Osm.Data.Raw.XML.OsmSource;
using Osm.Routing.Core.Interpreter;
using Osm.Routing.Core.Constraints;
using System.IO;
using System.Reflection;
using Osm.Core.Xml;
using System.Xml;

namespace Osm.UnitTests.Routing.Raw
{
    [TestClass]
    public class RawRoutingTests : SimpleRoutingTests<ResolvedPoint>
    {
        /// <summary>
        /// Builds a router.
        /// </summary>
        /// <returns></returns>
        public override IRouter<ResolvedPoint> BuildRouter(RoutingInterpreterBase interpreter, IRoutingConstraints constraints)
        {
            // build all the data source.
            OsmDataSource osm_data = new OsmDataSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("Osm.UnitTests.test_network.osm"));

            // build the router.
            return new Router(osm_data, interpreter, constraints);
        }

        /// <summary>
        /// Tests a simple shortest route calculation.
        /// </summary>
        [TestMethod]
        public void TestShortedDefault()
        {
            this.DoTestShortestDefault();
        }
    }
}