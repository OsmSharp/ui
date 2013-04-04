using System.ServiceProcess;

namespace OsmSharpService.WindowsService
{
    partial class OsmSharpServiceDebugger : ServiceBase
    {
        public OsmSharpServiceDebugger()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
