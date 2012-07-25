using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Osm.UI.WinForms.Layers
{
    public partial class LayerDetailUserControl : UserControl
    {
        public LayerDetailUserControl()
        {
            InitializeComponent();
        }

        private Osm.Map.Layers.ILayer _layer; 

        internal void SetLayer(Map.Layers.ILayer iLayer)
        {
            _layer = iLayer;

            this.lblLayerName.Text = iLayer.Name;
            this.chkVisible.Checked = iLayer.Visible;
        }

        private void chkVisible_CheckedChanged(object sender, EventArgs e)
        {
            _layer.Visible = this.chkVisible.Checked;
        }
    }
}
