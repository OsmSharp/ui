// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
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

namespace Osm.UI.WinForms.Test
{
    public partial class MapViewerForm : Form
    {
        public MapViewerForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Osm.Map.Map map = new Osm.Map.Map();
            map.Layers.Add(new TilesLayer());

            double latitude_center = 51.024154f;
            double longitude_center = 4.003801f;

            this.mapViewerUserControl1.Map = map;
            this.mapViewerUserControl1.Center = new GeoCoordinate(latitude_center, longitude_center);
            this.mapViewerUserControl1.ZoomFactor = 14.5f;
        }
    }
}
