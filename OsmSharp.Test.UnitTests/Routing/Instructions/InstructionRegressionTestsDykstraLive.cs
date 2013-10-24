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
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;

namespace OsmSharp.Test.Unittests.Routing.Instructions
{
    /// <summary>
    /// Holds regression tests based on dykstra routing live.
    /// </summary>
    [TestFixture]
    public class InstructionRegressionTestsDykstraLive : InstructionRegressionTestsBase
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
            DynamicGraphRouterDataSource<LiveEdge> memoryData =
                new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            LiveGraphOsmStreamTarget target_data = new LiveGraphOsmStreamTarget(
                memoryData, interpreter, memoryData.TagsIndex);
            XmlOsmStreamSource dataProcessorSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(manifestResourceName));
            OsmStreamFilterSort sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            IBasicRouter<LiveEdge> basicRouter = new DykstraRoutingLive(memoryData.TagsIndex);
            return Router.CreateLiveFrom(memoryData, basicRouter, interpreter);
        }

        /// <summary>
        /// Issue with generating instructions but where streetnames seem to be stripped.
        /// Some streetnames are missing from the instructions.
        /// </summary>
        [Test]
        public void InstructionRegressionDykstraLiveTest1()
        {
            this.DoInstructionRegressionTest1();
        }

        /// <summary>
        /// Issue with generating instructions.
        /// </summary>
        [Test]
        public void InstructionRegressionDykstraLiveTest2()
        {
            this.DoInstructionRegressionTest2();
        }
    }
}