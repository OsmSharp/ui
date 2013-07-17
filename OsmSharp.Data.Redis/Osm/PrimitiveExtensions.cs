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

using System.Collections.Generic;
using OsmSharp.Collections.Tags;
using OsmSharp.Data.Redis.Osm.Streams.Primitives;
using OsmSharp.Osm.Simple;

namespace OsmSharp.Data.Redis.Osm
{
    public static class PrimitiveExtensions
    {
        public static OsmNode ConvertTo(Node node)
        {
            OsmNode new_way = new OsmNode();
            new_way.Id = node.Id.Value;
            new_way.Latitude = node.Latitude.Value;
            new_way.Longitude = node.Longitude.Value;
            new_way.Tags = PrimitiveExtensions.ConvertTo(node.Tags);

            return new_way;
        }

        public static OsmWay ConvertTo(Way way, List<long> nodes)
        {
            OsmWay new_way = new OsmWay();
            new_way.Id = way.Id.Value;
            new_way.Nds = nodes;
            new_way.Tags = PrimitiveExtensions.ConvertTo(way.Tags);

            return new_way;
        }

        public static List<OsmTag> ConvertTo(TagsCollection tags)
        {
            List<OsmTag> new_tags = new List<OsmTag>();
            if (tags != null)
            {
                foreach (Tag pair in tags)
                {
                    OsmTag tag = new OsmTag();
                    tag.Key = pair.Key;
                    tag.Value = pair.Value;

                    new_tags.Add(tag);
                }
            }
            return new_tags;
        }

        public static string BuildRedisKeySparseVertex(long id)
        {
            return "sparse:" + id;
        }

        public static string BuildRedisKeySparseSimpleVertex(long id)
        {
            return "sparse_simple:" + id;
        }

        public static string BuildRedisKeySimpleVertex(long id)
        {
            return "vertex:" + id;
        }

        public static string BuildRedisKeySimpleArc(long id)
        {
            return "arc:" + id;
        }
    }
}
