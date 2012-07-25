using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Osm.Core;

namespace Demo.Routing
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            OsmGeo.ShapeInterperter = new SimpleShapeInterpreter();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DemoForm());
        }
    }
}
