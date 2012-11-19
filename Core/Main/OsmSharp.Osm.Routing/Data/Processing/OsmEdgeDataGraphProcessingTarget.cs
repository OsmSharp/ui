using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math;
using OsmSharp.Routing.Core.Interpreter.Roads;
using OsmSharp.Routing.Core.Graph;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Routing.Core.Graph.DynamicGraph;

namespace OsmSharp.Osm.Routing.Data.Processing
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
            : this(dynamic_graph, interpreter, tags_index, id_transformations, null)
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
            IRoutingInterpreter interpreter, ITagsIndex tags_index, GeoCoordinateBox box)
            : this(dynamic_graph, interpreter, tags_index, new Dictionary<long, uint>(), box)
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
            IRoutingInterpreter interpreter, ITagsIndex tags_index, IDictionary<long, uint> id_transformations, GeoCoordinateBox box)
            : base(dynamic_graph, interpreter, tags_index, id_transformations, box)
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
                tags, global::OsmSharp.Routing.Core.VehicleEnum.Car, from, to);
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
                forward = (direction_forward && direction.Value) || 
                    (!direction_forward && !direction.Value);
                backward = (direction_forward && !direction.Value) ||
                    (!direction_forward && direction.Value);
            }

            // initialize the edge data.
            return new OsmEdgeData((float)weight, forward, backward, tags_index.Add(
                tags));
        }
    }
}
