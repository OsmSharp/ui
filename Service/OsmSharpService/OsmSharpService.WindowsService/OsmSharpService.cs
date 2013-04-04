using System.Collections.Generic;
using System.ServiceProcess;
using OsmSharpService.Core;
using System.Configuration;

namespace OsmSharpService.WindowsService
{
    /// <summary>
    /// The OsmSharp service host.
    /// </summary>
    public partial class OsmSharpService : ServiceBase
    {
        /// <summary>
        /// Holds the list of processes.
        /// </summary>
        private readonly List<IProcessor> _processors;

        /// <summary>
        /// Holds the services host.
        /// </summary>
        private AppHost _host;

        /// <summary>
        /// Creates the service.
        /// </summary>
        public OsmSharpService()
        {
            InitializeComponent();

            _processors = new List<IProcessor>();
        }

        /// <summary>
        /// Called when the service needs to start.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            // initializes the processor(s).
            _processors.Add(OperationProcessor.GetInstance());

            // start all the processor(s).
            foreach (IProcessor processor in _processors)
            {
                processor.Start();
            }

            // start the self-hosting.
            _host = new AppHost();
            _host.Init();
            _host.Start(ConfigurationManager.AppSettings["hostname"]);
        }

        /// <summary>
        /// Called when the services needs to stop.
        /// </summary>
        protected override void OnStop()
        {
            // stops the http listener.
            _host.Stop();

            // stop all the processor(s).
            foreach (IProcessor processor in _processors)
            {
                processor.Stop();
            }
        }
    }
}
