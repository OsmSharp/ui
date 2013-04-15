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
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Osm.Map.Elements
{
    /// <summary>
    /// Represents a map element.
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// Returns true if the element is visible inside the given box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        bool IsVisibleIn(GeoCoordinateBox box);

        /// <summary>
        /// Returns the shortest distance to the given coordinate.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        double ShortestDistanceTo(GeoCoordinate coordinate);

        /// <summary>
        /// Returns true if the element in visible at the given zoom level.
        /// </summary>
        /// <param name="zoom_factor"></param>
        /// <returns></returns>
        bool IsVisibleAt(double zoom_factor);

        /// <summary>
        /// Returns a dictionary containing descriptive tags for this element.
        /// </summary>
        IDictionary<string, string> Tags
        {
            get;
        }
    }
}
