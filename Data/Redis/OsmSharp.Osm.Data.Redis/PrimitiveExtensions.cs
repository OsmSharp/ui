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
using OsmSharp.Osm.Simple;
using OsmSharp.Osm.Data.Redis.SimpleSchema.Processort.Primitives;

namespace OsmSharp.Osm.Data.Redis
{
    public static class PrimitiveExtensions
    {
        public static OsmNode ConvertTo(SimpleNode node)
        {
            OsmNode new_way = new OsmNode();
            new_way.Id = node.Id.Value;
            new_way.Latitude = node.Latitude.Value;
            new_way.Longitude = node.Longitude.Value;
            new_way.Tags = PrimitiveExtensions.ConvertTo(node.Tags);

            return new_way;
        }

        public static OsmWay ConvertTo(SimpleWay way, List<long> nodes)
        {
            OsmWay new_way = new OsmWay();
            new_way.Id = way.Id.Value;
            new_way.Nds = nodes;
            new_way.Tags = PrimitiveExtensions.ConvertTo(way.Tags);

            return new_way;
        }

        public static List<OsmTag> ConvertTo(IDictionary<string, string> tags)
        {
            List<OsmTag> new_tags = new List<OsmTag>();
            if (tags != null)
            {
                foreach (KeyValuePair<string, string> pair in tags)
                {
                    OsmTag tag = new OsmTag();
                    tag.Key = pair.Key;
                    tag.Value = pair.Value;

                    new_tags.Add(tag);
                }
            }
            return new_tags;
        }

        //public static string BuildRedisKey(this SparseVertex vertex)
        //{
        //    return PrimitiveExtensions.BuildRedisKeySparseVertex(vertex.Id);
        //}

        public static string BuildRedisKeySparseVertex(long id)
        {
            return "sparse:" + id;
        }

        //public static string BuildRedisKey(this SparseSimpleVertex vertex)
        //{
        //    return PrimitiveExtensions.BuildRedisKeySparseSimpleVertex(vertex.Id);
        //}

        public static string BuildRedisKeySparseSimpleVertex(long id)
        {
            return "sparse_simple:" + id;
        }

        //public static string BuildRedisKey(this SimpleVertex vertex)
        //{
        //    return PrimitiveExtensions.BuildRedisKeySimpleVertex(vertex.Id);
        //}

        public static string BuildRedisKeySimpleVertex(long id)
        {
            return "vertex:" + id;
        }

        //public static string BuildRedisKey(this SimpleArc arc)
        //{
        //    return PrimitiveExtensions.BuildRedisKeySimpleArc(arc.Id);
        //}

        public static string BuildRedisKeySimpleArc(long id)
        {
            return "arc:" + id;
        }

        ///// <summary>
        ///// Builds the redis key for a node.
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public static string BuildNodeRedisKey(long id)
        //{
        //    return "node:" + id;
        //}

        ///// <summary>
        ///// Returns the redis key.
        ///// </summary>
        ///// <param name="node"></param>
        ///// <returns></returns>
        //public static string GetRedisKey(this SimpleNode node)
        //{
        //    return PrimitiveExtensions.BuildNodeRedisKey(node.Id.Value);
        //}

        ///// <summary>
        ///// Builds the redis key for a way.
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public static string BuildWayRedisKey(long id)
        //{
        //    return "way:" + id;
        //}

        ///// <summary>
        ///// Returns the redis key.
        ///// </summary>
        ///// <param name="way"></param>
        ///// <returns></returns>
        //public static string GetRedisKey(this SimpleWay way)
        //{
        //    return PrimitiveExtensions.BuildWayRedisKey(way.Id.Value);
        //}

        ///// <summary>
        ///// Builds the redis key for a relation.
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public static string BuildRelationRedisKey(long id)
        //{
        //    return "rel:" + id;
        //}

        ///// <summary>
        ///// Returns the redis key.
        ///// </summary>
        ///// <param name="way"></param>
        ///// <returns></returns>
        //public static string GetRedisKey(this SimpleRelation relation)
        //{
        //    return PrimitiveExtensions.BuildRelationRedisKey(relation.Id.Value);
        //}
    }
}
