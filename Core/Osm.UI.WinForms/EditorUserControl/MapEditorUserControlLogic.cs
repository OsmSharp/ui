using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Renderer.Gdi.Targets.UserControlTarget;

namespace Osm.UI.WinForms.MapEditorUserControl
{
    /// <summary>
    /// Base class for all logic that can be applied to the map view user control.
    /// </summary>
    internal abstract class MapEditorUserControlLogic
    {
        /// <summary>
        /// The map view user control.
        /// </summary>
        private MapEditorUserControl _ctrl;

        /// <summary>
        /// Creates a new map view control.
        /// </summary>
        protected MapEditorUserControlLogic(MapEditorUserControl ctrl)
        {
            _ctrl = ctrl;
        }

        /// <summary>
        /// Returns the control this logic is for.
        /// </summary>
        public MapEditorUserControl Control
        {
            get
            {
                return _ctrl;
            }
        }

        public abstract MapEditorUserControlLogic OnMapMouseDown(UserControlTargetEventArgs e);
        public abstract MapEditorUserControlLogic OnMapMouseMove(UserControlTargetEventArgs e);
        public abstract MapEditorUserControlLogic OnMapMouseUp(UserControlTargetEventArgs e);
        public abstract MapEditorUserControlLogic OnMapMouseWheel(UserControlTargetEventArgs e);
    }
}
