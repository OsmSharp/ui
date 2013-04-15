// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.Geo;
using System.Windows.Forms;

namespace OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget
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
