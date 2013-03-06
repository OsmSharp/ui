using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Core;
using OsmSharp.Routing.Core.Interpreter;
using System.IO;
using OsmSharp.Osm.Core;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.CH.Routing;
using OsmSharp.Osm.Data.Core.Processor;
using OsmSharp.Osm.Data.PBF.Raw.Processor;
using OsmSharp.Osm.Data.XML.Processor;

namespace OsmSharp.Osm.Routing.Test.ManyToMany
{
    /// <summary>
    /// Executes many-to-many calculation performance tests using the CH data format.
    /// </summary>
    public class ManyToManyCustomCHTests : ManyToManyCustomTests
    {
        private MemoryRouterDataSource<CHEdgeData> _data;

        /// <summary>
        /// Creates a raw router.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <param name="constraints"></param>
        /// <returns></returns>
        protected override IRouter<RouterPoint> CreateRouter(
           Stream data_stream, IRoutingInterpreter interpreter, bool pbf)
        {
            if (_data == null)
            {
                OsmTagsIndex tags_index = new OsmTagsIndex();

                // do the data processing.
                _data =
                    new MemoryRouterDataSource<CHEdgeData>(tags_index);
                CHEdgeDataGraphProcessingTarget target_data = new CHEdgeDataGraphProcessingTarget(
                    _data, interpreter, _data.TagsIndex, VehicleEnum.Car);
                DataProcessorSource source;
                if (pbf)
                {
                    source = new PBFDataProcessorSource(data_stream);
                }
                else
                {
                    source = new XmlDataProcessorSource(data_stream);
                }
                DataProcessorFilterSort sorter = new DataProcessorFilterSort();
                sorter.RegisterSource(source);
                target_data.RegisterSource(sorter);
                target_data.Pull();

                // do the pre-processing part.
                INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(_data);
                CHPreProcessor pre_processor = new CHPreProcessor(_data,
                    new EdgeDifference(_data, witness_calculator), witness_calculator);
                pre_processor.Start();
            }
            return new Router<CHEdgeData>(_data, interpreter, new CHRouter(_data));
        }
    }
}
