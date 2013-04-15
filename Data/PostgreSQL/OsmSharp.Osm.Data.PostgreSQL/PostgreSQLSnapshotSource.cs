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
    /// Reads data from the OSM PostgreSQL snapshot schema.
    /// 
    /// http://wiki.openstreetmap.org/wiki/Osmosis/PostGIS_Setup#Procedure_to_import_data_into_PostgreSQL
    /// </summary>
    public class PostgreSQLSnapshotSource : IDataSourceReadOnly, IDisposable
    {
        /// <summary>
        /// Holds the connection string.
        /// </summary>
        private string _connection_string;

        /// <summary>
        /// The id of this datasource.
        /// </summary>
        private Guid _id;

        /// <summary>
        /// Creates a new snapshot datasource.
        /// </summary>
        /// <param name="connection_string"></param>
        public PostgreSQLSnapshotSource(string connection_string)
        {
            _connection_string = connection_string;
            _id = Guid.NewGuid();
        }

        /// <summary>
        /// Holds the PostgreSQL connection.
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

        /// <summary>
        /// Parses tags from hstore.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="target"></param>
        private void ParseTags(string tags,IDictionary<string,string> target)
        {
            if (tags.Length > 0)
            {
                string[] splitted_tags = Regex.Split(tags, "\", \"");
                //string[] splitted_tags = tags.Split(new string[]{"\",\""}, StringSplitOptions.None);

                string temp_tag = null;
                foreach (string splitted_tag in splitted_tags)
                {
                    temp_tag = splitted_tag;
                    temp_tag = temp_tag.Replace("\"", string.Empty);
                    int idx_equals = temp_tag.IndexOf('=');
                    string key = temp_tag.Substring(0, idx_equals);
                    string value = temp_tag.Substring(idx_equals + 2, temp_tag.Length - idx_equals - 2);
                    target.Add(key, value);
                }
            }
        }

        private string CreateIdIn(IList<long> ids)
        {
            StringBuilder in_string = new StringBuilder("(");
            foreach (long id in ids)
            {
                in_string.Append("'");
                in_string.Append(id);
                in_string.Append("',");
            }
            in_string.Remove(in_string.Length - 1, 1);
            in_string.Append(")");
            return in_string.ToString();
        }

        #region IDataSourceReadOnly Members

        /// <summary>
        /// Returns the bounding box of the data.
        /// </summary>
        public GeoCoordinateBox BoundingBox
        {
            get 
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Returns the name of this data source.
        /// </summary>
        public string Name
        {
            get 
            { 
                return "PostgreSQL Simple Data Source"; 
            }
        }

        /// <summary>
        /// Returns the id of the datasource.
        /// </summary>
        public Guid Id
        {
            get 
            {
                return _id;
            }
        }

        /// <summary>
        /// Returns true if there is a bounding box.
        /// </summary>
        public bool HasBoundinBox
        {
            get 
            { 
                return false; 
            }
        }

        /// <summary>
        /// Returns true if the data is readonly.
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
            NpgsqlConnection con = this.CreateConnection();
            NpgsqlCommand com = new NpgsqlCommand(" SELECT n.id, n.version, n.user_id, n.tstamp, n.changeset_id, n.tags, x(n.geom) AS longitude, y(n.geom) AS latitude FROM nodes n where n.id = :id");
            com.Connection = con;

            NpgsqlParameter param = new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Bigint);
            param.Value = id;
            com.Parameters.Add(param);

            NpgsqlDataReader reader = com.ExecuteReader();
            Node node = null;
            if (reader.Read())
            {
                // load/parse data.
                long returned_id = reader.GetInt64(0);
                int version = reader.GetInt32(1);
                int user_id = reader.GetInt32(2);
                DateTime time_stamp = reader.GetDateTime(3);
                long changeset_id = reader.GetInt64(4);
                string tags = reader.GetString(5);
                double longitude = reader.GetDouble(6);
                double latitude = reader.GetDouble(7);

                // create node.
                node = OsmBaseFactory.CreateNode(returned_id);
                node.Version = version;
                node.UserId = user_id;
                node.TimeStamp = time_stamp;
                node.ChangeSetId = changeset_id;
                this.ParseTags(tags,node.Tags);
                node.Coordinate = new GeoCoordinate(latitude, longitude);
            }
            reader.Close();

            return node;
        }

        /// <summary>
        /// Returns all the nodes with all the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Node> GetNodes(IList<long> ids)
        {
            List<Node> nodes = new List<Node>();

            NpgsqlConnection con = this.CreateConnection();

            string in_string = this.CreateIdIn(ids);

            NpgsqlCommand com = new NpgsqlCommand(" SELECT n.id, n.version, n.user_id, n.tstamp, n.changeset_id, n.tags, x(n.geom) AS longitude, y(n.geom) AS latitude FROM nodes n where n.id in " + in_string);
            com.Connection = con;

            NpgsqlDataReader reader = com.ExecuteReader();
            Node node = null;
            while (reader.Read())
            {
                // load/parse data.
                long returned_id = reader.GetInt64(0);
                int version = reader.GetInt32(1);
                int user_id = reader.GetInt32(2);
                DateTime time_stamp = reader.GetDateTime(3);
                long changeset_id = reader.GetInt64(4);
                string tags = reader.GetString(5);
                double longitude = reader.GetDouble(6);
                double latitude = reader.GetDouble(7);

                // create node.
                node = OsmBaseFactory.CreateNode(returned_id);
                node.Version = version;
                node.UserId = user_id;
                node.TimeStamp = time_stamp;
                node.ChangeSetId = changeset_id;
                this.ParseTags(tags, node.Tags);
                node.Coordinate = new GeoCoordinate(latitude, longitude);

                nodes.Add(node);
            }
            reader.Close();

            return nodes;
        }

        /// <summary>
        /// Returns the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Relation GetRelation(long id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all relations with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Relation> GetRelations(IList<long> ids)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all relations containing the given object.
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
            NpgsqlConnection con = this.CreateConnection();
            NpgsqlCommand com = new NpgsqlCommand("SELECT w.id,w.version,w.user_id,w.tstamp,w.changeset_id,w.tags, " +
                "n.id as node_id, n.version as node_version, n.user_id as node_user_id, n.tstamp as node_tstamp, n.changeset_id as node_changeset_id, n.tags as node_tags, x(n.geom) AS node_longitude, y(n.geom) AS node_latitude " +
                "from ways w " +
                "inner join way_nodes wn " +
                "on w.id = wn.way_id " +
                "inner join nodes n " +
                "on n.id = wn.node_id " +
                "where w.id = :id " +
                "order by wn.sequence_id ");
            com.Connection = con;
            NpgsqlParameter param = new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Bigint);
            param.Value = id;
            com.Parameters.Add(param);

            NpgsqlDataReader reader = com.ExecuteReader();
            Node node = null;
            Way way = null;
            while (reader.Read())
            {
                if (way == null)
                {                
                    // load/parse way data.
                    long returned_id = reader.GetInt64(0);
                    int version = reader.GetInt32(1);
                    int user_id = reader.GetInt32(2);
                    DateTime time_stamp = reader.GetDateTime(3);
                    long changeset_id = reader.GetInt64(4);
                    string tags = reader.GetString(5);
                                        
                    // create way.
                    way = OsmBaseFactory.CreateWay(returned_id);
                    way.Version = version;
                    way.UserId = user_id;
                    way.TimeStamp = time_stamp;
                    this.ParseTags(tags, way.Tags);
                    way.ChangeSetId = changeset_id;
                }

                // load/parse node data.
                long node_id = reader.GetInt64(6);
                int node_version = reader.GetInt32(7);
                int node_user_id = reader.GetInt32(8);
                DateTime node_time_stamp = reader.GetDateTime(9);
                long node_changeset_id = reader.GetInt64(10);
                string node_tags = reader.GetString(11);
                double longitude = reader.GetDouble(12);
                double latitude = reader.GetDouble(13);

                // create node.
                node = OsmBaseFactory.CreateNode(node_id);
                node.Version = node_version;
                node.UserId = node_user_id;
                node.TimeStamp = node_time_stamp;
                node.ChangeSetId = node_changeset_id;
                node.ChangeSetId = node_changeset_id;
                this.ParseTags(node_tags, node.Tags);
                node.Coordinate = new GeoCoordinate(latitude, longitude);

                // add node to way.
                way.Nodes.Add(node);
            }
            reader.Close();

            return way;
        }

        /// <summary>
        /// Returns all ways with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Way> GetWays(IList<long> ids)
        {
            List<Way> ways = new List<Way>();
            NpgsqlConnection con = this.CreateConnection();

            string in_string = this.CreateIdIn(ids);

            NpgsqlCommand com = new NpgsqlCommand("SELECT w.id,w.version,w.user_id,w.tstamp,w.changeset_id,w.tags, " +
                "n.id as node_id, n.version as node_version, n.user_id as node_user_id, n.tstamp as node_tstamp, n.changeset_id as node_changeset_id, n.tags as node_tags, x(n.geom) AS node_longitude, y(n.geom) AS node_latitude " +
                "from ways w " +
                "inner join way_nodes wn " +
                "on w.id = wn.way_id " +
                "inner join nodes n " +
                "on n.id = wn.node_id " +
                "where w.id in " +
                in_string.ToString() +
                " order by w.id,wn.sequence_id ");
            com.Connection = con;

            NpgsqlDataReader reader = com.ExecuteReader();
            Node node = null;
            Way way = null;
            while (reader.Read())
            {
                long returned_id = reader.GetInt64(0);
                if (way == null
                    || (way !=null && way.Id != returned_id))
                {
                    // load/parse way data.
                    int version = reader.GetInt32(1);
                    int user_id = reader.GetInt32(2);
                    DateTime time_stamp = reader.GetDateTime(3);
                    long changeset_id = reader.GetInt64(4);
                    string tags = reader.GetString(5);

                    // create way.
                    way = OsmBaseFactory.CreateWay(returned_id);
                    way.Version = version;
                    way.UserId = user_id;
                    way.TimeStamp = time_stamp;
                    this.ParseTags(tags, way.Tags);
                    way.ChangeSetId = changeset_id;

                    ways.Add(way);
                }

                // load/parse node data.
                long node_id = reader.GetInt64(6);
                int node_version = reader.GetInt32(7);
                int node_user_id = reader.GetInt32(8);
                DateTime node_time_stamp = reader.GetDateTime(9);
                long node_changeset_id = reader.GetInt64(10);
                string node_tags = reader.GetString(11);
                double longitude = reader.GetDouble(12);
                double latitude = reader.GetDouble(13);

                // create node.
                node = OsmBaseFactory.CreateNode(node_id);
                node.Version = node_version;
                node.UserId = node_user_id;
                node.TimeStamp = node_time_stamp;
                node.ChangeSetId = node_changeset_id;
                node.ChangeSetId = node_changeset_id;
                this.ParseTags(node_tags, node.Tags);
                node.Coordinate = new GeoCoordinate(latitude, longitude);

                // add node to way.
                way.Nodes.Add(node);
            }
            reader.Close();

            return ways;
        }

        /// <summary>
        /// Returns all the ways with the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IList<Way> GetWaysFor(Node node)
        {
            string sql
                = "SELECT w.id,w.version,w.user_id,w.tstamp,w.changeset_id,w.tags,  "
                + "n.id as node_id, n.version as node_version, n.user_id as node_user_id, n.tstamp as node_tstamp, n.changeset_id as node_changeset_id, n.tags as node_tags, x(n.geom) AS node_longitude, y(n.geom) AS node_latitude  "
                + "from ways w  "
                + "inner join way_nodes wn  "
                + "on w.id = wn.way_id  "
                + "inner join nodes n  "
                + "on n.id = wn.node_id  "
                + "where w.id in  "
                + "(select wn1.way_id "
                + "from way_nodes wn1 "
                + "where wn1.node_id = :node_id) "
                + "order by w.id,wn.sequence_id  "
                ;
            List<Way> ways = new List<Way>();
            NpgsqlConnection con = this.CreateConnection();

            NpgsqlCommand com = new NpgsqlCommand(sql);
            com.Connection = con;

            NpgsqlParameter param = new NpgsqlParameter("node_id", NpgsqlTypes.NpgsqlDbType.Bigint);
            param.Value = node.Id;
            com.Parameters.Add(param);

            NpgsqlDataReader reader = com.ExecuteReader();
            Way way = null;
            while (reader.Read())
            {
                long returned_id = reader.GetInt64(0);
                if (way == null
                    || (way != null && way.Id != returned_id))
                {
                    // load/parse way data.
                    int version = reader.GetInt32(1);
                    int user_id = reader.GetInt32(2);
                    DateTime time_stamp = reader.GetDateTime(3);
                    long changeset_id = reader.GetInt64(4);
                    string tags = reader.GetString(5);

                    // create way.
                    way = OsmBaseFactory.CreateWay(returned_id);
                    way.Version = version;
                    way.UserId = user_id;
                    way.TimeStamp = time_stamp;
                    this.ParseTags(tags, way.Tags);
                    way.ChangeSetId = changeset_id;

                    ways.Add(way);
                }

                // load/parse node data.
                long node_id = reader.GetInt64(6);
                int node_version = reader.GetInt32(7);
                int node_user_id = reader.GetInt32(8);
                DateTime node_time_stamp = reader.GetDateTime(9);
                long node_changeset_id = reader.GetInt64(10);
                string node_tags = reader.GetString(11);
                double longitude = reader.GetDouble(12);
                double latitude = reader.GetDouble(13);

                // create node.
                node = OsmBaseFactory.CreateNode(node_id);
                node.Version = node_version;
                node.UserId = node_user_id;
                node.TimeStamp = node_time_stamp;
                node.ChangeSetId = node_changeset_id;
                node.ChangeSetId = node_changeset_id;
                this.ParseTags(node_tags, node.Tags);
                node.Coordinate = new GeoCoordinate(latitude, longitude);

                // add node to way.
                way.Nodes.Add(node);
            }
            reader.Close();

            return ways;

        }

        /// <summary>
        /// Returns all objects within the given bounding box and that are valid according to the given filter.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter)
        {
            NpgsqlConnection con = this.CreateConnection();

            List<OsmGeo> base_list = new List<OsmGeo>();

            // query nodes.
            string nodes_sql
                = "SELECT n.id, n.version, n.user_id, n.tstamp, n.changeset_id, n.tags, x(n.geom) AS longitude, y(n.geom) AS latitude from nodes n where geom && SetSRID(box3d('BOX3D({0} {1},{2} {3})'),4326) "
                ;
            nodes_sql = string.Format(nodes_sql, box.MinLon, box.MaxLat, box.MaxLon, box.MinLat);

            NpgsqlCommand com = new NpgsqlCommand(nodes_sql);
            com.Connection = con;

            NpgsqlDataReader reader = com.ExecuteReader();
            Node node = null;
            while (reader.Read())
            {
                // load/parse data.
                long returned_id = reader.GetInt64(0);
                int version = reader.GetInt32(1);
                int user_id = reader.GetInt32(2);
                DateTime time_stamp = reader.GetDateTime(3);
                long changeset_id = reader.GetInt64(4);
                string tags = reader.GetString(5);
                double longitude = reader.GetDouble(6);
                double latitude = reader.GetDouble(7);

                // create node.
                node = OsmBaseFactory.CreateNode(returned_id);
                node.Version = version;
                node.UserId = user_id;
                node.TimeStamp = time_stamp;
                node.ChangeSetId = changeset_id;
                this.ParseTags(tags, node.Tags);
                node.Coordinate = new GeoCoordinate(latitude, longitude);

                base_list.Add(node);
            }
            reader.Close();
            string way_sql
                = "SELECT w.id,w.version,w.user_id,w.tstamp,w.changeset_id,w.tags,  n.id as node_id, n.version as node_version, n.user_id as node_user_id, n.tstamp as node_tstamp, n.changeset_id as node_changeset_id, n.tags as node_tags, x(n.geom) AS node_longitude, y(n.geom) AS node_latitude   "
                + "from ways w   "
                + "inner join way_nodes wn   "
                + "on w.id = wn.way_id   "
                + "inner join nodes n   "
                + "on n.id = wn.node_id "
                + "inner join  "
                + "( "
                + "select distinct wn1.way_id  "
                + "from way_nodes wn1  "
                + "inner join nodes n1  "
                + "on n1.id = wn1.node_id   "
                + "where n1.geom && SetSRID(box3d('BOX3D({0} {1},{2} {3})'),4326) "
                + ") w_in "
                + "on w_in.way_id = w.id  "
                ;


            way_sql = string.Format(way_sql, box.MinLon, box.MaxLat, box.MaxLon, box.MinLat);

            com = new NpgsqlCommand(way_sql);
            com.Connection = con;

            reader = com.ExecuteReader();
            Way way = null;
            while (reader.Read())
            {
                long returned_id = reader.GetInt64(0);
                if (way == null
                    || (way != null && way.Id != returned_id))
                {
                    // load/parse way data.
                    int version = reader.GetInt32(1);
                    int user_id = reader.GetInt32(2);
                    DateTime time_stamp = reader.GetDateTime(3);
                    long changeset_id = reader.GetInt64(4);
                    string tags = reader.GetString(5);

                    // create way.
                    way = OsmBaseFactory.CreateWay(returned_id);
                    way.Version = version;
                    way.UserId = user_id;
                    way.TimeStamp = time_stamp;
                    this.ParseTags(tags, way.Tags);
                    way.ChangeSetId = changeset_id;

                    base_list.Add(way);
                }

                // load/parse node data.
                long node_id = reader.GetInt64(6);
                int node_version = reader.GetInt32(7);
                int node_user_id = reader.GetInt32(8);
                DateTime node_time_stamp = reader.GetDateTime(9);
                long node_changeset_id = reader.GetInt64(10);
                string node_tags = reader.GetString(11);
                double longitude = reader.GetDouble(12);
                double latitude = reader.GetDouble(13);

                // create node.
                node = OsmBaseFactory.CreateNode(node_id);
                node.Version = node_version;
                node.UserId = node_user_id;
                node.TimeStamp = node_time_stamp;
                node.ChangeSetId = node_changeset_id;
                node.ChangeSetId = node_changeset_id;
                this.ParseTags(node_tags, node.Tags);
                node.Coordinate = new GeoCoordinate(latitude, longitude);

                // add node to way.
                way.Nodes.Add(node);
            }
            reader.Close();

            // TODO: relations

            List<OsmGeo> filtered_list = null;
            if (filter != null && filter != Filter.Any())
            {
                filtered_list = new List<OsmGeo>();

                foreach (OsmGeo base_object in base_list)
                {
                    if (filter.Evaluate(base_object))
                    {
                        filtered_list.Add(base_object);
                    }
                }
            }
            else
            {
                filtered_list = base_list;
            }

            return base_list;
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
