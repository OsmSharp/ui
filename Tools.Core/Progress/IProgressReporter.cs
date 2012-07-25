using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Core.Progress
{
    /// <summary>
    /// Implemented on objects that can report progress to users or to another part of the application.
    /// </summary>
    public interface IProgressReporter
    {
        /// <summary>
        /// Reports a status to the progress reporter.
        /// </summary>
        /// <param name="status"></param>
        void Report(ProgressStatus status);
    }
}
