// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

namespace OsmSharp.Math.Algorithms
{
    /// <summary>
    /// Contains all hilbert curve calculations.
    /// </summary>
    public static class HilbertCurve
    {
        // DISCLAIMER: some of this stuff is straight from wikipedia:
        // http://en.wikipedia.org/wiki/Hilbert_curve#Applications_and_mapping_algorithms

        /// <summary>
        /// Calculates hilbert distance.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="n">The accuracy, used to divide the lat/lon space.</param>
        /// <returns></returns>
        public static ulong HilbertDistance(float latitude, float longitude, int n)
        {
            // calculate x, y.
            ulong x = (ulong)(((longitude + 180) / 360.0) * n);
            ulong y = (ulong)(((latitude + 90) / 180.0) * n);

            // calculate hilbert value for x-y and n.
            return HilbertCurve.xy2d(n, x, y);
        }

        /// <summary>
        /// Calculates the hilbert distance.
        /// </summary>
        /// <param name="n">Size of space (height/width).</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns></returns>
        private static ulong xy2d(int n, ulong x, ulong y)
        {
            int rx, ry, s;
            ulong d = 0;
            for (s = n / 2; s > 0; s /= 2)
            {
                rx = (x & (ulong)s) > 0 ? 1 : 0;
                ry = (y & (ulong)s) > 0 ? 1 : 0;
                d += (ulong)(s * s * ((3 * rx) ^ ry));
                rot(s, ref x, ref y, rx, ry);
            }
            return d;
        }

        /// <summary>
        /// Rotate/flip a quadrant appropriately
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rx"></param>
        /// <param name="ry"></param>
        private static void rot(int n, ref ulong x, ref ulong y, int rx, int ry)
        {
            if (ry == 0)
            {
                if (rx == 1)
                {
                    x = (ulong)n - 1 - x;
                    y = (ulong)n - 1 - y;
                }

                //Swap x and y
                ulong t = x;
                x = y;
                y = t;
            }
        }
    }
}