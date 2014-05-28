using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OsmSharp.WinForms.UI.Sample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Native.Initialize();

            OsmSharp.Logging.Log.Enable();
            OsmSharp.Logging.Log.RegisterListener(
                new OsmSharp.WinForms.UI.Logging.ConsoleTraceListener());

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            ////Application.Run(new SampleControlForm());
            Application.Run(new MapControlForm());
        }
    }
}