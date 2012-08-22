using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Core.Progress
{
    /// <summary>
    /// Interface for progress enabled objects.
    /// </summary>
    public interface IProgressEnabled
    {
        /// <summary>
        /// Registers a progress reporter to recieve progress.
        /// </summary>
        /// <param name="reporter"></param>
        void RegisterProgressReporter(IProgressReporter reporter);

        /// <summary>
        /// Unregisters a progress reporter.
        /// </summary>
        /// <param name="reporter"></param>
        void UnRegisterProgressReporter(IProgressReporter reporter);
    }
}
