using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Renderer.Tweakers
{
    public abstract class Tweak
    {
        /// <summary>
        /// Tweaks the width 
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public abstract float TweakWidth(float zoom_factor, float width);
        /// <summary>
        /// Tweaks the font size.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public abstract float TweakFontSize(float zoom_factor, float size);
        /// <summary>
        /// Tweaks the color.
        /// </summary>
        /// <param name="argb"></param>
        /// <returns></returns>
        public abstract int TweakColor(float zoom_factor, int argb);
    }
}