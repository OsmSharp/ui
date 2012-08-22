using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Geo.Lambert.Ellipsoids
{
    /// <summary>
    /// Represents a hayford 1924 ellipsoid.
    /// </summary>
    public class Wgs1984Ellipsoid : LambertEllipsoid
    {
        public Wgs1984Ellipsoid()
            : base(6378137, 1 / 298.257223563)
        {

        }
    }
}
