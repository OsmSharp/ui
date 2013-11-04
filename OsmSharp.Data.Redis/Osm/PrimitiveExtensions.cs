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

using System.Collections.Generic;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using OsmSharp.Osm.Tiles;
using OsmSharp.Data.Redis.Osm.Primitives;
using OsmSharp.Math.Geo;

namespace OsmSharp.Data.Redis.Osm
{
    /// <summary>
    /// Holds extensions to convert between OsmSharp and Redis objects.
    /// </summary>
    public static class PrimitiveExtensions
    {
        #region ConvertTo Redis objects

        /// <summary>
        /// Converts the given node to a redis node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static RedisNode ConvertTo(Node node)
        {
            RedisNode redisNode = new RedisNode();
            redisNode.Id = node.Id.Value;
            redisNode.Latitude = node.Latitude.Value;
            redisNode.Longitude = node.Longitude.Value;
            redisNode.ChangeSetId = node.ChangeSetId;
            redisNode.TimeStamp = node.TimeStamp;
            redisNode.UserId = node.UserId;
            redisNode.UserName = node.UserName;
            redisNode.Version = node.Version;
            redisNode.Visible = node.Visible;
            redisNode.Tags = PrimitiveExtensions.ConvertTo(node.Tags);

            return redisNode;
        }

        /// <summary>
        /// Converts the given way to a redis way.
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        public static RedisWay ConvertTo(Way way)
        {
            RedisWay redisWay = new RedisWay();
            redisWay.Id = way.Id.Value;
            redisWay.ChangeSetId = way.ChangeSetId;
            redisWay.TimeStamp = way.TimeStamp;
            redisWay.UserId = way.UserId;
            redisWay.UserName = way.UserName;
            redisWay.Version = way.Version;
            redisWay.Visible = way.Visible;
            redisWay.Tags = PrimitiveExtensions.ConvertTo(way.Tags);
            if (way.Nodes != null)
            {
                redisWay.Nodes = new List<long>(way.Nodes);
            }

            return redisWay;
        }

        /// <summary>
        /// Converts the given relation to a redis relation.
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        public static RedisRelation ConvertTo(Relation relation)
        {
            RedisRelation redisWay = new RedisRelation();
            redisWay.Id = relation.Id.Value;
            redisWay.ChangeSetId = relation.ChangeSetId;
            redisWay.TimeStamp = relation.TimeStamp;
            redisWay.UserId = relation.UserId;
            redisWay.UserName = relation.UserName;
            redisWay.Version = relation.Version;
            redisWay.Visible = relation.Visible;
            redisWay.Tags = PrimitiveExtensions.ConvertTo(relation.Tags);
            redisWay.Members = PrimitiveExtensions.ConvertTo(relation.Members);

            return redisWay;
        }

        /// <summary>
        /// Converts the given tagscollection to a tags list.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static List<RedisTag> ConvertTo(TagsCollectionBase tags)
        {
            List<RedisTag> redisTags =null;
            if (tags != null)
            {
                redisTags = new List<RedisTag>();
                if (tags != null)
                {
                    foreach (Tag tag in tags)
                    {
                        RedisTag redisTag = new RedisTag();
                        redisTag.Key = tag.Key;
                        redisTag.Value = tag.Value;

                        redisTags.Add(redisTag);
                    }
                }
            }
                return redisTags;
        }

        /// <summary>
        /// Converts the given relationmembers to redis members.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<RedisRelationMember> ConvertTo(List<RelationMember> list)
        {
            if (list != null && list.Count > 0)
            {
                List<RedisRelationMember> redisMembers = new List<RedisRelationMember>(list.Count);
                for (int idx = 0; idx < list.Count; idx++)
                {
                    redisMembers.Add(new RedisRelationMember()
                    {
                        Ref = list[idx].MemberId.Value,
                        Role = list[idx].MemberRole,
                        Type = list[idx].MemberType.Value
                    });
                }
                return redisMembers;
            }
            return null;
        }

        #endregion

        #region ConvertFrom Redis objects

        /// <summary>
        /// Converts the given node from a redis node.
        /// </summary>
        /// <param name="redisNode"></param>
        /// <returns></returns>
        public static Node ConvertFrom(RedisNode redisNode)
        {
            Node node = new Node();
            node.Id = redisNode.Id.Value;
            node.Latitude = redisNode.Latitude.Value;
            node.Longitude = redisNode.Longitude.Value;
            node.ChangeSetId = redisNode.ChangeSetId;
            node.TimeStamp = redisNode.TimeStamp;
            node.UserId = redisNode.UserId;
            node.UserName = redisNode.UserName;
            node.Version = redisNode.Version;
            node.Visible = redisNode.Visible;
            node.Tags = PrimitiveExtensions.ConvertFrom(redisNode.Tags);

            return node;
        }

        /// <summary>
        /// Converts the given way from a redis way.
        /// </summary>
        /// <param name="redisWay"></param>
        /// <returns></returns>
        public static Way ConvertFrom(RedisWay redisWay)
        {
            Way way = new Way();
            way.Id = redisWay.Id.Value;
            way.ChangeSetId = redisWay.ChangeSetId;
            way.TimeStamp = redisWay.TimeStamp;
            way.UserId = redisWay.UserId;
            way.UserName = redisWay.UserName;
            way.Version = redisWay.Version;
            way.Visible = redisWay.Visible;
            way.Tags = PrimitiveExtensions.ConvertFrom(redisWay.Tags);
            if (redisWay.Nodes != null)
            {
                way.Nodes = new List<long>(redisWay.Nodes);
            }

            return way;
        }

        /// <summary>
        /// Converts the given relation from a redis relation.
        /// </summary>
        /// <param name="redisRelation"></param>
        /// <returns></returns>
        public static Relation ConvertFrom(RedisRelation redisRelation)
        {
            Relation relation = new Relation();
            relation.Id = redisRelation.Id.Value;
            relation.ChangeSetId = redisRelation.ChangeSetId;
            relation.TimeStamp = redisRelation.TimeStamp;
            relation.UserId = redisRelation.UserId;
            relation.UserName = redisRelation.UserName;
            relation.Version = redisRelation.Version;
            relation.Visible = redisRelation.Visible;
            relation.Tags = PrimitiveExtensions.ConvertFrom(redisRelation.Tags);
            relation.Members = PrimitiveExtensions.ConvertFrom(redisRelation.Members);

            return relation;
        }

        /// <summary>
        /// Converts the given tagscollection from a tags list.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static TagsCollectionBase ConvertFrom(List<RedisTag> tags)
        {
            TagsCollectionBase redisTags = null;
            if (tags != null)
            {
                redisTags = new TagsCollection();
                if (tags != null)
                {
                    foreach (RedisTag redisTag in tags)
                    {
                        Tag tag = new Tag();
                        tag.Key = redisTag.Key;
                        tag.Value = redisTag.Value;

                        redisTags.Add(tag);
                    }
                }
            }
            return redisTags;
        }

        /// <summary>
        /// Converts the given relationmembers from redis members.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<RelationMember> ConvertFrom(List<RedisRelationMember> list)
        {
            if (list != null && list.Count > 0)
            {
                List<RelationMember> members1 = new List<RelationMember>(list.Count);
                for (int idx = 0; idx < list.Count; idx++)
                {
                    members1.Add(new RelationMember()
                    {
                        MemberId = list[idx].Ref,
                        MemberRole = list[idx].Role,
                        MemberType = list[idx].Type
                    });
                }
                return members1;
            }
            return null;
        }

        #endregion

        #region Redis Keys

        /// <summary>
        /// Builds a redis key for this given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetRedisKey(this Node node)
        {
            return PrimitiveExtensions.BuildNodeRedisKey(node.Id.Value);
        }

        /// <summary>
        /// Builds a redis key for the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string BuildNodeRedisKey(long id)
        {
            return string.Format("n:{0}", id);
        }

        /// <summary>
        /// Builds a redis key for the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string BuildNodeWayListRedisKey(long id)
        {
            return string.Format("n:{0}.ways", id);
        }

        /// <summary>
        /// Builds an Osm-hash.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetOsmHash(this Node node)
        {
            Tile tile = Tile.CreateAroundLocation(new Math.Geo.GeoCoordinate(node.Latitude.Value, node.Longitude.Value),
                14);
            return tile.Id.ToString();
        }

        /// <summary>
        /// Builds a list of osm-hashes.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static List<string> GetOsmHashes(GeoCoordinateBox box)
        {
            List<string> hashes = new List<string>();
            TileRange tiles = TileRange.CreateAroundBoundingBox(box, 14);
            foreach (Tile tile in tiles)
            {
                hashes.Add(tile.Id.ToString());
            }
            return hashes;
        }

        /// <summary>
        /// Builds a redis key for this given way.
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        public static string GetRedisKey(this Way way)
        {
            return PrimitiveExtensions.BuildWayRedisKey(way.Id.Value);
        }

        /// <summary>
        /// Builds a redis key for the way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string BuildWayRedisKey(long id)
        {
            return string.Format("w:{0}", id);
        }

        /// <summary>
        /// Builds a redis key for this given relation.
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        public static string GetRedisKey(this Relation relation)
        {
            return PrimitiveExtensions.BuildRelationRedisKey(relation.Id.Value);
        }

        /// <summary>
        /// Builds a redis key for the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string BuildRelationRedisKey(long id)
        {
            return string.Format("r:{0}", id);
        }

        /// <summary>
        /// Builds a redis key for the relation members relation list.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string BuildMemberRelationListRedisKey(RelationMember member)
        {
            return PrimitiveExtensions.BuildMemberRelationListRedisKey(member.MemberType.Value, member.MemberId.Value);
        }

        /// <summary>
        /// Builds a redis key for the relation members relation list.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string BuildMemberRelationListRedisKey(OsmGeoType type, long id)
        {
            string typeString = "1";
            switch (type)
            {
                case OsmGeoType.Way:
                    typeString = "2";
                    break;
                case OsmGeoType.Relation:
                    typeString = "3";
                    break;
            }
            return string.Format("m:{0}.{1}", typeString, id);
        }

        #endregion
    }
}
