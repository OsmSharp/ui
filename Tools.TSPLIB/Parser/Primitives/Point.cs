
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.TSPLIB.Parser.Primitives
{
    internal class Point
    {
        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; private set; }
        public double Y { get; private set; }
    }
}
