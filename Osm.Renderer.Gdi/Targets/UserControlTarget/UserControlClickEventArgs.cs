using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Osm.Map.Elements;
using System.Windows.Forms;

namespace Osm.Renderer.Gdi.Targets.UserControlTarget
{
    public class UserControlClickEventArgs : UserControlTargetEventArgs
    {        
        private IList<IElement> _elements;

        internal UserControlClickEventArgs(
            MouseEventArgs args,
            GeoCoordinate position,
            IList<IElement> elements)
            :base(args,position)
        {
            _elements = elements;
        }

        public IList<IElement> Elements
        {
            get
            {
                return _elements;
            }
        }
    }
}
