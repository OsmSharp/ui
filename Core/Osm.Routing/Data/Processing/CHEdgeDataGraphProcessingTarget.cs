using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routing.CH.PreProcessing;
using Routing.Core.Interpreter.Roads;
using Tools.Math;
using Tools.Math.Geo;
using Routing.Core.Graph;
using Routing.Core.Interpreter;

namespace Osm.Routing.Data.Processing
{
    public class CHEdgeDataGraphProcessingTarget : DynamicGraphDataProcessorTarget<CHEdgeData>
    {
        public CHEdgeDataGraphProcessingTarget(IDynamicGraph<CHEdgeData> dynamic_graph,
            IRoutingInterpreter interpreter, ITagsIndex tags_index)
            :base(dynamic_graph, interpreter, tags_index)
        {

        }

        protected override CHEdgeData CalculateEdgeData(IEdgeInterpreter edge_interpreter, ITagsIndex tags_index, 
            IDictionary<string, string> tags, bool direction_forward, GeoCoordinate from, GeoCoordinate to)
        {
            double weight = edge_interpreter.Weight(
                tags, global::Routing.Core.VehicleEnum.Car, from, to);
            bool? direction = edge_interpreter.IsOneWay(tags);
            bool forward = false;
            bool backward = false;
            if (!direction.HasValue)
            { // both directions.
                forward = true;
                backward = true;
            }
            else
            { // define back/forward.
                forward = direction_forward && direction.Value;
                backward = !direction_forward && !direction.Value;
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
