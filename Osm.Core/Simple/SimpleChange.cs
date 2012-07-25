using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core.Simple
{
    public class SimpleChange
    {
        public List<SimpleOsmGeo> OsmGeo { get; set; }

        public SimpleChangeType Type { get; set; }
    }
}
