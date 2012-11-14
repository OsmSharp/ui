using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


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
