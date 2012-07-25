using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Osm.Core;

namespace Osm.UI.WinForms.Test
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            OsmGeo.ShapeInterperter = new TestShapeInterpreter();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MapViewerForm());
            Application.Run(new MapEditorForm());
            //Application.Run(new BrowserEditorForm());
        }
    }
}
