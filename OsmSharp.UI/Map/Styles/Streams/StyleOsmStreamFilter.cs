// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Osm;

namespace OsmSharp.UI.Map.Styles.Streams
{
    /// <summary>
    /// An osm stream filter that only leaves objects usefull for the given mapstyle.
    /// </summary>
    public class StyleOsmStreamFilter : OsmStreamFilterBase
    {
        /// <summary>
        /// Holds the style interpreter to filter for.
        /// </summary>
        private StyleInterpreter _styleInterpreter;

        /// <summary>
        /// Creates a new style osm stream interpreter.
        /// </summary>
        /// <param name="styleInterpreter"></param>
        public StyleOsmStreamFilter(StyleInterpreter styleInterpreter)
            :base(new OsmSharp.Osm.Cache.OsmDataCacheMemory())
        {
            _styleInterpreter = styleInterpreter;
        }

        /// <summary>
        /// Returns true if the given object is usefull for the given style.
        /// </summary>
        /// <param name="osmGeo"></param>
        /// <returns></returns>
        public override bool Include(OsmGeo osmGeo)
        {
            return _styleInterpreter.AppliesTo(osmGeo);
        }
    }
}