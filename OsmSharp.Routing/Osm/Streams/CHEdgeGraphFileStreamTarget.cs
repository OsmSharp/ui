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

using System.IO;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Data.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.Serialization.Sorted;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;
using OsmSharp.Osm;

namespace OsmSharp.Routing.Osm.Streams
{
    /// <summary>
    /// Implements a streaming target that converts the given OSM-data into a serialized graph.
    /// </summary>
    public class CHEdgeGraphFileStreamTarget : OsmStreamTarget
    {
        /// <summary>
        /// Holds the vehicle.
        /// </summary>
        private Vehicle _vehicle;

        /// <summary>
        /// Holds the graph output stream.
        /// </summary>
        private Stream _graphStream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="vehicle"></param>
        public CHEdgeGraphFileStreamTarget(Stream stream, Vehicle vehicle)
        {
            _graphStream = stream;

            var tagsIndex = new SimpleTagsIndex();
            var interpreter = new OsmRoutingInterpreter();
            _graph = new DynamicGraphRouterDataSource<CHEdgeData>(tagsIndex);
            _graphTarget = new CHEdgeGraphOsmStreamTarget(
                _graph, interpreter, _graph.TagsIndex, vehicle);
        }

        /// <summary>
        /// Holds the memory data source.
        /// </summary>
        private CHEdgeGraphOsmStreamTarget _graphTarget;
        
        /// <summary>
        /// Holds the graph.
        /// </summary>
        private DynamicGraphRouterDataSource<CHEdgeData> _graph;

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {
            _graphStream.Seek(0, SeekOrigin.Begin);
            _graphTarget.Initialize();
        }

        /// <summary>
        /// Adds a new node.
        /// </summary>
        /// <param name="simpleNode"></param>
        public override void AddNode(Node simpleNode)
        {
            _graphTarget.AddNode(simpleNode);
        }

        /// <summary>
        /// Adds a new way.
        /// </summary>
        /// <param name="simpleWay"></param>
        public override void AddWay(Way simpleWay)
        {
            _graphTarget.AddWay(simpleWay);
        }

        /// <summary>
        /// Adds a new relation.
        /// </summary>
        /// <param name="simpleRelation"></param>
        public override void AddRelation(Relation simpleRelation)
        {
            _graphTarget.AddRelation(simpleRelation);
        }

        /// <summary>
        /// Flushes all data.
        /// </summary>
        public override void Flush()
        {
            base.Flush();            
            
            // compress the graph.
            INodeWitnessCalculator witnessCalculator = new DykstraWitnessCalculator(_graph);
            var edgeDifference = new EdgeDifference(
                _graph, witnessCalculator);
            var preProcessor = new CHPreProcessor(
                _graph, edgeDifference, witnessCalculator);
            preProcessor.Start();

            // create serializer.
            var routingSerializer = new CHEdgeDataDataSourceSerializer(true);
            routingSerializer.Serialize(_graphStream, _graph);
            _graphStream.Flush();
        }
    }
}