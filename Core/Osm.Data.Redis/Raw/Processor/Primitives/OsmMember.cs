using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Data.Redis.Raw.Primitives
{
    public class OsmMember
    {
        public OsmMember()
        {
        }

        public OsmMember(string type, long _ref, string role)
        {
            this.Type = type;
            this.Ref = _ref;
            this.Role = role;
        }

        public string Type { get; set; }
        public long Ref { get; set; }
        public string Role { get; set; }
    }
}
