using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.UI.WinForms.MapEditorUserControl;
using Tools.Math.Geo;

namespace Osm.UI.WinForms.EditorUserControl.Logic
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
