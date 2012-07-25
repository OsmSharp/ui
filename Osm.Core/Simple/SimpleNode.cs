using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core.Simple
{
    public class SimpleNode : SimpleOsmGeo
    {
        public SimpleNode()
        {
            this.Type = SimpleOsmGeoType.Node;
        }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public override string ToString()
        {
            return string.Format("Node[{0}]", this.Id.Value);
        }
    }
}
