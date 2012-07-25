using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Osm.UI.WinForms.Layers;

namespace Osm.UI.WinForms.Layer
{
    public partial class LayerUserControl : UserControl
    {
        public LayerUserControl()
        {
            InitializeComponent();
        }

        private Osm.Map.Map _map;

        public void SetMap(Osm.Map.Map map)
        {
            _map = map;

            this.tableLayoutPanel1.Controls.Clear();
            this.tableLayoutPanel1.RowCount = _map.Layers.Count;

            for (int layer_idx = 0; layer_idx < _map.Layers.Count; layer_idx++)
            {
                LayerDetailUserControl ctrl = new LayerDetailUserControl();
                ctrl.SetLayer(_map.Layers[layer_idx]);

                this.tableLayoutPanel1.Controls.Add(ctrl);
            }
        }
    }
}
