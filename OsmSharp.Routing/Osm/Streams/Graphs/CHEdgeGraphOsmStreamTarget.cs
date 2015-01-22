// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using OsmSharp.Collections;
using OsmSharp.Collections.Coordinates;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Osm.Streams;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Interpreter.Roads;
using OsmSharp.Routing.Osm.Interpreter;
using System;
using System.Collections.Generic;

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
            : base(dynamicGraph, interpreter, tagsIndex)
        {
            if (!dynamicGraph.IsDirected) { throw new ArgumentOutOfRangeException("Only directed graphs can be used for contraction hiearchies."); }

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
        /// <param name="tagsForward"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="intermediates"></param>
        /// <returns></returns>
        protected override CHEdgeData CalculateEdgeData(IEdgeInterpreter edgeInterpreter, ITagsCollectionIndex tagsIndex,
            TagsCollectionBase tags, bool tagsForward, GeoCoordinate from, GeoCoordinate to, List<GeoCoordinateSimple> intermediates)
        {
            var direction = _vehicle.IsOneWay(tags);
            var forward = false;
            var backward = false;
            if (!direction.HasValue)
            { // both directions.
                forward = true;
                backward = true;
            }
            else
            { // define back/forward.
                if (tagsForward)
                { // relatively same direction.
                    forward = direction.Value;
                    backward = !direction.Value;
                }
                else
                { // relatively opposite direction.
                    forward = !direction.Value;
                    backward = direction.Value;
                }
            }

            // add tags.
            var tagsId = tagsIndex.Add(tags);

            // calculate weight including intermediates.
            float weight = 0;
            var previous = from;
            if (intermediates != null)
            {
                for (int idx = 0; idx < intermediates.Count; idx++)
                {
                    var current = new GeoCoordinate(intermediates[idx].Latitude, intermediates[idx].Longitude);
                    weight = weight + (float)_vehicle.Weight(tags, previous, current);
                    previous = current;
                }
            }
            weight = weight + (float)_vehicle.Weight(tags, previous, to);

            // initialize the edge data.
            return new CHEdgeData(tagsId, tagsForward, forward, backward, weight);
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
        /// <returns></returns>
        public static DynamicGraphRouterDataSource<CHEdgeData> Preprocess(OsmStreamSource reader,
            ITagsCollectionIndex tagsIndex, IOsmRoutingInterpreter interpreter, Vehicle vehicle)
        {
            // pull in the data.
            var graph = new DynamicGraphRouterDataSource<CHEdgeData>(new MemoryDirectedGraph<CHEdgeData>(), tagsIndex);
            var targetData = new CHEdgeGraphOsmStreamTarget(
                graph, interpreter, tagsIndex, vehicle);
            targetData.RegisterSource(reader);
            targetData.Pull();

            // compress the graph.
            var witnessCalculator = new DykstraWitnessCalculator();
            var edgeDifference = new EdgeDifferenceContractedSearchSpace(graph, witnessCalculator);
            var preProcessor = new CHPreProcessor(graph, edgeDifference, witnessCalculator);
            preProcessor.Start();

            return graph;
        }

        /// <summary>
        /// Preprocesses the data from the given OsmStreamReader and converts it directly to a routable data source.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public static DynamicGraphRouterDataSource<CHEdgeData> Preprocess(OsmStreamSource reader,
            IOsmRoutingInterpreter interpreter, Vehicle vehicle)
        {
            return CHEdgeGraphOsmStreamTarget.Preprocess(reader, new TagsTableCollectionIndex(), interpreter, vehicle);
        }

        #endregion
    }
}