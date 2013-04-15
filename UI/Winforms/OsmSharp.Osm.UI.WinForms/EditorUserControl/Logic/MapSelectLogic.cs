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
using OsmSharp.Osm.UI.WinForms.MapEditorUserControl;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Osm.UI.WinForms.EditorUserControl.Logic
{
    internal class MapSelectLogic :  MapEditorUserControlLogic
    {
        /// <summary>
        /// The place where selectiong started!
        /// </summary>
        private System.Drawing.Point? _selection_start;
        private GeoCoordinate _selection_start_geo;
        
        /// <summary>
        /// The place where selectiong started!
        /// </summary>
        private System.Drawing.Point? _selection_end;
        private GeoCoordinate _selection_end_geo;
                
        /// <summary>
        /// Creates a new map viewing logic.
        /// </summary>
        /// <param name="ctrl"></param>
        internal MapSelectLogic(Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl ctrl)
            :base(ctrl)
        {

        }


        public override MapEditorUserControlLogic OnMapMouseDown(Renderer.Gdi.Targets.UserControlTarget.UserControlTargetEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (this.Control.SelectionMode)
                {
                    _selection_start = e.Location;
                    _selection_start_geo = e.Position;
                }
                else
                {
                    MapViewingLogic new_logic = new MapViewingLogic(this.Control);
                    new_logic.OnMapMouseDown(e);
                    return new_logic;
                }
            }
            return this;
        }

        public override MapEditorUserControlLogic OnMapMouseUp(Renderer.Gdi.Targets.UserControlTarget.UserControlTargetEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (_selection_end_geo != null)
                {
                    this.Control.MapSelectionEnd(new GeoCoordinateBox(_selection_start_geo, _selection_end_geo));
                    _selection_end = null;
                    _selection_start = null;
                }
            }
            return new MapViewingLogic(this.Control);
        }

        public override MapEditorUserControlLogic OnMapMouseMove(Renderer.Gdi.Targets.UserControlTarget.UserControlTargetEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _selection_end = e.Location;
                _selection_end_geo = e.Position;

                this.Control.DrawSelectionBox(_selection_start.Value,_selection_end.Value);
            }
            return this;
        }

        public override MapEditorUserControlLogic OnMapMouseWheel(Renderer.Gdi.Targets.UserControlTarget.UserControlTargetEventArgs e)
        {
            return this;
        }
    }
}
