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

using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Streams;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Interpreter.Roads;
using OsmSharp.Routing.Osm.Interpreter;

namespace OsmSharp.Routing.Osm.Streams.Graphs
{
    /// <summary>
    /// A pre-processing target for OSM-data to a CH data structure.
    /// </summary>
    public class CHEdgeGraphOsmStreamTarget : DynamicGraphOsmStreamWriter<CHEdgeData>
    {
        /// <summary>
        /// Holds the vehicle profile this pre-processing target is for.
        /// </summary>
        private readonly Vehicle _vehicle;

        /// <summary>
        /// Creates a CH data processing target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="vehicle"></param>
        public CHEdgeGraphOsmStreamTarget(IDynamicGraphRouterDataSource<CHEdgeData> dynamicGraph,
            IOsmRoutingInterpreter interpreter, ITagsCollectionIndex tagsIndex, Vehicle vehicle)
            :base(dynamicGraph, interpreter, new CHEdgeDataComparer(), tagsIndex)
        {
            _vehicle = vehicle;
        }
        
        /// <summary>
        /// Initializes the processing.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            base.DynamicGraph.AddSupportedProfile(_vehicle);
        }

        /// <summary>
        /// Calculates edge data.
        /// </summary>
        /// <param name="edgeInterpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="tags"></param>
        /// <param name="directionForward"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        protected override CHEdgeData CalculateEdgeData(IEdgeInterpreter edgeInterpreter, ITagsCollectionIndex tagsIndex, 
            TagsCollectionBase tags, bool directionForward, GeoCoordinate from, GeoCoordinate to)
        {
            double weight = _vehicle.Weight(tags, from, to);
            bool? direction = _vehicle.IsOneWay(tags);
            bool forward = false;
            bool backward = false;
            if (!direction.HasValue)
            { // both directions.
                forward = true;
                backward = true;
            }
            else
            { // define back/forward.
                forward = (directionForward && direction.Value) ||
                    (!directionForward && !direction.Value);
                backward = (directionForward && !direction.Value) ||
                    (!directionForward && direction.Value);
            }

            // initialize the edge data.
            CHEdgeData edgeData = new CHEdgeData()
            {
                Weight = (float)weight,
                Tags = tagsIndex.Add(
                tags),
                ContractedVertexId = 0
            };
            edgeData.SetDirection(forward, backward, true);
            return edgeData;
        }

        /// <summary>
        /// Returns true if the edge is traversable.
        /// </summary>
        /// <param name="edgeInterpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        protected override bool CalculateIsTraversable(IEdgeInterpreter edgeInterpreter, 
            ITagsCollectionIndex tagsIndex, TagsCollectionBase tags)
        {
            return _vehicle.CanTraverse(tags);
        }

        #region Static Processing Functions

        /// <summary>
        /// Preprocesses the data from the given OsmStreamReader and converts it directly to a routable data source.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="keepDirectNeighbours"></param>
        /// <returns></returns>
        public static DynamicGraphRouterDataSource<CHEdgeData> Preprocess(OsmStreamSource reader,
            ITagsCollectionIndex tagsIndex, IOsmRoutingInterpreter interpreter, Vehicle vehicle, bool keepDirectNeighbours)
        {
            // pull in the data.
            var dynamicGraphRouterDataSource =
                new DynamicGraphRouterDataSource<CHEdgeData>(tagsIndex);
            var targetData = new CHEdgeGraphOsmStreamTarget(
                dynamicGraphRouterDataSource, interpreter, tagsIndex, vehicle);
            targetData.RegisterSource(reader);
            targetData.Pull();

            // compress the graph.
            INodeWitnessCalculator witnessCalculator = new DykstraWitnessCalculator();
            var edgeDifference = new EdgeDifference(
                    dynamicGraphRouterDataSource, witnessCalculator);
            var preProcessor = new CHPreProcessor(
                dynamicGraphRouterDataSource, edgeDifference, witnessCalculator, keepDirectNeighbours);
            preProcessor.Start();

            return dynamicGraphRouterDataSource;
        }
        
        /// <summary>
        /// Preprocesses the data from the given OsmStreamReader and converts it directly to a routable data source.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public static DynamicGraphRouterDataSource<CHEdgeData> Preprocess(OsmStreamSource reader,
                                                                        ITagsCollectionIndex tagsIndex,
                                                                        IOsmRoutingInterpreter interpreter,
                                                                        Vehicle vehicle)
        {
            return CHEdgeGraphOsmStreamTarget.Preprocess(reader, tagsIndex, interpreter, vehicle, true);
        }

        /// <summary>
        /// Preprocesses the data from the given OsmStreamReader and converts it directly to a routable data source.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public static DynamicGraphRouterDataSource<CHEdgeData> Preprocess(OsmStreamSource reader,
                                                                        IOsmRoutingInterpreter interpreter,
                                                                        Vehicle vehicle)
        {
            return CHEdgeGraphOsmStreamTarget.Preprocess(reader, new TagsTableCollectionIndex(), interpreter, vehicle);
        }

        #endregion
    }
}