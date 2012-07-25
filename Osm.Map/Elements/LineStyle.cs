using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Map.Elements
{
    public enum LineStyle
    {
        // Summary:
        //     Specifies a solid line.
        Solid = 0,
        //
        // Summary:
        //     Specifies a line consisting of dashes.
        Dash = 1,
        //
        // Summary:
        //     Specifies a line consisting of dots.
        Dot = 2,
        //
        // Summary:
        //     Specifies a line consisting of a repeating pattern of dash-dot.
        DashDot = 3,
        //
        // Summary:
        //     Specifies a line consisting of a repeating pattern of dash-dot-dot.
        DashDotDot = 4,
        //
        // Summary:
        //     Specifies a user-defined custom dash style.
        Custom = 5
    }
}
