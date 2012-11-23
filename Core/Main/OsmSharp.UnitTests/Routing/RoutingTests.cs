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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Routing.Core.Resolving;
using OsmSharp.Routing.Core;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Routing.Core.Constraints;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Core.Route;
using System.Reflection;
using OsmSharp.Osm.Routing.Interpreter;
using OsmSharp.Routing.Core.Constraints.Cars;
using OsmSharp.Routing.Core.Router;
using OsmSharp.Osm.Routing.Data;
using OsmSharp.Routing.Core.Graph;
using OsmSharp.Routing.Core.Graph.DynamicGraph;
using OsmSharp.Routing.Core.Graph.Router;
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
        public abstract IBasicRouterDataSource<EdgeData> BuildData(IRoutingInterpreter interpreter);

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortestDefault()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(new GeoCoordinate(51.0578532, 3.7192229));
            ResolvedType target = router.Resolve(new GeoCoordinate(51.0576193, 3.7191801));

            OsmSharpRoute route = router.Calculate(source, target);
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
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(new GeoCoordinate(51.0578532, 3.7192229));
            source.Tags.Add(new KeyValuePair<string, string>("name", "source"));
            ResolvedType target = router.Resolve(new GeoCoordinate(51.0576193, 3.7191801));
            target.Tags.Add(new KeyValuePair<string, string>("name", "target"));

            OsmSharpRoute route = router.Calculate(source, target);
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
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(new GeoCoordinate(51.0578532, 3.7192229));
            source.Tags.Add(new KeyValuePair<string, string>("name", "source"));
            ResolvedType target = router.Resolve(new GeoCoordinate(51.0576193, 3.7191801));
            target.Tags.Add(new KeyValuePair<string, string>("name", "target"));

            OsmSharpRoute route = router.Calculate(source, target);
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
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(new GeoCoordinate(51.0578532, 3.7192229));
            ResolvedType target = router.Resolve(new GeoCoordinate(51.0579235, 3.7199811));

            OsmSharpRoute route = router.Calculate(source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(6, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortest2()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(new GeoCoordinate(51.0579235, 3.7199811));
            ResolvedType target = router.Resolve(new GeoCoordinate(51.0578532, 3.7192229));

            OsmSharpRoute route = router.Calculate(source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(6, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortest3()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(new GeoCoordinate(51.0576193, 3.7191801));
            ResolvedType target = router.Resolve(new GeoCoordinate(51.0579235, 3.7199811));

            OsmSharpRoute route = router.Calculate(source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(6, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortest4()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(new GeoCoordinate(51.0579235, 3.7199811));
            ResolvedType target = router.Resolve(new GeoCoordinate(51.0576193, 3.7191801));

            OsmSharpRoute route = router.Calculate(source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(6, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortest5()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(new GeoCoordinate(51.0576193, 3.7191801));
            ResolvedType target = router.Resolve(new GeoCoordinate(51.0581001, 3.7200612));

            OsmSharpRoute route = router.Calculate(source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(7, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortestResolved1()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(new GeoCoordinate(51.0578153, 3.7193937));
            ResolvedType target = router.Resolve(new GeoCoordinate(51.0582408, 3.7194636));

            OsmSharpRoute route = router.Calculate(source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(10, route.Entries.Length);
        }

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortestResolved2()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType source = router.Resolve(new GeoCoordinate(51.0581843, 3.7201209)); // between 2 - 3
            ResolvedType target = router.Resolve(new GeoCoordinate(51.0581484, 3.7194957)); // between 9 - 8

            OsmSharpRoute route = router.Calculate(source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(5, route.Entries.Length);
        }

        /// <summary>
        /// Tests if the many-to-many weights are the same as the point-to-point weights.
        /// </summary>
        protected void DoTestManyToMany1()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType[] resolved_points = new ResolvedType[3];
            resolved_points[0] = router.Resolve(new GeoCoordinate(51.0578532, 3.7192229));
            resolved_points[1] = router.Resolve(new GeoCoordinate(51.0576193, 3.7191801));
            resolved_points[2] = router.Resolve(new GeoCoordinate(51.0581001, 3.7200612));

            double[][] weights = router.CalculateManyToManyWeight(resolved_points, resolved_points);

            for (int x = 0; x < weights.Length; x++)
            {
                for (int y = 0; y < weights.Length; y++)
                {
                    double many_to_many = weights[x][y];
                    double point_to_point = router.CalculateWeight(resolved_points[x], resolved_points[y]);

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
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            ResolvedType[] resolved_points = new ResolvedType[3];
            resolved_points[0] = router.Resolve(new GeoCoordinate(51.0578532, 3.7192229));
            resolved_points[1] = router.Resolve(new GeoCoordinate(51.0576193, 3.7191801));
            resolved_points[2] = router.Resolve(new GeoCoordinate(51.0581001, 3.7200612));

            // test connectivity succes.
            Assert.IsTrue(router.CheckConnectivity(resolved_points[0], 5));
            Assert.IsTrue(router.CheckConnectivity(resolved_points[1], 5));
            Assert.IsTrue(router.CheckConnectivity(resolved_points[2], 5));

            // test connectivity failiure.
            Assert.IsFalse(router.CheckConnectivity(resolved_points[0], 1000));
            Assert.IsFalse(router.CheckConnectivity(resolved_points[1], 1000));
            Assert.IsFalse(router.CheckConnectivity(resolved_points[2], 1000));
        }

        /// <summary>
        /// Test if the resolving of nodes returns those same nodes.
        /// 
        /// (does not work on a lazy loading data source!)
        /// </summary>
        protected void DoTestResolveAllNodes()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);
            for (int idx = 1; idx < data.VertexCount; idx++)
            {
                float latitude, longitude;
                if (data.GetVertex((uint)idx, out latitude, out longitude))
                {
                    ResolvedType point = router.Resolve(new GeoCoordinate(latitude, longitude));
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
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);

            double search_box_size = 0.001f;
            SearchClosestResult result;
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0578761, 3.7193972), null, search_box_size); //,-103,  -4,  -8
            Assert.IsTrue((result.Vertex1 == 20 && result.Vertex2 == 21) ||
                (result.Vertex1 == 21 && result.Vertex2 == 20));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0576510, 3.7194124), null, search_box_size); //,-104, -14, -12
            Assert.IsTrue((result.Vertex1 == 22 && result.Vertex2 == 23) ||
                (result.Vertex1 == 23 && result.Vertex2 == 22));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0576829, 3.7196791), null, search_box_size); //,-105, -12, -10
            Assert.IsTrue((result.Vertex1 == 22 && result.Vertex2 == 16) ||
                (result.Vertex1 == 16 && result.Vertex2 == 22));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0577819, 3.7196308), null, search_box_size); //,-106, -10,  -8
            Assert.IsTrue((result.Vertex1 == 21 && result.Vertex2 == 16) ||
                (result.Vertex1 == 16 && result.Vertex2 == 21));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0577516, 3.7198975), null, search_box_size); //,-107, -10, -18
            Assert.IsTrue((result.Vertex1 == 17 && result.Vertex2 == 16) ||
                (result.Vertex1 == 16 && result.Vertex2 == 17));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0578218, 3.7200626), null, search_box_size); //,-108, -18, -20
            Assert.IsTrue((result.Vertex1 == 17 && result.Vertex2 == 7) ||
                (result.Vertex1 == 7 && result.Vertex2 == 17));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0578170, 3.7202480), null, search_box_size); //,-109, -20, -76
            Assert.IsTrue((result.Vertex1 == 6 && result.Vertex2 == 7) ||
                (result.Vertex1 == 7 && result.Vertex2 == 6));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0577580, 3.7204004), null, search_box_size); //,-110, -76, -74
            Assert.IsTrue((result.Vertex1 == 5 && result.Vertex2 == 6) ||
                (result.Vertex1 == 6 && result.Vertex2 == 5));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0579032, 3.7204258), null, search_box_size); //,-111, -74, -72
            Assert.IsTrue((result.Vertex1 == 1 && result.Vertex2 == 5) ||
                (result.Vertex1 == 5 && result.Vertex2 == 1));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0580453, 3.7204614), null, search_box_size); //,-112, -72, -70
            Assert.IsTrue((result.Vertex1 == 4 && result.Vertex2 == 1) ||
                (result.Vertex1 == 1 && result.Vertex2 == 4));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0581938, 3.7203953), null, search_box_size); //,-113, -70, -68
            Assert.IsTrue((result.Vertex1 == 3 && result.Vertex2 == 4) ||
                (result.Vertex1 == 4 && result.Vertex2 == 3));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0581826, 3.7201413), null, search_box_size); //,-114, -46, -68
            Assert.IsTrue((result.Vertex1 == 3 && result.Vertex2 == 2) ||
                (result.Vertex1 == 2 && result.Vertex2 == 3));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0580310, 3.7201998), null, search_box_size); //,-115, -46, -72
            Assert.IsTrue((result.Vertex1 == 2 && result.Vertex2 == 1) ||
                (result.Vertex1 == 1 && result.Vertex2 == 2));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0579208, 3.7200525), null, search_box_size); //,-116, -20, -22
            Assert.IsTrue((result.Vertex1 == 11 && result.Vertex2 == 7) ||
                (result.Vertex1 == 7 && result.Vertex2 == 11));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0580134, 3.7199966), null, search_box_size); //,-117, -46, -22
            Assert.IsTrue((result.Vertex1 == 2 && result.Vertex2 == 11) ||
                (result.Vertex1 == 11 && result.Vertex2 == 2));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0581251, 3.7198950), null, search_box_size); //,-118, -46, -48
            Assert.IsTrue((result.Vertex1 == 18 && result.Vertex2 == 2) ||
                (result.Vertex1 == 2 && result.Vertex2 == 18));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0579479, 3.7197985), null, search_box_size); //,-119, -22, -56
            Assert.IsTrue((result.Vertex1 == 10 && result.Vertex2 == 11) ||
                (result.Vertex1 == 11 && result.Vertex2 == 10));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0580166, 3.7195496), null, search_box_size); //,-120, -56, -65
            Assert.IsTrue((result.Vertex1 == 10 && result.Vertex2 == 9) ||
                (result.Vertex1 == 9 && result.Vertex2 == 10));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0581299, 3.7195673), null, search_box_size); //,-121, -65, -50
            Assert.IsTrue((result.Vertex1 == 8 && result.Vertex2 == 9) ||
                (result.Vertex1 == 9 && result.Vertex2 == 8));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0581651, 3.7196664), null, search_box_size); //,-122, -50, -48
            Assert.IsTrue((result.Vertex1 == 8 && result.Vertex2 == 18) ||
                (result.Vertex1 == 18 && result.Vertex2 == 8));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0582050, 3.7194505), null, search_box_size); //,-123, -50, -52
            Assert.IsTrue((result.Vertex1 == 19 && result.Vertex2 == 8) ||
                (result.Vertex1 == 8 && result.Vertex2 == 19));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0582082, 3.7191330), null, search_box_size); //,-124, -52, -54
            Assert.IsTrue((result.Vertex1 == 15 && result.Vertex2 == 19) ||
                (result.Vertex1 == 19 && result.Vertex2 == 15));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0581651, 3.7189628), null, search_box_size); //,-125, -54, -62
            Assert.IsTrue((result.Vertex1 == 15 && result.Vertex2 == 14) ||
                (result.Vertex1 == 14 && result.Vertex2 == 15));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0580725, 3.7189781), null, search_box_size); //,-126, -62, -60
            Assert.IsTrue((result.Vertex1 == 14 && result.Vertex2 == 13) ||
                (result.Vertex1 == 13 && result.Vertex2 == 14));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0580006, 3.7191305), null, search_box_size); //,-127, -60, -58
            Assert.IsTrue((result.Vertex1 == 13 && result.Vertex2 == 12) ||
                (result.Vertex1 == 12 && result.Vertex2 == 13));
            result = basic_router.SearchClosest(data, new GeoCoordinate(51.0579783, 3.7194149), null, search_box_size); //,-128, -58, -56
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
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(data, interpreter, basic_router);
            
            // first test a non-between node.
            ResolvedType resolved = router.Resolve(new GeoCoordinate(51.0576193, 3.7191801));
            OsmSharpRoute route = router.Calculate(resolved, resolved);
            Assert.AreEqual(1, route.Entries.Length);
            Assert.AreEqual(0, route.TotalDistance);
            Assert.AreEqual(0, route.TotalTime);

            resolved = router.Resolve(new GeoCoordinate(51.0578761, 3.7193972)); //,-103,  -4,  -8
            route = router.Calculate(resolved, resolved);
            Assert.AreEqual(1, route.Entries.Length);
            Assert.AreEqual(0, route.TotalDistance);
            Assert.AreEqual(0, route.TotalTime);


            resolved = router.Resolve(new GeoCoordinate(51.0576510, 3.7194124)); //,-104, -14, -12
            route = router.Calculate(resolved, resolved);
            Assert.AreEqual(1, route.Entries.Length);
            Assert.AreEqual(0, route.TotalDistance);
            Assert.AreEqual(0, route.TotalTime);

            resolved = router.Resolve(new GeoCoordinate(51.0576829, 3.7196791)); //,-105, -12, -10
            route = router.Calculate(resolved, resolved);
            Assert.AreEqual(1, route.Entries.Length);
            Assert.AreEqual(0, route.TotalDistance);
            Assert.AreEqual(0, route.TotalTime);
        }
    }
}