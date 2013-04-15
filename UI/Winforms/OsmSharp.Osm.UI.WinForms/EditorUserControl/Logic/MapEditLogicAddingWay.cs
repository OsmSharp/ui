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
using OsmSharp.Osm.Map.Elements;
using System.Drawing;

namespace OsmSharp.Osm.UI.WinForms.EditorUserControl.Logic
{
    /// <summary>
    /// Logic applied to the map editor control when adding a new node.
    /// </summary>
    internal class MapEditLogicAddingWay : MapEditorUserControlLogic
    {
        /// <summary>
        /// The first dot added in the way.
        /// </summary>
        private ElementDot _first;

        /// <summary>
        /// The line being constructed.
        /// </summary>
        private ElementLine _line;

        /// <summary>
        /// The line being used for interactivity.
        /// </summary>
        private ElementLine _interactive_line;

        /// <summary>
        /// Creates a new map adding a node logic.
        /// </summary>
        /// <param name="ctrl"></param>
        internal MapEditLogicAddingWay(Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl ctrl)
            : base(ctrl)
        {

        }


        public override MapEditorUserControlLogic OnMapMouseDown(UserControlTargetEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (_first == null)
                {
                    _first = this.Control.ActiveLayer.AddDot(e.Position);
                }
                else
                {
                    if (_line == null)
                    {
                        _line = this.Control.ActiveLayer.AddLine(_first, e.Position,true);
                    }
                    else
                    {
                        _line = this.Control.ActiveLayer.AddLine(_line, e.Position,true);
                    }
                }

                this.Control.Target.Invalidate();
            }
            else
            {
                if (_interactive_line != null)
                {
                    this.Control.ActiveLayer.RemoveElement(_interactive_line);
                    this.Control.ActiveLayer.AddElement(_line);
                    this.Control.Target.Invalidate();
                }

                return new MapViewingLogic(this.Control);
            }

            return this;
        }

        public override MapEditorUserControlLogic OnMapMouseMove(UserControlTargetEventArgs e)
        {
            if (_first != null)
            {
                if (_interactive_line != null)
                {
                    this.Control.ActiveLayer.RemoveElement(_interactive_line);
                }

                if (_line == null)
                {
                    _interactive_line = this.Control.ActiveLayer.AddLine(_first, e.Position,false);
                }
                else
                {
                    _interactive_line = this.Control.ActiveLayer.AddLine(_line, e.Position,false);
                }
            }

            this.Control.Target.Invalidate();

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
