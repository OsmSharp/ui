using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Graph.DynamicGraph.SimpleWeighed;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Tools.Collections.Tags;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math;
using OsmSharp.Routing.Interpreter.Roads;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Graph.DynamicGraph;
using OsmSharp.Routing.Graph.DynamicGraph.PreProcessed;
using OsmSharp.Routing;

namespace OsmSharp.Routing.Osm.Data.Processing
{
    /// <summary>
    /// A pre-processed data graph processing target.
    /// </summary>
    public class PreProcessedDataGraphProcessingTarget : DynamicGraphDataProcessorTarget<PreProcessedEdge>
    {
        /// <summary>
        /// Holds the vehicle profile this pre-processing target is for.
        /// </summary>
        private readonly VehicleEnum _vehicle;

        /// <summary>
        /// Holds the data source.
        /// </summary>
        private IDynamicGraphRouterDataSource<PreProcessedEdge> _dynamic_data_source;

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="vehicle"></param>
        public PreProcessedDataGraphProcessingTarget(IDynamicGraphRouterDataSource<PreProcessedEdge> dynamicGraph,
            IRoutingInterpreter interpreter, ITagsIndex tagsIndex, VehicleEnum vehicle)
            : this(dynamicGraph, interpreter, tagsIndex, vehicle, new Dictionary<long, uint>())
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="vehicle"></param>
        /// <param name="idTransformations"></param>
        public PreProcessedDataGraphProcessingTarget(IDynamicGraphRouterDataSource<PreProcessedEdge> dynamicGraph,
            IRoutingInterpreter interpreter, ITagsIndex tagsIndex, VehicleEnum vehicle, IDictionary<long, uint> idTransformations)
            : this(dynamicGraph, interpreter, tagsIndex, vehicle, idTransformations, null)
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="vehicle"></param>
        /// <param name="box"></param>
        public PreProcessedDataGraphProcessingTarget(IDynamicGraphRouterDataSource<PreProcessedEdge> dynamicGraph,
            IRoutingInterpreter interpreter, ITagsIndex tagsIndex, VehicleEnum vehicle, GeoCoordinateBox box)
            : this(dynamicGraph, interpreter, tagsIndex, vehicle, new Dictionary<long, uint>(), box)
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="vehicle"></param>
        /// <param name="idTransformations"></param>
        /// <param name="box"></param>
        public PreProcessedDataGraphProcessingTarget(IDynamicGraphRouterDataSource<PreProcessedEdge> dynamicGraph,
            IRoutingInterpreter interpreter, ITagsIndex tagsIndex, VehicleEnum vehicle, IDictionary<long, uint> idTransformations, 
            GeoCoordinateBox box)
            : base(dynamicGraph, interpreter, null, tagsIndex, idTransformations, box)
        {
            _vehicle = vehicle;
            _dynamic_data_source = dynamicGraph;
        }

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            _dynamic_data_source.AddSupportedProfile(_vehicle);
        }

        /// <summary>
        /// Calculates edge data.
        /// </summary>
        /// <param name="tagsIndex"></param>
        /// <param name="tags"></param>
        /// <param name="directionForward"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="edgeInterpreter"></param>
        /// <returns></returns>
        protected override PreProcessedEdge CalculateEdgeData(IEdgeInterpreter edgeInterpreter, ITagsIndex tagsIndex, TagsCollection tags, 
            bool directionForward, GeoCoordinate from, GeoCoordinate to)
        {
            double weight = edgeInterpreter.Weight(
                tags, _vehicle, from, to);
            bool? direction = edgeInterpreter.IsOneWay(tags, _vehicle);
            bool forward = false;
            bool backward = false;
            if(!direction.HasValue)
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
            return new PreProcessedEdge((float)weight, forward, backward, tagsIndex.Add(
                tags));
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
            return edgeInterpreter.CanBeTraversedBy(tags, _vehicle);
        }
    }
}
