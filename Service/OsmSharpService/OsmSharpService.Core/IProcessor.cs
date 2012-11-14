using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharpService.Core
{
    /// <summary>
    /// Interface implemented on a request processor.
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// Starts the request processor.
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// Returns true if the processor is ready.
        /// </summary>
        /// <returns></returns>
        bool IsReady();

        /// <summary>
        /// Stops the request processor.
        /// </summary>
        void Stop();
    }
}