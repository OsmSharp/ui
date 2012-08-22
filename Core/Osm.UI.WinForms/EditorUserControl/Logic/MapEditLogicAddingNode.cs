using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Renderer.Gdi.Targets.UserControlTarget;
using Osm.UI.WinForms.MapEditorUserControl;

namespace Osm.UI.WinForms.EditorUserControl.Logic
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
