using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Geo.Lambert.International.Belgium
{
    /// <summary>
    /// The belgian 1972 lambert system.
    /// </summary>
    public class Belgium1972LambertProjection : LambertProjectionBase
    {
        public Belgium1972LambertProjection()
            : base("Belgium 1972 Projection",
            LambertEllipsoid.Hayford1924Ellipsoid,
            49.833334,
            51.166667,
            90,
            4.367487,
            150000.01256,
            5400088.438)
        {

        }
    }
}
