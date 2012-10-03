//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Data.Core.CH;
//using Osm.Data.Core.CH.Primitives;
//using Oracle.DataAccess.Client;
//using Osm.Data.Oracle.Raw.Processor;
//using Tools.Core.Collections;

//namespace Osm.Data.Oracle.CH
//{
//    public class OracleCHData : ICHData
//    {
//        private OracleConnection _connection;

//        private string _connection_string;

//        private OracleCommand BuildCommand()
//        {
//            OracleCommand command = new OracleCommand();
//            if (_connection == null)
//            {
//                _connection = new OracleConnection(_connection_string);
//                _connection.Open();
//            }
//            command.Connection = _connection;
//            return command;
//        }

//        public OracleCHData(string connection_string)
//        {
//            _connection_string = connection_string;
//        }

//        public CHVertex GetCHVertex(long id)
//        {
//            string sql = "select * from vertex where id = :id ";
//            OracleCommand command = this.BuildCommand();
//            command.CommandText = sql;
//            command.Parameters.Add("id", id);

//            OracleDataReader reader = command.ExecuteReader();
//            CHVertex vertex = null;
//            while (reader.Read())
//            {
//                long latitude_long = (long) reader["latitude"];
//                long longitude_long = (long) reader["longitude"];
//                int level = int.MaxValue;
//                if (reader["level_n"] != DBNull.Value)
//                    level = (int) reader["level_n"];


//                vertex = new CHVertex();
//                vertex.Id = id;
//                vertex.Latitude = (float) (latitude_long/10000000.0);
//                vertex.Longitude = (float) (longitude_long / 10000000.0);
//                vertex.Level = level;
//            }
//            reader.Close();
//            return vertex;
//        }

//        public IEnumerable<CHVertex> GetCHVertices(Tools.Math.Geo.GeoCoordinateBox box)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<CHVertex> GetCHVerticesNoLevel()
//        {
//            throw new NotImplementedException();
//        }

//        public void PersistCHVertex(CHVertex vertex)
//        {
//            this.DeleteCHVertex(vertex.Id);

//            // calculate the tile.
//            long tile = TileCalculations.xy2tile(
//                TileCalculations.lon2x(vertex.Longitude),
//                TileCalculations.lat2y(vertex.Latitude));
//            string sql = "insert into vertex (id, latitude, longitude, level_n, tile) values (:id, :latitude, :longitude, :level_n, :tile) ";
//            OracleCommand command = this.BuildCommand();
//            command.CommandText = sql;
//            command.Parameters.Add("id", vertex.Id);
//            command.Parameters.Add("latitude", vertex.Latitude*10000000.0);
//            command.Parameters.Add("longitude", vertex.Longitude*10000000.0);
//            if (vertex.Level == int.MaxValue)
//            {
//                command.Parameters.Add("level_n", DBNull.Value);
//            }
//            else
//            {
//                command.Parameters.Add("level_n", vertex.Level);
//            }
//            command.Parameters.Add("tile", tile);
//            command.ExecuteNonQuery();
//        }

//        public void DeleteCHVertex(long id)
//        {
//            string sql = "delete from vertex where id = :id";
//            OracleCommand command = this.BuildCommand();
//            command.CommandText = sql;
//            command.Parameters.Add("id", id);
//            command.ExecuteNonQuery();
//        }

//        public void DeleteNeighbours(long vertexid)
//        {
//            throw new NotImplementedException();
//        }

//        public void DeleteNeighbour(CHVertex vertex, CHVertexNeighbour neighbour, bool forward)
//        {
//            throw new NotImplementedException();
//        }

//        public void PersistCHVertexNeighbour(CHVertex vertex, CHVertexNeighbour arc, bool forward)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}