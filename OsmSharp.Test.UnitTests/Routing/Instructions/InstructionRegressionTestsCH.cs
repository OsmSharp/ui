// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using System.Reflection;
using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;
using OsmSharp.Routing.Osm.Interpreter;

namespace OsmSharp.Test.Unittests.Routing.Instructions
{
    /// <summary>
    /// Holds regression tests based on dykstra routing live.
    /// </summary>
    [TestFixture]
    public class InstructionRegressionTestsCH : InstructionRegressionTestsBase
    {
        /// <summary>
        /// Creates a router.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="manifestResourceName"></param>
        /// <returns></returns>
        protected override Router CreateRouter(IOsmRoutingInterpreter interpreter, string manifestResourceName)
        {
            SimpleTagsIndex tagsIndex = new SimpleTagsIndex();

            // do the data processing.
            var data = new DynamicGraphRouterDataSource<CHEdgeData>(tagsIndex);
            var targetData = new CHEdgeGraphOsmStreamTarget(
                data, interpreter, data.TagsIndex, Vehicle.Car);
            var dataProcessorSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(manifestResourceName));
            var sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            // do the pre-processing part.
            var witnessCalculator = new DykstraWitnessCalculator(data);
            var preProcessor = new CHPreProcessor(data,
                new EdgeDifference(data, witnessCalculator), witnessCalculator);
            preProcessor.Start();

            //IBasicRouter<LiveEdge> basicRouter = new DykstraRoutingLive(memoryData.TagsIndex);
            return Router.CreateCHFrom(data, new CHRouter(data), interpreter);
        }

        /// <summary>
        /// Issue with generation instructions but where streetnames seem to be stripped.
        /// Some streetnames are missing from the instructions.
        /// </summary>
        [Test]
        public void InstructionRegressionCHTest1()
        {
            this.DoInstructionRegressionTest1();
        }
    }
}