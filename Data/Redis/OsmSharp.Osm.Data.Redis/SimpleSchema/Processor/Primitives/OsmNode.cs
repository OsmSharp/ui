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

namespace OsmSharp.Osm.Data.Redis.SimpleSchema.Processort.Primitives
{
    public class OsmNode
    {
        public OsmNode()
        {
            Tags = new List<OsmTag>();
            Ways = new List<long>();
        }

        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        //public string User { get; set; }
        //public int Uid { get; set; }
        //public bool Visible { get; set; }
        //public string Version { get; set; }
        //public string Changeset { get; set; }
        //public DateTime Timestamp { get; set; }

        public List<OsmTag> Tags { get; set; }

        public List<long> Ways { get; set; }

        public string GetRedisKey()
        {
            return OsmNode.BuildRedisKey(this.Id);
        }

        public static string BuildRedisKey(long id)
        {
            return "node:" + id;
        }

        public string GetOsmHash()
        {
            var x = (uint)Math.Floor(((Longitude + 180.0) * 65536.0 / 360.0));
            var y = (uint)Math.Floor(((Latitude + 90.0) * 65536.0 / 180.0));
            return OsmNode.BuildOsmHashRedisKey(x, y);
        }

        public static string BuildOsmHashRedisKey(uint x, uint y)
        {
            return "hash:" + x + ":" + y;
        }

        public string GetGeoHash()
        {
            return GeoHash.Encode(this.Latitude, this.Longitude);
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
