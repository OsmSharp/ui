using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using OsmSharp.Tools.Math.Geo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using OsmSharp.Routing.Core;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Routing.Core.Constraints;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Routing.Core.Route;
using OsmSharp.Osm.Core;
using OsmSharp.Osm.Routing.Data;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Osm.Data.XML.Raw.Processor;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Core.Router;

namespace OsmSharp.Osm.UnitTests.Routing
{
    /// <summary>
    /// Base class with tests around IRouter objects.
    /// </summary>
    public abstract class RoutingComparisonTests
    {
        /// <summary>
        /// Returns a router test object.
        /// </summary>
        /// <returns></returns>
        public abstract IRouter<RouterPoint> BuildRouter(IRoutingInterpreter interpreter, string embedded_name);

        /// <summary>
        /// Builds a raw data source.
        /// </summary>
        /// <returns></returns>
        public MemoryRouterDataSource<OsmSharp.Osm.Routing.Data.OsmEdgeData> BuildRawDataSource(
            IRoutingInterpreter interpreter, string embedded_name)
        {
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<OsmEdgeData> data =
                new MemoryRouterDataSource<OsmEdgeData>(tags_index);
            OsmEdgeDataGraphProcessingTarget target_data = new OsmEdgeDataGraphProcessingTarget(
                data, interpreter, data.TagsIndex);
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(
                "OsmSharp.UnitTests.{0}", embedded_name)));
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            return data;
        }

        /// <summary>
        /// Builds a raw router to compare against.
        /// </summary>
        /// <returns></returns>
        public IRouter<RouterPoint> BuildRawRouter(IBasicRouterDataSource<OsmEdgeData> data, 
            IRoutingInterpreter interpreter)
        {
            // initialize the router.
            return new Router<OsmEdgeData>(
                    data, interpreter);
        }

        /// <summary>
        /// Compares all routes against the reference router.
        /// </summary>
        public void TestCompareAll(string embedded_name)
        {
            // build the routing settings.
            IRoutingInterpreter interpreter = new OsmSharp.Osm.Routing.Interpreter.OsmRoutingInterpreter();

            // get the osm data source.
            IBasicRouterDataSource<OsmEdgeData> data = this.BuildRawDataSource(interpreter, embedded_name);

            // build the reference router.
            IRouter<RouterPoint> reference_router = this.BuildRawRouter(
                this.BuildRawDataSource(interpreter, embedded_name), interpreter);

            // build the router to be tested.
            IRouter<RouterPoint> router = this.BuildRouter(interpreter, embedded_name);

            // loop over all nodes and resolve their locations.
            RouterPoint[] resolved_reference = new RouterPoint[data.VertexCount - 1];
            RouterPoint[] resolved = new RouterPoint[data.VertexCount - 1];
            for (uint idx = 1; idx < data.VertexCount; idx++)
            { // resolve each vertex.
                float latitude, longitude;
                if(data.GetVertex(idx, out latitude, out longitude))
                {
                    resolved_reference[idx - 1] = reference_router.Resolve(new GeoCoordinate(latitude, longitude));
                    resolved[idx - 1] = router.Resolve(new GeoCoordinate(latitude, longitude));
                }

                Assert.IsNotNull(resolved_reference[idx - 1]);
                Assert.IsNotNull(resolved[idx - 1]);

                Assert.AreEqual(resolved_reference[idx - 1].Location.Latitude,
                    resolved[idx - 1].Location.Latitude, 0.0001);
                Assert.AreEqual(resolved_reference[idx - 1].Location.Longitude,
                    resolved[idx - 1].Location.Longitude, 0.0001);
            }

            // check all the routes having the same weight(s).
            for (int from_idx = 0; from_idx < resolved.Length; from_idx++)
            {
                for (int to_idx = 0; to_idx < resolved.Length; to_idx++)
                {
                    OsmSharpRoute reference_route = reference_router.Calculate(
                        resolved_reference[from_idx], resolved_reference[to_idx]);
                    OsmSharpRoute route = router.Calculate(
                        resolved[from_idx], resolved[to_idx]);

                    Assert.IsNotNull(reference_route);
                    Assert.IsNotNull(route);
                    Assert.AreEqual(reference_route.TotalDistance, route.TotalDistance, 0.0001);
                    //Assert.AreEqual(reference_route.TotalTime, route.TotalTime, 0.0001);
                }
            }
        }
    }
}
