using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Core.Interpreter.Roads;
using OsmSharp.Tools.Math;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Core.Graph;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Routing.Core.Graph.DynamicGraph;
using OsmSharp.Routing.Core;

namespace OsmSharp.Osm.Routing.Data.Processing
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

        public CHEdgeDataGraphProcessingTarget(IDynamicGraph<CHEdgeData> dynamic_graph,
            IRoutingInterpreter interpreter, ITagsIndex tags_index, VehicleEnum vehicle)
            :base(dynamic_graph, interpreter, new CHEdgeDataComparer(), tags_index)
        {
            _vehicle = vehicle;
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
    }
}
