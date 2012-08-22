using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Osm.Renderer.Gdi
{
    public interface IGdiTarget : ITarget
    {
        /// <summary>
        /// Returns the graphics object to draw on.
        /// </summary>
        Graphics Graphics
        {
            get;
        }

        /// <summary>
        /// Returns the pen used to draw.
        /// </summary>
        Pen Pen
        {
            get;
        }
    }
}
