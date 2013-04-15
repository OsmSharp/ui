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
using OsmSharp.Osm.UI.WinForms.MapEditorUserControl;

namespace OsmSharp.Osm.UI.WinForms.EditorUserControl.Logic
{
    /// <summary>
    /// Logic applied to the map editor control when adding a new node.
    /// </summary>
    internal class MapEditLogicAddingNode : MapEditorUserControlLogic
    { 
        /// <summary>
        /// Creates a new map adding a node logic.
        /// </summary>
        /// <param name="ctrl"></param>
        internal MapEditLogicAddingNode(Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl ctrl)
            :base(ctrl)
        {

        }


        public override MapEditorUserControlLogic OnMapMouseDown(UserControlTargetEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.Control.ActiveLayer.AddDot(e.Position);

                this.Control.Target.Invalidate();
            }

            return new MapViewingLogic(this.Control);
        }

        public override MapEditorUserControlLogic OnMapMouseMove(UserControlTargetEventArgs e)
        {

            return this;
        }

        public override MapEditorUserControlLogic OnMapMouseUp(UserControlTargetEventArgs e)
        {

            return this;
        }

        public override MapEditorUserControlLogic OnMapMouseWheel(UserControlTargetEventArgs e)
        {

            return this;
        }
    }
}
