using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Data.SQLite.Raw.Processor
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
            return (uint)Math.Floor(((lon + 180.0) * 65536.0 / 360.0));
        }

        public static uint lat2y(double lat)
        {
            return (uint)Math.Floor(((lat + 90.0) * 65536.0 / 180.0));
        }
    }
}
