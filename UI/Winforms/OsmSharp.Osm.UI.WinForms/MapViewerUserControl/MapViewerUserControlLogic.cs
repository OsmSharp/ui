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
using OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget;

namespace OsmSharp.Osm.UI.WinForms.MapViewerUserControl
{
    /// <summary>
    /// Base class for all logic that can be applied to the map view user control.
    /// </summary>
    internal abstract class MapViewerUserControlLogic
    {
        /// <summary>
        /// The map view user control.
        /// </summary>
        private MapViewerUserControl _ctrl;

        /// <summary>
        /// Creates a new map view control.
        /// </summary>
        protected MapViewerUserControlLogic(MapViewerUserControl ctrl)
        {
            _ctrl = ctrl;
        }

        /// <summary>
        /// Returns the control this logic is for.
        /// </summary>
        public MapViewerUserControl Control
        {
            get
            {
                return _ctrl;
            }
        }

        public abstract MapViewerUserControlLogic OnMapMouseDown(UserControlTargetEventArgs e);
        public abstract MapViewerUserControlLogic OnMapMouseMove(UserControlTargetEventArgs e);
        public abstract MapViewerUserControlLogic OnMapMouseUp(UserControlTargetEventArgs e);
        public abstract MapViewerUserControlLogic OnMapMouseWheel(UserControlTargetEventArgs e);
    }
}
