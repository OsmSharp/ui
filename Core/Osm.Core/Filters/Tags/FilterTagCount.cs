using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core.Filters.Tags
{
    internal class FilterTagCount : FilterTag
    {
        private int _cnt;

        private bool _exact;

        public FilterTagCount()
        {
            _cnt = 0;
            _exact = false;
        }

        public FilterTagCount(int cnt,bool exact)
        {
            _cnt = cnt;
            _exact = exact;
        }

        public FilterTagCount(int cnt)
        {
            _cnt = cnt;
        }
        
        public override bool Evaluate(OsmBase obj)
        {
            if (_exact)
            {
                return obj.Tags.Count == _cnt;
            }
            return obj.Tags.Count >= _cnt;
        }
    }
}
