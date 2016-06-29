using System;
using System.Collections.Generic;
using OsmSharp.Math.Geo;

namespace OsmSharp.Wpf.UI.Extensions
{
    public static class GeoCoordinateBoxExtensions
    {
        private static List<Tuple<float, double>> _zoomLevels = new List<Tuple<float, double>>
        {
            new Tuple<float, double>(0, 360),
            new Tuple<float, double>(1, 180),
            new Tuple<float, double>(2, 90),
            new Tuple<float, double>(3, 45),
            new Tuple<float, double>(4, 22.5),
            new Tuple<float, double>(5, 11.25),
            new Tuple<float, double>(6, 5.625),
            new Tuple<float, double>(7, 2.813),
            new Tuple<float, double>(8, 1.406),
            new Tuple<float, double>(9, 0.703),
            new Tuple<float, double>(10, 0.352),
            new Tuple<float, double>(11, 0.176),
            new Tuple<float, double>(12, 0.088),
            new Tuple<float, double>(13, 0.044),
            new Tuple<float, double>(14, 0.022),
            new Tuple<float, double>(15, 0.011),
            new Tuple<float, double>(16, 0.005),
            new Tuple<float, double>(17, 0.003),
            new Tuple<float, double>(18, 0.001),
            new Tuple<float, double>(19, 0)// new Tuple<float, double>(19, 0.0005)
        };

        public static float GetZoomLevel(this GeoCoordinateBox box)
        {
            var level = 0f;
            var delta = System.Math.Max(box.DeltaLon, box.DeltaLat);

            for (int i = 0; i < _zoomLevels.Count - 1; i++)
            {
                var l1 = _zoomLevels[i];
                var l2 = _zoomLevels[i + 1];

                if (delta <= l1.Item2 && delta >= l2.Item2)
                {
                    var f = delta/(l2.Item2 - l1.Item2);
                    level = (float)(l2.Item1 + (l1.Item1 - l1.Item1)*f);
                    break;
                }
            }
            return level;
        }
    }
}
