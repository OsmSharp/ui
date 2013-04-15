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

namespace OsmSharp.Osm.UI.WinForms.BrowserEditorUserControl
{
    public partial class BrowserEditorUserControl : UserControl
    {
        public BrowserEditorUserControl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //this.webBrowser1.Url = new Uri(@"http://s-dbp-001:3000/");
        }

        public Uri Url 
        {
            get
            {
                return this.webBrowser1.Url;
            }
            set
            {
                this.webBrowser1.Url = value;
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                HtmlElement element = webBrowser1.Document.GetElementById("left");
                if (element != null)
                {
                    element.Style = "display:none";
                }
                element = webBrowser1.Document.GetElementById("greeting");
                if (element != null)
                {
                    element.Style = "display:none";
                }
                element = webBrowser1.Document.GetElementById("tabnav");
                if (element != null)
                {
                    element.Style = "display:none";
                }
                element = webBrowser1.Document.GetElementById("content");
                if (element != null)
                {
                    element.Style = "postition:absolute;top:0px;left:0px;height:100%;width:100%";
                }
            }
            catch(Exception)
            {

            }
        }
    }
}
