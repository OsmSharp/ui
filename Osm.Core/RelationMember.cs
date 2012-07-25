using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core
{
    [Serializable]
    public class RelationMember
    {
        public OsmGeo Member { get; set; }

        public string Role{ get; set; }
    }
}
