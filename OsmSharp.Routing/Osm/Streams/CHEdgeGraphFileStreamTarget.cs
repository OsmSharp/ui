//// OsmSharp - OpenStreetMap (OSM) SDK
//// Copyright (C) 2015 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

//using OsmSharp.Collections.Tags;
//using OsmSharp.Collections.Tags.Index;
//using OsmSharp.Routing.CH.PreProcessing;
//using OsmSharp.Routing.CH.PreProcessing.Ordering;
//using OsmSharp.Routing.CH.PreProcessing.Witnesses;
//using OsmSharp.Routing.CH.Serialization.Sorted;
//using OsmSharp.Routing.Graph;
//using OsmSharp.Routing.Osm.Interpreter;
//using OsmSharp.Routing.Osm.Streams.Graphs;
//using System;
//using System.IO;

//namespace OsmSharp.Routing.Osm.Streams
//{
//    /// <summary>
//    /// Implements a streaming target that converts the given OSM-data into a serialized graph.
//    /// </summary>
//    public class CHEdgeGraphFileStreamTarget : CHEdgeGraphOsmStreamTarget
//    {
//        /// <summary>
//        /// Holds the graph output stream.
//        /// </summary>
//        private Stream _graphStream;

//        /// <summary>
//        /// Creates a new target.
//        /// </summary>
//        /// <param name="stream"></param>
//        /// <param name="dynamicGraph"></param>
//        /// <param name="interpreter"></param>
//        /// <param name="tagsIndex"></param>
//        /// <param name="vehicle"></param>
//        public CHEdgeGraphFileStreamTarget(Stream stream, RouterDataSource<CHEdgeData> dynamicGraph,
//            IOsmRoutingInterpreter interpreter, ITagsCollectionIndex tagsIndex, Vehicle vehicle)
//            :base(dynamicGraph, interpreter, tagsIndex, vehicle)
//        {
//            _graph = dynamicGraph;
//            _graphStream = stream;
//        }
        
//        /// <summary>
//        /// Holds the graph.
//        /// </summary>
//        private RouterDataSource<CHEdgeData> _graph;

//        /// <summary>
//        /// Flushes all data.
//        /// </summary>
//        public override void Flush()
//        {
//            base.Flush();            
            
//            // compress the graph.
//            INodeWitnessCalculator witnessCalculator = new DykstraWitnessCalculator();
//            var edgeDifference = new EdgeDifferenceContractedSearchSpace(
//                _graph, witnessCalculator);
//            var preProcessor = new CHPreProcessor(
//                _graph, edgeDifference, witnessCalculator);
//            preProcessor.Start();

//            // create tags.
//            TagsCollectionBase metaTags = new TagsCollection();

//            // create serializer.
//            var routingSerializer = new CHEdgeDataDataSourceSerializer();
//            routingSerializer.Serialize(_graphStream, _graph, metaTags);
//            _graphStream.Flush();
//        }
//    }
//}