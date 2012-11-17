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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Osm.Data.XML.Raw.Processor;
using System.Reflection;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Core;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Osm.Core;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using OsmSharp.Routing.CH.Routing;
using OsmSharp.Routing.Core.Graph.Memory;

namespace OsmSharp.Osm.UnitTests.Routing.CH
{
    /// <summary>
    /// Tests the CH Sparse routing against a reference implementation.
    /// </summary>
    [TestClass]
    public class CHSparseComparisonTests : RoutingComparisonTests
    {
        /// <summary>
        /// Returns a new router.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="embedded_name"></param>
        /// <returns></returns>
        public override IRouter<RouterPoint> BuildRouter(IRoutingInterpreter interpreter, string embedded_name)
        {
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<CHEdgeData> data =
                new MemoryRouterDataSource<CHEdgeData>(tags_index);
            CHEdgeDataGraphProcessingTarget target_data = new CHEdgeDataGraphProcessingTarget(
                data, interpreter, data.TagsIndex);
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(
                "OsmSharp.UnitTests.{0}", embedded_name)));
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            // do the pre-processing part.
            CHPreProcessor pre_processor = new CHPreProcessor(data,
                new SparseOrdering(data), new DykstraWitnessCalculator(data));
            pre_processor.Start();

            return new Router<CHEdgeData>(data, interpreter, new CHRouter(
                data));
        }

        /// <summary>
        /// Compares all routes possible against a reference implementation.
        /// </summary>
        [TestMethod]
        public void TestCHSparseAgainstReference()
        {
            this.TestCompareAll("test_network.osm");
        }

        ///// <summary>
        ///// Compares all routes possible against a reference implementation.
        ///// </summary>
        //[TestMethod]
        //public void TestCHSparseAgainstReferenceRealNetwork()
        //{
        //    this.TestCompareAll("test_network_real1.osm");
        //}
    }
}
