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
