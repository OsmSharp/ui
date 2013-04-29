using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Map.Drawing2D;

namespace OsmSharp.Osm.Renderer.Gdi
{
    public static class GdiTools
    {
        /// <summary>
        /// Converts the OsmSharp line cap to System.Drawing linecap.
        /// </summary>
        /// <param name="lineCap"></param>
        /// <returns></returns>
        public static System.Drawing.Drawing2D.LineCap ConvertToGDI(
            this OsmSharp.Osm.Map.Drawing2D.LineCap lineCap)
        {
            return (System.Drawing.Drawing2D.LineCap)(int)(lineCap);
        }
    }
}
