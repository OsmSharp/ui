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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OsmSharp.Osm.UI.WinForms.MapViewerUserControl.Logic;
using OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget;
using OsmSharp.Osm.Renderer;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Osm.UI.WinForms.MapViewerUserControl
{
    /// <summary>
    /// Control used only for viewing the map.
    /// </summary>
    public partial class MapViewerUserControl : UserControl
    {
        /// <summary>
        /// Holds the active logic.
        /// </summary>
        private MapViewerUserControlLogic _logic;

        public MapViewerUserControl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);            

            // create the initial logic.
            _logic = new MapViewingLogic(this);
        }

        #region Properties

        /// <summary>
        /// Returns the current view.
        /// </summary>
        internal OsmSharp.Osm.Renderer.View View
        {
            get
            {
                return this.mapTarget.View;
            }
        }

        /// <summary>
        /// Returns the target.
        /// </summary>
        internal UserControlTarget Target
        {
            get
            {
                return this.mapTarget;
            }
        }

        /// <summary>
        /// Gets/Sets the map being displayed.
        /// </summary>
        public Map.Map Map
        {
            get
            {
                return this.Target.Map;
            }
            set
            {
                this.Target.Map = value;
            }
        }

        /// <summary>
        /// Gets/Sets the zoomfactor being displayed.
        /// </summary>
        public float ZoomFactor
        {
            get
            {
                return this.Target.ZoomFactor;
            }
            set
            {
                this.Target.ZoomFactor = value;
            }
        }

        /// <summary>
        /// Gets/Sets the center being displayed.
        /// </summary>
        public GeoCoordinate Center
        {
            get
            {
                return this.Target.Center;
            }
            set
            {
                this.Target.Center = value;
            }
        }

        #endregion

        #region Events -> Logics

        private void mapTarget_MapMouseDown(UserControlTargetEventArgs e)
        {
            _logic = _logic.OnMapMouseDown(e);
        }

        private void mapTarget_MapMouseMove(UserControlTargetEventArgs e)
        {
            _logic = _logic.OnMapMouseMove(e);
        }

        private void mapTarget_MapMouseUp(UserControlTargetEventArgs e)
        {
            _logic = _logic.OnMapMouseUp(e);
        }

        private void mapTarget_MapMouseWheel(UserControlTargetEventArgs e)
        {
            _logic = _logic.OnMapMouseWheel(e);
        }

        #endregion

    }
}
