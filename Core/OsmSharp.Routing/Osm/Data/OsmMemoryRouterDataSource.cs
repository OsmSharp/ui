//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Routing.Router;
//using OsmSharp.Tools.Math.Geo;
//using OsmSharp.Routing.Graph.Memory;
//using OsmSharp.Routing.Graph;
//using OsmSharp.Tools.Math.Structures;

//namespace OsmSharp.Routing.Osm.Data
//{
//    /// <summary>
//    /// A router data source for OSM data holding all data in-memory.
//    /// </summary>
//    public class OsmMemoryRouterDataSource : IRouterDataSource<OsmEdgeData>, IDynamicGraph<OsmEdgeData>
//    {
//        /// <summary>
//        /// Holds the basic graph.
//        /// </summary>
//        private MemoryDynamicGraph<OsmEdgeData> _graph;

//        /// <summary>
//        /// Holds the index of vertices per bounding box.
//        /// </summary>
//        private ILocatedObjectIndex<GeoCoordinate, uint> _vertex_index;

//        /// <summary>
//        /// Creates a new osm memory router data source.
//        /// </summary>
//        public OsmMemoryRouterDataSource()
//        {
//            _vertex_index = new LocatedObjectIndexList<GeoCoordinate, uint>();
//        }

//        /// <summary>
//        /// Returns all arcs inside the given bounding box.
//        /// </summary>
//        /// <param name="box"></param>
//        /// <returns></returns>
//        public KeyValuePair<uint, KeyValuePair<uint, OsmEdgeData>>[] GetArcs(
//            GeoCoordinateBox box)
//        {
//            // get all the vertices in the given box.
//            IEnumerable<uint> vertices = _vertex_index.GetInside(
//                box);

//            // loop over all vertices and get the arcs.
//            List<KeyValuePair<uint, KeyValuePair<uint, OsmEdgeData>>> arcs = new List<KeyValuePair<uint, KeyValuePair<uint, OsmEdgeData>>>();
//            foreach (uint vertex in vertices)
//            {
//                KeyValuePair<uint, OsmEdgeData>[] local_arcs = this.GetArcs(vertex);
//                foreach (KeyValuePair<uint, OsmEdgeData> local_arc in local_arcs)
//                {
//                    arcs.Add(new KeyValuePair<uint, KeyValuePair<uint, OsmEdgeData>>(
//                        vertex, local_arc));
//                }
//            }
//            return arcs.ToArray();
//        }

//        /// <summary>
//        /// Returns true if a given vertex is in the graph.
//        /// </summary>
//        /// <param name="id"></param>
//        /// <param name="latitude"></param>
//        /// <param name="longitude"></param>
//        /// <returns></returns>
//        public bool GetVertex(uint id, out float latitude, out float longitude)
//        {
//            return _graph.GetVertex(id, out latitude, out longitude);
//        }

//        /// <summary>
//        /// Returns an enumerable of all vertices.
//        /// </summary>
//        /// <returns></returns>
//        public IEnumerable<uint> GetVertices()
//        {
//            return _graph.GetVertices();
//        }

//        /// <summary>
//        /// Returns all arcs starting at a given vertex.
//        /// </summary>
//        /// <param name="vertex"></param>
//        /// <returns></returns>
//        public KeyValuePair<uint, OsmEdgeData>[] GetArcs(uint vertex)
//        {
//            return _graph.GetArcs(vertex);
//        }

//        /// <summary>
//        /// Adds a new vertex.
//        /// </summary>
//        /// <param name="latitude"></param>
//        /// <param name="longitude"></param>
//        /// <returns></returns>
//        public uint AddVertex(float latitude, float longitude)
//        {
//            uint vertex = _graph.AddVertex(latitude, longitude);
//            _vertex_index.Add(new GeoCoordinate(latitude, longitude),
//                vertex);
//            return vertex;
//        }

//        /// <summary>
//        /// Adds a new arc.
//        /// </summary>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <param name="data"></param>
//        public void AddArc(uint from, uint to, OsmEdgeData data)
//        {
//            _graph.AddArc(from, to, data);
//        }

//        /// <summary>
//        /// Deletes an arc.
//        /// </summary>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        public void DeleteArc(uint from, uint to)
//        {
//            _graph.DeleteArc(from, to);
//        }
//    }
//}
