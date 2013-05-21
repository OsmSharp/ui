using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Data.Streams;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Interpreter.Roads;
using OsmSharp.Collections.Tags;
using OsmSharp.Math;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing;

namespace OsmSharp.Routing.Osm.Data.Processing
{
    /// <summary>
    /// A pre-processing target for OSM-data to a CH data structure.
    /// </summary>
    public class CHEdgeGraphOsmStreamWriter : DynamicGraphOsmStreamWriter<CHEdgeData>
    {
        /// <summary>
        /// Holds the vehicle profile this pre-processing target is for.
        /// </summary>
        private readonly Vehicle _vehicle;

        /// <summary>
        /// Holds the data source.
        /// </summary>
        private readonly IDynamicGraphRouterDataSource<CHEdgeData> _dynamicDataSource;

        /// <summary>
        /// Creates a CH data processing target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="vehicle"></param>
        public CHEdgeGraphOsmStreamWriter(IDynamicGraphRouterDataSource<CHEdgeData> dynamicGraph,
            IRoutingInterpreter interpreter, ITagsIndex tagsIndex, Vehicle vehicle)
            :base(dynamicGraph, interpreter, new CHEdgeDataComparer(), tagsIndex)
        {
            _vehicle = vehicle;
            _dynamicDataSource = dynamicGraph;
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
        protected override CHEdgeData CalculateEdgeData(IEdgeInterpreter edgeInterpreter, ITagsIndex tagsIndex, 
            TagsCollection tags, bool directionForward, GeoCoordinate from, GeoCoordinate to)
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
            return new CHEdgeData()
            {
                Weight = (float)weight, 
                Forward = forward, 
                Backward = backward, 
                Tags = tagsIndex.Add(
                tags),
                ContractedVertexId = 0
            };
        }

        /// <summary>
        /// Returns true if the edge is traversable.
        /// </summary>
        /// <param name="edgeInterpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        protected override bool CalculateIsTraversable(IEdgeInterpreter edgeInterpreter, 
            ITagsIndex tagsIndex, TagsCollection tags)
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
        public static DynamicGraphRouterDataSource<CHEdgeData> Preprocess(OsmStreamReader reader,
                                                                        ITagsIndex tagsIndex,
                                                                        IRoutingInterpreter interpreter,
                                                                        Vehicle vehicle)
        {
            // pull in the data.
            var dynamicGraphRouterDataSource =
                new DynamicGraphRouterDataSource<CHEdgeData>(tagsIndex);
            var targetData = new CHEdgeGraphOsmStreamWriter(
                dynamicGraphRouterDataSource, interpreter, dynamicGraphRouterDataSource.TagsIndex, vehicle);
            targetData.RegisterSource(reader);
            targetData.Pull();

            // compress the graph.
            INodeWitnessCalculator witnessCalculator = new DykstraWitnessCalculator(dynamicGraphRouterDataSource);
            var edgeDifference = new EdgeDifference(
                dynamicGraphRouterDataSource, witnessCalculator);
            var preProcessor = new CHPreProcessor(
                dynamicGraphRouterDataSource, edgeDifference, witnessCalculator);
            preProcessor.Start();

            return dynamicGraphRouterDataSource;
        }

        /// <summary>
        /// Preprocesses the data from the given OsmStreamReader and converts it directly to a routable data source.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public static DynamicGraphRouterDataSource<CHEdgeData> Preprocess(OsmStreamReader reader,
                                                                        IRoutingInterpreter interpreter,
                                                                        Vehicle vehicle)
        {
            return CHEdgeGraphOsmStreamWriter.Preprocess(reader, new SimpleTagsIndex(), interpreter, vehicle);
        }

        #endregion
    }
}
