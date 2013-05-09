using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Routing;
using OsmSharp.Osm;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Tools.Collections.Tags;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Graph.Router.Dykstra;

namespace OsmSharp.UnitTests.Routing.EdgeMatcher
{
    /// <summary>
    /// Tests the OsmRoutingInterpreter.
    /// </summary>
    [TestFixture]
    public class EdgeMatcherTests
    {
        /// <summary>
        /// Tests the edge matcher function.
        /// </summary>
        [Test]
        public void TestEdgeMatcher()
        {
            IEdgeMatcher matcher = new DefaultEdgeMatcher();

            // create edge tags.
            var edgeTags = new SimpleTagsCollection();
            //edge_tags["highway"] = "footway";

            // create point tags.
            var pointTags = new SimpleTagsCollection();
            //point_tags["highway"] = "footway";

            // test with empty point tags.
            Assert.IsTrue(matcher.MatchWithEdge(VehicleEnum.Car, null, null));
            Assert.IsTrue(matcher.MatchWithEdge(VehicleEnum.Car, pointTags, null));

            // test with empty edge tags.
            pointTags["name"] = "Ben Abelshausen Boulevard";
            Assert.IsFalse(matcher.MatchWithEdge(VehicleEnum.Car, pointTags, null));
            Assert.IsFalse(matcher.MatchWithEdge(VehicleEnum.Car, pointTags, edgeTags));

            // test with matching name.
            edgeTags["name"] = "Ben Abelshausen Boulevard";
            Assert.IsTrue(matcher.MatchWithEdge(VehicleEnum.Car, pointTags, edgeTags));

            // test with none-matching name.
            edgeTags["name"] = "Jorieke Vyncke Boulevard";
            Assert.IsFalse(matcher.MatchWithEdge(VehicleEnum.Car, pointTags, edgeTags));
        }

        /// <summary>
        /// Tests the edge matcher in combination with dykstra routing.
        /// </summary>
        [Test]
        public void TestEdgeMatcherDykstra()
        {
            string name = "Ben Abelshausen Boulevard";
            IEdgeMatcher matc = new DefaultEdgeMatcher();

            this.TestResolveOnEdge(name, "footway", VehicleEnum.Pedestrian, matc, true);
            this.TestResolveOnEdge(name, "cycleway", VehicleEnum.Pedestrian, matc, false);
            this.TestResolveOnEdge(name, "bridleway", VehicleEnum.Pedestrian, matc, false);
            this.TestResolveOnEdge(name, "path", VehicleEnum.Pedestrian, matc, true);
            this.TestResolveOnEdge(name, "pedestrian", VehicleEnum.Pedestrian, matc, true);
            this.TestResolveOnEdge(name, "road", VehicleEnum.Pedestrian, matc, true);
            this.TestResolveOnEdge(name, "living_street", VehicleEnum.Pedestrian, matc, true);
            this.TestResolveOnEdge(name, "residential", VehicleEnum.Pedestrian, matc, true);
            this.TestResolveOnEdge(name, "unclassified", VehicleEnum.Pedestrian, matc, true);
            this.TestResolveOnEdge(name, "tertiary", VehicleEnum.Pedestrian, matc, true);
            this.TestResolveOnEdge(name, "secondary", VehicleEnum.Pedestrian, matc, true);
            this.TestResolveOnEdge(name, "primary", VehicleEnum.Pedestrian, matc, true);
            this.TestResolveOnEdge(name, "trunk", VehicleEnum.Pedestrian, matc, true);
            this.TestResolveOnEdge(name, "motorway", VehicleEnum.Pedestrian, matc, false);

            this.TestResolveOnEdge(name, "footway", VehicleEnum.Bicycle, matc, false);
            this.TestResolveOnEdge(name, "cycleway", VehicleEnum.Bicycle, matc, true);
            this.TestResolveOnEdge(name, "bridleway", VehicleEnum.Bicycle, matc, false);
            this.TestResolveOnEdge(name, "path", VehicleEnum.Bicycle, matc, true);
            this.TestResolveOnEdge(name, "pedestrian", VehicleEnum.Bicycle, matc, false);
            this.TestResolveOnEdge(name, "road", VehicleEnum.Bicycle, matc, true);
            this.TestResolveOnEdge(name, "living_street", VehicleEnum.Bicycle, matc, true);
            this.TestResolveOnEdge(name, "residential", VehicleEnum.Bicycle, matc, true);
            this.TestResolveOnEdge(name, "unclassified", VehicleEnum.Bicycle, matc, true);
            this.TestResolveOnEdge(name, "tertiary", VehicleEnum.Bicycle, matc, true);
            this.TestResolveOnEdge(name, "secondary", VehicleEnum.Bicycle, matc, true);
            this.TestResolveOnEdge(name, "primary", VehicleEnum.Bicycle, matc, true);
            this.TestResolveOnEdge(name, "trunk", VehicleEnum.Bicycle, matc, true);
            this.TestResolveOnEdge(name, "motorway", VehicleEnum.Bicycle, matc, false);

            this.TestResolveOnEdge(name, "footway", VehicleEnum.Moped, matc, false);
            this.TestResolveOnEdge(name, "cycleway", VehicleEnum.Moped, matc, false);
            this.TestResolveOnEdge(name, "bridleway", VehicleEnum.Moped, matc, false);
            this.TestResolveOnEdge(name, "path", VehicleEnum.Moped, matc, false);
            this.TestResolveOnEdge(name, "pedestrian", VehicleEnum.Moped, matc, false);
            this.TestResolveOnEdge(name, "road", VehicleEnum.Moped, matc, true);
            this.TestResolveOnEdge(name, "living_street", VehicleEnum.Moped, matc, true);
            this.TestResolveOnEdge(name, "residential", VehicleEnum.Moped, matc, true);
            this.TestResolveOnEdge(name, "unclassified", VehicleEnum.Moped, matc, true);
            this.TestResolveOnEdge(name, "tertiary", VehicleEnum.Moped, matc, true);
            this.TestResolveOnEdge(name, "secondary", VehicleEnum.Moped, matc, true);
            this.TestResolveOnEdge(name, "primary", VehicleEnum.Moped, matc, true);
            this.TestResolveOnEdge(name, "trunk", VehicleEnum.Moped, matc, true);
            this.TestResolveOnEdge(name, "motorway", VehicleEnum.Moped, matc, false);

            this.TestResolveOnEdge(name, "footway", VehicleEnum.MotorCycle, matc, false);
            this.TestResolveOnEdge(name, "cycleway", VehicleEnum.MotorCycle, matc, false);
            this.TestResolveOnEdge(name, "bridleway", VehicleEnum.MotorCycle, matc, false);
            this.TestResolveOnEdge(name, "path", VehicleEnum.MotorCycle, matc, false);
            this.TestResolveOnEdge(name, "pedestrian", VehicleEnum.MotorCycle, matc, false);
            this.TestResolveOnEdge(name, "road", VehicleEnum.MotorCycle, matc, true);
            this.TestResolveOnEdge(name, "living_street", VehicleEnum.MotorCycle, matc, true);
            this.TestResolveOnEdge(name, "residential", VehicleEnum.MotorCycle, matc, true);
            this.TestResolveOnEdge(name, "unclassified", VehicleEnum.MotorCycle, matc, true);
            this.TestResolveOnEdge(name, "tertiary", VehicleEnum.MotorCycle, matc, true);
            this.TestResolveOnEdge(name, "secondary", VehicleEnum.MotorCycle, matc, true);
            this.TestResolveOnEdge(name, "primary", VehicleEnum.MotorCycle, matc, true);
            this.TestResolveOnEdge(name, "trunk", VehicleEnum.MotorCycle, matc, true);
            this.TestResolveOnEdge(name, "motorway", VehicleEnum.MotorCycle, matc, true);

            this.TestResolveOnEdge(name, "footway", VehicleEnum.Car, matc, false);
            this.TestResolveOnEdge(name, "cycleway", VehicleEnum.Car, matc, false);
            this.TestResolveOnEdge(name, "bridleway", VehicleEnum.Car, matc, false);
            this.TestResolveOnEdge(name, "path", VehicleEnum.Car, matc, false);
            this.TestResolveOnEdge(name, "pedestrian", VehicleEnum.Car, matc, false);
            this.TestResolveOnEdge(name, "road", VehicleEnum.Car, matc, true);
            this.TestResolveOnEdge(name, "living_street", VehicleEnum.Car, matc, true);
            this.TestResolveOnEdge(name, "residential", VehicleEnum.Car, matc, true);
            this.TestResolveOnEdge(name, "unclassified", VehicleEnum.Car, matc, true);
            this.TestResolveOnEdge(name, "tertiary", VehicleEnum.Car, matc, true);
            this.TestResolveOnEdge(name, "secondary", VehicleEnum.Car, matc, true);
            this.TestResolveOnEdge(name, "primary", VehicleEnum.Car, matc, true);
            this.TestResolveOnEdge(name, "trunk", VehicleEnum.Car, matc, true);
            this.TestResolveOnEdge(name, "motorway", VehicleEnum.Car, matc, true);

            this.TestResolveOnEdge(name, "footway", VehicleEnum.SmallTruck, matc, false);
            this.TestResolveOnEdge(name, "cycleway", VehicleEnum.SmallTruck, matc, false);
            this.TestResolveOnEdge(name, "bridleway", VehicleEnum.SmallTruck, matc, false);
            this.TestResolveOnEdge(name, "path", VehicleEnum.SmallTruck, matc, false);
            this.TestResolveOnEdge(name, "pedestrian", VehicleEnum.SmallTruck, matc, false);
            this.TestResolveOnEdge(name, "road", VehicleEnum.SmallTruck, matc, true);
            this.TestResolveOnEdge(name, "living_street", VehicleEnum.SmallTruck, matc, true);
            this.TestResolveOnEdge(name, "residential", VehicleEnum.SmallTruck, matc, true);
            this.TestResolveOnEdge(name, "unclassified", VehicleEnum.SmallTruck, matc, true);
            this.TestResolveOnEdge(name, "tertiary", VehicleEnum.SmallTruck, matc, true);
            this.TestResolveOnEdge(name, "secondary", VehicleEnum.SmallTruck, matc, true);
            this.TestResolveOnEdge(name, "primary", VehicleEnum.SmallTruck, matc, true);
            this.TestResolveOnEdge(name, "trunk", VehicleEnum.SmallTruck, matc, true);
            this.TestResolveOnEdge(name, "motorway", VehicleEnum.SmallTruck, matc, true);

            this.TestResolveOnEdge(name, "footway", VehicleEnum.BigTruck, matc, false);
            this.TestResolveOnEdge(name, "cycleway", VehicleEnum.BigTruck, matc, false);
            this.TestResolveOnEdge(name, "bridleway", VehicleEnum.BigTruck, matc, false);
            this.TestResolveOnEdge(name, "path", VehicleEnum.BigTruck, matc, false);
            this.TestResolveOnEdge(name, "pedestrian", VehicleEnum.BigTruck, matc, false);
            this.TestResolveOnEdge(name, "road", VehicleEnum.BigTruck, matc, true);
            this.TestResolveOnEdge(name, "living_street", VehicleEnum.BigTruck, matc, true);
            this.TestResolveOnEdge(name, "residential", VehicleEnum.BigTruck, matc, true);
            this.TestResolveOnEdge(name, "unclassified", VehicleEnum.BigTruck, matc, true);
            this.TestResolveOnEdge(name, "tertiary", VehicleEnum.BigTruck, matc, true);
            this.TestResolveOnEdge(name, "secondary", VehicleEnum.BigTruck, matc, true);
            this.TestResolveOnEdge(name, "primary", VehicleEnum.BigTruck, matc, true);
            this.TestResolveOnEdge(name, "trunk", VehicleEnum.BigTruck, matc, true);
            this.TestResolveOnEdge(name, "motorway", VehicleEnum.BigTruck, matc, true);

            this.TestResolveOnEdge(name, "footway", VehicleEnum.Bus, matc, false);
            this.TestResolveOnEdge(name, "cycleway", VehicleEnum.Bus, matc, false);
            this.TestResolveOnEdge(name, "bridleway", VehicleEnum.Bus, matc, false);
            this.TestResolveOnEdge(name, "path", VehicleEnum.Bus, matc, false);
            this.TestResolveOnEdge(name, "pedestrian", VehicleEnum.Bus, matc, false);
            this.TestResolveOnEdge(name, "road", VehicleEnum.Bus, matc, true);
            this.TestResolveOnEdge(name, "living_street", VehicleEnum.Bus, matc, true);
            this.TestResolveOnEdge(name, "residential", VehicleEnum.Bus, matc, true);
            this.TestResolveOnEdge(name, "unclassified", VehicleEnum.Bus, matc, true);
            this.TestResolveOnEdge(name, "tertiary", VehicleEnum.Bus, matc, true);
            this.TestResolveOnEdge(name, "secondary", VehicleEnum.Bus, matc, true);
            this.TestResolveOnEdge(name, "primary", VehicleEnum.Bus, matc, true);
            this.TestResolveOnEdge(name, "trunk", VehicleEnum.Bus, matc, true);
            this.TestResolveOnEdge(name, "motorway", VehicleEnum.Bus, matc, true);
        }

        /// <summary>
        /// Tests the edge matcher in combination with dykstra routing.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="highway"></param>
        /// <param name="vehicle"></param>
        /// <param name="matcher"></param>
        /// <param name="found"></param>
        private void TestResolveOnEdge(string name, string highway,
            VehicleEnum vehicle, IEdgeMatcher matcher, bool found)
        {
            this.TestResolveOnEdgeSingle(name, highway, vehicle, null, null, !found);
            this.TestResolveOnEdgeSingle(name, highway, vehicle, matcher, null, !found);
            this.TestResolveOnEdgeSingle(name, highway, vehicle, matcher, name, !found);
        }

        /// <summary>
        /// Tests the edge matcher in combination with dykstra routing.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="highway"></param>
        /// <param name="vehicle"></param>
        /// <param name="matcher"></param>
        /// <param name="pointName"></param>
        /// <param name="notFound"></param>
        private void TestResolveOnEdgeSingle(string name, string highway, 
            VehicleEnum vehicle, IEdgeMatcher matcher, 
            string pointName, bool notFound)
        {
            var fromName = new GeoCoordinate(51.0003, 4.0007);
            var toName = new GeoCoordinate(51.0003, 4.0008);

            var fromNoname = new GeoCoordinate(51.0, 4.0007);
            var toNoname = new GeoCoordinate(51.0, 4.0008);

            TagsCollection pointTags = new SimpleTagsCollection();
            pointTags["name"] = pointName;

            TagsCollection tags = new SimpleTagsCollection();
            tags["highway"] = highway;
            //tags["name"] = name;

            var tagsIndex = new SimpleTagsIndex();

            // do the data processing.
            var data = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            uint vertexNoname1 = data.AddVertex((float)fromNoname.Latitude, (float)fromNoname.Longitude);
            uint vertexNoname2 = data.AddVertex((float)toNoname.Latitude, (float)toNoname.Longitude);
            data.AddArc(vertexNoname1, vertexNoname2, new LiveEdge()
            {
                Forward = true,
                Tags = tagsIndex.Add(tags)
            }, null);
            tags = new SimpleTagsCollection();
            tags["highway"] = highway;
            tags["name"] = name;
            uint vertexName1 = data.AddVertex((float)fromName.Latitude, (float)fromName.Longitude);
            uint vertexName2 = data.AddVertex((float)toName.Latitude, (float)toName.Longitude);
            data.AddArc(vertexName1, vertexName2, new LiveEdge()
            {
                Forward = true,
                Tags = tagsIndex.Add(tags)
            }, null);

            IRoutingInterpreter interpreter = new OsmRoutingInterpreter();

            // creates the data.
            IBasicRouter<LiveEdge> router = new DykstraRoutingLive(
                data.TagsIndex);

            var nonameLocation = new GeoCoordinate(
                (fromNoname.Latitude + toNoname.Latitude) / 2.0,
                (fromNoname.Longitude + toNoname.Longitude) / 2.0);
            var nameLocation = new GeoCoordinate(
                (fromName.Latitude + toName.Latitude) / 2.0,
                (fromName.Longitude + toName.Longitude) / 2.0);

            const float delta = 0.01f;
            SearchClosestResult result = router.SearchClosest(data, interpreter, vehicle, nonameLocation, delta, matcher, pointTags);
            if (result.Distance < double.MaxValue)
            { // there is a result.
                Assert.IsFalse(notFound, "A result was found but was supposed not to  be found!");

                if (name == pointName)
                { // the name location was supposed to be found!
                    Assert.IsTrue(result.Vertex1 == vertexName1 || result.Vertex1 == vertexName2);
                    Assert.IsTrue(result.Vertex2 == vertexName1 || result.Vertex2 == vertexName2);
                }
                else
                { // the noname location was supposed to be found!
                    Assert.IsTrue(result.Vertex1 == vertexNoname1 || result.Vertex1 == vertexNoname2);
                    Assert.IsTrue(result.Vertex2 == vertexNoname1 || result.Vertex2 == vertexNoname2);
                }
                return;
            }
            Assert.IsTrue(notFound, "A result was not found but was supposed to be found!");
        }
    }
}