using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Data.Redis.Raw.Primitives
{
    public class OsmWay
    {
        public OsmWay()
        {
            Nds = new List<long>();
            Tags = new List<OsmTag>();
        }

        public long Id { get; set; }
        public List<long> Nds { get; set; }
        public List<OsmTag> Tags { get; set; }

        public bool IsHighway { get { return Tags.Exists(t => t.Key == "highway"); } }

        
        public static string BuildRedisKey(long id)
        {
            return "way:" + id;
        }

        public string GetRedisKey()
        {
            return OsmWay.BuildRedisKey(this.Id);
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
