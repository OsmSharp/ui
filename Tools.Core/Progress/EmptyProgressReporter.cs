using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Core.Progress
{
    /// <summary>
    /// An empty progress reporter to use when not progress needs to be reported.
    /// </summary>
    public class EmptyProgressReporter : IProgressReporter
    {
        #region Singleton

        private static EmptyProgressReporter _instance;

        /// <summary>
        /// Returns an instance 
        /// </summary>
        public static EmptyProgressReporter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EmptyProgressReporter();
                }
                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Constructs a new empty progress reporter.
        /// </summary>
        private EmptyProgressReporter() { }


        #region IProgressReporter Members

        /// <summary>
        /// Reports progress to this reporter.
        /// </summary>
        /// <param name="status"></param>
        public void Report(ProgressStatus status)
        {

        }

        #endregion
    }
}
