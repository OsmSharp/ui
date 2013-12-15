// OsmSharp - OpenStreetMap (OSM) SDK
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
using Oracle.ManagedDataAccess.Client;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using OsmSharp.Osm.Streams.ChangeSets;

namespace OsmSharp.Data.Oracle.Osm.Streams
{
    /// <summary>
    /// A changeset apply target to apply changesets to an oracle database.
    /// </summary>
    public class OracleSimpleChangeSetApplyTarget : DataProcessorChangeSetTarget
    {
        /// <summary>
        /// Holds the connection.
        /// </summary>
        private OracleConnection _connection;

        /// <summary>
        /// Does pragmatic changes.
        /// </summary>
        private bool _pragmatic;
        
        /// <summary>
        /// Creates a new changeset target.
        /// </summary>
        /// <param name="connection_string"></param>
        /// <param name="pragmatic"></param>
        public OracleSimpleChangeSetApplyTarget(string connection_string, bool pragmatic)
        {
            _connection = new OracleConnection(connection_string);
            _pragmatic = pragmatic;
        }

        /// <summary>
        /// Apply the given changeset.
        /// </summary>
        /// <param name="change_set"></param>
        public override void ApplyChange(ChangeSet change_set)
        {
            if (change_set != null && change_set.Changes != null)
            {
                foreach (Change change in change_set.Changes)
                {
                    switch (change.Type)
                    {
                        case ChangeType.Create:
                            foreach (OsmGeo geo in change.OsmGeo)
                            {
                                // start applying the simplechange.
                                OracleTransaction trans = _connection.BeginTransaction();
                                try
                                {
                                    if (geo is Node)
                                    {
                                        this.Create(geo as Node);
                                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.Oracle.Osm.Streams.OracleOsmChangesetStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                                            "+(n:{0})", geo.Id.Value);
                                    }
                                    else if (geo is Way)
                                    {
                                        this.Create(geo as Way);
                                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.Oracle.Osm.Streams.OracleOsmChangesetStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                                            "+(w:{0})", geo.Id.Value);
                                    }
                                    else if (geo is Relation)
                                    {
                                        this.Create(geo as Relation);
                                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.Oracle.Osm.Streams.OracleOsmChangesetStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                                            "+(r:{0})", geo.Id.Value);
                                    }
                                    trans.Commit();
                                }
                                catch (OracleException ex)
                                {
                                    trans.Rollback();
                                    if (!_pragmatic)
                                    {
                                        throw ex;
                                    }
                                    else
                                    {
                                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.Oracle.Osm.Streams.OracleOsmChangesetStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                                            "+(E:{0}-{1})", geo.Id.Value, geo.Type.ToString());
                                    }
                                }
                            }
                            break;
                        case ChangeType.Delete:
                            foreach (OsmGeo geo in change.OsmGeo)
                            {
                                // start applying the simplechange.
                                OracleTransaction trans = _connection.BeginTransaction();
                                try
                                {
                                    if (geo is Node)
                                    {
                                        this.Delete(geo as Node);
                                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.Oracle.Osm.Streams.OracleOsmChangesetStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                                            "-(n:{0})", geo.Id.Value);
                                    }
                                    else if (geo is Way)
                                    {
                                        this.Delete(geo as Way);
                                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.Oracle.Osm.Streams.OracleOsmChangesetStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                                            "-(w:{0})", geo.Id.Value);
                                    }
                                    else if (geo is Relation)
                                    {
                                        this.Delete(geo as Relation);
                                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.Oracle.Osm.Streams.OracleOsmChangesetStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                                            "-(r:{0})", geo.Id.Value);
                                    }
                                    trans.Commit();
                                }
                                catch (OracleException ex)
                                {
                                    trans.Rollback();
                                    if (!_pragmatic)
                                    {
                                        throw ex;
                                    }
                                    else
                                    {
                                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.Oracle.Osm.Streams.OracleOsmChangesetStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                                            "-(E:{0}-{1})", geo.Id.Value, geo.Type.ToString());
                                    }
                                }
                            }
                            break;
                        case ChangeType.Modify:
                            foreach (OsmGeo geo in change.OsmGeo)
                            {
                                // start applying the simplechange.
                                OracleTransaction trans = _connection.BeginTransaction();
                                try
                                {
                                    if (geo is Node)
                                    {
                                        this.Modify(geo as Node);
                                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.Oracle.Osm.Streams.OracleOsmChangesetStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                                            "/(n:{0})", geo.Id.Value);
                                    }
                                    else if (geo is Way)
                                    {
                                        this.Modify(geo as Way);
                                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.Oracle.Osm.Streams.OracleOsmChangesetStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                                            "/(w:{0})", geo.Id.Value);
                                    }
                                    else if (geo is Relation)
                                    {
                                        this.Modify(geo as Relation);
                                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.Oracle.Osm.Streams.OracleOsmChangesetStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                                            "/(r:{0})", geo.Id.Value);
                                    }
                                    trans.Commit();
                                }
                                catch (OracleException ex)
                                {
                                    trans.Rollback();
                                    if (!_pragmatic)
                                    {
                                        throw ex;
                                    }
                                    else
                                    {
                                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.Oracle.Osm.Streams.OracleOsmChangesetStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                                            "/(E:{0}-{1})", geo.Id.Value, geo.Type.ToString());
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }
    

        private OracleCommand CreateCommand(string sql)
        {
            OracleCommand command = new OracleCommand(sql);
            command.Connection = _connection;
            return command;
        }

        #region Modify

        /// <summary>
        /// Modifies the given node.
        /// </summary>
        /// <param name="node"></param>
        private void Modify(Node node)
        {
            OracleCommand command;

            string sql = string.Empty;

            DateTime? timestamp = node.TimeStamp;
            if (timestamp.HasValue)
            {
                sql = "update node set latitude = :latitude, longitude = :longitude, changeset_id = :changeset_id, visible=:visible, timestamp=to_date('{0}','YYYY/MM/DD HH24:MI'), tile=:tile, version=:version, usr=:usr, usr_id=:usr_id where id=:id";
                sql = string.Format(sql,timestamp.Value.ToString("yyyy/MM/dd HH:mm"));
            }
            else
            {
                sql = "update node set latitude = :latitude, longitude = :longitude, changeset_id = :changeset_id, visible=:visible, timestamp=null, tile=:tile, version=:version where id=:id";
            }
            command = this.CreateCommand(sql);

            // format data and create parameters.
            int? latitude = (int)(node.Latitude * 10000000); // latitude should always contain a value.
            command.Parameters.Add("latitude",latitude.ConvertToDBValue<int>());

            int? longitude = (int)(node.Longitude * 10000000); // longitude should always containt a value.
            command.Parameters.Add("longitude",longitude.ConvertToDBValue<int>());

            long? changeset_id = node.ChangeSetId;
            command.Parameters.Add("changeset_id",changeset_id.ConvertToDBValue<long>());

            bool? visible = node.Visible;
            int visible_int = 1;
            if (!visible.HasValue || !visible.Value)
            {
                visible_int = 0;
            }
            command.Parameters.Add("visible", visible_int);

            // calculate the tile the node belongs to.
            long tile = TileCalculations.xy2tile(TileCalculations.lon2x(node.Longitude.Value), TileCalculations.lat2y(node.Latitude.Value));
            command.Parameters.Add("tile", tile);

            long? version = (long)node.Version;
            command.Parameters.Add("version",version.ConvertToDBValue<long>());

            command.Parameters.Add("usr", node.UserName);
            command.Parameters.Add("usr_id", node.UserId);

            long? id = node.Id;
            command.Parameters.Add("id", id.ConvertToDBValue<long>());


            // execute the update command.
            command.ExecuteNonQuery();
            command.Dispose();

            if (this.Exists("node",id.Value))
            {
                // modify the node tags.
                this.ModifyTags(node.Id.Value, node.Tags, "node_tags", "node_id");
            }

            // raise the modified event.
            this.RaiseChange(ChangeType.Modify, OsmGeoType.Node, node.Id.Value);
        }

        /// <summary>
        /// Modifies the given way.
        /// </summary>
        /// <param name="way"></param>
        private void Modify(Way way)
        {
            OracleCommand command;

            string sql = string.Empty;

            DateTime? timestamp = way.TimeStamp;
            if (timestamp.HasValue)
            {
                sql = "update way set changeset_id=:changeset_id,visible=:visible,timestamp=to_date('{0}','YYYY/MM/DD HH24:MI'),version=:version, usr=:usr, usr_id=:usr_id where id = :id";
                sql = string.Format(sql, timestamp.Value.ToString("yyyy/MM/dd HH:mm"));
            }
            else
            {
                sql = "update way set changeset_id=:changeset_id,visible=:visible,timestamp=null,version=:version where id = :id";
            }
            command = this.CreateCommand(sql);

            // format data and create parameters.
            long? id = way.Id;
            command.Parameters.Add(new OracleParameter("id", id.ConvertToDBValue<long>()));

            long? changeset_id = way.ChangeSetId;
            command.Parameters.Add(new OracleParameter("changeset_id", changeset_id.ConvertToDBValue<long>()));

            bool? visible = way.Visible;
            int visible_int = 1;
            if (!visible.HasValue || !visible.Value)
            {
                visible_int = 0;
            }
            command.Parameters.Add("visible", visible_int);

            long? version = (long)way.Version;
            command.Parameters.Add(new OracleParameter("version", version.ConvertToDBValue<long>()));

            command.Parameters.Add("usr", way.UserName);
            command.Parameters.Add("usr_id", way.UserId);

            command.ExecuteNonQuery();
            command.Dispose();
            if (this.Exists("way", way.Id.Value))
            {
                // update tags.
                this.ModifyTags(way.Id.Value, way.Tags, "way_tags", "way_id");

                // modify nodes.
                this.ModifyWayNodes(way.Id.Value, way.Nodes);
            }

            // raise the modified event.
            this.RaiseChange(ChangeType.Modify, OsmGeoType.Way, way.Id.Value);
        }

        private void ModifyWayNodes(long id, IList<long> nodes)
        {
            OracleCommand command;

            command = this.CreateCommand("select node_id from way_nodes where way_id=:id order by sequence_id");
            command.Parameters.Add(new OracleParameter("id",id));

            OracleDataReader reader = command.ExecuteReader();

            int idx = 0;
            while (reader.Read())
            {
                long node_id = (long)reader["node_id"];

                if (idx < nodes.Count)
                {
                    if (node_id != nodes[idx])
                    {
                        command = this.CreateCommand("update way_nodes set node_id=:node_id where sequence_id = :sequence_id and way_id = :way_id");
                        command.Parameters.Add(new OracleParameter("node_id", nodes[idx]));
                        command.Parameters.Add(new OracleParameter("sequence_id", idx));
                        command.Parameters.Add(new OracleParameter("way_id", id));

                        command.ExecuteNonQuery();
                        command.Dispose();

                        // raise the modified event.
                        this.RaiseChange(ChangeType.Modify, OsmGeoType.Node, node_id);
                        this.RaiseChange(ChangeType.Modify, OsmGeoType.Node, nodes[idx]);
                    }
                }
                else
                {
                    command = this.CreateCommand("delete from way_nodes where sequence_id >= :sequence_id and way_id = :way_id");
                    command.Parameters.Add(new OracleParameter("sequence_id", idx));
                    command.Parameters.Add(new OracleParameter("way_id", id));

                    command.ExecuteNonQuery();
                    command.Dispose();

                    // raise the modified event.
                    this.RaiseChange(ChangeType.Modify, OsmGeoType.Node, node_id);

                    break;
                }

                idx++;
            }
            reader.Close();

            while (idx < nodes.Count)
            {
                command = this.CreateCommand("insert into way_nodes (way_id,node_id,sequence_id) values (:way_id,:node_id,:sequence_id)");
                command.Parameters.Add(new OracleParameter("way_id", id));
                command.Parameters.Add(new OracleParameter("node_id", nodes[idx]));
                command.Parameters.Add(new OracleParameter("sequence_id", idx));

                command.ExecuteNonQuery();
                command.Dispose();

                // raise the modified event.
                this.RaiseChange(ChangeType.Modify, OsmGeoType.Node, nodes[idx]);

                idx++;
            }
        }

        private void Modify(Relation relation)
        {
            OracleCommand command;
            string sql = string.Empty;

            DateTime? timestamp = relation.TimeStamp;
            if (timestamp.HasValue)
            {
                sql = "update relation set changeset_id=:changeset_id,visible=:visible,timestamp=to_date('{0}','YYYY/MM/DD HH24:MI'),version=:version, usr=:usr, usr_id=:usr_id where id = :id";
                sql = string.Format(sql, timestamp.Value.ToString("yyyy/MM/dd HH:mm"));
            }
            else
            {
                sql = "update relation set changeset_id=:changeset_id,visible=:visible,timestamp=null,version=:version, usr_id=:usr_id where id = :id";
            }
            command = this.CreateCommand(sql);

            // format data and create parameters.
            long? changeset_id = relation.ChangeSetId;
            command.Parameters.Add(new OracleParameter("changeset_id", changeset_id.ConvertToDBValue<long>()));

            bool? visible = relation.Visible;
            int visible_int = 1;
            if (!visible.HasValue || !visible.Value)
            {
                visible_int = 0;
            }
            command.Parameters.Add("visible", visible_int);

            long? version = (long)relation.Version;
            command.Parameters.Add(new OracleParameter("version", version.ConvertToDBValue<long>()));

            command.Parameters.Add("usr", relation.UserName);
            command.Parameters.Add("usr_id", relation.UserId);
            
            long? id = relation.Id;
            command.Parameters.Add(new OracleParameter("id", id.ConvertToDBValue<long>()));

            command.ExecuteNonQuery();
            command.Dispose();

            if (this.Exists("relation",id.Value))
            {
                // update tags.
                this.ModifyTags(relation.Id.Value, relation.Tags, "relation_tags", "relation_id");

                // update members
                this.ModifyRelationMembers(relation.Id.Value, relation.Members);
            }
            
            // raise the modified event.
            this.RaiseChange(ChangeType.Modify, OsmGeoType.Relation, relation.Id.Value);
        }

        private void ModifyRelationMembers(long id, IList<RelationMember> members)
        {
            OracleCommand command;

            command = this.CreateCommand("select * from relation_members where relation_id=:id order by sequence_id");
            command.Parameters.Add(new OracleParameter("id", id));

            OracleDataReader reader = command.ExecuteReader();

            int idx = 0;
            while (reader.NextResult())
            {
                string member_type = reader["member_type"].ToStringEmptyWhenNull();
                long member_id = (long)reader["member_id"];
                string member_role = reader["member_role"].ToStringEmptyWhenNull();
                
                string new_member_type = members[idx].MemberType.ToString();
                string new_member_role = members[idx].MemberRole;
                long new_member_id = members[idx].MemberId.Value;

                if (member_id != new_member_id || member_role != new_member_role || member_type != new_member_type)
                {
                    command = this.CreateCommand("update relation_members set member_id=:member_id,member_type=:member_type,member_role=:member_role where sequence_id = :sequence_id and relation_id = :relation_id");
                    command.Parameters.Add(new OracleParameter("member_id", new_member_id));
                    command.Parameters.Add(new OracleParameter("member_type", new_member_type));
                    command.Parameters.Add(new OracleParameter("member_role", new_member_role));
                    command.Parameters.Add(new OracleParameter("sequence_id", idx + 1));
                    command.Parameters.Add(new OracleParameter("relation_id", id));

                    command.ExecuteNonQuery();
                    command.Dispose();
                }

                idx++;
            }
            reader.Close();

            while (idx < members.Count)
            {
                string new_member_type = members[idx].MemberType.ToString();
                string new_member_role = members[idx].MemberRole;
                long new_member_id = members[idx].MemberId.Value;

                command = this.CreateCommand("insert into relation_members (relation_id,member_type,member_id,member_role,sequence_id) values (:relation_id,:member_type,:member_id,:member_role,:sequence_id)");
                command.Parameters.Add(new OracleParameter("relation_id", id));
                command.Parameters.Add(new OracleParameter("member_type", new_member_type));
                command.Parameters.Add(new OracleParameter("member_id", new_member_id));
                command.Parameters.Add(new OracleParameter("member_role", new_member_role)); 
                command.Parameters.Add(new OracleParameter("sequence_id", idx + 1));

                command.ExecuteNonQuery();
                command.Dispose();

                idx++;
            }
        }

        private bool Exists(string table, long id)
        {
            // adjust the data based on the tags already present.
            OracleCommand command = this.CreateCommand(string.Format("select count(*) from {0} where ID=:id", table));
            command.Parameters.Add("id", id);

            object scalar = command.ExecuteScalar();

            return (decimal)scalar == 1;
        }

        private void ModifyTags(long id, TagsCollectionBase neTags, string table, string refColumn)
        {
            OracleCommand command;

            TagsCollectionBase tagsToInsert = null;
            if (neTags == null)
            {
                tagsToInsert = new TagsCollection();
            }
            else
            {
                tagsToInsert = new TagsCollection(neTags);
            }

            // suppose there are no tags present yet.
            TagsCollectionBase tags_to_update = new TagsCollection();
            TagsCollectionBase tags_to_delete = new TagsCollection();            

            // adjust the data based on the tags already present.
            command = this.CreateCommand(string.Format("select * from {0} where {1}=:{1}",table,refColumn));
            command.Parameters.Add(refColumn, id);

            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string k = reader["key"].ToStringEmptyWhenNull();
                string v = reader["value"].ToStringEmptyWhenNull();

                if (tagsToInsert.ContainsKey(k))
                {
                    // there is at least an update or no insert.
                    string new_value = tagsToInsert[k];
                    tagsToInsert.RemoveKeyValue(new Tag(k, v));

                    // see if there is an update needed.
                    if (new_value != v)
                    {
                        // tags need to be updated.
                        tags_to_update.Add(k, new_value);
                    }
                }
                else
                { 
                    // tags are not found; delete them!
                    tags_to_delete.Add(k, v);
                }
            }
            reader.Close();

            // delete old tags.
            foreach(Tag tag in tags_to_delete)
            {
                command = this.CreateCommand(string.Format("delete from {0} where {1}=:{1} and key=:key", table, refColumn));
                command.Parameters.Add(new OracleParameter(refColumn, id));
                command.Parameters.Add(new OracleParameter("key", tag.Key));

                command.ExecuteNonQuery();
                command.Dispose();
            }

            // update tags.
            foreach (Tag pair in tags_to_update)
            {
                command = this.CreateCommand(string.Format("update {0} set value=:value where {1}=:{1} and key=:key", table, refColumn));
                command.Parameters.Add(new OracleParameter("value", pair.Value));
                command.Parameters.Add(new OracleParameter(refColumn, id));
                command.Parameters.Add(new OracleParameter("key", pair.Key));

                command.ExecuteNonQuery();
                command.Dispose();
            }

            // insert tags.
            foreach (Tag pair in tagsToInsert)
            {
                command = this.CreateCommand(string.Format("insert into {0} ({1},key,value) values (:{1},:key,:value)",table,refColumn));
                command.Parameters.Add(new OracleParameter(refColumn, id));
                command.Parameters.Add(new OracleParameter("key", pair.Key));
                command.Parameters.Add(new OracleParameter("value", pair.Value));

                command.ExecuteNonQuery();
                command.Dispose();
            }
        }

        #endregion

        #region Create

        private void Create(Node node)
        {
            OracleCommand command;

            command = this.CreateCommand("insert into node (id,latitude,longitude,changeset_id,visible,timestamp,tile,version,usr,usr_id) values (:id,:latitude,:longitude,:changeset_id,:visible,:timestamp,:tile,:version,:usr,:usr_id)");

            // format data and create parameters.
            long? id = node.Id;
            command.Parameters.Add(new OracleParameter("id", id.ConvertToDBValue<long>()));

            int? latitude = (int)(node.Latitude * 10000000); // latitude should always contain a value.
            command.Parameters.Add(new OracleParameter("latitude", latitude.ConvertToDBValue<int>()));

            int? longitude = (int)(node.Longitude * 10000000); // longitude should always containt a value.
            command.Parameters.Add(new OracleParameter("longitude", longitude.ConvertToDBValue<int>()));

            long? changeset_id = node.ChangeSetId;
            command.Parameters.Add(new OracleParameter("changeset_id", changeset_id.ConvertToDBValue<long>()));

            bool? visible = node.Visible;
            int visible_int = 1;
            if (!visible.HasValue)
            {
                visible_int = 1;
            }
            command.Parameters.Add(new OracleParameter("visible", visible_int));

            DateTime? timestamp = node.TimeStamp;
            command.Parameters.Add(new OracleParameter("ts", timestamp.ConvertToDBValue<DateTime>()));

            // calculate the tile the node belongs to.
            long tile = TileCalculations.xy2tile(TileCalculations.lon2x(node.Longitude.Value), TileCalculations.lat2y(node.Latitude.Value));
            command.Parameters.Add(new OracleParameter("tile", tile));

            long? version = (long)node.Version;
            command.Parameters.Add(new OracleParameter("version", version.ConvertToDBValue<long>()));

            
            command.Parameters.Add("usr", node.UserName);
            command.Parameters.Add("usr_id", node.UserId);

            command.ExecuteNonQuery();
            command.Dispose();

            // insert tags.
            this.CreateTags(node.Id.Value, node.Tags, "node_tags", "node_id");

        } 

        private void Create(Way way)
        {
            OracleCommand command;

            DateTime? timestamp = way.TimeStamp;
            string timestamp_str = timestamp.Value.ToString("HH:mm dd/MM/yyyy");          

            command = this.CreateCommand(string.Format(
                "insert into way (id,changeset_id,timestamp,visible,version,usr,usr_id) values (:id,:changeset_id,to_date('{0}','HH24:MI DD/MM/YYYY'),:visible,:version,:usr,:usr_id)",
                timestamp_str));

            // format data and create parameters.
            long? id = way.Id;
            command.Parameters.Add(new OracleParameter("id", id.ConvertToDBValue<long>()));

            long? changeset_id = way.ChangeSetId;
            command.Parameters.Add(new OracleParameter("changeset_id", changeset_id.ConvertToDBValue<long>()));

            bool? visible = way.Visible;
            int visible_int = 1;
            if (!visible.HasValue)
            {
                visible_int = 0;
            }
            command.Parameters.Add(new OracleParameter("visible", visible_int));


            long? version = (long)way.Version;
            command.Parameters.Add(new OracleParameter("version", version.ConvertToDBValue<long>()));

            command.Parameters.Add("usr", way.UserName);
            command.Parameters.Add("usr_id", way.UserId);

            command.ExecuteNonQuery();
            command.Dispose();

            // insert tags.
            this.CreateTags(way.Id.Value, way.Tags, "way_tags", "way_id");

            // insert way_nodes.
            this.CreateWayNodes(way.Id.Value, way.Nodes);
        }

        private void CreateWayNodes(long id, IList<long> nodes)
        {
            OracleCommand command;

            command = this.CreateCommand("select node_id from way_nodes where way_id=:id order by sequence_id");
            command.Parameters.Add(new OracleParameter("id", id));

            OracleDataReader reader = command.ExecuteReader();

            int idx = 0;
            while (idx < nodes.Count)
            {
                try
                {
                    command = this.CreateCommand("insert into way_nodes (way_id,node_id,sequence_id) values (:way_id,:node_id,:sequence_id)");
                    command.Parameters.Add(new OracleParameter("way_id", id));
                    command.Parameters.Add(new OracleParameter("node_id", nodes[idx]));
                    command.Parameters.Add(new OracleParameter("sequence_id", idx + 1));

                    command.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.Oracle.Osm.Streams.OracleOsmChangesetStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                        "Could not insert way_nodes record for way {0} and node {1}:{2}",
                        id, nodes[idx],
                        ex.Message);
                }
                finally
                {
                    command.Dispose();
                }
                idx++;
            }
            reader.Close();
        }

        private void Create(Relation relation)
        {
            OracleCommand command;
            DateTime? timestamp = relation.TimeStamp;
            string timestamp_str = timestamp.Value.ToString("HH:mm dd/MM/yyyy");

            command = this.CreateCommand(string.Format(
                "insert into relation (id,changeset_id,timestamp,visible,version,usr,usr_id) values (:id,:changeset_id,to_date('{0}','HH24:MI DD/MM/YYYY'),:visible,:version,:usr,:usr_id)",
                timestamp_str));

            // format data and create parameters.
            long? id = relation.Id;
            command.Parameters.Add(new OracleParameter("id", id.ConvertToDBValue<long>()));

            long? changeset_id = relation.ChangeSetId;
            command.Parameters.Add(new OracleParameter("changeset_id", changeset_id.ConvertToDBValue<long>()));

            bool? visible = relation.Visible;
            int visible_int = 1;
            if (!visible.HasValue)
            {
                visible_int = 0;
            }
            command.Parameters.Add(new OracleParameter("visible", visible_int));

            long? version = (long)relation.Version;
            command.Parameters.Add(new OracleParameter("version", version.ConvertToDBValue<long>()));

            command.Parameters.Add("usr", relation.UserName);
            command.Parameters.Add("usr_id", relation.UserId);

            command.ExecuteNonQuery();
            command.Dispose();

            // insert tags.
            this.CreateTags(relation.Id.Value, relation.Tags, "relation_tags", "relation_id");

            // insert members.
            this.CreateRelationMembers(relation.Id.Value, relation.Members);
        }

        private void CreateRelationMembers(long id, IList<RelationMember> members)
        {
            OracleCommand command;

            command = this.CreateCommand("select * from relation_members where relation_id=:id order by sequence_id");
            command.Parameters.Add(new OracleParameter("id", id));

            int idx = 0;
            while (idx < members.Count)
            {
                string new_member_type = members[idx].MemberType.ToString();
                string new_member_role = members[idx].MemberRole;
                long new_member_id = members[idx].MemberId.Value;

                command = this.CreateCommand("insert into relation_members (relation_id,member_type,member_id,member_role,sequence_id) values (:relation_id,:member_type,:member_id,:member_role,:sequence_id)");
                command.Parameters.Add(new OracleParameter("relation_id", id));
                command.Parameters.Add(new OracleParameter("member_type", new_member_type));
                command.Parameters.Add(new OracleParameter("member_id", new_member_id));
                command.Parameters.Add(new OracleParameter("member_role", new_member_role));
                command.Parameters.Add(new OracleParameter("sequence_id", idx + 1));

                command.ExecuteNonQuery();
                command.Dispose();

                idx++;
            }
        }

        /// <summary>
        /// Creates the tags in the given table.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="new_tags"></param>
        /// <param name="table"></param>
        /// <param name="ref_column"></param>
        public void CreateTags(long id, TagsCollectionBase new_tags, string table, string ref_column)
        {
            OracleCommand command;

            // copy the source tags dictionary.
            TagsCollectionBase tagsToInsert;
            if (new_tags == null)
            {
                tagsToInsert = new TagsCollection();
            }
            else
            {
                tagsToInsert = new TagsCollection(new_tags);
            }

            // insert tags.
            foreach (Tag pair in tagsToInsert)
            {
                command = this.CreateCommand(string.Format("insert into {0} ({1},key,value) values (:{1},:key,:value)", table, ref_column));
                command.Parameters.Add(new OracleParameter("ref_column", id));
                command.Parameters.Add(new OracleParameter("key", pair.Key));
                command.Parameters.Add(new OracleParameter("value", pair.Value));

                command.ExecuteNonQuery();
                command.Dispose();
            }
        }

        #endregion

        #region Delete

        private void Delete(Node node)
        {
            OracleCommand command;

            command = this.CreateCommand("delete from node_tags where node_id = :node_id");
            command.Parameters.Add(new OracleParameter("node_id", node.Id.Value));
            command.ExecuteNonQuery();
            command.Dispose();

            command = this.CreateCommand("delete from node where id = :id");
            command.Parameters.Add(new OracleParameter("id", node.Id.Value));
            command.ExecuteNonQuery();
            command.Dispose();
        }

        private void Delete(Way way)
        {
            OracleCommand command;

            command = this.CreateCommand("delete from way_nodes where way_id = :way_id");
            command.Parameters.Add(new OracleParameter("way_id", way.Id.Value));
            command.ExecuteNonQuery();
            command.Dispose();

            command = this.CreateCommand("delete from way_tags where way_id = :way_id");
            command.Parameters.Add(new OracleParameter("way_id", way.Id.Value));
            command.ExecuteNonQuery();
            command.Dispose();

            command = this.CreateCommand("delete from way where id = :id");
            command.Parameters.Add(new OracleParameter("id", way.Id.Value));
            command.ExecuteNonQuery();
            command.Dispose();
        }

        private void Delete(Relation relation)
        {
            OracleCommand command;

            command = this.CreateCommand("delete from relation_members where relation_id = :relation_id");
            command.Parameters.Add(new OracleParameter("relation_id", relation.Id.Value));
            command.ExecuteNonQuery();
            command.Dispose();

            command = this.CreateCommand("delete from relation_tags where relation_id = :relation_id");
            command.Parameters.Add(new OracleParameter("relation_id", relation.Id.Value));
            command.ExecuteNonQuery();
            command.Dispose();

            command = this.CreateCommand("delete from relation where id = :id");
            command.Parameters.Add(new OracleParameter("id", relation.Id.Value));
            command.ExecuteNonQuery();
            command.Dispose();
        }

        #endregion

        /// <summary>
        /// Closes this changeset target.
        /// </summary>
        public override void Close()
        {
            base.Close();

            if (_connection != null)
            {
                _connection.Close();
                _connection = null;
            }
        }

        /// <summary>
        /// Initializes this change set apply target.
        /// </summary>
        public override void Initialize()
        {
            _connection.Open();

            //this.CreateNodeTables();
        }

        #region Events

        /// <summary>
        /// Delegate for raising changes to objects.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="object_type"></param>
        /// <param name="id"></param>
        public delegate void ChangeDelegate(ChangeType type, OsmGeoType object_type, long id);

        /// <summary>
        /// Event raised when an object was changed.
        /// </summary>
        public event ChangeDelegate Change;

        /// <summary>
        /// Raises the chance event.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="object_type"></param>
        /// <param name="id"></param>
        private void RaiseChange(ChangeType type, OsmGeoType object_type, long id)
        {
            if (Change != null)
            {
                Change(type, object_type, id);
            }
        }

        #endregion
    }
}
