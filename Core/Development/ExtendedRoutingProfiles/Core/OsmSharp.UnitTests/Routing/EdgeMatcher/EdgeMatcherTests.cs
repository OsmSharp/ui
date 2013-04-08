using System.Collections.Generic;
using NUnit.Framework;
using OsmSharp.Osm;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.DynamicGraph.SimpleWeighed;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.UnitTests.Routing.EdgeMatcher
{
    /// <summary>
    ///     Tests the OsmRoutingInterpreter.
    /// </summary>
    [TestFixture]
    public class EdgeMatcherTests
    {
        /// <summary>
        ///     Tests the edge matcher function.
        /// </summary>
        [Test]
        public void TestEdgeMatcher()
        {
            IEdgeMatcher matcher = new DefaultEdgeMatcher();

            // create edge tags.
            var edge_tags = new Dictionary<string, string>();
            //edge_tags["highway"] = "footway";

            // create point tags.
            var point_tags = new Dictionary<string, string>();
            //point_tags["highway"] = "footway";

            // test with empty point tags.
            Assert.IsTrue(matcher.MatchWithEdge(Vehicle.Car, null, null));
            Assert.IsTrue(matcher.MatchWithEdge(Vehicle.Car, point_tags, null));

            // test with empty edge tags.
            point_tags["name"] = "Ben Abelshausen Boulevard";
            Assert.IsFalse(matcher.MatchWithEdge(Vehicle.Car, point_tags, null));
            Assert.IsFalse(matcher.MatchWithEdge(Vehicle.Car, point_tags, edge_tags));

            // test with matching name.
            edge_tags["name"] = "Ben Abelshausen Boulevard";
            Assert.IsTrue(matcher.MatchWithEdge(Vehicle.Car, point_tags, edge_tags));

            // test with none-matching name.
            edge_tags["name"] = "Jorieke Vyncke Boulevard";
            Assert.IsFalse(matcher.MatchWithEdge(Vehicle.Car, point_tags, edge_tags));
        }

        /// <summary>
        ///     Tests the edge matcher in combination with dykstra routing.
        /// </summary>
        [Test]
        public void TestEdgeMatcherDykstra()
        {
            var name = "Ben Abelshausen Boulevard";
            IEdgeMatcher matc = new DefaultEdgeMatcher();

            TestResolveOnEdge(name, "footway", Vehicle.Pedestrian, matc, true);
            TestResolveOnEdge(name, "cycleway", Vehicle.Pedestrian, matc, false);
            TestResolveOnEdge(name, "bridleway", Vehicle.Pedestrian, matc, false);
            TestResolveOnEdge(name, "path", Vehicle.Pedestrian, matc, true);
            TestResolveOnEdge(name, "pedestrian", Vehicle.Pedestrian, matc, true);
            TestResolveOnEdge(name, "road", Vehicle.Pedestrian, matc, true);
            TestResolveOnEdge(name, "living_street", Vehicle.Pedestrian, matc, true);
            TestResolveOnEdge(name, "residential", Vehicle.Pedestrian, matc, true);
            TestResolveOnEdge(name, "unclassified", Vehicle.Pedestrian, matc, true);
            TestResolveOnEdge(name, "tertiary", Vehicle.Pedestrian, matc, true);
            TestResolveOnEdge(name, "secondary", Vehicle.Pedestrian, matc, true);
            TestResolveOnEdge(name, "primary", Vehicle.Pedestrian, matc, true);
            TestResolveOnEdge(name, "trunk", Vehicle.Pedestrian, matc, true);
            TestResolveOnEdge(name, "motorway", Vehicle.Pedestrian, matc, false);

            TestResolveOnEdge(name, "footway", Vehicle.Bicycle, matc, false);
            TestResolveOnEdge(name, "cycleway", Vehicle.Bicycle, matc, true);
            TestResolveOnEdge(name, "bridleway", Vehicle.Bicycle, matc, false);
            TestResolveOnEdge(name, "path", Vehicle.Bicycle, matc, true);
            TestResolveOnEdge(name, "pedestrian", Vehicle.Bicycle, matc, false);
            TestResolveOnEdge(name, "road", Vehicle.Bicycle, matc, true);
            TestResolveOnEdge(name, "living_street", Vehicle.Bicycle, matc, true);
            TestResolveOnEdge(name, "residential", Vehicle.Bicycle, matc, true);
            TestResolveOnEdge(name, "unclassified", Vehicle.Bicycle, matc, true);
            TestResolveOnEdge(name, "tertiary", Vehicle.Bicycle, matc, true);
            TestResolveOnEdge(name, "secondary", Vehicle.Bicycle, matc, true);
            TestResolveOnEdge(name, "primary", Vehicle.Bicycle, matc, true);
            TestResolveOnEdge(name, "trunk", Vehicle.Bicycle, matc, true);
            TestResolveOnEdge(name, "motorway", Vehicle.Bicycle, matc, false);

            TestResolveOnEdge(name, "footway", Vehicle.Moped, matc, false);
            TestResolveOnEdge(name, "cycleway", Vehicle.Moped, matc, false);
            TestResolveOnEdge(name, "bridleway", Vehicle.Moped, matc, false);
            TestResolveOnEdge(name, "path", Vehicle.Moped, matc, false);
            TestResolveOnEdge(name, "pedestrian", Vehicle.Moped, matc, false);
            TestResolveOnEdge(name, "road", Vehicle.Moped, matc, true);
            TestResolveOnEdge(name, "living_street", Vehicle.Moped, matc, true);
            TestResolveOnEdge(name, "residential", Vehicle.Moped, matc, true);
            TestResolveOnEdge(name, "unclassified", Vehicle.Moped, matc, true);
            TestResolveOnEdge(name, "tertiary", Vehicle.Moped, matc, true);
            TestResolveOnEdge(name, "secondary", Vehicle.Moped, matc, true);
            TestResolveOnEdge(name, "primary", Vehicle.Moped, matc, true);
            TestResolveOnEdge(name, "trunk", Vehicle.Moped, matc, true);
            TestResolveOnEdge(name, "motorway", Vehicle.Moped, matc, false);

            TestResolveOnEdge(name, "footway", Vehicle.MotorCycle, matc, false);
            TestResolveOnEdge(name, "cycleway", Vehicle.MotorCycle, matc, false);
            TestResolveOnEdge(name, "bridleway", Vehicle.MotorCycle, matc, false);
            TestResolveOnEdge(name, "path", Vehicle.MotorCycle, matc, false);
            TestResolveOnEdge(name, "pedestrian", Vehicle.MotorCycle, matc, false);
            TestResolveOnEdge(name, "road", Vehicle.MotorCycle, matc, true);
            TestResolveOnEdge(name, "living_street", Vehicle.MotorCycle, matc, true);
            TestResolveOnEdge(name, "residential", Vehicle.MotorCycle, matc, true);
            TestResolveOnEdge(name, "unclassified", Vehicle.MotorCycle, matc, true);
            TestResolveOnEdge(name, "tertiary", Vehicle.MotorCycle, matc, true);
            TestResolveOnEdge(name, "secondary", Vehicle.MotorCycle, matc, true);
            TestResolveOnEdge(name, "primary", Vehicle.MotorCycle, matc, true);
            TestResolveOnEdge(name, "trunk", Vehicle.MotorCycle, matc, true);
            TestResolveOnEdge(name, "motorway", Vehicle.MotorCycle, matc, true);

            TestResolveOnEdge(name, "footway", Vehicle.Car, matc, false);
            TestResolveOnEdge(name, "cycleway", Vehicle.Car, matc, false);
            TestResolveOnEdge(name, "bridleway", Vehicle.Car, matc, false);
            TestResolveOnEdge(name, "path", Vehicle.Car, matc, false);
            TestResolveOnEdge(name, "pedestrian", Vehicle.Car, matc, false);
            TestResolveOnEdge(name, "road", Vehicle.Car, matc, true);
            TestResolveOnEdge(name, "living_street", Vehicle.Car, matc, true);
            TestResolveOnEdge(name, "residential", Vehicle.Car, matc, true);
            TestResolveOnEdge(name, "unclassified", Vehicle.Car, matc, true);
            TestResolveOnEdge(name, "tertiary", Vehicle.Car, matc, true);
            TestResolveOnEdge(name, "secondary", Vehicle.Car, matc, true);
            TestResolveOnEdge(name, "primary", Vehicle.Car, matc, true);
            TestResolveOnEdge(name, "trunk", Vehicle.Car, matc, true);
            TestResolveOnEdge(name, "motorway", Vehicle.Car, matc, true);

            TestResolveOnEdge(name, "footway", Vehicle.SmallTruck, matc, false);
            TestResolveOnEdge(name, "cycleway", Vehicle.SmallTruck, matc, false);
            TestResolveOnEdge(name, "bridleway", Vehicle.SmallTruck, matc, false);
            TestResolveOnEdge(name, "path", Vehicle.SmallTruck, matc, false);
            TestResolveOnEdge(name, "pedestrian", Vehicle.SmallTruck, matc, false);
            TestResolveOnEdge(name, "road", Vehicle.SmallTruck, matc, true);
            TestResolveOnEdge(name, "living_street", Vehicle.SmallTruck, matc, true);
            TestResolveOnEdge(name, "residential", Vehicle.SmallTruck, matc, true);
            TestResolveOnEdge(name, "unclassified", Vehicle.SmallTruck, matc, true);
            TestResolveOnEdge(name, "tertiary", Vehicle.SmallTruck, matc, true);
            TestResolveOnEdge(name, "secondary", Vehicle.SmallTruck, matc, true);
            TestResolveOnEdge(name, "primary", Vehicle.SmallTruck, matc, true);
            TestResolveOnEdge(name, "trunk", Vehicle.SmallTruck, matc, true);
            TestResolveOnEdge(name, "motorway", Vehicle.SmallTruck, matc, true);

            TestResolveOnEdge(name, "footway", Vehicle.BigTruck, matc, false);
            TestResolveOnEdge(name, "cycleway", Vehicle.BigTruck, matc, false);
            TestResolveOnEdge(name, "bridleway", Vehicle.BigTruck, matc, false);
            TestResolveOnEdge(name, "path", Vehicle.BigTruck, matc, false);
            TestResolveOnEdge(name, "pedestrian", Vehicle.BigTruck, matc, false);
            TestResolveOnEdge(name, "road", Vehicle.BigTruck, matc, true);
            TestResolveOnEdge(name, "living_street", Vehicle.BigTruck, matc, true);
            TestResolveOnEdge(name, "residential", Vehicle.BigTruck, matc, true);
            TestResolveOnEdge(name, "unclassified", Vehicle.BigTruck, matc, true);
            TestResolveOnEdge(name, "tertiary", Vehicle.BigTruck, matc, true);
            TestResolveOnEdge(name, "secondary", Vehicle.BigTruck, matc, true);
            TestResolveOnEdge(name, "primary", Vehicle.BigTruck, matc, true);
            TestResolveOnEdge(name, "trunk", Vehicle.BigTruck, matc, true);
            TestResolveOnEdge(name, "motorway", Vehicle.BigTruck, matc, true);

            TestResolveOnEdge(name, "footway", Vehicle.Bus, matc, false);
            TestResolveOnEdge(name, "cycleway", Vehicle.Bus, matc, false);
            TestResolveOnEdge(name, "bridleway", Vehicle.Bus, matc, false);
            TestResolveOnEdge(name, "path", Vehicle.Bus, matc, false);
            TestResolveOnEdge(name, "pedestrian", Vehicle.Bus, matc, false);
            TestResolveOnEdge(name, "road", Vehicle.Bus, matc, true);
            TestResolveOnEdge(name, "living_street", Vehicle.Bus, matc, true);
            TestResolveOnEdge(name, "residential", Vehicle.Bus, matc, true);
            TestResolveOnEdge(name, "unclassified", Vehicle.Bus, matc, true);
            TestResolveOnEdge(name, "tertiary", Vehicle.Bus, matc, true);
            TestResolveOnEdge(name, "secondary", Vehicle.Bus, matc, true);
            TestResolveOnEdge(name, "primary", Vehicle.Bus, matc, true);
            TestResolveOnEdge(name, "trunk", Vehicle.Bus, matc, true);
            TestResolveOnEdge(name, "motorway", Vehicle.Bus, matc, true);
        }

        /// <summary>
        ///     Tests the edge matcher in combination with dykstra routing.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="highway"></param>
        /// <param name="vehicle"></param>
        /// <param name="matcher"></param>
        /// <param name="found"></param>
        private void TestResolveOnEdge(string name, string highway, Vehicle vehicle, IEdgeMatcher matcher, bool found)
        {
            TestResolveOnEdgeSingle(name, highway, vehicle, null, null, !found);
            TestResolveOnEdgeSingle(name, highway, vehicle, matcher, null, !found);
            TestResolveOnEdgeSingle(name, highway, vehicle, matcher, name, !found);
        }

        /// <summary>
        ///     Tests the edge matcher in combination with dykstra routing.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="highway"></param>
        /// <param name="vehicle"></param>
        /// <param name="matcher"></param>
        /// <param name="point_name"></param>
        /// <param name="not_found"></param>
        private void TestResolveOnEdgeSingle(string name, string highway,
                                             Vehicle vehicle, IEdgeMatcher matcher,
                                             string point_name, bool not_found)
        {
            var from_name = new GeoCoordinate(51.0003, 4.0007);
            var to_name = new GeoCoordinate(51.0003, 4.0008);

            var from_noname = new GeoCoordinate(51.0, 4.0007);
            var to_noname = new GeoCoordinate(51.0, 4.0008);

            var point_tags = new Dictionary<string, string>();
            point_tags["name"] = point_name;

            var tags = new Dictionary<string, string>();
            tags["highway"] = highway;
            //tags["name"] = name;

            var tags_index = new OsmTagsIndex();

            // do the data processing.
            var data =
                new DynamicGraphRouterDataSource<SimpleWeighedEdge>(tags_index);
            var vertex_noname1 = data.AddVertex((float) from_noname.Latitude, (float) from_noname.Longitude);
            var vertex_noname2 = data.AddVertex((float) to_noname.Latitude, (float) to_noname.Longitude);
            data.AddArc(vertex_noname1, vertex_noname2, new SimpleWeighedEdge
                {
                    IsForward = true,
                    Tags = tags_index.Add(tags),
                    Weight = 100
                }, null);
            tags = new Dictionary<string, string>();
            tags["highway"] = highway;
            tags["name"] = name;
            var vertex_name1 = data.AddVertex((float) from_name.Latitude, (float) from_name.Longitude);
            var vertex_name2 = data.AddVertex((float) to_name.Latitude, (float) to_name.Longitude);
            data.AddArc(vertex_name1, vertex_name2, new SimpleWeighedEdge
                {
                    IsForward = true,
                    Tags = tags_index.Add(tags),
                    Weight = 100
                }, null);

            IRoutingInterpreter interpreter = new OsmRoutingInterpreter();

            // creates the data.
            IBasicRouter<SimpleWeighedEdge> router = new DykstraRoutingLive(
                data.TagsIndex);

            var noname_location = new GeoCoordinate(
                (from_noname.Latitude + to_noname.Latitude) / 2.0,
                (from_noname.Longitude + to_noname.Longitude) / 2.0);
            var name_location = new GeoCoordinate(
                (from_name.Latitude + to_name.Latitude) / 2.0,
                (from_name.Longitude + to_name.Longitude) / 2.0);

            var delta = 0.01f;
            var result = router.SearchClosest(data, interpreter, vehicle, noname_location, delta, matcher, point_tags);
            if (result.Distance < double.MaxValue)
            {
                // there is a result.
                Assert.IsFalse(not_found, "A result was found but was supposed not to  be found!");

                if (name == point_name)
                {
                    // the name location was supposed to be found!
                    Assert.IsTrue(result.Vertex1 == vertex_name1 || result.Vertex1 == vertex_name2);
                    Assert.IsTrue(result.Vertex2 == vertex_name1 || result.Vertex2 == vertex_name2);
                }
                else
                {
                    // the noname location was supposed to be found!
                    Assert.IsTrue(result.Vertex1 == vertex_noname1 || result.Vertex1 == vertex_noname2);
                    Assert.IsTrue(result.Vertex2 == vertex_noname1 || result.Vertex2 == vertex_noname2);
                }
                return;
            }
            Assert.IsTrue(not_found, "A result was not found but was supposed to be found!");
        }
    }
}