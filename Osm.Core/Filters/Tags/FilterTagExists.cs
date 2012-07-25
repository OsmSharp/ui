using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core.Filters.Tags
{
    internal class FilterTagExists : FilterTag
    {
        private string _tag;

        public FilterTagExists(string tag)
        {
            _tag = tag;
        }

        public override bool Evaluate(OsmBase obj)
        {
            return obj.Tags.ContainsKey(_tag);
        }
    }
}
