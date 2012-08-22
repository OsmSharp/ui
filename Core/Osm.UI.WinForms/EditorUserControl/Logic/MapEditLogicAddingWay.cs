using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Renderer.Gdi.Targets.UserControlTarget;
using Osm.UI.WinForms.MapEditorUserControl;
using Osm.Map.Elements;
using System.Drawing;

namespace Osm.UI.WinForms.EditorUserControl.Logic
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
