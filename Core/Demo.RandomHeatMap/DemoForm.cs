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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Osm.Map.Layers.Tiles;
using Tools.Math.Geo;
using Osm.Map.Layers.Custom;
using Osm.Renderer.Gdi.Targets.UserControlTarget;
using Osm.Core;
using Tools.Math.Units.Distance;
using System.Threading;

namespace Demo.RandomHeatMap
{
    public partial class DemoForm : Form
    {
        
        public DemoForm()
        {
            InitializeComponent();

            this.mapEditorUserControl.SelectionMode = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // create the map and all it's layers.
            Osm.Map.Map map = new Osm.Map.Map();
            map.Layers.Add(new TilesLayer());
            
            // center and zoom!
            double latitude_center = 51.04312f;
            double longitude_center = 3.71939f;
            this.mapEditorUserControl.Map = map;
            this.mapEditorUserControl.Center = new GeoCoordinate(latitude_center, longitude_center);
            this.mapEditorUserControl.ZoomFactor = 16;
            

            this.mapEditorUserControl.mapTarget.AddCustomLayer(new DemoHeatLayer());
        }
    }
}