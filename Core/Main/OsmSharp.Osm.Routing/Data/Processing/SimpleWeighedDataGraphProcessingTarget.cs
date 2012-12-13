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
using OsmSharp.Routing.Core.Graph.DynamicGraph.SimpleWeighed;
using OsmSharp.Routing.Core;

namespace OsmSharp.Osm.Routing.Data.Processing
{
    /// <summary>
    /// A data processing target accepting raw OSM data and converting it into routable data.
    /// </summary>
    public class SimpleWeighedDataGraphProcessingTarget : DynamicGraphDataProcessorTarget<SimpleWeighedEdge>
    {
        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamic_graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tags_index"></param>
        /// <param name="id_transformations"></param>
        public SimpleWeighedDataGraphProcessingTarget(IDynamicGraph<SimpleWeighedEdge> dynamic_graph,
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
        public SimpleWeighedDataGraphProcessingTarget(IDynamicGraph<SimpleWeighedEdge> dynamic_graph,
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
        public SimpleWeighedDataGraphProcessingTarget(IDynamicGraph<SimpleWeighedEdge> dynamic_graph,
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
        public SimpleWeighedDataGraphProcessingTarget(IDynamicGraph<SimpleWeighedEdge> dynamic_graph,
            IRoutingInterpreter interpreter, ITagsIndex tags_index, IDictionary<long, uint> id_transformations, GeoCoordinateBox box)
            : base(dynamic_graph, interpreter, null, tags_index, id_transformations, box)
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
        protected override SimpleWeighedEdge CalculateEdgeData(IEdgeInterpreter edge_interpreter, ITagsIndex tags_index, IDictionary<string, string> tags,
            bool direction_forward, GeoCoordinate from, GeoCoordinate to)
        {
            // use the distance as weight.
            double distance = from.DistanceReal(to).Value;

            return new SimpleWeighedEdge()
            {
                IsForward = direction_forward,
                Tags = tags_index.Add(tags),
                Weight = distance
            };
        }
    }
}
