using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Geo
{
    public static class Extensions
    {
        public static double DistanceEstimate(this GeoCoordinate[] coordinates, int start, int lenght)
        {
            double distance = 0;
            for (int idx = start; idx < lenght + start; idx++)
            {
                if (idx + 1 < lenght + start)
                {
                    distance = distance +
                        coordinates[idx].DistanceEstimate(coordinates[idx + 1]).Value;
                }
            }
            return distance;
        }
    }
}
