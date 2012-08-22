using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core.Filters
{
    internal class FilterCombined : Filter
    {
        private Filter _filter1;
        private FilterCombineOperatorEnum _op;
        private Filter _filter2;

        public FilterCombined(Filter filter1, FilterCombineOperatorEnum op, Filter filter2)
        {
            _op = op;
            _filter1 = filter1;
            _filter2 = filter2;
        }

        public override bool Evaluate(OsmBase obj)
        {
            switch (_op)
            {
                case FilterCombineOperatorEnum.And:
                    return _filter1.Evaluate(obj) && _filter2.Evaluate(obj);
                case FilterCombineOperatorEnum.Or:
                    return _filter1.Evaluate(obj) || _filter2.Evaluate(obj);
                case FilterCombineOperatorEnum.Not:
                    return !_filter1.Evaluate(obj);
            }
            return false;
        }
    }

    internal enum FilterCombineOperatorEnum
    {
        And,
        Or,
        Not
    }
}
