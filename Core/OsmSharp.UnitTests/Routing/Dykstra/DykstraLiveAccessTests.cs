using NUnit.Framework;
using OsmSharp.Osm.Data.Streams.Filters;
using OsmSharp.Osm.Data.Xml.Processor;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using System.Reflection;
using OsmSharp.Routing.Osm.Data.Processing;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Tools.Collections.Tags;

namespace OsmSharp.UnitTests.Routing.Dykstra
{
    /// <summary>
    /// Does some raw routing tests.
    /// </summary>
    [TestFixture]
    public class DykstraLiveAccessTests : RoutingAccessTests<LiveEdge>
    {
        /// <summary>
        /// Builds a router.
        /// </summary>
        /// <returns></returns>
        public override Router BuildRouter(IBasicRouterDataSource<LiveEdge> data, 
            IRoutingInterpreter interpreter,
                IBasicRouter<LiveEdge> basicRouter)
        {
            // initialize the router.
            return Router.CreateLiveFrom(data, basicRouter, interpreter);
        }

        /// <summary>
        /// Builds a basic router.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override IBasicRouter<LiveEdge> BuildBasicRouter(IBasicRouterDataSource<LiveEdge> data)
        {
            return new DykstraRoutingLive(data.TagsIndex);
        }

        /// <summary>
        /// Builds data source.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="embeddedString"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public override IBasicRouterDataSource<LiveEdge> BuildData(IRoutingInterpreter interpreter,
                                                                            string embeddedString, VehicleEnum vehicle)
        {
            var tagsIndex = new SimpleTagsIndex();

            // do the data processing.
            var memoryData =
                new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamWriter(
                memoryData, interpreter, memoryData.TagsIndex);
            var dataProcessorSource = new XmlOsmStreamReader(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedString));
            var sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            return memoryData;
        }

        /// <summary>
        /// Test access restrictions.
        /// </summary>
        [Test]
        public void TestDykstraLiveAccessHighways()
        {
            this.DoAccessTestsHighways();
        }
    }
}