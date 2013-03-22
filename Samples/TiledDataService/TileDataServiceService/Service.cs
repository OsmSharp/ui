using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using TiledDataService;

namespace TileDataServiceService
{
    /// <summary>
    /// Windows service serving tiled data.
    /// </summary>
    public partial class TileDataServiceService : ServiceBase
    {
        /// <summary>
        /// Holds the application host.
        /// </summary>
        private AppHost _host;

        /// <summary>
        /// Creates the service.
        /// </summary>
        public TileDataServiceService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the service starts.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            // get the hostname.
            string hostname = ConfigurationManager.AppSettings["hostname"];

            // start the self-hosting.
            _host = new AppHost();
            _host.Init();
            _host.Start(hostname);

        }

        /// <summary>
        /// Called when the service stops.
        /// </summary>
        protected override void OnStop()
        {
            _host.Stop();
        }
    }
}
