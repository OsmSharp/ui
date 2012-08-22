using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core.Filters
{
    internal class FilterType : Filter
    {
        private OsmType _type;

        public FilterType(OsmType type)
        {
            _type = type;
        }

        public override bool Evaluate(OsmBase obj)
        {
            return obj.Type == _type;
        }
    }
}
