//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Routing.CH.PreProcessing;
//using OsmSharp.Routing.Graph;
//using OsmSharp.Routing.Graph.Memory;
//using OsmSharp.Tools.Math;

//namespace OsmSharp.Routing.CH.Routing
//{
//    /// <summary>
//    /// Represents a data source for CH data.
//    /// </summary>
//    public class CHDataSource
//    {
//        /// <summary>
//        /// Holds the tags index.
//        /// </summary>
//        private ITagsIndex _tags_index;

//        /// <summary>
//        /// Holds the graph.
//        /// </summary>
//        private IDynamicGraph<CHEdgeData> _graph;

//        /// <summary>
//        /// Creates a new CH data source.
//        /// </summary>
//        /// <param name="graph"></param>
//        public CHDataSource(ITagsIndex tags_index)
//            : this(new MemoryDynamicGraph<CHEdgeData>(), tags_index)
//        {

//        }

//        /// <summary>
//        /// Creates a new CH data source.
//        /// </summary>
//        /// <param name="target"></param>
//        /// <param name="tags_index"></param>
//        public CHDataSource(IDynamicGraph<CHEdgeData> graph, ITagsIndex tags_index)
//        {
//            _tags_index = tags_index;
//            _graph = graph;
//        }

//        /// <summary>
//        /// Returns the graph.
//        /// </summary>
//        public IDynamicGraph<CHEdgeData> Graph
//        {
//            get
//            {
//                return _graph;
//            }
//        }

//        /// <summary>
//        /// Returns the tags with the given id.
//        /// </summary>
//        /// <param name="tags_int"></param>
//        /// <returns></returns>
//        public IDictionary<string, string> Get(uint tags_int)
//        {
//            return _tags_index.Get(tags_int);
//        }

//        /// <summary>
//        /// Adds tags to this index.
//        /// </summary>
//        /// <param name="tags"></param>
//        /// <returns></returns>
//        public uint Add(IDictionary<string, string> tags)
//        {
//            return _tags_index.Add(tags);
//        }
//    }
//}
