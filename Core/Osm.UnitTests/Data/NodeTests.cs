using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osm.Core;
using Osm.Core.Factory;

namespace Osm.UnitTests.Data
{
    [TestClass]
    public class NodeTests
    {
        /// <summary>
        /// Regression test for bug in ToString() for a node without coordinates.
        /// </summary>
        [TestMethod]
        public void NodeToStringTests()
        {
            Node test_node = OsmBaseFactory.CreateNode(-1);
            string description = test_node.ToString(); // 
            test_node.Coordinate = new Tools.Math.Geo.GeoCoordinate(0, 0);
            description = test_node.ToString();
        }
    }
}
