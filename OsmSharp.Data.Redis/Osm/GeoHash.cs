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

namespace OsmSharp.Data.Redis.Osm
{
    /// <summary>
    /// Implements a custom geohash for redis.
    /// </summary>
    public static class GeoHash
    {
        private static readonly char[] chars = {
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm',
        'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
        'y', 'z'
        };
        private static readonly Dictionary<char, int> map = new Dictionary<char, int>();
        private const int precision = 12;
        private static readonly int[] bits = { 16, 8, 4, 2, 1 };

        static GeoHash()
        {
            for (int i = 0; i < chars.Length; i++)
                map.Add(chars[i], i);
        }

        /// <summary>
        /// Encodes to a geohash string.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static String Encode(double latitude, double longitude)
        {
            double[] latInterval = { -90.0, 90.0 };
            double[] lonInterval = { -180.0, 180.0 };

            var geohash = new StringBuilder();
            bool isEven = true;
            int bit = 0, ch = 0;

            while (geohash.Length < precision)
            {
                double mid;
                if (isEven)
                {
                    mid = (lonInterval[0] + lonInterval[1]) / 2;
                    if (longitude > mid)
                    {
                        ch |= bits[bit];
                        lonInterval[0] = mid;
                    }
                    else
                    {
                        lonInterval[1] = mid;
                    }

                }
                else
                {
                    mid = (latInterval[0] + latInterval[1]) / 2;
                    if (latitude > mid)
                    {
                        ch |= bits[bit];
                        latInterval[0] = mid;
                    }
                    else
                    {
                        latInterval[1] = mid;
                    }
                }

                isEven = isEven ? false : true;

                if (bit < 4)
                    bit++;
                else
                {
                    geohash.Append(chars[ch]);
                    bit = 0;
                    ch = 0;
                }
            }

            return geohash.ToString();
        }

        /// <summary>
        /// Decodes a geohash string.
        /// </summary>
        /// <param name="geohash"></param>
        /// <returns></returns>
        public static double[] Decode(String geohash)
        {
            double[] ge = DecodeExactly(geohash);
            double lat = ge[0];
            double lon = ge[1];
            double latErr = ge[2];
            double lonErr = ge[3];

            double latPrecision = System.Math.Max(1, System.Math.Round(-System.Math.Log10(latErr))) - 1;
            double lonPrecision = System.Math.Max(1, System.Math.Round(-System.Math.Log10(lonErr))) - 1;

            lat = GetPrecision(lat, latPrecision);
            lon = GetPrecision(lon, lonPrecision);

            return new[] { lat, lon };
        }

        /// <summary>
        /// Decodes a geohash string.
        /// </summary>
        /// <param name="geohash"></param>
        /// <returns></returns>
        public static double[] DecodeExactly(String geohash)
        {
            double[] latInterval = { -90.0, 90.0 };
            double[] lonInterval = { -180.0, 180.0 };

            double latErr = 90.0;
            double lonErr = 180.0;
            bool isEven = true;
            int sz = geohash.Length;
            int bsz = bits.Length;
            for (int i = 0; i < sz; i++)
            {

                int cd = map[geohash[i]];

                for (int z = 0; z < bsz; z++)
                {
                    int mask = bits[z];
                    if (isEven)
                    {
                        lonErr /= 2;
                        if ((cd & mask) != 0)
                            lonInterval[0] = (lonInterval[0] + lonInterval[1]) / 2;
                        else
                            lonInterval[1] = (lonInterval[0] + lonInterval[1]) / 2;

                    }
                    else
                    {
                        latErr /= 2;

                        if ((cd & mask) != 0)
                            latInterval[0] = (latInterval[0] + latInterval[1]) / 2;
                        else
                            latInterval[1] = (latInterval[0] + latInterval[1]) / 2;
                    }
                    isEven = isEven ? false : true;
                }

            }
            double latitude = (latInterval[0] + latInterval[1]) / 2;
            double longitude = (lonInterval[0] + lonInterval[1]) / 2;

            return new[] { latitude, longitude, latErr, lonErr };
        }

        /// <summary>
        /// Returns the precision.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static double GetPrecision(double x, double precision)
        {
            double @base = System.Math.Pow(10, -precision);
            double diff = x % @base;
            return x - diff;
        }
    }
}
