using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core.Simple
{
    public class SimpleRelation : SimpleOsmGeo
    {
        public SimpleRelation()
        {
            this.Type = SimpleOsmGeoType.Relation;
        }

        public List<SimpleRelationMember> Members { get; set; }
        
        public override string ToString()
        {
            return string.Format("Relation[{0}]", this.Id.Value);
        }
    }
}
