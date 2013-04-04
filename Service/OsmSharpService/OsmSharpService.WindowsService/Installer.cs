using System.ComponentModel;


namespace OsmSharpService.WindowsService
{
    /// <summary>
    /// The installer for the OsmSharpService.
    /// </summary>
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        /// <summary>
        /// Creates the installer.
        /// </summary>
        public Installer()
        {
            InitializeComponent();
        }
    }
}
