//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Routing.HH;
//using Oracle.DataAccess.Client;
//using Osm.Data.Oracle;
//using Osm.Data;
//using Osm.Routing.Graphs;
//using Osm.Routing.HH.Highways;
//using Osm.Routing.Graphs.Interpreter;
//using Tools.Math.Graph.Helpers;
//using Osm.Data.Cache;
//using Osm.Routing.Oracle;

//namespace Osm.Routing.Oracle.HH
//{
//    public class HighwayHierarchy : IHighwayHierarchy
//    {
//        private OracleConnection _connection;

//        public HighwayHierarchy(GraphInterpreterBase interpreter, string connection_string)
//        {
//            _connection = new OracleConnection(connection_string);
//            _connection.Open();
//            _osm_data = new DataSourceCache(new OracleSimpleSource(connection_string),15);
//            _level_0_graph = new Graph(interpreter, _osm_data);

//            _vertices_to_update = new HashSet<long>();
//        }

//        #region IHighwayHierarchy Members

//        private IDataSourceReadOnly _osm_data;

//        private Graph _level_0_graph;

//        public IDataSourceReadOnly Data
//        {
//            get 
//            {
//                return _osm_data; 
//            }
//        }

//        public IEnumerable<GraphVertex> GetHighwayNodes(int level)
//        {
//            if (level == 0)
//            { // the first level is just the regular OSM data.
//                return new HighwayHierarchyLevel0Enumerable(_connection, _osm_data);
//            }
//            else
//            { // the other levels are custom.
//                return new HighwayHierarchyEnumerable(_connection, _osm_data, level);
//            }
//        }

//        /// <summary>
//        /// Holds the current uncontracted set.
//        /// </summary>
//        private HashSet<GraphVertex> _uncontracted = new HashSet<GraphVertex>();

//        /// <summary>
//        /// Returns a new vertex from the uncontracted set.
//        /// </summary>
//        /// <param name="level"></param>
//        /// <returns></returns>
//        public GraphVertex GetUncontracted(int level)
//        {
//            if (_uncontracted.Count > 0)
//            {
//                return _uncontracted.First<GraphVertex>();
//            }
//            string sql = "select n.id " +
//                "from hh_node n " +
//                "where n.level_idx > contracted_idx " +
//                "and n.level_idx = :level_idx ";
//            OracleCommand command = new OracleCommand(sql);
//            command.Connection = _connection;
//            command.Parameters.Add("level_idx", level);
//            OracleDataReader reader = command.ExecuteReader();
//            GraphVertex vertex = null;
//            if (reader.Read())
//            {
//                long id = (long)reader["id"];

//                vertex =  new GraphVertex(_osm_data.GetNode(id));
//            }
//            reader.Close();
//            reader.Dispose();
//            return vertex;
//        }

//        /// <summary>
//        /// Marks an edge as uncontracted.
//        /// </summary>
//        /// <param name="level"></param>
//        /// <param name="vertex"></param>
//        public void MarkUncontracted(GraphVertex vertex)
//        {
//            _uncontracted.Add(vertex);
//            //string sql = "update hh_node n " +
//            //    "set n.contracted_idx = n.level_idx - 1, n.core = 0 " +
//            //    "where n.id = :id ";
//            //OracleCommand command = new OracleCommand(sql);
//            //command.Connection = _connection;
//            //command.Parameters.Add("id", vertex.Node.Id);
//            //command.ExecuteNonQuery();            
//        }


//        /// <summary>
//        /// Adds the vertex to the core at the given level.
//        /// </summary>
//        /// <param name="level"></param>
//        /// <param name="vertex"></param>
//        public void AddCore(int level, GraphVertex vertex)
//        {
//            _uncontracted.Remove(vertex);

//            string sql = "update hh_node set core = 1, contracted_idx = level_idx " +
//                "where id = :node_id ";
//            OracleCommand command = new OracleCommand(sql);
//            command.Connection = _connection;
//            command.Parameters.Add("node_id", vertex.Node.Id);
//            command.ExecuteNonQuery();
//        }

//        /// <summary>
//        /// Contracts the given vertex given the shortcuts.
//        /// </summary>
//        /// <param name="vertex"></param>
//        /// <param name="shortcuts"></param>
//        public void ContractVertex(GraphVertex vertex, HashSet<HighwayEdge> shortcuts)
//        {
//            // contract the vertex by adding the shortcut edges to the core and removing the shortcut edge from it.
//            _uncontracted.Remove(vertex);

//            // remove the bypassed vertex.
//            string sql = "update hh_node n set core = 0, n.contracted_idx = n.level_idx " +
//                "where n.id = :node_id "; // this node cannot show up anymore as a neigbour!

//            OracleCommand command = new OracleCommand(sql);
//            command.Connection = _connection;
//            command.Parameters.Add("node_id", vertex.Node.Id);
//            command.ExecuteNonQuery();

//            // add the shortcutted vertices.
//            if (shortcuts.Count > 0)
//            {
//                //HashSet<long> core_nodes = new HashSet<long>();
//                foreach (HighwayEdge edge in shortcuts)
//                {
//                    //core_nodes.Add(edge.From.Node.Id);
//                    //core_nodes.Add(edge.To.Node.Id);
//                    this.MarkUncontracted(edge.From);
//                    this.MarkUncontracted(edge.To);
//                }
//                //sql = "update hh_node set core = 1 " +
//                //    "where id in (:node_id) ";
//                //command = new OracleCommand(sql);
//                //command.Connection = _connection;
//                //command.AddParameterCollection<long>("node_id", core_nodes);
//                //command.ExecuteNonQuery();

//                //add the new edges.
//                foreach (HighwayEdge shortcut in shortcuts)
//                {
//                    sql = "select count(*) from hh_edge h where h.node_from = :node_from and h.node_to = :node_to ";

//                    command = new OracleCommand(sql);
//                    command.Connection = _connection;
//                    command.Parameters.Add("node_from", shortcut.From.Node.Id);
//                    command.Parameters.Add("node_to", shortcut.To.Node.Id);
//                    decimal count = (decimal)command.ExecuteScalar();

//                    if (count == 0)
//                    {
//                        sql = "insert into hh_edge h (node_from, node_to, weight) "
//                                    + "values (:node_from, :node_to, :weight) ";
//                        command = new OracleCommand(sql);
//                        command.Connection = _connection;
//                        command.Parameters.Add("node_from", shortcut.From.Node.Id);
//                        command.Parameters.Add("node_to", shortcut.To.Node.Id);
//                        command.Parameters.Add("weight", shortcut.Weight);
//                        command.ExecuteNonQuery();
//                    }
//                }
//            }
//        }

//        public bool ContainsVertex(int level, GraphVertex vertex)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion

//        /// <summary>
//        /// Adds an edge on the given level.
//        /// </summary>
//        /// <param name="level"></param>
//        /// <param name="edge"></param>
//        public void AddEdge(int level, HighwayEdge edge)
//        {
//            switch (level)
//            {
//                case 0:
//                    throw new Exception(string.Format("Cannot add an edge on level 0: level 0 is source data!"));
//                case 1:
//                    // only here insert the edge.
//                    //this.UpsertVertex(level, edge.From, false);
//                    //this.UpsertVertex(level, edge.To, false);
//                    this.InsertVertex(edge.From);
//                    this.InsertVertex(edge.To);

//                    this.InsertEdge(edge);

//                    //// also insert reverse edge.
//                    //HighwayEdge reverse_edge = new HighwayEdge();
//                    //reverse_edge.To = edge.From;
//                    //reverse_edge.From = edge.To;
//                    //reverse_edge.Weight = edge.Weight;
//                    //this.InsertEdge(reverse_edge);
//                    break;
//                default:
//                    // only upsert the vertices.
//                    this.UpdateVertex(level, edge.From);
//                    this.UpdateVertex(level, edge.To);
//                    break;
//            }
//        }

//        #region Edges

//        /// <summary>
//        /// Keeps a list of inserted edges.
//        /// </summary>
//        private HashSet<HighwayEdge> inserted_edges = new HashSet<HighwayEdge>();

//        /// <summary>
//        /// Keeps a list of edges to insert.
//        /// </summary>
//        private HashSet<HighwayEdge> _edges_to_insert = new HashSet<HighwayEdge>();

//        /// <summary>
//        /// Inserts an edge into the target db.
//        /// </summary>
//        /// <param name="edge"></param>
//        private void InsertEdge(HighwayEdge edge)
//        {
//            if (!inserted_edges.Contains(edge))
//            {
//                if (_edges_to_insert.Count > 100)
//                {
//                    this.FlushVertices();
//                    this.FlushEdges();
//                }
//                else
//                {
//                    _edges_to_insert.Add(edge);
//                }

//                inserted_edges.Add(edge);
//            }
//        }

//        /// <summary>
//        /// Flushes all the edges.
//        /// </summary>
//        private void FlushEdges()
//        {
//            OracleCommand command = new OracleCommand();
//            command.Connection = _connection;

//            StringBuilder sql_builder = new StringBuilder("insert all ");
//            string sql = "into hh_edge (node_from, node_to, weight) "
//                        + "values (:node_from_{0}, :node_to_{0}, :weight_{0}) ";
//            int idx = 0;
//            foreach (HighwayEdge edge in _edges_to_insert)
//            {
//                sql_builder.Append(string.Format(sql, idx));

//                command.Parameters.Add(string.Format("node_from_{0}", idx), edge.From.Node.Id);
//                command.Parameters.Add(string.Format("node_to_{0}", idx), edge.To.Node.Id);
//                command.Parameters.Add(string.Format("weight_{0}", idx), edge.Weight);
//                idx++;
//            }
//            if (idx > 0)
//            {
//                sql_builder.Append("select * from dual");

//                command.CommandText = sql_builder.ToString();
//                command.ExecuteNonQuery();
//            }
//            _edges_to_insert.Clear();
//        }

//        #endregion

//        #region Vertices

//        /// <summary>
//        /// Keeps a list of inserted vertices.
//        /// </summary>
//        private HashSet<GraphVertex> inserted_vertices = new HashSet<GraphVertex>();

//        /// <summary>
//        /// Keeps a list of vertices to insert.
//        /// </summary>
//        private HashSet<GraphVertex> _vertices_to_insert = new HashSet<GraphVertex>();

//        /// <summary>
//        /// Inserts an edge into the target db.
//        /// </summary>
//        /// <param name="edge"></param>
//        private void InsertVertex(GraphVertex vertex)
//        {
//            if (!inserted_vertices.Contains(vertex))
//            {
//                _vertices_to_insert.Add(vertex);
//                inserted_vertices.Add(vertex);
//            }
//        }



//        private HashSet<long> _vertices_to_update;

//        private int _vertices_to_update_level;

//        private void UpdateVertex(int level, GraphVertex vertex)
//        {
//            _vertices_to_update_level = level;
//            _vertices_to_update.Add(vertex.Node.Id);
//            if (_vertices_to_update.Count > 100000)
//            {
//                this.FlushVertices();
//            }
//        }

//        /// <summary>
//        /// Flushes all vertices to the database.
//        /// </summary>
//        private void FlushVertices()
//        {
//            OracleCommand command = null;
//            string sql;
//            if (_vertices_to_insert.Count > 0)
//            {
//                command = new OracleCommand();
//                command.Connection = _connection;

//                StringBuilder sql_builder = new StringBuilder("insert all ");
//                sql = "  INTO hh_node (id, level_idx, core, contracted_idx) "
//                    + "  VALUES (:id_{0}, 1, 0, 0) ";
//                int idx = 0;
//                foreach (GraphVertex vertex in _vertices_to_insert)
//                {
//                    sql_builder.Append(string.Format(sql, idx));

//                    command.Parameters.Add(string.Format("id_{0}", idx), vertex.Node.Id);
//                    idx++;
//                }
//                if (idx > 0)
//                {
//                    sql_builder.Append("select * from dual");

//                    command.CommandText = sql_builder.ToString();
//                    command.ExecuteNonQuery();
//                }
//                _vertices_to_insert.Clear();
//            }
//            if (_vertices_to_update.Count > 0)
//            {
//                command = new OracleCommand();
//                command.Connection = _connection;

//                sql = "  update hh_node set level_idx = :level_idx "
//                    + "  where id in (:ids) ";
//                command.CommandText = sql;
//                command.Parameters.Add("level_idx", _vertices_to_update_level);
//                command.AddParameterCollection<long>("ids", _vertices_to_update);
//                command.ExecuteNonQuery();

//                _vertices_to_update.Clear();
//            }
//        }

//        #endregion


//        //private void UpsertVertex(int level, GraphVertex vertex, bool core)
//        //{
//        //    // try to update first.
//        //    string sql = "UPDATE hh_node n "
//        //        + "SET  level_idx = :level_idx, core = :core "
//        //        + "WHERE n.id = :node_id ";
//        //        //+ "IF SQL%NOTFOUND then "
//        //        //+ "  INSERT INTO hh_node n (id, level_idx, core) "
//        //        //+ "  VALUES (:id, :level_idx, :core) "
//        //        //+ "END IF ";
//        //    OracleCommand command = new OracleCommand(sql);
//        //    command.Connection = _connection;
//        //    command.Parameters.Add("level_idx", level);
//        //    command.Parameters.Add("core", core ? 1 : 0);
//        //    command.Parameters.Add("node_id", vertex.Node.Id);
//        //    if (0 == command.ExecuteNonQuery())
//        //    { // insert.            
//        //        sql = "  INSERT INTO hh_node n (id, level_idx, core, contracted_idx) "
//        //            + "  VALUES (:id, :level_idx, :core, :contracted_idx) ";
//        //        command = new OracleCommand(sql);
//        //        command.Connection = _connection;
//        //        command.Parameters.Add("id", vertex.Node.Id);
//        //        command.Parameters.Add("level_idx", level);
//        //        command.Parameters.Add("core", core ? 1 : 0);
//        //        command.Parameters.Add("contracted_idx", level - 1);

//        //        command.ExecuteNonQuery();
//        //    }
//        //}

//        HashSet<HighwayEdge> IHighwayHierarchy.GetNeigbours(int level, 
//            GraphVertex vertex, HashSet<GraphVertex> exceptions)
//        {
//            switch (level)
//            {
//                case 0:
//                    // get the level 0 neighbours.
//                    HashSet<VertexAlongEdge<Osm.Core.Way, GraphVertex>> level_0_neighbours =
//                        _level_0_graph.GetNeighbours(vertex, exceptions);

//                    return this.Convert(vertex, level_0_neighbours);
//                default:
//                    // get the neighbours at the given level.
//                    HashSet<HighwayEdge> edges = new HashSet<HighwayEdge>();

//                    string sql = "select e.* " +
//                            "from hh_edge e " +
//                            "inner join hh_node nf " +
//                            "on nf.id = e.node_from " +
//                            "inner join hh_node nt " +
//                            "on nt.id = e.node_to " +
//                            "and ((nt.level_idx > nt.contracted_idx) " + // not contracted yet.
//                            "or ((nt.level_idx = nt.contracted_idx) and nt.core = 1)) " + // part of the core and contracted.
//                            "where e.node_from = :node_from " +
//                            "and nt.level_idx = :level_idx";
//                    OracleCommand com = new OracleCommand(sql);
//                    com.Connection = _connection;
//                    com.Parameters.Add("node_from", vertex.Node.Id);
//                    com.Parameters.Add("level_idx", level);

//                    OracleDataReader reader = com.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        long node_to = (long)reader["node_to"];
//                        float weight = (float)(decimal)reader["weight"];

//                        GraphVertex to = new GraphVertex(_osm_data.GetNode(node_to));

//                        HighwayEdge edge = new HighwayEdge();
//                        edge.From = vertex;
//                        edge.To = to;
//                        edge.Weight = weight;

//                        edges.Add(edge);
//                    }
//                    reader.Close();
//                    reader.Dispose();

//                    return edges;
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

//        HashSet<HighwayEdge> IHighwayHierarchy.GetNeigboursReversed(int level, GraphVertex vertex, HashSet<GraphVertex> exceptions)
//        {
//            switch (level)
//            {
//                case 0:
//                    // TODO: to fix this try to create a graph based on OSM data using highway edge as an edge.
//                    // get the level 0 neighbours.
//                    HashSet<VertexAlongEdge<Osm.Core.Way, GraphVertex>> level_0_neighbours =
//                        _level_0_graph.GetNeighboursReversed(vertex, exceptions);

//                    return this.Convert(vertex, level_0_neighbours);
//                default:
//                    // get the neighbours at the given level.
//                    HashSet<HighwayEdge> edges = new HashSet<HighwayEdge>();
//                    string sql = "select e.* " +
//                        "from hh_edge e " +
//                        "inner join hh_node nf " +
//                        "on nf.id = e.node_from " +
//                        "inner join hh_node nt " +
//                        "on nt.id = e.node_to " +
//                        "and ((nf.level_idx > nf.contracted_idx) " + // not contracted yet.
//                         "or ((nf.level_idx = nf.contracted_idx) and nf.core = 1)) " + // part of the core and contracted.
//                        "where e.node_to = :node_to " +
//                        "and nf.level_idx = :level_idx ";
//                    OracleCommand com = new OracleCommand(sql);
//                    com.Connection = _connection;
//                    com.Parameters.Add("node_to", vertex.Node.Id);
//                    com.Parameters.Add("level_idx", level);

//                    OracleDataReader reader = com.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        long node_from = (long)reader["node_from"];
//                        float weight = (float)(decimal)reader["weight"];

//                        GraphVertex from = new GraphVertex(_osm_data.GetNode(node_from));

//                        HighwayEdge edge = new HighwayEdge();
//                        edge.From = from;
//                        edge.To = vertex;
//                        edge.Weight = weight;

//                        edges.Add(edge);
//                    }
//                    reader.Close();
//                    reader.Dispose();

//                    return edges;
//            }
//        }

//        public void ClearTarget()
//        {
//            // clear all highway nodes.
//            string sql = "delete from hh_node";
//            OracleCommand command = new OracleCommand(sql);
//            command.Connection = _connection;
//            command.ExecuteNonQuery();

//            // clear all highway edges.
//            sql = "delete from hh_edge";
//            command = new OracleCommand(sql);
//            command.Connection = _connection;
//            command.ExecuteNonQuery();
//        }


//        public void StartCore(int _level)
//        {
//            _uncontracted = new HashSet<GraphVertex>();

//            this.FlushVertices();
//            this.FlushEdges();
//        }
//    }
//}
