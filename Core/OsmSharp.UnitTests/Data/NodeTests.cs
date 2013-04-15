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
using NUnit.Framework;
using OsmSharp.Osm;
using OsmSharp.Osm.Factory;

namespace OsmSharp.Osm.UnitTests.Data
{
    /// <summary>
    /// Contains some regression tests on nodes.
    /// </summary>
    [TestFixture]
    public class NodeTests
    {
        /// <summary>
        /// Regression test for bug in ToString() for a node without coordinates.
        /// </summary>
        [Test]
        public void NodeToStringTests()
        {
            Node test_node = OsmBaseFactory.CreateNode(-1);
            string description = test_node.ToString(); // 
            test_node.Coordinate = new OsmSharp.Tools.Math.Geo.GeoCoordinate(0, 0);
            description = test_node.ToString();
        }
    }
}
