using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;

namespace Osm.Core
{
    public static class OsmHash
    {
        public static string GetOsmHashAsString(GeoCoordinate coordinate)
        {
            return OsmHash.GetOsmHashAsString(coordinate.Latitude, coordinate.Longitude);
        }

        public static string GetOsmHashAsString(double latitude, double longitude)
        {
            return OsmHash.GetOsmHashAsString(
                OsmHash.lon2x(longitude),
                OsmHash.lat2y(latitude));
        }

        public static string GetOsmHashAsString(uint x, uint y)
        {
            return "oh:" + x + ":" + y;
        }

        #region Tile Calculations

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

        public static uint lon2x(double lon)
        {
            return (uint)Math.Floor(((lon + 180.0) * 65536.0 / 360.0));
        }

        public static uint lat2y(double lat)
        {
            return (uint)Math.Floor(((lat + 90.0) * 65536.0 / 180.0));
        }

        #endregion
    }
}
