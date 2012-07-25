using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.Sparse;
using Osm.Data.Core.Sparse.Primitives;
using Oracle.DataAccess.Client;

namespace Osm.Data.Oracle.Sparse
{
    public class OracleSparseData : ISparseData
    {
        private OracleConnection _connection;

        private OracleCommand BuildCommand()
        {
            OracleCommand command = new OracleCommand();
            command.Connection = _connection;
            return command;
        }

        public SparseVertex GetSparseVertex(long id)
        {
            string sql = "select * from sparse s where id = :id ";
            OracleCommand command = this.BuildCommand();
            command.CommandText = sql;
            command.Parameters.Add("id", id);

            OracleDataReader reader = command.ExecuteReader();
            SparseVertex vertex = null;
            while (reader.Read())
            {
                long latitude_long = (long)reader["latitude"];
                long longitude_long = (long)reader["longitude"];

                vertex = new SparseVertex();
                vertex.Id = id;
                vertex.Coordinates[0] = (double)latitude_long / 10000000.0;
                vertex.Coordinates[1] = (double)longitude_long / 10000000.0;
            }

            sql = "select * from sparse_neighbours sn where vertex_id = :id ";
            command = this.BuildCommand();
            command.CommandText = sql;
            command.Parameters.Add("id", id);

            reader = command.ExecuteReader();
            while (reader.Read())
            {
                SparseVertexNeighbour neighbour = new SparseVertexNeighbour();

                long vertex_id = (long)reader["vertex_id"];
                double weight = (double)reader["weight"];
                int? directed = (int)reader["directed"];

                neighbour.Id = vertex_id;
                neighbour.Tags = null;
                neighbour.Weight = weight;
            }
            return vertex;
        }

        public List<SparseVertex> GetSparseVertices(IList<long> ids)
        {
            throw new NotImplementedException();
        }

        public void PersistSparseVertex(SparseVertex vertex)
        {
            string sql = "insert into sparse s (id, latitude, longitude) values (:id, :latitude, :longitude) ";
            OracleCommand command = this.BuildCommand();
            command.CommandText = sql;
            command.Parameters.Add("id", vertex.Id);
            command.Parameters.Add("latitude", (long)vertex.Coordinates[0] * 10000000.0);
            command.Parameters.Add("longitude", (long)vertex.Coordinates[1] * 10000000.0);

            command.ExecuteNonQuery();

            foreach (SparseVertexNeighbour neighbour in vertex.Neighbours)
            {
                sql = "insert into sparse_neighbours s (id, vertex_id, weight, directed) values (:id, :vertex_id, :weight, :directed) ";
                command = this.BuildCommand();
                command.CommandText = sql;
                command.Parameters.Add("id", vertex.Id);
                command.Parameters.Add("vertex_id", neighbour.Id);
                command.Parameters.Add("weight", neighbour.Weight);
                if (!neighbour.Directed)
                {
                    command.Parameters.Add("directed", DBNull.Value);
                }
                else
                {
                    command.Parameters.Add("directed", neighbour.Direction);
                }
                command.ExecuteNonQuery();

                for (int idx = 0; idx < neighbour.Nodes.Length; idx++)
                {
                    sql = "insert into sparse_neighbour_nodes s (id, neighbour_id, idx, node_id) values (:id, :neighbour_id, :idx, :node_id) ";
                    command = this.BuildCommand();
                    command.CommandText = sql;
                    command.Parameters.Add("id", vertex.Id);
                    command.Parameters.Add("neighbour_id", neighbour.Id);
                    command.Parameters.Add("idx", idx);
                    command.Parameters.Add("node_id", neighbour.Nodes[idx]);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteSparseVertex(long id)
        {
            string sql = "delete from sparse_neighbour_nodes where id = :id";

            OracleCommand command = this.BuildCommand();
            command.CommandText = sql;
            command.Parameters.Add("id", id);

            command.ExecuteNonQuery();

            sql = "delete from sparse_neighbours where id = :id";

            command = this.BuildCommand();
            command.CommandText = sql;
            command.Parameters.Add("id", id);

            command.ExecuteNonQuery();

            sql = "delete from sparse where id = :id";

            command = this.BuildCommand();
            command.CommandText = sql;
            command.Parameters.Add("id", id);

            command.ExecuteNonQuery();
        }

        #region Sparse Simple 

        public void PersistSparseSimpleVertex(SparseSimpleVertex vertex)
        {
            string sql = "insert into simple_vertex v (id, latitude, longitude) values (:id, :latitude, :longitude) ";
            OracleCommand command = this.BuildCommand();
            command.CommandText = sql;
            command.Parameters.Add("id", vertex.Id);
            command.Parameters.Add("latitude", (long)vertex.Latitude * 10000000.0);
            command.Parameters.Add("longitude", (long)vertex.Longitude * 10000000.0);

            command.ExecuteNonQuery();
        }

        public SparseSimpleVertex GetSparseSimpleVertex(long id)
        {
            string sql = "select * from sparse_simple sv where sv.id = :id ";
            OracleCommand command = this.BuildCommand();
            command.CommandText = sql;
            command.Parameters.Add("id");

            List<long> nodes = new List<long>();
            OracleDataReader reader = command.ExecuteReader();
            SparseSimpleVertex vertex = null;
            while (reader.Read())
            {
                long latitude_long = (long)reader["latitude"];
                long longitude_long = (long)reader["longitude"];
                long neighbour1 = (long)reader["neighbour1"];
                long neighbour2 = (long)reader["neighbour2"];

                vertex = new SparseSimpleVertex();
                vertex.Id = id;
                vertex.Latitude = (double)latitude_long / 10000000.0;
                vertex.Longitude = (double)longitude_long / 10000000.0;
                vertex.Neighbour1 = neighbour1;
                vertex.Neighbour2 = neighbour2;
            }
            return vertex;
        }

        public List<SparseSimpleVertex> GetSparseSimpleVertices(Tools.Math.Geo.GeoCoordinateBox box)
        {
            throw new NotImplementedException();
        }

        public List<SparseSimpleVertex> GetSparseSimpleVertices(IList<long> ids)
        {
            List<SparseSimpleVertex> vertices = new List<SparseSimpleVertex>();
            foreach (long arc_id in ids)
            {
                vertices.Add(this.GetSparseSimpleVertex(arc_id));
            }
            return vertices;
        }

        public void DeleteSparseSimpleVertex(long id)
        {
            string sql = "delete from sparse_simple where id = :id";

            OracleCommand command = this.BuildCommand();
            command.CommandText = sql;
            command.Parameters.Add("id", id);

            command.ExecuteNonQuery();
        }


        #endregion

        #region Simple Vertices/Arcs

        public void PersistSimpleVertex(SimpleVertex vertex)
        {
            string sql = "insert into simple_vertex v (id, latitude, longitude) values (:id, :latitude, :longitude) ";
            OracleCommand command = this.BuildCommand();
            command.CommandText = sql;
            command.Parameters.Add("id", vertex.Id);
            command.Parameters.Add("latitude", (long)vertex.Latitude * 10000000.0);
            command.Parameters.Add("longitude", (long)vertex.Longitude * 10000000.0);

            command.ExecuteNonQuery();
        }

        public SimpleVertex GetSimpleVertex(long id)
        {
            string sql = "select * from simple_vertex sv where sv.id = :id ";
            OracleCommand command = this.BuildCommand();
            command.CommandText = sql;
            command.Parameters.Add("id");

            List<long> nodes = new List<long>();
            OracleDataReader reader = command.ExecuteReader();
            SimpleVertex vertex = null;
            while (reader.Read())
            {
                long latitude_long = (long)reader["latitude"];
                long longitude_long = (long)reader["longitude"];

                vertex = new SimpleVertex();
                vertex.Id = id;
                vertex.Latitude = (double)latitude_long / 10000000.0;
                vertex.Longitude = (double)longitude_long / 10000000.0;
            }
            return vertex;
        }

        public List<SimpleVertex> GetSimpleVertices(IList<long> ids)
        {
            List<SimpleVertex> vertices = new List<SimpleVertex>();
            foreach (long arc_id in ids)
            {
                vertices.Add(this.GetSimpleVertex(arc_id));
            }
            return vertices;
        }

        public void DeleteSimpleVertex(long id)
        {
            string sql = "delete from simple_vertex where id = :id";

            OracleCommand command = this.BuildCommand();
            command.CommandText = sql;
            command.Parameters.Add("id", id);

            command.ExecuteNonQuery();
        }

        public void PersistSimpleArc(SimpleArc arc)
        {
            this.DeleteSimpleArc(arc.Id);

            int idx = 0;
            foreach (long vertex_id in arc.Nodes)
            {
                string sql = "insert into simple_arc (id, vertex_id, idx) value (:id, :vertex_id, :idx) ";

                OracleCommand command = this.BuildCommand();
                command.CommandText = sql;
                command.Parameters.Add("id", arc.Id);
                command.Parameters.Add("vertex_id", vertex_id);
                command.Parameters.Add("idx", idx);

                command.ExecuteNonQuery();

                idx++;
            }
        }

        public SimpleArc GetSimpleArc(long id)
        {
            string sql = "select * from simple_arc sa where sa.id = :id order by idx asc ";
            OracleCommand command = this.BuildCommand();
            command.CommandText = sql;
            command.Parameters.Add("id");

            List<long> nodes = new List<long>();
            OracleDataReader reader = command.ExecuteReader();
            SimpleArc arc = null;
            while (reader.Read())
            {
                long vertex_id = (long)reader["vertex_id"];
                nodes.Add(vertex_id);
            }
            if (nodes.Count > 0)
            {
                arc = new SimpleArc();
                arc.Id = id;
                arc.Nodes = nodes.ToArray();
            }
            return arc;
        }

        public List<SimpleArc> GetSimpleArcs(IList<long> ids)
        {
            List<SimpleArc> arcs = new List<SimpleArc>();
            foreach (long arc_id in ids)
            {
                arcs.Add(this.GetSimpleArc(arc_id));
            }
            return arcs;
        }

        public void DeleteSimpleArc(long id)
        {
            string sql = "delete from simple_arc where id = :id";

            OracleCommand command = this.BuildCommand();
            command.CommandText = sql;
            command.Parameters.Add("id", id);

            command.ExecuteNonQuery();
        }

        #endregion
    }
}
