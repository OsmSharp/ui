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
using OsmSharp.Osm.Map.Layers.Tiles;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Map.Layers.Custom;
using OsmSharp.Tools.GeoCoding;

namespace OsmSharp.Osm.UI.WinForms.Test
{
    public partial class MapEditorForm : Form
    {
        public MapEditorForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            OsmSharp.Osm.Map.Map map = new OsmSharp.Osm.Map.Map();
            map.Layers.Add(new TilesLayer());


            // do some small geocoding test here! 
            IGeoCoderResult result = Facade.Code("Tools.GeoCoding.Nomatim", "Belgium", "2275", "Wechelderzande", "Zand", "25");


            double latitude_center = 51.024154f;
            double longitude_center = 4.003801f;

            this.mapEditorUserControl1.Map = map;
            this.mapEditorUserControl1.Center = new GeoCoordinate(latitude_center, longitude_center);
            this.mapEditorUserControl1.ZoomFactor = 16;

            CustomLayer custom_layer =
                new CustomLayer();

            map.Layers.Add(custom_layer);

            this.mapEditorUserControl1.ActiveLayer = custom_layer;
        }

        private void fromCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void mapEditorUserControl1_MapClick(Renderer.Gdi.Targets.UserControlTarget.UserControlTargetEventArgs e)
        {
            Console.WriteLine(e.Position);
        }
    }
}
