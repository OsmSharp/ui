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
using OsmSharp.Collections;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Simple;
using OsmSharp.Osm.Simple.Cache;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Relation class.
    /// </summary>
    public class Relation : OsmGeo
    {
        /// <summary>
        /// Holds the members of this relation.
        /// </summary>
        private readonly IList<RelationMember> _members;

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <param name="id"></param>
        internal protected Relation(long id)
            :base(id)
        {
            _members = new List<RelationMember>();
        }

        /// <summary>
        /// Creates a new relation using a string table.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stringTable"></param>
        internal protected Relation(ObjectTable<string> stringTable, long id)
            : base(stringTable, id)
        {
            _members = new List<RelationMember>();
        }

        /// <summary>
        /// Returns the relation type.
        /// </summary>
        public override OsmType Type
        {
            get { return OsmType.Relation; }
        }

        /// <summary>
        /// Gets the relation members.
        /// </summary>
        public IList<RelationMember> Members
        {
            get
            {
                return _members;
            }
        }

        /// <summary>
        /// Find a member in this relation with the given role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public OsmBase FindMember(string role)
        {
            if (this.Members != null)
            {
                foreach (RelationMember member in this.Members)
                {
                    if (member.Role == role)
                    {
                        return member.Member;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Converts this relation into it's simple counterpart.
        /// </summary>
        /// <returns></returns>
        public override SimpleOsmGeo ToSimple()
        {
            var relation = new SimpleRelation();
            relation.Id = this.Id;
            relation.ChangeSetId = this.ChangeSetId;
            relation.Tags = this.Tags;
            relation.TimeStamp = this.TimeStamp;
            relation.UserId = this.UserId;
            relation.UserName = this.User;
            relation.Version = (ulong?)this.Version;
            relation.Visible = this.Visible;

            relation.Members = new List<SimpleRelationMember>();
            foreach (RelationMember member in this.Members)
            {
                var simple_member = new SimpleRelationMember();
                simple_member.MemberId = member.Member.Id;
                simple_member.MemberRole = member.Role;
                switch(member.Member.Type)
                {
                    case OsmType.Node:
                        simple_member.MemberType = SimpleRelationMemberType.Node;
                        break;
                    case OsmType.Relation:
                        simple_member.MemberType = SimpleRelationMemberType.Relation;
                        break;
                    case OsmType.Way:
                        simple_member.MemberType = SimpleRelationMemberType.Way;
                        break;
                }
                relation.Members.Add(simple_member);
            }
            return relation;
        }

        /// <summary>
        /// Returns all the coordinates in this way in the same order as the nodes.
        /// </summary>
        /// <returns></returns>
        public IList<GeoCoordinate> GetCoordinates()
        {
            var coordinates = new List<GeoCoordinate>();

            for (int idx = 0; idx < this.Members.Count; idx++)
            {
                if (this.Members[idx].Member is Node)
                {
                    var node = this.Members[idx].Member as Node;
                    coordinates.Add(node.Coordinate);
                }
                else if (this.Members[idx].Member is Way)
                {
                    var way = this.Members[idx].Member as Way;
                    coordinates.AddRange(way.GetCoordinates());
                }
                else if (this.Members[idx].Member is Relation)
                {
                    var relation = this.Members[idx].Member as Relation;
                    coordinates.AddRange(relation.GetCoordinates());
                }
            }

            return coordinates;
        }

        #region Relation factory functions

        /// <summary>
        /// Creates a relation with a new id.
        /// </summary>
        /// <returns></returns>
        public static Relation Create()
        {
            return Create(OsmBaseIdGenerator.NewId());
        }

        /// <summary>
        /// Creates a relation with a given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Relation Create(long id)
        {
            return new Relation(id);
        }

        /// <summary>
        /// Creates a relation from a SimpleRelation.
        /// </summary>
        /// <param name="simpleRelation"></param>
        /// <param name="nodes"></param>
        /// <param name="ways"></param>
        /// <param name="relations"></param>
        /// <returns></returns>
        public static Relation CreateFrom(SimpleRelation simpleRelation,
            IDictionary<long, Node> nodes,
            IDictionary<long, Way> ways,
            IDictionary<long, Relation> relations)
        {
            if (simpleRelation == null) throw new ArgumentNullException("simpleRelation");
            if (nodes == null) throw new ArgumentNullException("nodes");
            if (ways == null) throw new ArgumentNullException("ways");
            if (relations == null) throw new ArgumentNullException("relations");
            if (simpleRelation.Id == null) throw new Exception("simpleRelation.Id is null");

            Relation relation = Create(simpleRelation.Id.Value);

            relation.ChangeSetId = simpleRelation.ChangeSetId;
            foreach (Tag pair in simpleRelation.Tags)
            {
                relation.Tags.Add(pair);
            }
            for (int idx = 0; idx < simpleRelation.Members.Count; idx++)
            {
                long memberId = simpleRelation.Members[idx].MemberId.Value;
                string role = simpleRelation.Members[idx].MemberRole;

                var member = new RelationMember();
                member.Role = role;
                switch (simpleRelation.Members[idx].MemberType.Value)
                {
                    case SimpleRelationMemberType.Node:
                        Node node = null;
                        if (nodes.TryGetValue(memberId, out node))
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
                        if (ways.TryGetValue(memberId, out way))
                        {
                            member.Member = way;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case SimpleRelationMemberType.Relation:
                        Relation relationMember = null;
                        if (relations.TryGetValue(memberId, out relationMember))
                        {
                            member.Member = relationMember;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                }
                relation.Members.Add(member);
            }
            relation.TimeStamp = simpleRelation.TimeStamp;
            relation.User = simpleRelation.UserName;
            relation.UserId = simpleRelation.UserId;
            relation.Version = simpleRelation.Version.HasValue ? (long)simpleRelation.Version.Value : (long?)null;
            relation.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;

            return relation;
        }

        /// <summary>
        /// Creates a relation from a SimpleRelation.
        /// </summary>
        /// <param name="simpleRelation"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static Relation CreateFrom(SimpleRelation simpleRelation,
            OsmDataCache cache)
        {
            if (simpleRelation == null) throw new ArgumentNullException("simpleRelation");
            if (cache == null) throw new ArgumentNullException("cache");
            if (simpleRelation.Id == null) throw new Exception("simpleRelation.Id is null");

            Relation relation = Create(simpleRelation.Id.Value);

            relation.ChangeSetId = simpleRelation.ChangeSetId;
            foreach (Tag pair in simpleRelation.Tags)
            {
                relation.Tags.Add(pair);
            }
            for (int idx = 0; idx < simpleRelation.Members.Count; idx++)
            {
                long memberId = simpleRelation.Members[idx].MemberId.Value;
                string role = simpleRelation.Members[idx].MemberRole;

                var member = new RelationMember();
                member.Role = role;
                switch (simpleRelation.Members[idx].MemberType.Value)
                {
                    case SimpleRelationMemberType.Node:
                        SimpleNode simpleNode = null;
                        if (cache.TryGetNode(memberId, out simpleNode))
                        {
                            member.Member = Node.CreateFrom(simpleNode);
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case SimpleRelationMemberType.Way:
                        SimpleWay simpleWay = null;
                        if (cache.TryGetWay(memberId, out simpleWay))
                        {
                            member.Member = Way.CreateFrom(simpleWay, cache);
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case SimpleRelationMemberType.Relation:
                        SimpleRelation relationMember = null;
                        if (cache.TryGetRelation(memberId, out relationMember))
                        {
                            member.Member = Relation.CreateFrom(relationMember, cache);
                        }
                        else
                        {
                            return null;
                        }
                        break;
                }
                relation.Members.Add(member);
            }
            relation.TimeStamp = simpleRelation.TimeStamp;
            relation.User = simpleRelation.UserName;
            relation.UserId = simpleRelation.UserId;
            relation.Version = simpleRelation.Version.HasValue ? (long)simpleRelation.Version.Value : (long?)null;
            relation.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;

            return relation;
        }

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static Relation Create(ObjectTable<string> table)
        {
            return Create(table, OsmBaseIdGenerator.NewId());
        }

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Relation Create(ObjectTable<string> table, long id)
        {
            return new Relation(table, id);
        }

        /// <summary>
        /// Creates a new relation from a SimpleRelation.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="simpleRelation"></param>
        /// <param name="nodes"></param>
        /// <param name="ways"></param>
        /// <param name="relations"></param>
        /// <returns></returns>
        public static Relation CreateFrom(ObjectTable<string> table, SimpleRelation simpleRelation,
            IDictionary<long, Node> nodes,
            IDictionary<long, Way> ways,
            IDictionary<long, Relation> relations)
        {
            if (simpleRelation == null) throw new ArgumentNullException("simpleRelation");
            if (nodes == null) throw new ArgumentNullException("nodes");
            if (ways == null) throw new ArgumentNullException("ways");
            if (relations == null) throw new ArgumentNullException("relations");
            if (simpleRelation.Id == null) throw new Exception("simpleRelation.Id is null");

            Relation relation = Create(table, simpleRelation.Id.Value);

            relation.ChangeSetId = simpleRelation.ChangeSetId;
            foreach (Tag pair in simpleRelation.Tags)
            {
                relation.Tags.Add(pair);
            }
            for (int idx = 0; idx < simpleRelation.Members.Count; idx++)
            {
                long memberId = simpleRelation.Members[idx].MemberId.Value;
                string role = simpleRelation.Members[idx].MemberRole;

                var member = new RelationMember();
                member.Role = role;
                switch (simpleRelation.Members[idx].MemberType.Value)
                {
                    case SimpleRelationMemberType.Node:
                        Node node = null;
                        if (nodes.TryGetValue(memberId, out node))
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
                        if (ways.TryGetValue(memberId, out way))
                        {
                            member.Member = way;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case SimpleRelationMemberType.Relation:
                        Relation relationMember = null;
                        if (relations.TryGetValue(memberId, out relationMember))
                        {
                            member.Member = relationMember;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                }
                relation.Members.Add(member);
            }
            relation.TimeStamp = simpleRelation.TimeStamp;
            relation.User = simpleRelation.UserName;
            relation.UserId = simpleRelation.UserId;
            relation.Version = simpleRelation.Version.HasValue ? (long)simpleRelation.Version.Value : (long?)null;
            relation.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;

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

        #endregion
    }
}
