using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.DynamicGraph;
using Osm.Routing.CH.PreProcessing.Tags;
using Osm.Routing.CH.PreProcessing;
using Osm.Data.Core.DynamicGraph.Memory;

namespace Osm.Routing.CH.Routing
{
    /// <summary>
    /// Represents a data source for CH data.
    /// </summary>
    public class CHDataSource
    {
        /// <summary>
        /// Holds the tags index.
        /// </summary>
        private OsmTagsIndex _tags_index;

        /// <summary>
        /// Holds the graph.
        /// </summary>
        private IDynamicGraph<CHEdgeData> _graph;

        /// <summary>
        /// Creates a new CH data source.
        /// </summary>
        /// <param name="graph"></param>
        public CHDataSource()
            : this(new MemoryDynamicGraph<CHEdgeData>(), new OsmTagsIndex())
        {

        }

        /// <summary>
        /// Creates a new CH data source.
        /// </summary>
        /// <param name="graph"></param>
        public CHDataSource(IDynamicGraph<CHEdgeData> graph)
            : this(graph, new OsmTagsIndex())
        {

        }

        /// <summary>
        /// Creates a new CH data source.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="tags_index"></param>
        public CHDataSource(IDynamicGraph<CHEdgeData> graph, OsmTagsIndex tags_index)
        {
            _tags_index = tags_index;
            _graph = graph;
        }

        /// <summary>
        /// Returns the graph.
        /// </summary>
        public IDynamicGraph<CHEdgeData> Graph
        {
            get
            {
                return _graph;
            }
        }

        /// <summary>
        /// Returns the tags with the given id.
        /// </summary>
        /// <param name="tags_int"></param>
        /// <returns></returns>
        public IDictionary<string, string> Get(uint tags_int)
        {
            return _tags_index.Get(tags_int);
        }

        /// <summary>
        /// Adds tags to this index.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public uint Add(IDictionary<string, string> tags)
        {
            return _tags_index.Add(tags);
        }
    }
}
