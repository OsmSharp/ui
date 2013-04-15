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
using OsmSharp.Tools.Collections;
using OsmSharp.Osm.Simple;

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
        private IList<RelationMember> _members;

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
        /// <param name="string_table"></param>
        internal protected Relation(ObjectTable<string> string_table, long id)
            : base(string_table, id)
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
            SimpleRelation relation = new SimpleRelation();
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
                SimpleRelationMember simple_member = new SimpleRelationMember();
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
    }
}
