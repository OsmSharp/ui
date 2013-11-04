// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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
using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Streams;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Map.Styles.Streams;

namespace OsmSharp.UI.Test.Unittests.Map.Styles.Streams
{
    /// <summary>
    /// Contains tests for the osm stream style filter.
    /// </summary>
    [TestFixture]
    public class StyleOsmStreamFilterTests
    {
        /// <summary>
        /// Tests the filtering of a simple node.
        /// </summary>
        [Test]
        public void TestStyleOsmStreamNodes()
        {
            List<OsmGeo> result = this.FilterUsingStyleInterpreter(new OsmGeo[] {
                Node.Create(1, 0, 0) }.ToOsmStreamSource(),
                "node { " +
                "   color: black; " +
                "} ");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 1 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 0));

            result = this.FilterUsingStyleInterpreter(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, new TagsCollection(
                    Tag.Create("amenity", "bar")), 1, 1)}.ToOsmStreamSource(),
                "node[amenity] { " +
                "   color: black; " +
                "} ");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 2 &&
                (x as Node).Latitude == 1 &&
                (x as Node).Longitude == 1));
        }

        /// <summary>
        /// Tests the filtering of a simple way.
        /// </summary>
        [Test]
        public void TestStyleOsmStreamWays()
        {
            List<OsmGeo> result = this.FilterUsingStyleInterpreter(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Way.Create(1, new TagsCollection(
                        Tag.Create("highway", "residential")), 1, 2, 3) }.ToOsmStreamSource(),
                "way { " +
                "   color: black; " +
                "} ");

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 1 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 2 &&
                (x as Node).Latitude == 1 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 3 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 1));
            Assert.IsTrue(result.Any(x => x is Way &&
                x.Id == 1 &&
                (x as Way).Nodes != null &&
                (x as Way).Nodes.Count == 3 &&
                (x as Way).Nodes[0] == 1 &&
                (x as Way).Nodes[1] == 2 &&
                (x as Way).Nodes[2] == 3));

            result = this.FilterUsingStyleInterpreter(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Node.Create(4, 1, 1),
                Way.Create(1, new TagsCollection(
                        Tag.Create("highway", "residential")), 1, 2, 3),
                Way.Create(2, 1, 2, 4) }.ToOsmStreamSource(),
                "way[highway] { " +
                "   color: black; " +
                "} ");

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 1 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 2 &&
                (x as Node).Latitude == 1 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 3 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 1));
            Assert.IsTrue(result.Any(x => x is Way &&
                x.Id == 1 &&
                (x as Way).Nodes != null &&
                (x as Way).Nodes.Count == 3 &&
                (x as Way).Nodes[0] == 1 &&
                (x as Way).Nodes[1] == 2 &&
                (x as Way).Nodes[2] == 3));
        }

        /// <summary>
        /// Tests the filtering of a simple relation.
        /// </summary>
        [Test]
        public void TestStyleOsmStreamRelations()
        {
            List<OsmGeo> result = this.FilterUsingStyleInterpreter(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Way.Create(1, 1, 2, 3),
                Relation.Create(1, RelationMember.Create(1, "way", OsmGeoType.Way)) }.ToOsmStreamSource(),
                "relation { " +
                "   color: black; " +
                "} ");

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 1 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 2 &&
                (x as Node).Latitude == 1 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 3 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 1));
            Assert.IsTrue(result.Any(x => x is Way &&
                x.Id == 1 &&
                (x as Way).Nodes != null &&
                (x as Way).Nodes.Count == 3 &&
                (x as Way).Nodes[0] == 1 &&
                (x as Way).Nodes[1] == 2 &&
                (x as Way).Nodes[2] == 3));
            Assert.IsTrue(result.Any(x => x is Relation &&
                x.Id == 1 &&
                (x as Relation).Members != null &&
                (x as Relation).Members.Count == 1 &&
                (x as Relation).Members[0].MemberId == 1));

            result = this.FilterUsingStyleInterpreter(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Node.Create(4, 1, 1),
                Way.Create(1, 1, 2, 3),
                Way.Create(2, 1, 2, 4),
                Relation.Create(1, RelationMember.Create(1, "way", OsmGeoType.Way)),
                Relation.Create(2, new TagsCollection(
                    Tag.Create("boundary", "yes")), RelationMember.Create(2, "way", OsmGeoType.Way)) }.ToOsmStreamSource(),
                "relation[boundary] { " +
                "   color: black; " +
                "} ");

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 1 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 2 &&
                (x as Node).Latitude == 1 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 4 &&
                (x as Node).Latitude == 1 &&
                (x as Node).Longitude == 1));
            Assert.IsTrue(result.Any(x => x is Way &&
                x.Id == 2 &&
                (x as Way).Nodes != null &&
                (x as Way).Nodes.Count == 3 &&
                (x as Way).Nodes[0] == 1 &&
                (x as Way).Nodes[1] == 2 &&
                (x as Way).Nodes[2] == 4));
            Assert.IsTrue(result.Any(x => x is Relation &&
                x.Id == 2 &&
                (x as Relation).Members != null &&
                (x as Relation).Members.Count == 1 &&
                (x as Relation).Members[0].MemberId == 2));
        }

        /// <summary>
        /// Tests the filtering of a simple way area.
        /// </summary>
        [Test]
        public void TestStyleOsmStreamWayAreas()
        {
            List<OsmGeo> result = this.FilterUsingStyleInterpreter(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Way.Create(1, new TagsCollection(
                        Tag.Create("area", "yes")), 1, 2, 3, 1) }.ToOsmStreamSource(),
                "area { " +
                "   color: black; " +
                "} ");

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 1 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 2 &&
                (x as Node).Latitude == 1 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 3 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 1));
            Assert.IsTrue(result.Any(x => x is Way &&
                x.Id == 1 &&
                (x as Way).Nodes != null &&
                (x as Way).Nodes.Count == 4 &&
                (x as Way).Nodes[0] == 1 &&
                (x as Way).Nodes[1] == 2 &&
                (x as Way).Nodes[2] == 3 &&
                (x as Way).Nodes[3] == 1));

            result = this.FilterUsingStyleInterpreter(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Node.Create(4, 1, 1),
                Way.Create(1, new TagsCollection(
                        Tag.Create("area", "yes")), 1, 2, 3, 1),
                Way.Create(2, 1, 2, 4, 1) }.ToOsmStreamSource(),
                "area { " +
                "   color: black; " +
                "} ");

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 1 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 2 &&
                (x as Node).Latitude == 1 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 3 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 1));
            Assert.IsTrue(result.Any(x => x is Way &&
                x.Id == 1 &&
                (x as Way).Nodes != null &&
                (x as Way).Nodes.Count == 4 &&
                (x as Way).Nodes[0] == 1 &&
                (x as Way).Nodes[1] == 2 &&
                (x as Way).Nodes[2] == 3 &&
                (x as Way).Nodes[3] == 1));
        }

        /// <summary>
        /// Tests the filtering of a simple relation area.
        /// </summary>
        [Test]
        public void TestStyleOsmStreamRelationAreas()
        {
            List<OsmGeo> result = this.FilterUsingStyleInterpreter(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Way.Create(1, 1, 2, 3),
                Relation.Create(1, new TagsCollection(
                    Tag.Create("type", "multipolygon")), 
                    RelationMember.Create(1, "way", OsmGeoType.Way)) }.ToOsmStreamSource(),
                "area { " +
                "   color: black; " +
                "} ");

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 1 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 2 &&
                (x as Node).Latitude == 1 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 3 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 1));
            Assert.IsTrue(result.Any(x => x is Way &&
                x.Id == 1 &&
                (x as Way).Nodes != null &&
                (x as Way).Nodes.Count == 3 &&
                (x as Way).Nodes[0] == 1 &&
                (x as Way).Nodes[1] == 2 &&
                (x as Way).Nodes[2] == 3));
            Assert.IsTrue(result.Any(x => x is Relation &&
                x.Id == 1 &&
                (x as Relation).Members != null &&
                (x as Relation).Members.Count == 1 &&
                (x as Relation).Members[0].MemberId == 1));

            result = this.FilterUsingStyleInterpreter(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Node.Create(4, 1, 1),
                Way.Create(1, 1, 2, 3),
                Way.Create(2, 1, 2, 4),
                Relation.Create(1, RelationMember.Create(1, "way", OsmGeoType.Way)),
                Relation.Create(2, new TagsCollection(
                    Tag.Create("type", "boundary")), RelationMember.Create(2, "way", OsmGeoType.Way)) }.ToOsmStreamSource(),
                "area { " +
                "   color: black; " +
                "} ");

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 1 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 2 &&
                (x as Node).Latitude == 1 &&
                (x as Node).Longitude == 0));
            Assert.IsTrue(result.Any(x => x is Node &&
                x.Id == 4 &&
                (x as Node).Latitude == 1 &&
                (x as Node).Longitude == 1));
            Assert.IsTrue(result.Any(x => x is Way &&
                x.Id == 2 &&
                (x as Way).Nodes != null &&
                (x as Way).Nodes.Count == 3 &&
                (x as Way).Nodes[0] == 1 &&
                (x as Way).Nodes[1] == 2 &&
                (x as Way).Nodes[2] == 4));
            Assert.IsTrue(result.Any(x => x is Relation &&
                x.Id == 2 &&
                (x as Relation).Members != null &&
                (x as Relation).Members.Count == 1 &&
                (x as Relation).Members[0].MemberId == 2));
        }

        /// <summary>
        /// Executes the style filtering code.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="css"></param>
        /// <returns></returns>
        private List<OsmGeo> FilterUsingStyleInterpreter(IEnumerable<OsmGeo> source, string css)
        {
            MapCSSInterpreter interpreter = new MapCSSInterpreter(css,
                new MapCSSDictionaryImageSource());
            StyleOsmStreamFilter filter = new StyleOsmStreamFilter(interpreter);
            filter.RegisterSource(source.ToOsmStreamSource());
            return new List<OsmGeo>(filter);            
        }
    }
}
