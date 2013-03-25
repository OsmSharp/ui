using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing;
using System.IO;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Osm;
using OsmSharp.Routing.Graph.DynamicGraph.SimpleWeighed;
using OsmSharp.Routing.Osm.Data.Processing;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Osm.Data.Core.Processor;
using OsmSharp.Osm.Data.PBF.Raw.Processor;
using OsmSharp.Osm.Data.XML.Processor;

namespace OsmSharp.Routing.Osm.Test.ManyToMany
{
    /// <summary>
    /// Does some tests on the many to many calculations of the dykstra live variant.
    /// </summary>
    public class ManyToManyCustomDykstraLiveTests : ManyToManyCustomTests
    {
        private DynamicGraphRouterDataSource<SimpleWeighedEdge> _osm_data;

        /// <summary>
        /// Creates a router.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        protected override IRouter<RouterPoint> CreateRouter(Stream data,
            IRoutingInterpreter interpreter, bool pbf)
        {
            if (_osm_data == null)
            {
                OsmTagsIndex tags_index = new OsmTagsIndex();
                //OsmTagsIndex tags_index = null;

                // do the data processing.
                //_osm_data =
                //    new DynamicGraphRouterDataSource<SimpleWeighedEdge>(
                //        tags_index);
                _osm_data =
                    new DynamicGraphRouterDataSource<SimpleWeighedEdge>(
                        new MemoryDynamicGraphSimpleWeighed(),
                        tags_index);
                //_osm_data =
                //    new DynamicGraphRouterDataSource<SimpleWeighedEdge>(
                //        new OsmSharp.Routing.Graph.DynamicGraph.Memory.MemoryDynamicGraphIncidenceArray<SimpleWeighedEdge>(),
                //        tags_index);
                SimpleWeighedDataGraphProcessingTarget target_data = new SimpleWeighedDataGraphProcessingTarget(
                    _osm_data, interpreter, _osm_data.TagsIndex, VehicleEnum.Car);
                DataProcessorSource source;
                if (pbf)
                {
                    source = new PBFDataProcessorSource(data);
                }
                else
                {
                    source = new XmlDataProcessorSource(data);
                }

                DataProcessorFilterSort sorter = new DataProcessorFilterSort();
                sorter.RegisterSource(source);

                // report progress.
                source = new OsmSharp.Osm.Data.Core.Processor.Progress.ProgressDataProcessorSource(sorter);

                //target_data.RegisterSource(sorter);
                target_data.RegisterSource(source);
                target_data.Pull();
            }

            return new Router<SimpleWeighedEdge>(_osm_data, interpreter,
                new DykstraRoutingLive(_osm_data.TagsIndex));
        }
    }
}
