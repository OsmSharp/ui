using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Math.Primitives
{
    /// <summary>
    /// A polyline.
    /// </summary>
    public class Polyline2D
    {
        /// <summary>
        /// Calculates the length of the given polyline.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Length(double[] x, double[] y)
        {
            double length = 0;
            if (x.Length > 1)
            {
                for (int idx = 1; idx < x.Length; idx++)
                {
                    length = length + new LineF2D(new PointF2D(x[idx - 1], y[idx - 1]),
                        new PointF2D(x[idx], y[idx])).Length;
                }
            }
            return length;
        }

        /// <summary>
        /// Returns a position on the given polyline corresponding with the given position. (0 smaller than position smaller than polyline.lenght).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static PointF2D PositionAtPosition(double[] x, double[] y, double position)
        {
            if (x.Length < 2) throw new ArgumentOutOfRangeException("Given coordinates do not represent a polyline.");

            double lenght = 0;
            LineF2D localLine = null;
            for (int idx = 1; idx < x.Length; idx++)
            {
                localLine = new LineF2D(new PointF2D(x[idx - 1], y[idx - 1]),
                    new PointF2D(x[idx], y[idx]));
                double localLength = localLine.Length;
                if(lenght + localLength > position)
                { // position is between point at idx and idx + 1.
                    double localPosition = position - lenght;
                    VectorF2D vector = localLine.Direction.Normalize() * localPosition;
                    return localLine.Point1 + vector;
                }
                lenght = lenght + localLength;
            }
            return localLine.Point1 + (localLine.Direction.Normalize() * (position - lenght));
        }
    }
}
