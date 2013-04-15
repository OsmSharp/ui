// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using OsmSharp.Osm;
using OsmSharp.Osm.Simple;
using OsmSharp.Tools.Collections;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Osm.Factory
{
    /// <summary>
    /// A factory for OSM base objects.
    /// </summary>
    public static class OsmBaseFactory
    {
        /// <summary>
        /// Creates a new node with a new id.
        /// </summary>
        /// <returns></returns>
        public static Node CreateNode()
        {
            return CreateNode(OsmBaseIdGenerator.NewId());
        }

        /// <summary>
        /// Creates a new node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Node CreateNode(long id)
        {
            return new Node(id);
        }

        /// <summary>
        /// Creates a new node using the given stringtable.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static Node CreateNode(ObjectTable<string> table)
        {
            return CreateNode(table, OsmBaseIdGenerator.NewId());
        }

        /// <summary>
        /// Creates a new node using a given string table with the given id.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Node CreateNode(ObjectTable<string> table, long id)
        {
            return new Node(table, id);
        }

        /// <summary>
        /// Creates a new node from a SimpleNode.
        /// </summary>
        /// <param name="simple_node"></param>
        /// <returns></returns>
        public static Node CreateNodeFrom(SimpleNode simple_node)
        {
            Node node = OsmBaseFactory.CreateNode(simple_node.Id.Value);

            node.ChangeSetId = simple_node.ChangeSetId;
            node.Coordinate = new GeoCoordinate(simple_node.Latitude.Value, simple_node.Longitude.Value);
            if (simple_node.Tags != null)
            {
                foreach (KeyValuePair<string, string> pair in simple_node.Tags)
                {
                    node.Tags.Add(pair);
                }
            }
            node.TimeStamp = simple_node.TimeStamp;
            node.User = simple_node.UserName;
            node.UserId = simple_node.UserId;
            node.Version = simple_node.Version.HasValue ? (long)simple_node.Version.Value : (long?)null;
            node.Visible = simple_node.Visible.HasValue ? simple_node.Visible.Value : false;

            return node;
        }

        /// <summary>
        /// Creates a new node from a SimpleNode using a string table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="simple_node"></param>
        /// <returns></returns>
        public static Node CreateNodeFrom(ObjectTable<string> table, SimpleNode simple_node)
        {
            Node node = OsmBaseFactory.CreateNode(table, simple_node.Id.Value);

            node.ChangeSetId = simple_node.ChangeSetId;
            node.Coordinate = new GeoCoordinate(simple_node.Latitude.Value, simple_node.Longitude.Value);
            if (simple_node.Tags != null)
            {
                foreach (KeyValuePair<string, string> pair in simple_node.Tags)
                {
                    node.Tags.Add(pair);
                }
            }
            node.TimeStamp = simple_node.TimeStamp;
            node.User = simple_node.UserName;
            node.UserId = simple_node.UserId;
            node.Version = simple_node.Version.HasValue ? (long)simple_node.Version.Value : (long?)null;
            node.Visible = simple_node.Visible.HasValue ? simple_node.Visible.Value : false;

            return node;
        }

        /// <summary>
        /// Creates a new way.
        /// </summary>
        /// <returns></returns>
        public static Way CreateWay()
        {
            return CreateWay(OsmBaseIdGenerator.NewId());
        }

        /// <summary>
        /// Creates a new way with a given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Way CreateWay(long id)
        {
            return new Way(id);
        }

        /// <summary>
        /// Creates a new way from a SimpleWay given a dictionary with nodes.
        /// </summary>
        /// <param name="simple_way"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static Way CreateWayFrom(SimpleWay simple_way, IDictionary<long, Node> nodes)
        {
            Way way = OsmBaseFactory.CreateWay(simple_way.Id.Value);

            way.ChangeSetId = simple_way.ChangeSetId;
            foreach (KeyValuePair<string, string> pair in simple_way.Tags)
            {
                way.Tags.Add(pair);
            }
            for (int idx = 0; idx < simple_way.Nodes.Count; idx++)
            {
                long node_id = simple_way.Nodes[idx];
                Node node = null;
                if (nodes.TryGetValue(node_id, out node))
                {
                    way.Nodes.Add(node);
                }
                else
                {
                    return null;
                }
            }
            way.TimeStamp = simple_way.TimeStamp;
            way.User = simple_way.UserName;
            way.UserId = simple_way.UserId;
            way.Version = simple_way.Version.HasValue ? (long)simple_way.Version.Value : (long?)null;
            way.Visible = simple_way.Visible.HasValue ? simple_way.Visible.Value : false;

            return way;
        }

        /// <summary>
        /// Creates a new way with a new id given a stringtable.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static Way CreateWay(ObjectTable<string> table)
        {
            return CreateWay(table, OsmBaseIdGenerator.NewId());
        }

        /// <summary>
        /// Creates a new way with a given id given a stringtable.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Way CreateWay(ObjectTable<string> table, long id)
        {
            return new Way(table, id);
        }

        /// <summary>
        /// Creates a new way from a SimpleWay given a stringtable.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="simple_way"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static Way CreateWayFrom(ObjectTable<string> table, SimpleWay simple_way, IDictionary<long, Node> nodes)
        {
            Way way = OsmBaseFactory.CreateWay(table, simple_way.Id.Value);

            way.ChangeSetId = simple_way.ChangeSetId;
            foreach (KeyValuePair<string, string> pair in simple_way.Tags)
            {
                way.Tags.Add(pair);
            }
            for (int idx = 0; idx < simple_way.Nodes.Count; idx++)
            {
                long node_id = simple_way.Nodes[idx];
                Node node = null;
                if (nodes.TryGetValue(node_id, out node))
                {
                    way.Nodes.Add(node);
                }
                else
                {
                    return null;
                }
            }
            way.TimeStamp = simple_way.TimeStamp;
            way.User = simple_way.UserName;
            way.UserId = simple_way.UserId;
            way.Version = simple_way.Version.HasValue ? (long)simple_way.Version.Value : (long?)null;
            way.Visible = simple_way.Visible.HasValue ? simple_way.Visible.Value : false;

            return way;
        }

        /// <summary>
        /// Creates a relation with a new id.
        /// </summary>
        /// <returns></returns>
        public static Relation CreateRelation()
        {
            return CreateRelation(OsmBaseIdGenerator.NewId());
        }

        /// <summary>
        /// Creates a relation with a given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Relation CreateRelation(long id)
        {
            return new Relation(id);
        }

        /// <summary>
        /// Creates a relation from a SimpleRelation.
        /// </summary>
        /// <param name="simple_relation"></param>
        /// <param name="nodes"></param>
        /// <param name="ways"></param>
        /// <param name="relations"></param>
        /// <returns></returns>
        public static Relation CreateRelationFrom(SimpleRelation simple_relation, 
            IDictionary<long, Node> nodes,
            IDictionary<long, Way> ways,
            IDictionary<long, Relation> relations)
        {
            Relation relation = OsmBaseFactory.CreateRelation(simple_relation.Id.Value);

            relation.ChangeSetId = simple_relation.ChangeSetId;
            foreach (KeyValuePair<string, string> pair in simple_relation.Tags)
            {
                relation.Tags.Add(pair);
            }
            for (int idx = 0; idx < simple_relation.Members.Count; idx++)
            {
                long member_id = simple_relation.Members[idx].MemberId.Value;
                string role = simple_relation.Members[idx].MemberRole;

                RelationMember member = new RelationMember();
                member.Role = role;
                switch (simple_relation.Members[idx].MemberType.Value)
                {
                    case SimpleRelationMemberType.Node:
                        Node node = null;
                        if (nodes.TryGetValue(member_id, out node))
                        {
                            member.Member = node;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case SimpleRelationMemberType.Way:
                        Way way = null;
                        if (ways.TryGetValue(member_id, out way))
                        {
                            member.Member = way;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case SimpleRelationMemberType.Relation:
                        Relation relation_member = null;
                        if (relations.TryGetValue(member_id, out relation_member))
                        {
                            member.Member = relation_member;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                }
                relation.Members.Add(member);
            }
            relation.TimeStamp = simple_relation.TimeStamp;
            relation.User = simple_relation.UserName;
            relation.UserId = simple_relation.UserId;
            relation.Version = simple_relation.Version.HasValue ? (long)simple_relation.Version.Value : (long?)null;
            relation.Visible = simple_relation.Visible.HasValue ? simple_relation.Visible.Value : false;

            return relation;
        }

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static Relation CreateRelation(ObjectTable<string> table)
        {
            return CreateRelation(table, OsmBaseIdGenerator.NewId());
        }

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Relation CreateRelation(ObjectTable<string> table, long id)
        {
            return new Relation(table, id);
        }

        /// <summary>
        /// Creates a new relation from a SimpleRelation.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="simple_relation"></param>
        /// <param name="nodes"></param>
        /// <param name="ways"></param>
        /// <param name="relations"></param>
        /// <returns></returns>
        public static Relation CreateRelationFrom(ObjectTable<string> table, SimpleRelation simple_relation,
            IDictionary<long, Node> nodes,
            IDictionary<long, Way> ways,
            IDictionary<long, Relation> relations)
        {
            Relation relation = OsmBaseFactory.CreateRelation(table, simple_relation.Id.Value);

            relation.ChangeSetId = simple_relation.ChangeSetId;
            foreach (KeyValuePair<string, string> pair in simple_relation.Tags)
            {
                relation.Tags.Add(pair);
            }
            for (int idx = 0; idx < simple_relation.Members.Count; idx++)
            {
                long member_id = simple_relation.Members[idx].MemberId.Value;
                string role = simple_relation.Members[idx].MemberRole;

                RelationMember member = new RelationMember();
                member.Role = role;
                switch (simple_relation.Members[idx].MemberType.Value)
                {
                    case SimpleRelationMemberType.Node:
                        Node node = null;
                        if (nodes.TryGetValue(member_id, out node))
                        {
                            member.Member = node;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case SimpleRelationMemberType.Way:
                        Way way = null;
                        if (ways.TryGetValue(member_id, out way))
                        {
                            member.Member = way;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case SimpleRelationMemberType.Relation:
                        Relation relation_member = null;
                        if (relations.TryGetValue(member_id, out relation_member))
                        {
                            member.Member = relation_member;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                }
                relation.Members.Add(member);
            }
            relation.TimeStamp = simple_relation.TimeStamp;
            relation.User = simple_relation.UserName;
            relation.UserId = simple_relation.UserId;
            relation.Version = simple_relation.Version.HasValue ? (long)simple_relation.Version.Value : (long?)null;
            relation.Visible = simple_relation.Visible.HasValue ? simple_relation.Visible.Value : false;

            return relation;
        }

        /// <summary>
        /// Creates a new changeset.
        /// </summary>
        /// <returns></returns>
        public static ChangeSet CreateChangeSet()
        {
            return CreateChangeSet(OsmBaseIdGenerator.NewId());
        }

        /// <summary>
        /// Creates a new changeset.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ChangeSet CreateChangeSet(long id)
        {
            return new ChangeSet(id);
        }
    }
}