using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Renderer.Gdi
{
    public static class Extensions
    {
        public static System.Drawing.PointF[] ConvertToDrawing(
            this Tools.Math.PointF2D[] points)
        {
            System.Drawing.PointF[] drawing_points = new System.Drawing.PointF[points.Length];

            for (int idx = 0; idx < points.Length; idx++)
            {
                drawing_points[idx] = new System.Drawing.PointF(
                    (float)points[idx][0],
                    (float)points[idx][1]);
            }

            return drawing_points;
        }

        public static System.Drawing.PointF ConvertToDrawing(
            this Tools.Math.PointF2D point)
        {
            return new System.Drawing.PointF(
                    (float)point[0],
                    (float)point[1]);
        }
    }
}
