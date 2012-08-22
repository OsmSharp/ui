using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Osm.UI.WinForms.EditorUserControl.Controls
{
    public partial class LogControl : UserControl
    {
        public LogControl()
        {
            InitializeComponent();
        }

        #region OutputListener Members

        public void Write(string value)
        {
            this.richTextBox1.Text = value + this.richTextBox1.Text;
        }

        public void WriteLine(string value)
        {
            this.richTextBox1.Text = value + Environment.NewLine + this.richTextBox1.Text;
        }

        #endregion

    }
}
