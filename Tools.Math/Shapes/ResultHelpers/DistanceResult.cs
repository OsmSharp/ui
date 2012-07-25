using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Shapes.ResultHelpers
{
    public class DistanceResult<PointType>
        where PointType : PointF2D
    {
        internal DistanceResult()
        {

        }

        public double Distance { get; internal set; }

        public PrimitiveSimpleF2D ClosestPrimitive { get; internal set; }
    }
}
