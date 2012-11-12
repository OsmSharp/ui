using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Tools.Math;
using Routing.Core.Interpreter.Roads;
using Routing.Core.Graph;
using Routing.Core.Interpreter;

namespace Osm.Routing.Data.Processing
{
    public class OsmEdgeDataGraphProcessingTarget : DynamicGraphDataProcessorTarget<OsmEdgeData>
    {
        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamic_graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tags_index"></param>
        /// <param name="id_transformations"></param>
        public OsmEdgeDataGraphProcessingTarget(IDynamicGraph<OsmEdgeData> dynamic_graph,
            IRoutingInterpreter interpreter, ITagsIndex tags_index)
            : this(dynamic_graph, interpreter, tags_index, new Dictionary<long, uint>())
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamic_graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tags_index"></param>
        /// <param name="id_transformations"></param>
        public OsmEdgeDataGraphProcessingTarget(IDynamicGraph<OsmEdgeData> dynamic_graph,
            IRoutingInterpreter interpreter, ITagsIndex tags_index, IDictionary<long, uint> id_transformations)
            : base(dynamic_graph, interpreter, tags_index, id_transformations)
        {

        }

        /// <summary>
        /// Calculates edge data.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="direction_forward"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        protected override OsmEdgeData CalculateEdgeData(IEdgeInterpreter edge_interpreter, ITagsIndex tags_index, IDictionary<string, string> tags, 
            bool direction_forward, GeoCoordinate from, GeoCoordinate to)
        {
            double weight = edge_interpreter.Weight(
                tags, global::Routing.Core.VehicleEnum.Car, from, to);
            bool? direction = edge_interpreter.IsOneWay(tags);
            bool forward = false;
            bool backward = false;
            if(!direction.HasValue)
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
            return new OsmEdgeData((float)weight, forward, backward, tags_index.Add(
                tags));
        }
    }
}
