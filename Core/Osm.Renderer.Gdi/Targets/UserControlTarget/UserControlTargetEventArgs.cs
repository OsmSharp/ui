using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using System.Windows.Forms;

namespace Osm.Renderer.Gdi.Targets.UserControlTarget
{
    public class UserControlTargetEventArgs : MouseEventArgs
    {
        private GeoCoordinate _position;

        internal UserControlTargetEventArgs(MouseEventArgs mouse_args,
            GeoCoordinate position)
            : base(mouse_args.Button,mouse_args.Clicks,mouse_args.X,mouse_args.Y,mouse_args.Delta)
        {
            _position = position;
        }

        public GeoCoordinate Position
        {
            get
            {
                return _position;
            }
        }
    }
}
