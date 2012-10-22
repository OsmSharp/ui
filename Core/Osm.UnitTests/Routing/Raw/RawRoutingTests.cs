// OsmSharp - OpenStreetMap tools & library.
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
        public void TestRawShortedDefault()
        {
            this.DoTestShortestDefault();
        }

        /// <summary>
        /// Tests if the raw router preserves tags.
        /// </summary>
        [TestMethod]
        public void TestRawResolvedTags()
        {
            this.DoTestResolvedTags();
        }
    }
}