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
using OsmSharp.Osm.Map.Elements;
using System.Windows.Forms;

namespace OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget
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
