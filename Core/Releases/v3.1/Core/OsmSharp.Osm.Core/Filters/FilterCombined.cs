// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Osm.Core.Filters
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
