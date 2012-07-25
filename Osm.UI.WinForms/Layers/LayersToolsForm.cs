using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Osm.UI.WinForms.Layers
{
    public partial class LayersToolsForm : Form
    {
        public LayersToolsForm()
        {
            InitializeComponent();
        }

        public void SetMap(Osm.Map.Map map)
        {
            this.layerUserControl1.SetMap(map);
        }
    }
}
