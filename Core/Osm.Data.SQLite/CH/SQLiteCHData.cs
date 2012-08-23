using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Osm.Data.Core.CH;
using Osm.Data.Core.CH.Primitives;
using Osm.Data.SQLite.Raw.Processor;

namespace Osm.Data.Oracle.CH
{

	public class SQLiteCHData : ICHData
	{
		private readonly string _connection_string;
		private SQLiteConnection _connection;
		private readonly SQLiteCommand _insertCmdVertex;
		private readonly SQLiteCommand _selectOneCmd;
		private readonly SQLiteCommand _selectManyCmd;
		private readonly SQLiteCommand _selectNeighboursCmd;
		private readonly SQLiteCommand _insertCmdNeighbour;
		
		private const int max_cache_nodes = 1000;
		private readonly Dictionary<long, CHVertex> _cache = new Dictionary<long, CHVertex>(max_cache_nodes);

		private SQLiteCommand BuildCommand()
		{
			if (_connection != null)
				return _connection.CreateCommand();
			
			_connection = new SQLiteConnection(_connection_string);
			_connection.Open();
			using (SQLiteCommand sqlite_cmd = _connection.CreateCommand())
			{
				sqlite_cmd.CommandText =
					@"CREATE TABLE IF NOT EXISTS [vertex] ([id] INTEGER  NOT NULL PRIMARY KEY,[latitude] INTEGER  NULL,[longitude] INTEGER NULL,[level_n] int NULL,[tile] INTEGER NULL); " +
					@"CREATE TABLE IF NOT EXISTS [vertex_neighbour] ([vertex_id] INTEGER  NOT NULL,[neighbour_id] INTEGER NOT NULL,[weight] INTEGER NULL,[forward] int NULL, [contracted_vertex_id] INTEGER NULL); " +
					@"CREATE INDEX IF NOT EXISTS [IDX_VERTEX_LAT] ON [vertex]([latitude] ASC); " +
					@"CREATE INDEX IF NOT EXISTS [IDX_VERTEX_LON] ON [vertex]([longitude] ASC); " +
					@"CREATE INDEX IF NOT EXISTS [IDX_VERTEX_NEIGHBOUR] ON [vertex_neighbour]([vertex_id] ASC); " +
					@"";
				sqlite_cmd.ExecuteNonQuery();
			}
			return _connection.CreateCommand();
		}

		public SQLiteCHData(string connection_string)
		{
			_connection_string = connection_string;

			_insertCmdVertex = BuildCommand();
			_insertCmdVertex.CommandText = @"insert or replace into vertex (id, latitude, longitude, level_n, tile) values (:id, :latitude, :longitude, :level_n, :tile) ";
			_insertCmdVertex.Parameters.Add("id", DbType.Int64);
			_insertCmdVertex.Parameters.Add("latitude", DbType.Int64);
			_insertCmdVertex.Parameters.Add("longitude", DbType.Int64);
			_insertCmdVertex.Parameters.Add("level_n", DbType.Int32);
			_insertCmdVertex.Parameters.Add("tile", DbType.Int64);

			_insertCmdNeighbour = BuildCommand();
			_insertCmdNeighbour.CommandText = @"insert or replace into vertex_neighbour (vertex_id, neighbour_id, weight, forward, contracted_vertex_id) values (:vertex_id, :neighbour_id, :weight, :forward, :contracted_vertex_id) ";
			_insertCmdNeighbour.Parameters.Add("vertex_id", DbType.Int64);
			_insertCmdNeighbour.Parameters.Add("neighbour_id", DbType.Int64);
			_insertCmdNeighbour.Parameters.Add("weight", DbType.Int64);
			_insertCmdNeighbour.Parameters.Add("forward", DbType.Int32);
			_insertCmdNeighbour.Parameters.Add("contracted_vertex_id", DbType.Int64);

			_selectOneCmd = BuildCommand();
			_selectOneCmd.CommandText = @"select id, latitude, longitude, level_n from vertex where id = :id ";
			_selectOneCmd.Parameters.Add("id", DbType.Int64);

			_selectManyCmd = BuildCommand();
			_selectManyCmd.CommandText = @"select id, latitude, longitude, level_n from vertex where longitude <= :maxLon AND longitude >= :minLon AND latitude <= :maxLat AND latitude >= :minLat;";
			_selectManyCmd.Parameters.Add("maxLon", DbType.Int64);
			_selectManyCmd.Parameters.Add("minLon", DbType.Int64);
			_selectManyCmd.Parameters.Add("maxLat", DbType.Int64);
			_selectManyCmd.Parameters.Add("minLat", DbType.Int64);

			_selectNeighboursCmd = BuildCommand();
			_selectNeighboursCmd.CommandText = @"select vertex_id, neighbour_id, weight, forward, contracted_vertex_id from vertex_neighbour where vertex_id = :id";
			_selectNeighboursCmd.Parameters.Add("id", DbType.Int64);
		}

		public CHVertex GetCHVertex(long id)
		{
			CHVertex vertex = null;
			_selectOneCmd.Parameters[0].Value = id;
			using (SQLiteDataReader reader = _selectOneCmd.ExecuteReader())
			{
				if (reader.Read())
				{
					long latitude_long = (long) reader["latitude"];
					long longitude_long = (long) reader["longitude"];
					int level = int.MaxValue;
					if (reader["level_n"] != DBNull.Value)
						level = (int) reader["level_n"];

					vertex = new CHVertex();
					vertex.Id = id;
					vertex.Latitude = latitude_long/10000000.0;
					vertex.Longitude = longitude_long/10000000.0;
					vertex.Level = level;
					reader.Close();
				}
			}
			if(vertex == null)
			return vertex;

			GetNeighbours(vertex);
			return vertex;
		}

		private void GetNeighbours(CHVertex vertex)
		{
			//Get neighbours
			_selectNeighboursCmd.Parameters[0].Value = vertex.Id;
			using (SQLiteDataReader readnb = _selectNeighboursCmd.ExecuteReader())
			{
				while (readnb.Read())
					AddNeighbours(vertex, readnb);
				readnb.Close();
			}
		}

		private static void AddNeighbours(CHVertex vertex, SQLiteDataReader readnb)
		{
			CHVertexNeighbour nb = new CHVertexNeighbour {Id = readnb.GetInt64(1), Weight = (float) (readnb.GetInt64(2)/10000000.0), ContractedVertexId = readnb.IsDBNull(4)? -1 : readnb.GetInt64(4)};
			if (readnb.GetInt32(3) == 1)
				vertex.ForwardNeighbours.Add(nb);
			else
				vertex.BackwardNeighbours.Add(nb);
		}

		public IEnumerable<CHVertex> GetCHVertices(Tools.Math.Geo.GeoCoordinateBox box)
		{
			_selectManyCmd.Parameters[0].Value = box.MaxLon*10000000.0;
			_selectManyCmd.Parameters[1].Value = box.MinLon*10000000.0;
			_selectManyCmd.Parameters[2].Value = box.MaxLat*10000000.0;
			_selectManyCmd.Parameters[3].Value = box.MinLat*10000000.0;
			string idsin = string.Empty;
			HashSet<CHVertex> vertices_in_box = new HashSet<CHVertex>();
			Dictionary<long, CHVertex> dic = new Dictionary<long, CHVertex>();
			using (SQLiteDataReader reader = _selectManyCmd.ExecuteReader())
			{
				while (reader.Read())
				{
					long latitude_long = (long) reader["latitude"];
					long longitude_long = (long) reader["longitude"];

					CHVertex vertex = new CHVertex();
					vertex.Id = (long) reader["id"];
					vertex.Latitude = latitude_long/10000000.0;
					vertex.Longitude = longitude_long/10000000.0;
					vertex.Level = reader["level_n"] != DBNull.Value ? (int) reader["level_n"] : int.MaxValue;
					vertices_in_box.Add(vertex);
					dic.Add(vertex.Id, vertex);
					idsin = string.IsNullOrEmpty(idsin) ? vertex.Id.ToString() : idsin + "," + vertex.Id;
				}
				reader.Close();
			}

			//Get neigbours
			if (!string.IsNullOrEmpty(idsin))
			{
				using (SQLiteCommand cmd = BuildCommand())
				{
					cmd.CommandText = string.Format(@"select vertex_id, neighbour_id, weight, forward, contracted_vertex_id from vertex_neighbour where vertex_id IN ({0}); ", idsin);
					using (SQLiteDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							CHVertex vertex = dic[reader.GetInt64(0)];
							AddNeighbours(vertex, reader);
						}
						reader.Close();
					}
				}
			}

			return vertices_in_box.Where(v => v.BackwardNeighbours.Count > 0 || v.ForwardNeighbours.Count > 0);
		}

		public IEnumerable<CHVertex> GetCHVerticesNoLevel()
		{
			using (SQLiteCommand cmd = BuildCommand())
			{
				cmd.CommandText = @"select id, latitude, longitude, level_n from vertex where level_n is null;";
				using (SQLiteDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						CHVertex vertex = new CHVertex { Id = reader.GetInt64(0), Latitude = reader.GetInt64(1) / 10000000.0, Longitude = reader.GetInt64(2) / 10000000.0, Level = reader.IsDBNull(3)? int.MaxValue : reader.GetInt32(3)};
						GetNeighbours(vertex);
						yield return vertex;
					}
					reader.Close();
				}
			}
		}

		public void PersistCHVertex(CHVertex vertex)
		{
			_insertCmdVertex.Parameters[0].Value = vertex.Id;
			_insertCmdVertex.Parameters[1].Value = vertex.Latitude*10000000.0;
			_insertCmdVertex.Parameters[2].Value = vertex.Longitude*10000000.0;
			_insertCmdVertex.Parameters[3].Value = vertex.Level == int.MaxValue ? DBNull.Value : (object) vertex.Level;
			// calculate the tile.
			_insertCmdVertex.Parameters[4].Value = TileCalculations.xy2tile(TileCalculations.lon2x(vertex.Longitude), TileCalculations.lat2y(vertex.Latitude));
			_insertCmdVertex.ExecuteNonQuery();
		}


		public void PersistCHVertexNeighbour(CHVertex vertex, CHVertexNeighbour arc, bool forward)
		{
			_insertCmdNeighbour.Parameters[0].Value = vertex.Id;
			_insertCmdNeighbour.Parameters[1].Value = arc.Id;
			_insertCmdNeighbour.Parameters[2].Value = arc.Weight * 10000000.0;
			_insertCmdNeighbour.Parameters[3].Value = forward? 1: 0;
			_insertCmdNeighbour.Parameters[4].Value = arc.ContractedVertexId <= 1? DBNull.Value: (object)arc.ContractedVertexId;
			if (arc.ContractedVertexId == 1)
			{
			}
			_insertCmdNeighbour.ExecuteNonQuery();
		}

		public void DeleteCHVertex(long id)
		{
			string sql = "delete from vertex where id = :id";
			using (SQLiteCommand command = this.BuildCommand())
			{
				command.CommandText = sql;
				command.Parameters.Add("id", DbType.Int64);
				command.Parameters[0].Value = id;
				command.ExecuteNonQuery();
			}
		}

		public void DeleteNeighbours(long vertexid)
		{
			string sql = "delete from vertex_neighbour where vertex_id = :vertex_id";
			using (SQLiteCommand command = this.BuildCommand())
			{
				command.CommandText = sql;
				command.Parameters.Add("vertex_id", DbType.Int64);
				command.Parameters[0].Value = vertexid;
				command.ExecuteNonQuery();
			}
		}

		public void DeleteNeighbour(CHVertex vertex, CHVertexNeighbour neighbour, bool forward)
		{
			string sql = "delete from vertex_neighbour where vertex_id = :vertex_id and neighbour_id= :neighbour_id and forward = :forward";
			using (SQLiteCommand command = this.BuildCommand())
			{
				command.CommandText = sql;
				command.Parameters.Add("vertex_id", DbType.Int64);
				command.Parameters[0].Value = vertex.Id;
				command.Parameters.Add("neighbour_id", DbType.Int64);
				command.Parameters[1].Value = neighbour.Id;
				command.Parameters.Add("forward", DbType.Int32);
				command.Parameters[1].Value = forward?1:0;
				command.ExecuteNonQuery();
			}
		}
	}
}
