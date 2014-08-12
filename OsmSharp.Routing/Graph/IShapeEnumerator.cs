// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2014 Abelshausen Ben
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

using OsmSharp.Math.Geo.Simple;
using System.Collections.Generic;

namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// Represents an ordered list of coordinates representing a shape.
    /// </summary>
    public interface IShapeEnumerator : IEnumerable<GeoCoordinateSimple>, IEnumerator<GeoCoordinateSimple>
    {
        /// <summary>
        /// Returns the current latitude.
        /// </summary>
        float Latitude
        {
            get;
        }

        /// <summary>
        /// Returns the current longitude.
        /// </summary>
        float Longitude
        {
            get;
        }

        /// <summary>
        /// Returns the number of coordinates.
        /// </summary>
        int Count
        {
            get;
        }
    }
}