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
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.UI.WinForms.MapEditorUserControl;

namespace OsmSharp.Osm.UI.WinForms.EditorUserControl.Logic
{
    /// <summary>
    /// Class applied to the map viewer user control when the user is moving/zooming the map.
    /// </summary>
    internal class MapViewingLogic : MapEditorUserControlLogic
    {
        /// <summary>
        /// True when moving the map.
        /// </summary>
        private bool _moving_map = false;

        /// <summary>
        /// Holds the latest x position.
        /// </summary>
        private int _prev_x;

        /// <summary>
        /// Holds the latest y position.
        /// </summary>
        private int _prev_y;

        /// <summary>
        /// Creates a new map viewing logic.
        /// </summary>
        /// <param name="ctrl"></param>
        internal MapViewingLogic(Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl ctrl)
            :base(ctrl)
        {

        }

        public override MapEditorUserControlLogic OnMapMouseDown(UserControlTargetEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (this.Control.SelectionMode)
                {
                    MapSelectLogic select = new MapSelectLogic(this.Control);
                    select.OnMapMouseDown(e);
                    return select;
                }
                else
                {
                    _moving_map = true;
                    _prev_x = e.X;
                    _prev_y = e.Y;
                }
            }
            return this;
        }

        public override MapEditorUserControlLogic OnMapMouseMove(UserControlTargetEventArgs e)
        {
            if (_moving_map)
            {
                double delta_x = -(e.X - _prev_x);
                double delta_y = e.Y - _prev_y;

                double delta_lon = this.Control.View.ConvertFromTargetXSize(this.Control.Target, (float)delta_x);
                double delta_lat = this.Control.View.ConvertFromTargetYSize(this.Control.Target, (float)delta_y);

                this.Control.Target.Center = new GeoCoordinate(
                    this.Control.Target.Center.Latitude + delta_lat,
                    this.Control.Target.Center.Longitude + delta_lon);

                _prev_x = e.X;
                _prev_y = e.Y;

                this.Control.Target.Invalidate();
            }
            else
            {
                this.Control.MapMouseMoveWithoutMovingMap(e);
            }
            return this;
        }

        public override MapEditorUserControlLogic OnMapMouseUp(UserControlTargetEventArgs e)
        {
            _moving_map = false;

            return this;
        }

        public override MapEditorUserControlLogic OnMapMouseWheel(UserControlTargetEventArgs e)
        {
            this.Control.Target.ZoomFactor = this.Control.Target.ZoomFactor + e.Delta * 0.001f;

            this.Control.Target.Invalidate();

            return this;
        }
    }
}
