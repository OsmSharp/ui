using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Renderer.Gdi.Layers
{
    /// <summary>
    /// Custom layer.
    /// </summary>
    public abstract class GdiCustomLayer
    {
        /// <summary>
        /// The rendering function; allows any rendering on top of a GDI+ control.
        /// </summary>
        /// <param name="target"></param>
        public abstract void Render(Osm.Renderer.Gdi.IGdiTarget target, Osm.Renderer.View view);
    }
}
