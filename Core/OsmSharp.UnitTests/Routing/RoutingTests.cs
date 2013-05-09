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

using System.Collections.Generic;
using NUnit.Framework;
using OsmSharp.Routing;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Route;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Tools.Math;
using OsmSharp.Routing.Routers;

namespace OsmSharp.UnitTests.Routing
{
    /// <summary>
    /// Base class with tests around the Router object.
    /// </summary>
    public abstract class SimpleRoutingTests<TEdgeData>
        where TEdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Builds the router.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <param name="basicRouter"></param>
        /// <returns></returns>
        public abstract Router BuildRouter(IBasicRouterDataSource<TEdgeData> data,
            IRoutingInterpreter interpreter, IBasicRouter<TEdgeData> basicRouter);

        /// <summary>
        /// Builds the basic router.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract IBasicRouter<TEdgeData> BuildBasicRouter(IBasicRouterDataSource<TEdgeData> data);

        /// <summary>
        /// Builds the data.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="embeddedString"></param>
        /// <returns></returns>
        public abstract IBasicRouterDataSource<TEdgeData> BuildData(IRoutingInterpreter interpreter, 
            string embeddedString);

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortestDefault()
        {
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(
                data, interpreter, basicRouter);
            RouterPoint source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578532, 3.7192229));
            RouterPoint target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));

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
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(
                data, interpreter, basicRouter);
            RouterPoint source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578532, 3.7192229));
            source.Tags.Add(new KeyValuePair<string, string>("name", "source"));
            RouterPoint target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));
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
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(
                data, interpreter, basicRouter);
            RouterPoint source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578532, 3.7192229));
            source.Tags.Add(new KeyValuePair<string, string>("name", "source"));
            RouterPoint target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));
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
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(
                data, interpreter, basicRouter);
            RouterPoint source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578532, 3.7192229));
            RouterPoint target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0579235, 3.7199811));

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(6, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortest2()
        {
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(
                data, interpreter, basicRouter);
            RouterPoint source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0579235, 3.7199811));
            RouterPoint target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578532, 3.7192229));

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(6, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortest3()
        {
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(
                data, interpreter, basicRouter);
            RouterPoint source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));
            RouterPoint target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0579235, 3.7199811));

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(6, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortest4()
        {
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(
                data, interpreter, basicRouter);
            RouterPoint source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0579235, 3.7199811));
            RouterPoint target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(6, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortest5()
        {
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<TEdgeData> basic_router = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(
                data, interpreter, basic_router);
            RouterPoint source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));
            RouterPoint target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0581001, 3.7200612));

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(7, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortestResolved1()
        {
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(
                data, interpreter, basicRouter);
            RouterPoint source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578153, 3.7193937));
            RouterPoint target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0582408, 3.7194636));

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(10, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortestResolved2()
        {
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(
                data, interpreter, basicRouter);
            RouterPoint source = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0581843, 3.7201209)); // between 2 - 3
            RouterPoint target = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0581484, 3.7194957)); // between 9 - 8

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(5, route.Entries.Length);
        }

        /// <summary>
        /// Tests if the many-to-many weights are the same as the point-to-point weights.
        /// </summary>
        protected void DoTestManyToMany1()
        {
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(
                data, interpreter, basicRouter);
            var resolvedPoints = new RouterPoint[3];
            resolvedPoints[0] = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578532, 3.7192229));
            resolvedPoints[1] = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));
            resolvedPoints[2] = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0581001, 3.7200612));

            double[][] weights = router.CalculateManyToManyWeight(VehicleEnum.Car, resolvedPoints, resolvedPoints);

            for (int x = 0; x < weights.Length; x++)
            {
                for (int y = 0; y < weights.Length; y++)
                {
                    double manyToMany = weights[x][y];
                    double pointToPoint = router.CalculateWeight(VehicleEnum.Car, resolvedPoints[x], resolvedPoints[y]);

                    Assert.AreEqual(pointToPoint, manyToMany);
                }
            }
        }

        /// <summary>
        /// Test if the connectivity test succeed/fail.
        /// </summary>
        protected void DoTestConnectivity1()
        {
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(
                data, interpreter, basicRouter);
            var resolvedPoints = new RouterPoint[3];
            resolvedPoints[0] = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0578532, 3.7192229));
            resolvedPoints[1] = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));
            resolvedPoints[2] = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0581001, 3.7200612));

            // test connectivity succes.
            Assert.IsTrue(router.CheckConnectivity(VehicleEnum.Car, resolvedPoints[0], 5));
            //Assert.IsTrue(router.CheckConnectivity(VehicleEnum.Car, resolved_points[1], 5));
            Assert.IsTrue(router.CheckConnectivity(VehicleEnum.Car, resolvedPoints[2], 5));

            // test connectivity failiure.
            Assert.IsFalse(router.CheckConnectivity(VehicleEnum.Car, resolvedPoints[0], 1000));
            Assert.IsFalse(router.CheckConnectivity(VehicleEnum.Car, resolvedPoints[1], 1000));
            Assert.IsFalse(router.CheckConnectivity(VehicleEnum.Car, resolvedPoints[2], 1000));
        }

        /// <summary>
        /// Test if the resolving of nodes returns those same nodes.
        /// 
        /// (does not work on a lazy loading data source!)
        /// </summary>
        protected void DoTestResolveAllNodes()
        {
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(
                data, interpreter, basicRouter);
            for (int idx = 1; idx < data.VertexCount; idx++)
            {
                float latitude, longitude;
                if (data.GetVertex((uint)idx, out latitude, out longitude))
                {
                    RouterPoint point = router.Resolve(VehicleEnum.Car, new GeoCoordinate(latitude, longitude));
                    Assert.AreEqual(idx, (point as RouterPoint).Id);
                }
            }
        }

        /// <summary>
        /// Test resolving all nodes.
        /// </summary>
        protected void DoTestResolveBetweenNodes()
        {
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);

           const float delta = 0.001f;
            SearchClosestResult result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0578761, 3.7193972), delta, null, null);
            Assert.IsTrue((result.Vertex1 == 20 && result.Vertex2 == 21) ||
                (result.Vertex1 == 21 && result.Vertex2 == 20));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0576510, 3.7194124), delta, null, null); //,-104, -14, -12
            Assert.IsTrue((result.Vertex1 == 22 && result.Vertex2 == 23) ||
                (result.Vertex1 == 23 && result.Vertex2 == 22));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0576829, 3.7196791), delta, null, null); //,-105, -12, -10
            Assert.IsTrue((result.Vertex1 == 22 && result.Vertex2 == 16) ||
                (result.Vertex1 == 16 && result.Vertex2 == 22));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0577819, 3.7196308), delta, null, null); //,-106, -10,  -8
            Assert.IsTrue((result.Vertex1 == 21 && result.Vertex2 == 16) ||
                (result.Vertex1 == 16 && result.Vertex2 == 21));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0577516, 3.7198975), delta, null, null); //,-107, -10, -18
            Assert.IsTrue((result.Vertex1 == 17 && result.Vertex2 == 16) ||
                (result.Vertex1 == 16 && result.Vertex2 == 17));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0578218, 3.7200626), delta, null, null); //,-108, -18, -20
            Assert.IsTrue((result.Vertex1 == 17 && result.Vertex2 == 7) ||
                (result.Vertex1 == 7 && result.Vertex2 == 17));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0578170, 3.7202480), delta, null, null); //,-109, -20, -76
            Assert.IsTrue((result.Vertex1 == 6 && result.Vertex2 == 7) ||
                (result.Vertex1 == 7 && result.Vertex2 == 6));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0577580, 3.7204004), delta, null, null); //,-110, -76, -74
            Assert.IsTrue((result.Vertex1 == 5 && result.Vertex2 == 6) ||
                (result.Vertex1 == 6 && result.Vertex2 == 5));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0579032, 3.7204258), delta, null, null); //,-111, -74, -72
            Assert.IsTrue((result.Vertex1 == 1 && result.Vertex2 == 5) ||
                (result.Vertex1 == 5 && result.Vertex2 == 1));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0580453, 3.7204614), delta, null, null); //,-112, -72, -70
            Assert.IsTrue((result.Vertex1 == 4 && result.Vertex2 == 1) ||
                (result.Vertex1 == 1 && result.Vertex2 == 4));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0581938, 3.7203953), delta, null, null); //,-113, -70, -68
            Assert.IsTrue((result.Vertex1 == 3 && result.Vertex2 == 4) ||
                (result.Vertex1 == 4 && result.Vertex2 == 3));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0581826, 3.7201413), delta, null, null); //,-114, -46, -68
            Assert.IsTrue((result.Vertex1 == 3 && result.Vertex2 == 2) ||
                (result.Vertex1 == 2 && result.Vertex2 == 3));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0580310, 3.7201998), delta, null, null); //,-115, -46, -72
            Assert.IsTrue((result.Vertex1 == 2 && result.Vertex2 == 1) ||
                (result.Vertex1 == 1 && result.Vertex2 == 2));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0579208, 3.7200525), delta, null, null); //,-116, -20, -22
            Assert.IsTrue((result.Vertex1 == 11 && result.Vertex2 == 7) ||
                (result.Vertex1 == 7 && result.Vertex2 == 11));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0580134, 3.7199966), delta, null, null); //,-117, -46, -22
            Assert.IsTrue((result.Vertex1 == 2 && result.Vertex2 == 11) ||
                (result.Vertex1 == 11 && result.Vertex2 == 2));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0581251, 3.7198950), delta, null, null); //,-118, -46, -48
            Assert.IsTrue((result.Vertex1 == 18 && result.Vertex2 == 2) ||
                (result.Vertex1 == 2 && result.Vertex2 == 18));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0579479, 3.7197985), delta, null, null); //,-119, -22, -56
            Assert.IsTrue((result.Vertex1 == 10 && result.Vertex2 == 11) ||
                (result.Vertex1 == 11 && result.Vertex2 == 10));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0580166, 3.7195496), delta, null, null); //,-120, -56, -65
            Assert.IsTrue((result.Vertex1 == 10 && result.Vertex2 == 9) ||
                (result.Vertex1 == 9 && result.Vertex2 == 10));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0581299, 3.7195673), delta, null, null); //,-121, -65, -50
            Assert.IsTrue((result.Vertex1 == 8 && result.Vertex2 == 9) ||
                (result.Vertex1 == 9 && result.Vertex2 == 8));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0581651, 3.7196664), delta, null, null); //,-122, -50, -48
            Assert.IsTrue((result.Vertex1 == 8 && result.Vertex2 == 18) ||
                (result.Vertex1 == 18 && result.Vertex2 == 8));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0582050, 3.7194505), delta, null, null); //,-123, -50, -52
            Assert.IsTrue((result.Vertex1 == 19 && result.Vertex2 == 8) ||
                (result.Vertex1 == 8 && result.Vertex2 == 19));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0582082, 3.7191330), delta, null, null); //,-124, -52, -54
            Assert.IsTrue((result.Vertex1 == 15 && result.Vertex2 == 19) ||
                (result.Vertex1 == 19 && result.Vertex2 == 15));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0581651, 3.7189628), delta, null, null); //,-125, -54, -62
            Assert.IsTrue((result.Vertex1 == 15 && result.Vertex2 == 14) ||
                (result.Vertex1 == 14 && result.Vertex2 == 15));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0580725, 3.7189781), delta, null, null); //,-126, -62, -60
            Assert.IsTrue((result.Vertex1 == 14 && result.Vertex2 == 13) ||
                (result.Vertex1 == 13 && result.Vertex2 == 14));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0580006, 3.7191305), delta, null, null); //,-127, -60, -58
            Assert.IsTrue((result.Vertex1 == 13 && result.Vertex2 == 12) ||
                (result.Vertex1 == 12 && result.Vertex2 == 13));
            result = basicRouter.SearchClosest(data, interpreter, VehicleEnum.Car, new GeoCoordinate(51.0579783, 3.7194149), delta, null, null); //,-128, -58, -56
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
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(data, interpreter, basicRouter);
            
            // first test a non-between node.
            RouterPoint resolved = router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.0576193, 3.7191801));
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
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");

            var vertex20 = new GeoCoordinate(51.0578532, 3.7192229);
            var vertex21 = new GeoCoordinate(51.0578518, 3.7195654);
            var vertex16 = new GeoCoordinate(51.0577299, 3.719745);

            for (double position1 = 0.1; position1 < 0.91; position1 = position1 + 0.1)
            {
                PointF2D point = vertex20 + ((vertex21 - vertex20) * position1);
                var vertex2021 = new GeoCoordinate(point[1], point[0]);
                for (double position2 = 0.1; position2 < 0.91; position2 = position2 + 0.1)
                {
                    point = vertex21 + ((vertex16 - vertex21) * position2);
                    var vertex2116 = new GeoCoordinate(point[1], point[0]);

                    // calculate route.
                    IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
                    Router router = this.BuildRouter(data, interpreter, basicRouter);

                    OsmSharpRoute route = router.Calculate(VehicleEnum.Car, 
                        router.Resolve(VehicleEnum.Car, vertex2021),
                        router.Resolve(VehicleEnum.Car, vertex2116));

                    Assert.AreEqual(3, route.Entries.Length);
                    Assert.AreEqual(vertex2021.Latitude, route.Entries[0].Latitude, 0.0001);
                    Assert.AreEqual(vertex2021.Longitude, route.Entries[0].Longitude, 0.0001);

                    Assert.AreEqual(vertex21.Latitude, route.Entries[1].Latitude, 0.0001);
                    Assert.AreEqual(vertex21.Longitude, route.Entries[1].Longitude, 0.0001);

                    Assert.AreEqual(vertex2116.Latitude, route.Entries[2].Latitude, 0.0001);
                    Assert.AreEqual(vertex2116.Longitude, route.Entries[2].Longitude, 0.0001);
                }
            }
        }

        /// <summary>
        /// Test if routes between two resolved nodes are correctly calculated.
        /// </summary>
        protected void DoTestResolveBetweenTwo()
        {
            // initialize data.
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");

            var vertex20 = new GeoCoordinate(51.0578532, 3.7192229);
            var vertex21 = new GeoCoordinate(51.0578518, 3.7195654);

            for (double position1 = 0.1; position1 < 0.91; position1 = position1 + 0.1)
            {
                PointF2D point = vertex20 + ((vertex21 - vertex20) * position1);
                var vertex2021 = new GeoCoordinate(point[1], point[0]);

                point = vertex21 + ((vertex20 - vertex21) * position1);
                var vertex2120 = new GeoCoordinate(point[1], point[0]);

                // calculate route.
                IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
                Router router = this.BuildRouter(data, interpreter, basicRouter);

                OsmSharpRoute route = router.Calculate(VehicleEnum.Car,
                    router.Resolve(VehicleEnum.Car, vertex2021),
                    router.Resolve(VehicleEnum.Car, vertex2120));

                if (vertex2021.Latitude != vertex2120.Latitude &&
                    vertex2021.Longitude != vertex2120.Longitude)
                {
                    Assert.AreEqual(2, route.Entries.Length);
                    Assert.AreEqual(vertex2021.Latitude, route.Entries[0].Latitude, 0.0001);
                    Assert.AreEqual(vertex2021.Longitude, route.Entries[0].Longitude, 0.0001);

                    Assert.AreEqual(vertex2120.Latitude, route.Entries[1].Latitude, 0.0001);
                    Assert.AreEqual(vertex2120.Longitude, route.Entries[1].Longitude, 0.0001);
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
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");

            var vertex20 = new GeoCoordinate(51.0578532, 3.7192229);
            var vertex21 = new GeoCoordinate(51.0578518, 3.7195654);
            var vertex16 = new GeoCoordinate(51.0577299, 3.719745);

            PointF2D point = vertex20 + ((vertex21 - vertex20) * 0.5);
            var vertex2021 = new GeoCoordinate(point[1], point[0]);

            point = vertex21 + ((vertex16 - vertex21) * 0.5);
            var vertex2116 = new GeoCoordinate(point[1], point[0]);

            // calculate route.
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(data, interpreter, basicRouter);

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car,
                router.Resolve(VehicleEnum.Car, vertex2021),
                router.Resolve(VehicleEnum.Car, vertex2116));

            Assert.AreEqual(3, route.Entries.Length);
            Assert.AreEqual(vertex2021.Latitude, route.Entries[0].Latitude, 0.0001);
            Assert.AreEqual(vertex2021.Longitude, route.Entries[0].Longitude, 0.0001);

            Assert.AreEqual(vertex21.Latitude, route.Entries[1].Latitude, 0.0001);
            Assert.AreEqual(vertex21.Longitude, route.Entries[1].Longitude, 0.0001);

            Assert.AreEqual(vertex2116.Latitude, route.Entries[2].Latitude, 0.0001);
            Assert.AreEqual(vertex2116.Longitude, route.Entries[2].Longitude, 0.0001);
        }

        /// <summary>
        /// Test if routes between resolved nodes are correctly calculated.
        /// 
        /// 20--x---x--21---------16
        /// </summary>
        protected void DoTestResolveCase2()
        {
            // initialize data.
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");

            var vertex20 = new GeoCoordinate(51.0578532, 3.7192229);
            var vertex21 = new GeoCoordinate(51.0578518, 3.7195654);
            var vertex16 = new GeoCoordinate(51.0577299, 3.719745);

            PointF2D point = vertex20 + ((vertex21 - vertex20) * 0.25);
            var vertex20211 = new GeoCoordinate(point[1], point[0]);

            point = vertex20 + ((vertex21 - vertex20) * 0.75);
            var vertex20212 = new GeoCoordinate(point[1], point[0]);

            // calculate route.
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(data, interpreter, basicRouter);

            OsmSharpRoute route = router.Calculate(VehicleEnum.Car,
                router.Resolve(VehicleEnum.Car, vertex20211),
                router.Resolve(VehicleEnum.Car, vertex20212));

            Assert.AreEqual(2, route.Entries.Length);
            Assert.AreEqual(vertex20211.Latitude, route.Entries[0].Latitude, 0.0001);
            Assert.AreEqual(vertex20211.Longitude, route.Entries[0].Longitude, 0.0001);

            //Assert.AreEqual(vertex_21.Latitude, route.Entries[1].Latitude, 0.0001);
            //Assert.AreEqual(vertex_21.Longitude, route.Entries[1].Longitude, 0.0001);

            Assert.AreEqual(vertex20212.Latitude, route.Entries[1].Latitude, 0.0001);
            Assert.AreEqual(vertex20212.Longitude, route.Entries[1].Longitude, 0.0001);
        }

        /// <summary>
        /// Resolves coordinates at the same locations and checks tag preservation.
        /// </summary>
        protected void DoTestResolveSameLocation()
        {
            var vertex16 = new GeoCoordinate(51.0577299, 3.719745);

            // initialize data.
            var interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<TEdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_network.osm");

            // create router.
            IBasicRouter<TEdgeData> basicRouter = this.BuildBasicRouter(data);
            Router router = this.BuildRouter(data, interpreter, basicRouter);

            // define test tags.
            var tags1 = new Dictionary<string, string>();
            tags1.Add("test1", "yes");
            var tags2 = new Dictionary<string, string>();
            tags2.Add("test2", "yes");

            // resolve points.
            RouterPoint point1 = router.Resolve(VehicleEnum.Car, vertex16);
            point1.Tags.Add(new KeyValuePair<string, string>("test1","yes"));

            // test presence of tags.
            Assert.AreEqual(1, point1.Tags.Count);
            Assert.AreEqual("test1", point1.Tags[0].Key);
            Assert.AreEqual("yes", point1.Tags[0].Value);

            // resolve point again.
            RouterPoint point2 = router.Resolve(VehicleEnum.Car, vertex16);

            // the tags should be here still!
            Assert.AreEqual(1, point2.Tags.Count);
            Assert.AreEqual("test1", point2.Tags[0].Key);
            Assert.AreEqual("yes", point2.Tags[0].Value);
        }
    }
}