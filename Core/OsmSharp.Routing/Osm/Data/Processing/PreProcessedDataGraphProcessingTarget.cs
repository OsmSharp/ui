using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Graph.DynamicGraph.SimpleWeighed;
using OsmSharp.Routing.Graph.Router;
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
        /// <param name="dynamic_graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tags_index"></param>
        /// <param name="vehicle"></param>
        public PreProcessedDataGraphProcessingTarget(IDynamicGraphRouterDataSource<PreProcessedEdge> dynamic_graph,
            IRoutingInterpreter interpreter, ITagsIndex tags_index, VehicleEnum vehicle)
            : this(dynamic_graph, interpreter, tags_index, vehicle, new Dictionary<long, uint>())
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamic_graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tags_index"></param>
        /// <param name="vehicle"></param>
        /// <param name="id_transformations"></param>
        public PreProcessedDataGraphProcessingTarget(IDynamicGraphRouterDataSource<PreProcessedEdge> dynamic_graph,
            IRoutingInterpreter interpreter, ITagsIndex tags_index, VehicleEnum vehicle, IDictionary<long, uint> id_transformations)
            : this(dynamic_graph, interpreter, tags_index, vehicle, id_transformations, null)
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamic_graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tags_index"></param>
        /// <param name="vehicle"></param>
        /// <param name="box"></param>
        public PreProcessedDataGraphProcessingTarget(IDynamicGraphRouterDataSource<PreProcessedEdge> dynamic_graph,
            IRoutingInterpreter interpreter, ITagsIndex tags_index, VehicleEnum vehicle, GeoCoordinateBox box)
            : this(dynamic_graph, interpreter, tags_index, vehicle, new Dictionary<long, uint>(), box)
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamic_graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tags_index"></param>
        /// <param name="vehicle"></param>
        /// <param name="id_transformations"></param>
        /// <param name="box"></param>
        public PreProcessedDataGraphProcessingTarget(IDynamicGraphRouterDataSource<PreProcessedEdge> dynamic_graph,
            IRoutingInterpreter interpreter, ITagsIndex tags_index, VehicleEnum vehicle, IDictionary<long, uint> id_transformations, 
            GeoCoordinateBox box)
            : base(dynamic_graph, interpreter, null, tags_index, id_transformations, box)
        {
            _vehicle = vehicle;
            _dynamic_data_source = dynamic_graph;
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
        /// <param name="tags_index"></param>
        /// <param name="tags"></param>
        /// <param name="direction_forward"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="edge_interpreter"></param>
        /// <returns></returns>
        protected override PreProcessedEdge CalculateEdgeData(IEdgeInterpreter edge_interpreter, ITagsIndex tags_index, IDictionary<string, string> tags, 
            bool direction_forward, GeoCoordinate from, GeoCoordinate to)
        {
            double weight = edge_interpreter.Weight(
                tags, _vehicle, from, to);
            bool? direction = edge_interpreter.IsOneWay(tags, _vehicle);
            bool forward = false;
            bool backward = false;
            if(!direction.HasValue)
            { // both directions.
                forward = true;
                backward = true;
            }
            else
            { // define back/forward.
                forward = (direction_forward && direction.Value) || 
                    (!direction_forward && !direction.Value);
                backward = (direction_forward && !direction.Value) ||
                    (!direction_forward && direction.Value);
            }

            // initialize the edge data.
            return new PreProcessedEdge((float)weight, forward, backward, tags_index.Add(
                tags));
        }

        /// <summary>
        /// Returns true if the edge is traversable.
        /// </summary>
        /// <param name="edge_interpreter"></param>
        /// <param name="tags_index"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        protected override bool CalculateIsTraversable(IEdgeInterpreter edge_interpreter,
            ITagsIndex tags_index, IDictionary<string, string> tags)
        {
            return edge_interpreter.CanBeTraversedBy(tags, _vehicle);
        }
    }
}
