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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OsmSharp.Routing;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Constraints;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Route;
using System.Reflection;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Constraints.Cars;
using OsmSharp.Routing.Router;
using OsmSharp.Routing.Osm.Data;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.DynamicGraph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Tools.Math;

namespace OsmSharp.Osm.UnitTests.Routing
{
    /// <summary>
    /// Base class with tests around IRouter objects.
    /// </summary>
    public abstract class SimpleRoutingTests<ResolvedType, EdgeData>
        where EdgeData : IDynamicGraphEdgeData
        where ResolvedType : IRouterPoint
    {
        /// <summary>
        /// Builds the router;
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <param name="basic_router"></param>
        /// <returns></returns>
        public abstract IRouter<ResolvedType> BuildRouter(IBasicRouterDataSource<EdgeData> data,
            IRoutingInterpreter interpreter, IBasicRouter<EdgeData> basic_router);

        /// <summary>
        /// Builds the basic router.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract IBasicRouter<EdgeData> BuildBasicRouter(IBasicRouterDataSource<EdgeData> data);

        /// <summary>
        /// Builds the data.
        /// </summary>
        /// <returns></returns>
        public abstract IBasicRouterDataSource<EdgeData> BuildData(IRoutingInterpreter interpreter, string embedded_string);

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortestDefault()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578532, 3.7192229));
            ResolvedType target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(5, route.Entries.Length);

            float latitude, longitude;
            data.GetVertex(20, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[0].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[0].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Start, route.Entries[0].Type);

            data.GetVertex(21, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[1].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[1].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Along, route.Entries[1].Type);

            data.GetVertex(16, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[2].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[2].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Along, route.Entries[2].Type);

            data.GetVertex(22, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[3].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[3].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Along, route.Entries[3].Type);

            data.GetVertex(23, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[4].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[4].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Stop, route.Entries[4].Type);
        }

        /// <summary>
        /// Tests that a router preserves tags given to resolved points.
        /// </summary>
        protected void DoTestResolvedTags()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578532, 3.7192229));
            source.Tags.Add(new KeyValuePair<string, string>("name", "source"));
            ResolvedType target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));
            target.Tags.Add(new KeyValuePair<string, string>("name", "target"));

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(5, route.Entries.Length);

            float latitude, longitude;
            data.GetVertex(20, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[0].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[0].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Start, route.Entries[0].Type);
            Assert.IsNotNull(route.Entries[0].Points[0].Tags);
            Assert.AreEqual(1, route.Entries[0].Points[0].Tags.Length);
            Assert.AreEqual("source", route.Entries[0].Points[0].Tags[0].Value);

            data.GetVertex(23, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[4].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[4].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Stop, route.Entries[4].Type);
            Assert.IsNotNull(route.Entries[4].Points[0].Tags);
            Assert.AreEqual(1, route.Entries[4].Points[0].Tags.Length);
            Assert.AreEqual("target", route.Entries[4].Points[0].Tags[0].Value);
        }

        /// <summary>
        /// Tests that a router preserves tags that are located on ways/arcs in the route.
        /// </summary>
        protected void DoTestArcTags()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578532, 3.7192229));
            source.Tags.Add(new KeyValuePair<string, string>("name", "source"));
            ResolvedType target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));
            target.Tags.Add(new KeyValuePair<string, string>("name", "target"));

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(5, route.Entries.Length);

            Assert.AreEqual("highway", route.Entries[1].Tags[0].Key);
            Assert.AreEqual("residential", route.Entries[1].Tags[0].Value);

            Assert.AreEqual("highway", route.Entries[2].Tags[0].Key);
            Assert.AreEqual("residential", route.Entries[2].Tags[0].Value);

            Assert.AreEqual("highway", route.Entries[3].Tags[0].Key);
            Assert.AreEqual("residential", route.Entries[3].Tags[0].Value);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortest1()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578532, 3.7192229));
            ResolvedType target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0579235, 3.7199811));

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(6, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortest2()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0579235, 3.7199811));
            ResolvedType target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578532, 3.7192229));

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(6, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortest3()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));
            ResolvedType target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0579235, 3.7199811));

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(6, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortest4()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0579235, 3.7199811));
            ResolvedType target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(6, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortest5()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));
            ResolvedType target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0581001, 3.7200612));

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(7, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortestResolved1()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578153, 3.7193937));
            ResolvedType target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0582408, 3.7194636));

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(10, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortestResolved2()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0581843, 3.7201209)); // between 2 - 3
            ResolvedType target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0581484, 3.7194957)); // between 9 - 8

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(5, route.Entries.Length);
        }

        /// <summary>
        /// Tests if the many-to-many weights are the same as the point-to-point weights.
        /// </summary>
        protected void DoTestManyToMany1()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType[] resolved_points = new ResolvedType[3];
            resolved_points[0] = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578532, 3.7192229));
            resolved_points[1] = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));
            resolved_points[2] = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0581001, 3.7200612));

            double[][] weights = router.CalculateManyToManyWeight(VehicleEnum.Car, resolved_points, resolved_points);

            for (int x = 0; x < weights.Length; x++)
            {
                for (int y = 0; y < weights.Length; y++)
                {
                    double many_to_many = weights[x][y];
                    double point_to_point = router.CalculateWeight(VehicleEnum.Car, resolved_points[x], resolved_points[y]);

                    Assert.AreEqual(point_to_point, many_to_many);
                }
            }
        }

        /// <summary>
        /// Test if the connectivity test succeed/fail.
        /// </summary>
        protected void DoTestConnectivity1()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType[] resolved_points = new ResolvedType[3];
            resolved_points[0] = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578532, 3.7192229));
            resolved_points[1] = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));
            resolved_points[2] = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0581001, 3.7200612));

            // test connectivity succes.
            Assert.IsTrue(router.CheckConnectivity(VehicleEnum.Car, resolved_points[0], 5));
            //Assert.IsTrue(router.CheckConnectivity(VehicleEnum.Car, resolved_points[1], 5));
            Assert.IsTrue(router.CheckConnectivity(VehicleEnum.Car, resolved_points[2], 5));

            // test connectivity failiure.
            Assert.IsFalse(router.CheckConnectivity(VehicleEnum.Car, resolved_points[0], 1000));
            Assert.IsFalse(router.CheckConnectivity(VehicleEnum.Car, resolved_points[1], 1000));
            Assert.IsFalse(router.CheckConnectivity(VehicleEnum.Car, resolved_points[2], 1000));
        }

        /// <summary>
        /// Test if the resolving of nodes returns those same nodes.
        /// 
        /// (does not work on a lazy loading data source!)
        /// </summary>
        protected void DoTestResolveAllNodes()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            for (int idx = 1; idx < data.VertexCount; idx++)
            {
                float latitude, longitude;
                if (data.GetVertex((uint)idx, out latitude, out longitude))
                {
                    ResolvedType point = router.Resolve(VehicleEnum.Car, new GeoCoordinate(latitude, longitude));
                    Assert.AreEqual(idx, (point as RouterPoint).Id);
                }
            }
        }

        /// <summary>
        /// Test resolving all nodes.
        /// </summary>
        protected void DoTestResolveBetweenNodes()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);

           float delta = 0.001f;
            SearchClosestResult result;
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0578761, 3.7193972), delta, null, null); //,-103,  -4,  -8
            Assert.IsTrue((result.Vertex1 == 20 && result.Vertex2 == 21) ||
                (result.Vertex1 == 21 && result.Vertex2 == 20));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0576510, 3.7194124), delta, null, null); //,-104, -14, -12
            Assert.IsTrue((result.Vertex1 == 22 && result.Vertex2 == 23) ||
                (result.Vertex1 == 23 && result.Vertex2 == 22));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0576829, 3.7196791), delta, null, null); //,-105, -12, -10
            Assert.IsTrue((result.Vertex1 == 22 && result.Vertex2 == 16) ||
                (result.Vertex1 == 16 && result.Vertex2 == 22));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0577819, 3.7196308), delta, null, null); //,-106, -10,  -8
            Assert.IsTrue((result.Vertex1 == 21 && result.Vertex2 == 16) ||
                (result.Vertex1 == 16 && result.Vertex2 == 21));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0577516, 3.7198975), delta, null, null); //,-107, -10, -18
            Assert.IsTrue((result.Vertex1 == 17 && result.Vertex2 == 16) ||
                (result.Vertex1 == 16 && result.Vertex2 == 17));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0578218, 3.7200626), delta, null, null); //,-108, -18, -20
            Assert.IsTrue((result.Vertex1 == 17 && result.Vertex2 == 7) ||
                (result.Vertex1 == 7 && result.Vertex2 == 17));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0578170, 3.7202480), delta, null, null); //,-109, -20, -76
            Assert.IsTrue((result.Vertex1 == 6 && result.Vertex2 == 7) ||
                (result.Vertex1 == 7 && result.Vertex2 == 6));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0577580, 3.7204004), delta, null, null); //,-110, -76, -74
            Assert.IsTrue((result.Vertex1 == 5 && result.Vertex2 == 6) ||
                (result.Vertex1 == 6 && result.Vertex2 == 5));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0579032, 3.7204258), delta, null, null); //,-111, -74, -72
            Assert.IsTrue((result.Vertex1 == 1 && result.Vertex2 == 5) ||
                (result.Vertex1 == 5 && result.Vertex2 == 1));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0580453, 3.7204614), delta, null, null); //,-112, -72, -70
            Assert.IsTrue((result.Vertex1 == 4 && result.Vertex2 == 1) ||
                (result.Vertex1 == 1 && result.Vertex2 == 4));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0581938, 3.7203953), delta, null, null); //,-113, -70, -68
            Assert.IsTrue((result.Vertex1 == 3 && result.Vertex2 == 4) ||
                (result.Vertex1 == 4 && result.Vertex2 == 3));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0581826, 3.7201413), delta, null, null); //,-114, -46, -68
            Assert.IsTrue((result.Vertex1 == 3 && result.Vertex2 == 2) ||
                (result.Vertex1 == 2 && result.Vertex2 == 3));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0580310, 3.7201998), delta, null, null); //,-115, -46, -72
            Assert.IsTrue((result.Vertex1 == 2 && result.Vertex2 == 1) ||
                (result.Vertex1 == 1 && result.Vertex2 == 2));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0579208, 3.7200525), delta, null, null); //,-116, -20, -22
            Assert.IsTrue((result.Vertex1 == 11 && result.Vertex2 == 7) ||
                (result.Vertex1 == 7 && result.Vertex2 == 11));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0580134, 3.7199966), delta, null, null); //,-117, -46, -22
            Assert.IsTrue((result.Vertex1 == 2 && result.Vertex2 == 11) ||
                (result.Vertex1 == 11 && result.Vertex2 == 2));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0581251, 3.7198950), delta, null, null); //,-118, -46, -48
            Assert.IsTrue((result.Vertex1 == 18 && result.Vertex2 == 2) ||
                (result.Vertex1 == 2 && result.Vertex2 == 18));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0579479, 3.7197985), delta, null, null); //,-119, -22, -56
            Assert.IsTrue((result.Vertex1 == 10 && result.Vertex2 == 11) ||
                (result.Vertex1 == 11 && result.Vertex2 == 10));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0580166, 3.7195496), delta, null, null); //,-120, -56, -65
            Assert.IsTrue((result.Vertex1 == 10 && result.Vertex2 == 9) ||
                (result.Vertex1 == 9 && result.Vertex2 == 10));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0581299, 3.7195673), delta, null, null); //,-121, -65, -50
            Assert.IsTrue((result.Vertex1 == 8 && result.Vertex2 == 9) ||
                (result.Vertex1 == 9 && result.Vertex2 == 8));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0581651, 3.7196664), delta, null, null); //,-122, -50, -48
            Assert.IsTrue((result.Vertex1 == 8 && result.Vertex2 == 18) ||
                (result.Vertex1 == 18 && result.Vertex2 == 8));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0582050, 3.7194505), delta, null, null); //,-123, -50, -52
            Assert.IsTrue((result.Vertex1 == 19 && result.Vertex2 == 8) ||
                (result.Vertex1 == 8 && result.Vertex2 == 19));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0582082, 3.7191330), delta, null, null); //,-124, -52, -54
            Assert.IsTrue((result.Vertex1 == 15 && result.Vertex2 == 19) ||
                (result.Vertex1 == 19 && result.Vertex2 == 15));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0581651, 3.7189628), delta, null, null); //,-125, -54, -62
            Assert.IsTrue((result.Vertex1 == 15 && result.Vertex2 == 14) ||
                (result.Vertex1 == 14 && result.Vertex2 == 15));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0580725, 3.7189781), delta, null, null); //,-126, -62, -60
            Assert.IsTrue((result.Vertex1 == 14 && result.Vertex2 == 13) ||
                (result.Vertex1 == 13 && result.Vertex2 == 14));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0580006, 3.7191305), delta, null, null); //,-127, -60, -58
            Assert.IsTrue((result.Vertex1 == 13 && result.Vertex2 == 12) ||
                (result.Vertex1 == 12 && result.Vertex2 == 13));
            result = basic_router.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0579783, 3.7194149), delta, null, null); //,-128, -58, -56
            Assert.IsTrue((result.Vertex1 == 10 && result.Vertex2 == 12) ||
                (result.Vertex1 == 12 && result.Vertex2 == 10));
        }

        /// <summary>
        /// Test if routes from a resolved node to itself is correctly calculated.
        /// 
        /// Regression Test: Routing to self with a resolved node returns a route to the nearest real node and back.
        /// </summary>
        protected void DoTestResolveBetweenRouteToSelf()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(data, interpreter, basic_router);
            
            // first test a non-between node.
            ResolvedType resolved = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));
            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, resolved, resolved);
            Assert.AreEqual(1, route.Entries.Length);
            Assert.AreEqual(0, route.TotalDistance);
            Assert.AreEqual(0, route.TotalTime);

            resolved = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578761, 3.7193972)); //,-103,  -4,  -8
            route = router.Calculate(VehicleEnum.Car, resolved, resolved);
            Assert.AreEqual(1, route.Entries.Length);
            Assert.AreEqual(0, route.TotalDistance);
            Assert.AreEqual(0, route.TotalTime);


            resolved = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576510, 3.7194124)); //,-104, -14, -12
            route = router.Calculate(VehicleEnum.Car, resolved, resolved);
            Assert.AreEqual(1, route.Entries.Length);
            Assert.AreEqual(0, route.TotalDistance);
            Assert.AreEqual(0, route.TotalTime);

            resolved = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576829, 3.7196791)); //,-105, -12, -10
            route = router.Calculate(VehicleEnum.Car, resolved, resolved);
            Assert.AreEqual(1, route.Entries.Length);
            Assert.AreEqual(0, route.TotalDistance);
            Assert.AreEqual(0, route.TotalTime);
        }

        /// <summary>
        /// Test if routes between two resolved nodes are correctly calculated.
        /// </summary>
        protected void DoTestResolveBetweenClose()
        {
            // initialize data.
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");

            GeoCoordinate vertex_20 = new GeoCoordinate(51.0578532, 3.7192229);
            GeoCoordinate vertex_21 = new GeoCoordinate(51.0578518, 3.7195654);
            GeoCoordinate vertex_16 = new GeoCoordinate(51.0577299, 3.719745);

            for (double position1 = 0.1; position1 < 0.91; position1 = position1 + 0.1)
            {
                PointF2D point = vertex_20 + ((vertex_21 - vertex_20) * position1);
                GeoCoordinate vertex_20_21 = new GeoCoordinate(point[1], point[0]);
                for (double position2 = 0.1; position2 < 0.91; position2 = position2 + 0.1)
                {
                    point = vertex_21 + ((vertex_16 - vertex_21) * position2);
                    GeoCoordinate vertex_21_16 = new GeoCoordinate(point[1], point[0]);

                    // calculate route.
                    IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
                    IRouter<ResolvedType> router = this.BuildRouter(data, interpreter, basic_router);

                    OsmSharpRoute route = router.Calculate(VehicleEnum.Car, 
                        router.Resolve(VehicleEnum.Car, vertex_20_21),
                        router.Resolve(VehicleEnum.Car, vertex_21_16));

                    Assert.AreEqual(3, route.Entries.Length);
                    Assert.AreEqual(vertex_20_21.Latitude, route.Entries[0].Latitude, 0.0001);
                    Assert.AreEqual(vertex_20_21.Longitude, route.Entries[0].Longitude, 0.0001);

                    Assert.AreEqual(vertex_21.Latitude, route.Entries[1].Latitude, 0.0001);
                    Assert.AreEqual(vertex_21.Longitude, route.Entries[1].Longitude, 0.0001);

                    Assert.AreEqual(vertex_21_16.Latitude, route.Entries[2].Latitude, 0.0001);
                    Assert.AreEqual(vertex_21_16.Longitude, route.Entries[2].Longitude, 0.0001);
                }
            }
        }

        /// <summary>
        /// Test if routes between two resolved nodes are correctly calculated.
        /// </summary>
        protected void DoTestResolveBetweenTwo()
        {
            // initialize data.
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");

            GeoCoordinate vertex_20 = new GeoCoordinate(51.0578532, 3.7192229);
            GeoCoordinate vertex_21 = new GeoCoordinate(51.0578518, 3.7195654);

            for (double position1 = 0.1; position1 < 0.91; position1 = position1 + 0.1)
            {
                PointF2D point = vertex_20 + ((vertex_21 - vertex_20) * position1);
                GeoCoordinate vertex_20_21 = new GeoCoordinate(point[1], point[0]);

                point = vertex_21 + ((vertex_20 - vertex_21) * position1);
                GeoCoordinate vertex_21_20 = new GeoCoordinate(point[1], point[0]);

                // calculate route.
                IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
                IRouter<ResolvedType> router = this.BuildRouter(data, interpreter, basic_router);

                OsmSharpRoute route = router.Calculate(VehicleEnum.Car,
                    router.Resolve(VehicleEnum.Car, vertex_20_21),
                    router.Resolve(VehicleEnum.Car, vertex_21_20));

                if (vertex_20_21.Latitude != vertex_21_20.Latitude &&
                    vertex_20_21.Longitude != vertex_21_20.Longitude)
                {
                    Assert.AreEqual(2, route.Entries.Length);
                    Assert.AreEqual(vertex_20_21.Latitude, route.Entries[0].Latitude, 0.0001);
                    Assert.AreEqual(vertex_20_21.Longitude, route.Entries[0].Longitude, 0.0001);

                    Assert.AreEqual(vertex_21_20.Latitude, route.Entries[1].Latitude, 0.0001);
                    Assert.AreEqual(vertex_21_20.Longitude, route.Entries[1].Longitude, 0.0001);
                }
            }
        }

        /// <summary>
        /// Test if routes between resolved nodes are correctly calculated.
        /// 
        /// 20----x----21----x----16
        /// </summary>
        protected void DoTestResolveCase1()
        {
            // initialize data.
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");

            GeoCoordinate vertex_20 = new GeoCoordinate(51.0578532, 3.7192229);
            GeoCoordinate vertex_21 = new GeoCoordinate(51.0578518, 3.7195654);
            GeoCoordinate vertex_16 = new GeoCoordinate(51.0577299, 3.719745);

            PointF2D point = vertex_20 + ((vertex_21 - vertex_20) * 0.5);
            GeoCoordinate vertex_20_21 = new GeoCoordinate(point[1], point[0]);

            point = vertex_21 + ((vertex_16 - vertex_21) * 0.5);
            GeoCoordinate vertex_21_16 = new GeoCoordinate(point[1], point[0]);

            // calculate route.
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(data, interpreter, basic_router);

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car,
                router.Resolve(VehicleEnum.Car, vertex_20_21),
                router.Resolve(VehicleEnum.Car, vertex_21_16));

            Assert.AreEqual(3, route.Entries.Length);
            Assert.AreEqual(vertex_20_21.Latitude, route.Entries[0].Latitude, 0.0001);
            Assert.AreEqual(vertex_20_21.Longitude, route.Entries[0].Longitude, 0.0001);

            Assert.AreEqual(vertex_21.Latitude, route.Entries[1].Latitude, 0.0001);
            Assert.AreEqual(vertex_21.Longitude, route.Entries[1].Longitude, 0.0001);

            Assert.AreEqual(vertex_21_16.Latitude, route.Entries[2].Latitude, 0.0001);
            Assert.AreEqual(vertex_21_16.Longitude, route.Entries[2].Longitude, 0.0001);
        }

        /// <summary>
        /// Test if routes between resolved nodes are correctly calculated.
        /// 
        /// 20--x---x--21---------16
        /// </summary>
        protected void DoTestResolveCase2()
        {
            // initialize data.
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");

            GeoCoordinate vertex_20 = new GeoCoordinate(51.0578532, 3.7192229);
            GeoCoordinate vertex_21 = new GeoCoordinate(51.0578518, 3.7195654);
            GeoCoordinate vertex_16 = new GeoCoordinate(51.0577299, 3.719745);

            PointF2D point = vertex_20 + ((vertex_21 - vertex_20) * 0.25);
            GeoCoordinate vertex_20_21_1 = new GeoCoordinate(point[1], point[0]);

            point = vertex_20 + ((vertex_21 - vertex_20) * 0.75);
            GeoCoordinate vertex_20_21_2 = new GeoCoordinate(point[1], point[0]);

            // calculate route.
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(data, interpreter, basic_router);

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car,
                router.Resolve(VehicleEnum.Car, vertex_20_21_1),
                router.Resolve(VehicleEnum.Car, vertex_20_21_2));

            Assert.AreEqual(2, route.Entries.Length);
            Assert.AreEqual(vertex_20_21_1.Latitude, route.Entries[0].Latitude, 0.0001);
            Assert.AreEqual(vertex_20_21_1.Longitude, route.Entries[0].Longitude, 0.0001);

            //Assert.AreEqual(vertex_21.Latitude, route.Entries[1].Latitude, 0.0001);
            //Assert.AreEqual(vertex_21.Longitude, route.Entries[1].Longitude, 0.0001);

            Assert.AreEqual(vertex_20_21_2.Latitude, route.Entries[1].Latitude, 0.0001);
            Assert.AreEqual(vertex_20_21_2.Longitude, route.Entries[1].Longitude, 0.0001);
        }

        /// <summary>
        /// Resolves coordinates at the same locations and checks tag preservation.
        /// </summary>
        protected void DoTestResolveSameLocation()
        {
            //var vertex_20 = new GeoCoordinate(51.0578532, 3.7192229);
            //var vertex_21 = new GeoCoordinate(51.0578518, 3.7195654);
            var vertex_16 = new GeoCoordinate(51.0577299, 3.719745);

            // initialize data.
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");

            // create router.
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(data, interpreter, basic_router);

            // define test tags.
            var tags1 = new Dictionary<string, string>();
            tags1.Add("test1", "yes");
            var tags2 = new Dictionary<string, string>();
            tags2.Add("test2", "yes");

            // resolve points.
            ResolvedType point1 = router.Resolve(VehicleEnum.Car, vertex_16);
            point1.Tags.Add(new KeyValuePair<string, string>("test1","yes"));

            // test presence of tags.
            Assert.AreEqual(1, point1.Tags.Count);
            Assert.AreEqual("test1", point1.Tags[0].Key);
            Assert.AreEqual("yes", point1.Tags[0].Value);

            // resolve point again.
            ResolvedType point2 = router.Resolve(VehicleEnum.Car, vertex_16);

            // the tags should be here still!
            Assert.AreEqual(1, point2.Tags.Count);
            Assert.AreEqual("test1", point2.Tags[0].Key);
            Assert.AreEqual("yes", point2.Tags[0].Value);

        }
    }
}