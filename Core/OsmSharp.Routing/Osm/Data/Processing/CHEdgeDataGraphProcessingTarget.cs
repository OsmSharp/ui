using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Interpreter.Roads;
using OsmSharp.Tools.Math;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Graph.DynamicGraph;
using OsmSharp.Routing;

namespace OsmSharp.Routing.Osm.Data.Processing
{
    /// <summary>
    /// A pre-processing target for OSM-data to a CH data structure.
    /// </summary>
    public class CHEdgeDataGraphProcessingTarget : DynamicGraphDataProcessorTarget<CHEdgeData>
    {
        /// <summary>
        /// Holds the vehicle profile this pre-processing target is for.
        /// </summary>
        private VehicleEnum _vehicle;

        /// <summary>
        /// Holds the data source.
        /// </summary>
        private IDynamicGraphRouterDataSource<CHEdgeData> _dynamic_data_source;

        /// <summary>
        /// Creates a CH data processing target.
        /// </summary>
        /// <param name="dynamic_graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tags_index"></param>
        /// <param name="vehicle"></param>
        public CHEdgeDataGraphProcessingTarget(IDynamicGraphRouterDataSource<CHEdgeData> dynamic_graph,
            IRoutingInterpreter interpreter, ITagsIndex tags_index, VehicleEnum vehicle)
            :base(dynamic_graph, interpreter, new CHEdgeDataComparer(), tags_index)
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
        /// <param name="edge_interpreter"></param>
        /// <param name="tags_index"></param>
        /// <param name="tags"></param>
        /// <param name="direction_forward"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        protected override CHEdgeData CalculateEdgeData(IEdgeInterpreter edge_interpreter, ITagsIndex tags_index, 
            IDictionary<string, string> tags, bool direction_forward, GeoCoordinate from, GeoCoordinate to)
        {
            double weight = edge_interpreter.Weight(
                tags, _vehicle, from, to);
            bool? direction = edge_interpreter.IsOneWay(tags, _vehicle);
            bool forward = false;
            bool backward = false;
            if (!direction.HasValue)
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
            return new CHEdgeData()
            {
                Weight = (float)weight, 
                Forward = forward, 
                Backward = backward, 
                Tags = tags_index.Add(
                tags),
                ContractedVertexId = 0
            };
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
