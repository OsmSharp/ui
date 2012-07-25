using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Geo.Lambert.Ellipsoids
{
    /// <summary>
    /// Represents a hayford 1924 ellipsoid.
    /// </summary>
    public class Hayford1924Ellipsoid : LambertEllipsoid
    {
        public Hayford1924Ellipsoid()
            : base(6378388f, 1.0f / 297f)
        {

        }
    }
}
