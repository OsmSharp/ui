using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Renderer.Gdi.Targets.UserControlTarget;

namespace Osm.UI.WinForms.MapViewerUserControl
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
