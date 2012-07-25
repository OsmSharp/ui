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
using Tools.GeoCoding;

namespace Osm.UI.WinForms.Test
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

            Osm.Map.Map map = new Osm.Map.Map();
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
