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
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Tiles;

namespace OsmSharp.Data.Redis.Osm
{
    /// <summary>
    /// Converts OSM objects to a hashstring.
    /// </summary>
    public static class OsmHash
    {
        /// <summary>
        /// Returns the osm hash as a string.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public static string GetOsmHashAsString(GeoCoordinate coordinate)
        {
            return OsmHash.GetOsmHashAsString(coordinate.Latitude, coordinate.Longitude);
        }

        /// <summary>
        /// Returns the osm hash as a string.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static string GetOsmHashAsString(double latitude, double longitude)
        {
            Tile tile = Tile.CreateAroundLocation(latitude, longitude, 15);
            return OsmHash.GetOsmHashAsString(
                tile.X,
                tile.Y);
        }

        /// <summary>
        /// Returns the osm hash as a string.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static string GetOsmHashAsString(int x, int y)
        {
            return "oh:" + x + ":" + y;
        }
    }
}
