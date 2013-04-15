using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Routing;
using OsmSharp.Osm;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.DynamicGraph.SimpleWeighed;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Graph.Router;
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
            Dictionary<string, string> edge_tags = new Dictionary<string, string>();
            //edge_tags["highway"] = "footway";

            // create point tags.
            Dictionary<string, string> point_tags = new Dictionary<string, string>();
            //point_tags["highway"] = "footway";

            // test with empty point tags.
            Assert.IsTrue(matcher.MatchWithEdge(VehicleEnum.Car, null, null));
            Assert.IsTrue(matcher.MatchWithEdge(VehicleEnum.Car, point_tags, null));

            // test with empty edge tags.
            point_tags["name"] = "Ben Abelshausen Boulevard";
            Assert.IsFalse(matcher.MatchWithEdge(VehicleEnum.Car, point_tags, null));
            Assert.IsFalse(matcher.MatchWithEdge(VehicleEnum.Car, point_tags, edge_tags));

            // test with matching name.
            edge_tags["name"] = "Ben Abelshausen Boulevard";
            Assert.IsTrue(matcher.MatchWithEdge(VehicleEnum.Car, point_tags, edge_tags));

            // test with none-matching name.
            edge_tags["name"] = "Jorieke Vyncke Boulevard";
            Assert.IsFalse(matcher.MatchWithEdge(VehicleEnum.Car, point_tags, edge_tags));
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
        /// <param name="point_name"></param>
        /// <param name="not_found"></param>
        private void TestResolveOnEdgeSingle(string name, string highway, 
            VehicleEnum vehicle, IEdgeMatcher matcher, 
            string point_name, bool not_found)
        {
            GeoCoordinate from_name = new GeoCoordinate(51.0003, 4.0007);
            GeoCoordinate to_name = new GeoCoordinate(51.0003, 4.0008);

            GeoCoordinate from_noname = new GeoCoordinate(51.0, 4.0007);
            GeoCoordinate to_noname = new GeoCoordinate(51.0, 4.0008);

            Dictionary<string, string> point_tags = new Dictionary<string, string>();
            point_tags["name"] = point_name;

            Dictionary<string, string> tags = new Dictionary<string, string>();
            tags["highway"] = highway;
            //tags["name"] = name;

            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            DynamicGraphRouterDataSource<SimpleWeighedEdge> data =
                new DynamicGraphRouterDataSource<SimpleWeighedEdge>(tags_index);
            uint vertex_noname1 = data.AddVertex((float)from_noname.Latitude, (float)from_noname.Longitude);
            uint vertex_noname2 = data.AddVertex((float)to_noname.Latitude, (float)to_noname.Longitude);
            data.AddArc(vertex_noname1, vertex_noname2, new SimpleWeighedEdge()
            {
                IsForward = true,
                Tags = tags_index.Add(tags),
                Weight = 100
            }, null); 
            tags = new Dictionary<string, string>();
            tags["highway"] = highway;
            tags["name"] = name;
            uint vertex_name1 = data.AddVertex((float)from_name.Latitude, (float)from_name.Longitude);
            uint vertex_name2 = data.AddVertex((float)to_name.Latitude, (float)to_name.Longitude);
            data.AddArc(vertex_name1, vertex_name2, new SimpleWeighedEdge()
            {
                IsForward = true,
                Tags = tags_index.Add(tags),
                Weight = 100
            }, null);

            IRoutingInterpreter interpreter = new OsmRoutingInterpreter();

            // creates the data.
            IBasicRouter<SimpleWeighedEdge> router = new DykstraRoutingLive(
                data.TagsIndex);

            GeoCoordinate noname_location = new GeoCoordinate(
                (from_noname.Latitude + to_noname.Latitude) / 2.0,
                (from_noname.Longitude + to_noname.Longitude) / 2.0);
            GeoCoordinate name_location = new GeoCoordinate(
                (from_name.Latitude + to_name.Latitude) / 2.0,
                (from_name.Longitude + to_name.Longitude) / 2.0);

            float delta = 0.01f;
            SearchClosestResult result = router.SearchClosest(data, interpreter, vehicle, noname_location, delta, matcher, point_tags);
            if (result.Distance < double.MaxValue)
            { // there is a result.
                Assert.IsFalse(not_found, "A result was found but was supposed not to  be found!");

                if (name == point_name)
                { // the name location was supposed to be found!
                    Assert.IsTrue(result.Vertex1 == vertex_name1 || result.Vertex1 == vertex_name2);
                    Assert.IsTrue(result.Vertex2 == vertex_name1 || result.Vertex2 == vertex_name2);
                }
                else
                { // the noname location was supposed to be found!
                    Assert.IsTrue(result.Vertex1 == vertex_noname1 || result.Vertex1 == vertex_noname2);
                    Assert.IsTrue(result.Vertex2 == vertex_noname1 || result.Vertex2 == vertex_noname2);
                }
                return;
            }
            Assert.IsTrue(not_found, "A result was not found but was supposed to be found!");
        }
    }
}
