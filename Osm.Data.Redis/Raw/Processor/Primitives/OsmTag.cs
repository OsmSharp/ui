using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Data.Redis.Raw.Primitives
{
    public class OsmTag
    {
        public OsmTag()
        {
        }

        public OsmTag(string k, string v)
        {
            this.Key = k;
            this.Value = v;
        }

        public string Key { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return this.Key + " = " + this.Value;
        }
    }
}
