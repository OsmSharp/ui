//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Routing.Graphs.Interpreter;
//using Osm.Routing.HH.Highways;
//using Osm.Routing.Graphs;
//using Tools.Math.Graph.Helpers;
//using Osm.Data.Xml.OsmSource;

//namespace Osm.Routing.HH.Memory
//{
//    public class MemoryHighwayHierarchy : IHighwayHierarchy
//    {
//        private OsmDataSource _data_source;

//        private Graphs.Graph _level_0_graph;

//        private Dictionary<int, HashSet<Graphs.GraphVertex>> _vertices_per_level;

//        private Dictionary<Graphs.GraphVertex, HashSet<HighwayEdge>> _edges_by_from;
//        private Dictionary<Graphs.GraphVertex, HashSet<HighwayEdge>> _edges_by_to;

//        public MemoryHighwayHierarchy(OsmDataSource data_source, GraphInterpreterBase interpreter)
//        {
//            _data_source = data_source;
//            _level_0_graph = new Graphs.Graph(interpreter, data_source);

//            _vertices_per_level = new Dictionary<int, HashSet<GraphVertex>>();
//            _edges_by_from = new Dictionary<GraphVertex, HashSet<HighwayEdge>>();
//            _edges_by_to = new Dictionary<GraphVertex, HashSet<HighwayEdge>>();
//        }

//        public Data.IDataSourceReadOnly Data
//        {
//            get
//            {
//                return _data_source;
//            }
//        }

//        public IEnumerable<Graphs.GraphVertex> GetHighwayNodes(int level)
//        {
//            if (level == 0)
//            {
//                return null;// new ;
//            }
//            else
//            {
//                HashSet<Graphs.GraphVertex> vertices = null;
//                if (!_vertices_per_level.TryGetValue(level, out vertices))
//                {
//                    vertices = new HashSet<Graphs.GraphVertex>();
//                }
//                return vertices;
//            }
//        }

//        public void AddEdges(int level, HashSet<Highways.HighwayEdge> edges)
//        {
//            foreach (Highways.HighwayEdge edge in edges)
//            {
//                this.AddEdge(level, edge);
//            }
//        }

//        public void AddEdge(int level, Highways.HighwayEdge edge)
//        {
//            if (level > 1)
//            {
//                // Add vertices.
//                HashSet<Graphs.GraphVertex> vertices = null;
//                if (!_vertices_per_level.TryGetValue(level, out vertices))
//                {
//                    vertices = new HashSet<Graphs.GraphVertex>();
//                    _vertices_per_level.Add(level, vertices);
//                }
//                vertices.Add(edge.From);
//                vertices.Add(edge.To);

//                // Add edges to foward set.
//                HashSet<HighwayEdge> edge_starting_with = null;
//                if (!_edges_by_from.TryGetValue(edge.From, out edge_starting_with))
//                {
//                    edge_starting_with = new HashSet<HighwayEdge>();
//                    _edges_by_from.Add(edge.From, edge_starting_with);
//                }
//                edge_starting_with.Add(edge);

//                // Add edges to backward set.
//                HashSet<HighwayEdge> edge_ending_with = null;
//                if (!_edges_by_to.TryGetValue(edge.To, out edge_ending_with))
//                {
//                    edge_ending_with = new HashSet<HighwayEdge>();
//                    _edges_by_to.Add(edge.From, edge_ending_with);
//                }
//                edge_ending_with.Add(edge);
//            }
//        }

//        private Dictionary<int, Stack<Graphs.GraphVertex>> _uncontracted;

//        public Graphs.GraphVertex GetUncontracted(int level)
//        {
//            return _uncontracted[level].Pop();
//        }

//        public void MarkUncontracted(int level, Graphs.GraphVertex vertex)
//        {
//            _uncontracted[level].Push(vertex);
//        }

//        public void ContractVertex(Graphs.GraphVertex vertex, HashSet<Highways.HighwayEdge> shortcuts)
//        {
//            throw new NotImplementedException();
//        }

//        public HashSet<HighwayEdge> GetNeigbours(int level,
//            Graphs.GraphVertex vertex, HashSet<Graphs.GraphVertex> exceptions)
//        {
//            if (level == 0)
//            {
//                // TODO: to fix this try to create a graph based on OSM data using highway edge as an edge.
//                // get the level 0 neighbours.
//                HashSet<VertexAlongEdge<Osm.Core.Way, GraphVertex>> level_0_neighbours =
//                    _level_0_graph.GetNeighbours(vertex, exceptions);

//                return this.Convert(vertex, level_0_neighbours);
//            }
//            else
//            {
//                return null;
//            }
//        }

//        private HashSet<HighwayEdge> Convert(GraphVertex vertex,
//            HashSet<VertexAlongEdge<Osm.Core.Way, GraphVertex>> level_0_neighbours)
//        {
//            // convert to highway edges.
//            HashSet<HighwayEdge> level_0_highways =
//                new HashSet<HighwayEdge>();
//            foreach (VertexAlongEdge<Osm.Core.Way, GraphVertex> neighbour in level_0_neighbours)
//            {
//                // create highway edge.
//                HighwayEdge edge = new HighwayEdge();
//                edge.From = vertex;
//                edge.To = neighbour.Vertex;
//                edge.Weight = neighbour.Weight;

//                // create vertexalongedge.
//                level_0_highways.Add(edge);
//            }

//            return level_0_highways;
//        }

//        public HashSet<HighwayEdge> GetNeigboursReversed(
//            int level, Graphs.GraphVertex vertex, HashSet<Graphs.GraphVertex> exceptions)
//        {
//            if (level == 0)
//            {
//                // TODO: to fix this try to create a graph based on OSM data using highway edge as an edge.
//                // get the level 0 neighbours.
//                HashSet<VertexAlongEdge<Osm.Core.Way, GraphVertex>> level_0_neighbours =
//                    _level_0_graph.GetNeighboursReversed(vertex, exceptions);

//                return this.Convert(vertex, level_0_neighbours);
//            }
//            else
//            {
//                HashSet<HighwayEdge> edges = null;
//                if (!_edges_by_from.TryGetValue(vertex, out edges))
//                {
//                    return null;
//                }
//                HashSet<HighwayEdge> neighbours = new HashSet<HighwayEdge>();
//                foreach (HighwayEdge edge in edges)
//                {
//                    //if(edge.L
//                }
//            }
//            return null;
//        }

//        public bool ContainsVertex(
//            int level, Graphs.GraphVertex vertex)
//        {
//            if (level == 0)
//            {
//                // TODO: fix this because all vertices will belong to level 0 like this!
//                return _level_0_graph.ContainsVertex(vertex);
//            }
//            else
//            {
//                HashSet<GraphVertex> vertices = null;
//                if (_vertices_per_level.TryGetValue(level, out vertices))
//                {
//                    return vertices.Contains(vertex);
//                }
//                return false;
//            }
//        }

//        public void ClearTarget()
//        {
//            throw new NotImplementedException();
//        }


//        public void AddCore(int level, GraphVertex vertex)
//        {

//        }

//        void IHighwayHierarchy.ClearTarget()
//        {
//            throw new NotImplementedException();
//        }

//        //Data.IDataSourceReadOnly IHighwayHierarchy.Data
//        //{
//        //    get { throw new NotImplementedException(); }
//        //}

//        IEnumerable<GraphVertex> IHighwayHierarchy.GetHighwayNodes(int level)
//        {
//            throw new NotImplementedException();
//        }

//        void IHighwayHierarchy.AddEdge(int level, HighwayEdge edges)
//        {
//            throw new NotImplementedException();
//        }

//        GraphVertex IHighwayHierarchy.GetUncontracted(int level)
//        {
//            throw new NotImplementedException();
//        }

//        void IHighwayHierarchy.MarkUncontracted(GraphVertex vertex)
//        {
//            throw new NotImplementedException();
//        }

//        void IHighwayHierarchy.ContractVertex(GraphVertex vertex, HashSet<HighwayEdge> shortcuts)
//        {
//            throw new NotImplementedException();
//        }

//        HashSet<HighwayEdge> IHighwayHierarchy.GetNeigbours(int level, GraphVertex vertex, HashSet<GraphVertex> exceptions)
//        {
//            throw new NotImplementedException();
//        }

//        HashSet<HighwayEdge> IHighwayHierarchy.GetNeigboursReversed(int level, GraphVertex vertex, HashSet<GraphVertex> exceptions)
//        {
//            throw new NotImplementedException();
//        }

//        bool IHighwayHierarchy.ContainsVertex(int level, GraphVertex vertex)
//        {
//            throw new NotImplementedException();
//        }

//        void IHighwayHierarchy.AddCore(int level, GraphVertex vertex)
//        {
//            throw new NotImplementedException();
//        }


//        public void StartCore(int _level)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class Level0VertexEnumerable : IEnumerable<GraphVertex>
//    {
//        private OsmDataSource _source;

//        public Level0VertexEnumerable(OsmDataSource source)
//        {
//            _source = source;
//        }

//        public IEnumerator<GraphVertex> GetEnumerator()
//        {
//            return new Level0VertexEnumerator(_source);
//        }

//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            return new Level0VertexEnumerator(_source);
//        }
//    }

//    public class Level0VertexEnumerator : IEnumerator<GraphVertex>
//    {
//        private IEnumerator<Osm.Core.OsmBase> _bases;

//        private HashSet<Osm.Core.Node> _nodes;

//        private GraphVertex _current;

//        public Level0VertexEnumerator(OsmDataSource source)
//        {
//            _bases = source.Get(Osm.Core.Filters.Filter.Exists("highway")).GetEnumerator();
//            _nodes = new HashSet<Osm.Core.Node>();
//        }

//        public GraphVertex Current
//        {
//            get 
//            {
//                return _current; 
//            }
//        }

//        public void Dispose()
//        {
//            _bases = null;
//        }

//        object System.Collections.IEnumerator.Current
//        {
//            get 
//            {
//                return this.Current;
//            }
//        }

//        public bool MoveNext()
//        {
//            return false;
//        }

//        public void Reset()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
