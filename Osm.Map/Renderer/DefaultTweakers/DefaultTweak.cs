using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Renderer.Tweakers
{
    public class DefaultTweak : Tweak
    {         
        private static DefaultTweak _tweak;

        public static DefaultTweak Instance
        {
            get
            {
                if (_tweak == null)
                {
                    _tweak = new DefaultTweak();
                }
                return _tweak;
            }
        }

        private DefaultTweak()
        {

        }

        public override float TweakWidth(float zoom_factor, float width)
        {
            return width;
        }

        public override float TweakFontSize(float zoom_factor, float size)
        {
            return size;
        }

        public override int TweakColor(float zoom_factor, int argb)
        {
            return argb;
        }
    }
}
