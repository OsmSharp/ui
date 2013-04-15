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

namespace OsmSharp.Osm.UI.WinForms.EditorUserControl.Controls
{
    public partial class LogControl : UserControl, Tools.Output.IOutputStream
    {
        public LogControl()
        {
            InitializeComponent();
        }

        private delegate void StringDelegate(string value);

        #region OutputListener Members

        public void Write(string value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new StringDelegate(Write), value);
                return;
            }

            this.richTextBox1.Text = value + this.richTextBox1.Text;
        }
        private string _value = string.Empty;

        public void WriteLine(string value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new StringDelegate(WriteLine), value);
                return;
            }

            if(_value != value)
            {
                this.richTextBox1.Text = value + Environment.NewLine + this.richTextBox1.Text;
                _value = value;
            }
        }

        private delegate void ReportProgressDelegate(double progress, string key, string message);

        public void ReportProgress(double progress, string key, string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ReportProgressDelegate(ReportProgress), progress, key, message);
                return;
            }

            this.WriteLine(string.Format("{0}: {1}%",
                message, System.Math.Round(progress * 100, 0)));
        }

        #endregion
    }
}
