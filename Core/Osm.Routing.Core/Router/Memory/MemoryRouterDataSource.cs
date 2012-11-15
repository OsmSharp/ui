using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Core.Graph;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math.Structures;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Tools.Math;

namespace OsmSharp.Routing.Core.Router.Memory
{
    /// <summary>
    /// A memory data source.
    /// </summary>
    /// <typeparam name="EdgeData"></typeparam>
    public class MemoryRouterDataSource<EdgeData> : IRouterDataSource<EdgeData>, IDynamicGraph<EdgeData>
        where EdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the basic graph.
        /// </summary>
        private MemoryDynamicGraph<EdgeData> _graph;

        /// <summary>
        /// Holds the index of vertices per bounding box.
        /// </summary>
        private ILocatedObjectIndex<GeoCoordinate, uint> _vertex_index;

        /// <summary>
        /// Holds the tags index.
        /// </summary>
        private ITagsIndex _tags_index;

        /// <summary>
        /// Creates a new osm memory router data source.
        /// </summary>
        public MemoryRouterDataSource(ITagsIndex tags_index)
        {
            _graph = new MemoryDynamicGraph<EdgeData>();
            _vertex_index = new LocatedObjectIndexList<GeoCoordinate, uint>();

            _tags_index = tags_index;
        }

        /// <summary>
        /// Returns all arcs inside the given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public KeyValuePair<uint, KeyValuePair<uint, EdgeData>>[] GetArcs(
            GeoCoordinateBox box)
        {
            // get all the vertices in the given box.
            IEnumerable<uint> vertices = _vertex_index.GetInside(
                box);

            // loop over all vertices and get the arcs.
            List<KeyValuePair<uint, KeyValuePair<uint, EdgeData>>> arcs =
                new List<KeyValuePair<uint, KeyValuePair<uint, EdgeData>>>();
            foreach (uint vertex in vertices)
            {
                KeyValuePair<uint, EdgeData>[] local_arcs = this.GetArcs(vertex);
                foreach (KeyValuePair<uint, EdgeData> local_arc in local_arcs)
                {
                    arcs.Add(new KeyValuePair<uint, KeyValuePair<uint, EdgeData>>(
                        vertex, local_arc));
                }
            }
            return arcs.ToArray();
        }

        /// <summary>
        /// Returns true if a given vertex is in the graph.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public bool GetVertex(uint id, out float latitude, out float longitude)
        {
            return _graph.GetVertex(id, out latitude, out longitude);
        }

        /// <summary>
        /// Returns an enumerable of all vertices.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<uint> GetVertices()
        {
            return _graph.GetVertices();
        }

        /// <summary>
        /// Returns all arcs starting at a given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public KeyValuePair<uint, EdgeData>[] GetArcs(uint vertex)
        {
            return _graph.GetArcs(vertex);
        }

        /// <summary>
        /// Adds a new vertex.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public uint AddVertex(float latitude, float longitude)
        {
            uint vertex = _graph.AddVertex(latitude, longitude);
            _vertex_index.Add(new GeoCoordinate(latitude, longitude),
                vertex);
            return vertex;
        }

        /// <summary>
        /// Adds a new arc.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="data"></param>
        public void AddArc(uint from, uint to, EdgeData data)
        {
            _graph.AddArc(from, to, data);
        }

        /// <summary>
        /// Deletes an arc.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void DeleteArc(uint from, uint to)
        {
            _graph.DeleteArc(from, to);
        }

        /// <summary>
        /// Returns the tags index.
        /// </summary>
        public ITagsIndex TagsIndex
        {
            get
            { 
                return _tags_index; 
            }
        }

        /// <summary>
        /// Returns the number of vertices in this graph.
        /// </summary>
        public uint VertexCount
        {
            get { return _graph.VertexCount; }
        }
    }
}
