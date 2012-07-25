using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core.Filters.Tags
{
    internal class FilterTagMatch : FilterTag
    {
        private string _tag;

        private string _value;

        public FilterTagMatch(string tag, string value)
        {
            _tag = tag;
            _value = value;
        }

        public override bool Evaluate(OsmBase obj)
        {
            return obj.Tags.ContainsKey(_tag)
                && obj.Tags[_tag] == _value;
        }
    }
}
