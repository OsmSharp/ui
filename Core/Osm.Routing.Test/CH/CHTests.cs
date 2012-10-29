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

using Osm.Routing.Core;
using Osm.Routing.CH.Routing;
using Osm.Routing.Core.Interpreter;
using Osm.Routing.Core.Constraints;
using Osm.Data.XML.Raw.Processor;
using System.IO;
using Osm.Data.Core.Processor.Filter.Sort;
using Osm.Routing.CH.Processor;
using Osm.Routing.CH.PreProcessing;
using Osm.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using Osm.Routing.CH.PreProcessing.Witnesses;
namespace Osm.Routing.Test.CH
{
    class CHTest
    {
        public static void Test(string name, int test_count)
        {

        }

        /// <summary>
        /// Returns a new router.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="constraints"></param>
        /// <returns></returns>
        public static IRouter<CHResolvedPoint> BuildRouter(string xml, RoutingInterpreterBase interpreter,
            IRoutingConstraints constraints)
        {
            // build the memory data source.
            CHDataSource data = new CHDataSource();

            // load the data.
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(new FileStream(xml, FileMode.Open));
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            CHDataProcessorTarget ch_target = new CHDataProcessorTarget(data);
            ch_target.RegisterSource(sorter);
            ch_target.Pull();

            // do the pre-processing part.
            CHPreProcessor pre_processor = new CHPreProcessor(data.Graph,
                new SparseOrdering(data.Graph), new DykstraWitnessCalculator(data.Graph));
            pre_processor.Start();

            // create the router from the contracted data.
            return new Router(data);
        }
    }
}