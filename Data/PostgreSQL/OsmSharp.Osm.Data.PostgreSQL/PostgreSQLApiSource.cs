// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm;
using Npgsql;
using OsmSharp.Osm.Factory;
using OsmSharp.Osm.Filters;
using System.Text.RegularExpressions;

namespace OsmSharp.Osm.Data.PostgreSQL
{
    /// <summary>
    /// Reads OSM data from an pre-imported API database.
    /// 
    /// http://wiki.openstreetmap.org/wiki/Rails_port/Database_schema
    /// </summary>
    public class PostgreSQLApiSource : IDataSourceReadOnly, IDisposable
    {
        /// <summary>
        /// Holds the connection string.
        /// </summary>
        private string _connection_string;

        /// <summary>
        /// Id of this data source.
        /// </summary>
        private Guid _id;

        /// <summary>
        /// Creates a new API datasource.
        /// </summary>
        /// <param name="connection_string"></param>
        public PostgreSQLApiSource(string connection_string)
        {
            _connection_string = connection_string;
            _id = Guid.NewGuid();
        }

        /// <summary>
        /// The postgresql connection.
        /// </summary>
        private NpgsqlConnection _connection;

        /// <summary>
        /// Creates/gets the connection.
        /// </summary>
        /// <returns></returns>
        private NpgsqlConnection CreateConnection()
        {
            if (_connection == null)
            {
                _connection = new NpgsqlConnection(_connection_string);
                _connection.Open();
            }
            return _connection;
        }

        #region IDataSourceReadOnly Members

        /// <summary>
        /// Returns the bounding box of the data in this rouce.
        /// </summary>
        public GeoCoordinateBox BoundingBox
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// The name of this data source.
        /// </summary>
        public string Name
        {
            get
            {
                return "PostgreSQL API Data Source";
            }
        }

        /// <summary>
        /// The id of this data source.
        /// </summary>
        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Return true if there is a boundingbox.
        /// </summary>
        public bool HasBoundinBox
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Node GetNode(long id)
        {
            IList<Node> nodes = this.GetNodes(new List<long>(new long[] { id }));
            if (nodes.Count > 0)
            {
                return nodes[0];
            }
            return null;
        }

        /// <summary>
        /// Returns all nodes with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Node> GetNodes(IList<long> ids)
        {
            IList<Node> return_list = new List<Node>();
            if (ids.Count > 0)
            {
                // initialize connection.
                NpgsqlConnection con = this.CreateConnection();
                // STEP 1: query nodes table.
                //id	latitude	longitude	changeset_id	visible	timestamp	tile	version
                string sql
                    = "SELECT * FROM current_nodes WHERE (current_nodes.id IN ({0})) ";
                ;
                sql = string.Format(sql, this.ConstructIdList(ids));

                NpgsqlCommand com = new NpgsqlCommand(sql);
                com.Connection = con;
                NpgsqlDataReader reader = com.ExecuteReader();
                Node node = null;
                Dictionary<long, Node> nodes = new Dictionary<long, Node>();
                List<long> node_ids = new List<long>();
                while (reader.Read())
                {
                    // load/parse data.
                    long returned_id = reader.GetInt64(0);
                    int latitude_int = reader.GetInt32(1);
                    int longitude_int = reader.GetInt32(2);
                    long changeset_id = reader.GetInt64(3);
                    bool visible = reader.GetBoolean(4);
                    DateTime timestamp = reader.GetDateTime(5);
                    long tile = reader.GetInt64(6);
                    long version = reader.GetInt64(7);

                    // create node.
                    node = OsmBaseFactory.CreateNode(returned_id);
                    node.Version = version;
                    //node.UserId = user_id;
                    node.TimeStamp = timestamp;
                    node.ChangeSetId = changeset_id;
                    node.Coordinate = new GeoCoordinate(
                        ((double)latitude_int) / 10000000.0, ((double)longitude_int) / 10000000.0);

                    nodes.Add(node.Id, node);
                    node_ids.Add(node.Id);
                }
                reader.Close();

                // STEP2: Load all node tags.
                this.LoadNodeTags(nodes);

                return_list = nodes.Values.ToList<Node>();
            }
            return return_list;
        }

        /// <summary>
        /// Returns the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Relation GetRelation(long id)
        {
            // TODO: implement this
            return null;
        }

        /// <summary>
        /// Returns all relations with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Relation> GetRelations(IList<long> ids)
        {
            // TODO: implement this
            return new List<Relation>();
        }

        /// <summary>
        /// Returns all relations for the given ids.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IList<Relation> GetRelationsFor(Osm.OsmBase obj)
        {
            // TODO: implement this
            return new List<Relation>();
        }

        /// <summary>
        /// Returns the way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Way GetWay(long id)
        {
            IList<Way> ways = this.GetWays(new List<long>(new long[] { id }));
            if (ways.Count > 0)
            {
                return ways[0];
            }
            return null;
        }

        /// <summary>
        /// Returns all ways with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Way> GetWays(IList<long> ids)
        {
            return this.GetWays(ids, null);
        }

        /// <summary>
        /// Returns all ways that have the given nodes.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private IList<Way> GetWays(IList<long> ids,Dictionary<long,Node> nodes)
        {
            if (ids.Count > 0)
            {
                NpgsqlConnection con = this.CreateConnection();

                // STEP2: Load ways.
                Dictionary<long, Way> ways = new Dictionary<long, Way>();
                string sql = "SELECT * FROM current_ways WHERE (current_ways.id IN ({0})) ";
                sql = string.Format(sql, this.ConstructIdList(ids));
                NpgsqlCommand com = new NpgsqlCommand(sql);
                com.Connection = con;
                NpgsqlDataReader reader = com.ExecuteReader();
                Way way;
                while (reader.Read())
                {
                    long id = reader.GetInt64(0);
                    long changeset_id = reader.GetInt64(1);
                    DateTime timestamp = reader.GetDateTime(2);
                    bool visible = reader.GetBoolean(3);
                    long version = reader.GetInt64(4);

                    // create way.
                    way = OsmBaseFactory.CreateWay(id);
                    way.Version = version;
                    //node.UserId = user_id;
                    way.TimeStamp = timestamp;
                    way.ChangeSetId = changeset_id;

                    ways.Add(way.Id, way);
                }
                reader.Close();

                //STEP3: Load all node-way relations
                sql = "SELECT current_way_nodes.* FROM current_way_nodes WHERE (current_way_nodes.id IN ({0})) ORDER BY sequence_id";
                sql = string.Format(sql, this.ConstructIdList(ids));
                com = new NpgsqlCommand(sql);
                com.Connection = con;
                reader = com.ExecuteReader();
                List<long> missing_node_ids = new List<long>();
                while (reader.Read())
                {
                    long id = reader.GetInt64(0);
                    long node_id = reader.GetInt64(1);
                    long sequence_id = reader.GetInt64(2);

                    if (nodes == null || !nodes.ContainsKey(node_id))
                    {
                        missing_node_ids.Add(node_id);
                    }
                }
                reader.Close();

                //STEP4: Load all missing nodes.
                IList<Node> missing_nodes = this.GetNodes(missing_node_ids);
                Dictionary<long, Node> way_nodes = new Dictionary<long, Node>(nodes);
                foreach (Node node in missing_nodes)
                {
                    way_nodes.Add(node.Id, node);
                }

                //STEP5: assign nodes to way.
                sql = "SELECT current_way_nodes.* FROM current_way_nodes WHERE (current_way_nodes.id IN ({0})) ORDER BY sequence_id";
                sql = string.Format(sql, this.ConstructIdList(ids));
                com = new NpgsqlCommand(sql);
                com.Connection = con;
                reader = com.ExecuteReader();
                while (reader.Read())
                {
                    long id = reader.GetInt64(0);
                    long node_id = reader.GetInt64(1);
                    long sequence_id = reader.GetInt64(2);

                    ways[id].Nodes.Add(way_nodes[node_id]);
                }
                reader.Close();


                //STEP4: Load all tags.
                sql = "SELECT current_way_tags.* FROM current_way_tags WHERE (current_way_tags.id IN ({0})) ";
                sql = string.Format(sql, this.ConstructIdList(ids));
                com = new NpgsqlCommand(sql);
                com.Connection = con;
                reader = com.ExecuteReader();
                while (reader.Read())
                {
                    long id = reader.GetInt64(0);
                    string key = reader.GetString(1);
                    string value = reader.GetString(2);

                    // TODO: check if the node is present in the collection if not load it.
                    ways[id].Tags.Add(key, value);
                }
                reader.Close();

                return ways.Values.ToList<Way>();
            }
            return new List<Way>();
        }

        /// <summary>
        /// Returns all ways that have the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IList<Way> GetWaysFor(Node node)
        {
            Dictionary<long,Node> nodes = new Dictionary<long,Node>();
            nodes.Add(node.Id,node);
            return this.GetWaysForNodes(nodes);
        }

        /// <summary>
        /// Returns all ways that have the given nodes.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public IList<Way> GetWaysForNodes(Dictionary<long,Node> nodes)
        {
            if (nodes.Count > 0)
            {
                NpgsqlConnection con = this.CreateConnection();

                // STEP1: Load ways that exist for the given nodes.
                List<long> way_ids = new List<long>();
                string sql = "SELECT * FROM current_way_nodes WHERE (current_way_nodes.node_id IN ({0})) ";
                sql = string.Format(sql, this.ConstructIdList(nodes.Keys.ToList<long>()));
                NpgsqlCommand com = new NpgsqlCommand(sql);
                com.Connection = con;
                NpgsqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    long id = reader.GetInt64(0);
                    if (!way_ids.Contains(id))
                    {
                        way_ids.Add(id);
                    }
                }
                reader.Close();

                return this.GetWays(way_ids, nodes);
            }
            return new List<Way>();
        }

        #region Tile Calculations

        uint xy2tile(uint x, uint y)
        {
            uint tile = 0;
            int i;

            for (i = 15; i >= 0; i--)
            {
                tile = (tile << 1) | ((x >> i) & 1);
                tile = (tile << 1) | ((y >> i) & 1);
            }

            return tile;
        }

        uint lon2x(double lon)
        {
            return (uint)Math.Floor(((lon + 180.0) * 65536.0 / 360.0));
        }

        uint lat2y(double lat)
        {
            return (uint)Math.Floor(((lat + 90.0) * 65536.0 / 180.0));
        }

        #endregion

        /// <summary>
        /// Returns all objects withing the given bounding box and the objects that are valid according to the filter.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter)
        {
            // initialize connection.
            NpgsqlConnection con = this.CreateConnection();
            List<OsmGeo> base_list = new List<OsmGeo>();

            // calculate bounding box parameters to query db.
            int latitude_min = (int)(box.MinLat * 10000000.0);
            int longitude_min = (int)(box.MinLon * 10000000.0);
            int latitude_max = (int)(box.MaxLat * 10000000.0);
            int longitude_max = (int)(box.MaxLon * 10000000.0);

            // TODO: improve this to allow loading of bigger bb's.
            uint box_1 = xy2tile(lon2x(box.MinLon), lat2y(box.MinLat));
            uint box_2 = xy2tile(lon2x(box.MaxLon), lat2y(box.MaxLat));

            // STEP 1: query nodes table.
            //id	latitude	longitude	changeset_id	visible	timestamp	tile	version
            string sql
                = "SELECT * FROM current_nodes WHERE (current_nodes.visible = 't') AND (( tile IN ({0},{1}) ) AND latitude BETWEEN {2} AND {3} AND longitude BETWEEN {4} AND {5}) LIMIT 50001 ";
            sql = string.Format(sql,
                    box_1.ToString(),
                    box_2.ToString(),
                    latitude_min.ToString(),
                    latitude_max.ToString(),
                    longitude_min.ToString(),
                    longitude_max.ToString());

            // TODO: parameters.
            NpgsqlCommand com = new NpgsqlCommand(sql);
            com.Connection = con;
            NpgsqlDataReader reader = com.ExecuteReader();
            Node node = null;
            Dictionary<long, Node> nodes = new Dictionary<long, Node>();
            List<long> node_ids = new List<long>();
            while (reader.Read())
            {
                // load/parse data.
                long returned_id = reader.GetInt64(0);
                int latitude_int = reader.GetInt32(1);
                int longitude_int = reader.GetInt32(2);
                long changeset_id = reader.GetInt64(3);
                bool visible = reader.GetBoolean(4);
                DateTime timestamp = reader.GetDateTime(5);
                long tile = reader.GetInt64(6);
                long version = reader.GetInt64(7);

                // create node.
                node = OsmBaseFactory.CreateNode(returned_id);
                node.Version = version;
                //node.UserId = user_id;
                node.TimeStamp = timestamp;
                node.ChangeSetId = changeset_id;
                node.Coordinate = new GeoCoordinate(
                    ((double)latitude_int) / 10000000.0, ((double)longitude_int) / 10000000.0);

                nodes.Add(node.Id,node);
                node_ids.Add(node.Id);
            }
            reader.Close();

            // STEP2: Load all node tags.
            this.LoadNodeTags(nodes);            

            // STEP3: Load all ways for the given nodes.
            IList<Way> ways = this.GetWaysForNodes(nodes);



            // Add all objects to the base list.
            foreach (Node node_result in nodes.Values.ToList<Node>())
            {
                base_list.Add(node_result);
            }
            foreach (Way way in ways)
            {
                base_list.Add(way);
            }
            return base_list;
        }

        private string ConstructIdList(IList<long> ids)
        {
            string return_string = string.Empty;
            if (ids.Count > 0)
            {
                return_string = return_string + ids[0].ToString();
                for (int i = 1; i < ids.Count; i++)
                {
                    return_string = return_string + "," + ids[i].ToString();
                }
            }
            return return_string;
        }

        private void LoadNodeTags(Dictionary<long,Node> nodes)
        {
            if (nodes.Count > 0)
            {
                string sql = "SELECT current_node_tags.* FROM current_node_tags WHERE (current_node_tags.id IN ({0})) ";
                sql = string.Format(sql, this.ConstructIdList(nodes.Keys.ToList<long>()));
                NpgsqlConnection con = this.CreateConnection();
                NpgsqlCommand com = new NpgsqlCommand(sql);
                com.Connection = con;
                NpgsqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    long returned_id = reader.GetInt64(0);
                    string key = reader.GetString(1);
                    string value = reader.GetString(2);

                    nodes[returned_id].Tags.Add(key, value);
                }
            }
        }

        #endregion

        /// <summary>
        /// Closes this datasource.
        /// </summary>
        public void Close()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection = null;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Releases all resources.
        /// </summary>
        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }

        #endregion
    }
}
