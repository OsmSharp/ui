using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core.Simple
{
    public class SimpleWay : SimpleOsmGeo
    {
        public SimpleWay()
        {
            this.Type = SimpleOsmGeoType.Way;
        }

        public List<long>  Nodes { get; set; }


        public override string ToString()
        {
            return string.Format("Way[{0}]", this.Id.Value);
        }
    }
}
