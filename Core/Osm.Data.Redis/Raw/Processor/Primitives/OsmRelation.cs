using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Data.Redis.Raw.Primitives
{
    public class OsmRelation
    {
        public OsmRelation()
        {
            Members = new List<OsmMember>();
            Tags = new List<OsmTag>();
        }

        public long Id { get; set; }
        public List<OsmMember> Members { get; set; }
        public List<OsmTag> Tags { get; set; }

        public string GetRedisKey()
        {
            return "rel:" + this.Id;
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
