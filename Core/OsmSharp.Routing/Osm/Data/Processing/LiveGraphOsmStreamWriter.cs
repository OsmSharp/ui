using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Interpreter.Roads;
using OsmSharp.Routing.Interpreter;

namespace OsmSharp.Routing.Osm.Data.Processing
{
    /// <summary>
    /// A data processing target containing edges with the orignal OSM-tags and their original OSM-direction.
    /// </summary>
    public class LiveGraphOsmStreamWriter : DynamicGraphOsmStreamWriter<LiveEdge>
    {
        /// <summary>
        /// Holds the data source.
        /// </summary>
        private readonly IDynamicGraphRouterDataSource<LiveEdge> _dynamicDataSource;

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter">Inteprets the OSM-data.</param>
        /// <param name="tagsIndex">Holds all the tags.</param>
        public LiveGraphOsmStreamWriter(IDynamicGraphRouterDataSource<LiveEdge> dynamicGraph,
            IRoutingInterpreter interpreter, ITagsIndex tagsIndex)
            : this(dynamicGraph, interpreter, tagsIndex, new Dictionary<long, uint>())
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="idTransformations"></param>
        public LiveGraphOsmStreamWriter(IDynamicGraphRouterDataSource<LiveEdge> dynamicGraph,
            IRoutingInterpreter interpreter, ITagsIndex tagsIndex, IDictionary<long, uint> idTransformations)
            : this(dynamicGraph, interpreter, tagsIndex, idTransformations, null)
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="box"></param>
        public LiveGraphOsmStreamWriter(IDynamicGraphRouterDataSource<LiveEdge> dynamicGraph,
            IRoutingInterpreter interpreter, ITagsIndex tagsIndex, GeoCoordinateBox box)
            : this(dynamicGraph, interpreter, tagsIndex, new Dictionary<long, uint>(), box)
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="idTransformations"></param>
        /// <param name="box"></param>
        public LiveGraphOsmStreamWriter(IDynamicGraphRouterDataSource<LiveEdge> dynamicGraph,
            IRoutingInterpreter interpreter, ITagsIndex tagsIndex, IDictionary<long, uint> idTransformations, 
            GeoCoordinateBox box)
            : base(dynamicGraph, interpreter, null, tagsIndex, idTransformations, box)
        {
            _dynamicDataSource = dynamicGraph;
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
        protected override LiveEdge CalculateEdgeData(IEdgeInterpreter edgeInterpreter, ITagsIndex tagsIndex, 
            TagsCollection tags, bool directionForward, GeoCoordinate from, GeoCoordinate to)
        {
            if (edgeInterpreter == null) throw new ArgumentNullException("edgeInterpreter");
            if (tagsIndex == null) throw new ArgumentNullException("tagsIndex");
            if (tags == null) throw new ArgumentNullException("tags");
            if (from == null) throw new ArgumentNullException("from");
            if (to == null) throw new ArgumentNullException("to");

            return new LiveEdge()
            {
                Forward = directionForward,
                Tags = tagsIndex.Add(tags)
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
            return edgeInterpreter.IsRoutable(tags);
        }
    }
}
