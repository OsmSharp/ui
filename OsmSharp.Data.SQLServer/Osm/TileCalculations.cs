// OsmSharp - OpenStreetMap (OSM) SDK
//
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

namespace OsmSharp.Data.SQLServer.Osm
{
    internal static class TileCalculations
    {
        public static long xy2tile(uint x, uint y)
        {
            uint tile = 0;
            int i;

            for (i = 15; i >= 0; i--)
            {
                tile = (tile << 1) | ((x >> i) & 1);
                tile = (tile << 1) | ((y >> i) & 1);
            }

            return Convert.ToInt64(tile);
        }

        public static uint lon2x(double lon)
        {
            return (uint)System.Math.Floor(((lon + 180.0) * 65536.0 / 360.0));
        }

        public static uint lat2y(double lat)
        {
            return (uint)System.Math.Floor(((lat + 90.0) * 65536.0 / 180.0));
        }
    }
}