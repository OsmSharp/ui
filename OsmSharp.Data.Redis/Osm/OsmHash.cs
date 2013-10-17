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

namespace OsmSharp.Data.Redis.Osm
{
    /// <summary>
    /// Converts OSM objects to a hashstring.
    /// </summary>
    public static class OsmHash
    {
        /// <summary>
        /// 
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
            return OsmHash.GetOsmHashAsString(
                OsmHash.lon2x(longitude),
                OsmHash.lat2y(latitude));
        }

        /// <summary>
        /// Returns the osm hash as a string.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static string GetOsmHashAsString(uint x, uint y)
        {
            return "oh:" + x + ":" + y;
        }

        #region Tile Calculations

        /// <summary>
        /// Returns a hashed version based on the x- and y-coordinate.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static uint xy2tile(uint x, uint y)
        {
            uint tile = 0;
            int i;

            for (i = 15; i >= 0; i--)
            {
                tile = (tile << 1) | ((x >> i) & 1);
                tile = (tile << 1) | ((y >> i) & 1);
            }

            return tile;
        }

        /// <summary>
        /// Converts the given lon to a tiled x-coordinate.
        /// </summary>
        /// <param name="lon"></param>
        /// <returns></returns>
        public static uint lon2x(double lon)
        {
            return (uint)System.Math.Floor(((lon + 180.0) * 65536.0 / 360.0));
        }

        /// <summary>
        /// Converts the given lat to a tiled y-coordinate.
        /// </summary>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static uint lat2y(double lat)
        {
            return (uint)System.Math.Floor(((lat + 90.0) * 65536.0 / 180.0));
        }

        #endregion
    }
}
