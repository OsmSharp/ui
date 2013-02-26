using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Core.Graph.DynamicGraph;
using System.Reflection;
using System.IO;
using OsmSharp.Osm.Routing.Interpreter;
using OsmSharp.Routing.Core.Router;
using OsmSharp.Routing.Core.Graph.Router;
using OsmSharp.Routing.Core;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Core.Interpreter;

namespace OsmSharp.Osm.Routing.Test.Instructions
{
    internal abstract class InstructionTests<EdgeData>
        where EdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Executes some general random query performance evaluation(s).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="test_count"></param>
        public void ExecuteTest(string name, int test_count)
        {
            string xml_embedded = string.Format("OsmSharp.Osm.Routing.Test.TestData.{0}.osm", name);

            this.ExecuteTest(name, Assembly.GetExecutingAssembly().GetManifestResourceStream(xml_embedded), false, test_count);
        }

        /// <summary>
        /// Executes some general random query performance evaluation(s).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data_stream"></param>
        /// <param name="pbf"></param>
        /// <param name="test_count"></param>
        public void ExecuteTest(string name, Stream data_stream, bool pbf, int test_count)
        {
            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Test {0} -> {1}x", name, test_count);

            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(data_stream, pbf,
                interpreter, null);

            // build router;
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<RouterPoint> router = this.BuildRouter(data, interpreter, basic_router);

            // generate random route pairs.
            List<KeyValuePair<RouterPoint, RouterPoint>> test_pairs =
                new List<KeyValuePair<RouterPoint, RouterPoint>>(test_count);
            while (test_pairs.Count < test_count)
            {
                uint first = (uint)OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(
                    (int)data.VertexCount - 1) + 1;
                uint second = (uint)OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(
                    (int)data.VertexCount - 1) + 1;

                float latitude_first, longitude_first;
                data.GetVertex(first, out latitude_first, out longitude_first);
                RouterPoint first_resolved = router.Resolve(VehicleEnum.Car,
                    new GeoCoordinate(latitude_first, longitude_first));

                float latitude_second, longitude_second;
                data.GetVertex(second, out latitude_second, out longitude_second);
                RouterPoint second_resolved = router.Resolve(VehicleEnum.Car,
                    new GeoCoordinate(latitude_second, longitude_second));


                if (((second_resolved != null) &&
                    (first_resolved != null)) &&
                    (router.CheckConnectivity(VehicleEnum.Car, first_resolved, 30) &&
                    router.CheckConnectivity(VehicleEnum.Car, second_resolved, 30)))
                {
                    test_pairs.Add(new KeyValuePair<RouterPoint, RouterPoint>(
                        first_resolved, second_resolved));
                }

                OsmSharp.Tools.Core.Output.OutputStreamHost.ReportProgress(test_pairs.Count, test_count, "Osm.Routing.Test.Point2Point.Point2PointTest<EdgeData>.Execute",
                    "Building pairs list...");
            }

            foreach (KeyValuePair<RouterPoint, RouterPoint> pair in test_pairs)
            {
                OsmSharp.Routing.Core.Route.OsmSharpRoute route = router.Calculate(VehicleEnum.Car, pair.Key, pair.Value);
                if (route != null)
                {
                    OsmSharp.Routing.Instructions.InstructionGenerator generator = new OsmSharp.Routing.Instructions.InstructionGenerator();
                    List<OsmSharp.Routing.Instructions.Instruction> instructions = 
                        generator.Generate(route, interpreter);
                }
            }
        }

        #region Abstract Router Building Functions

        /// <summary>
        /// Builds a router.
        /// </summary>
        /// <returns></returns>
        public abstract IRouter<RouterPoint> BuildRouter(IBasicRouterDataSource<EdgeData> data,
            IRoutingInterpreter interpreter, IBasicRouter<EdgeData> router_basic);

        /// <summary>
        /// Builds data source.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        public abstract IBasicRouterDataSource<EdgeData> BuildData(Stream data, bool pbf, IRoutingInterpreter interpreter, GeoCoordinateBox box);

        /// <summary>
        /// Builds a basic router.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract IBasicRouter<EdgeData> BuildBasicRouter(IBasicRouterDataSource<EdgeData> data);

        #endregion
    }
}
