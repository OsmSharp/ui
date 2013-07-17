using System;
using System.Net;

namespace System.Diagnostics
{
#if WINDOWS_PHONE || __ANDROID__
    /// <summary>
    /// Another class for compatibility with windows phone.
    /// </summary>
    public class TraceSource
    {
        public TraceSource(string name)
        {

        }

        /// <summary>
        /// Traces an event.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="message"></param>
        internal void TraceEvent(TraceEventType type, int id, string message)
        {

        }

        /// <summary>
        /// Traces an event.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        internal void TraceEvent(TraceEventType type, int id, string message, object[] args)
        {

        }
    }

    /// <summary>
    /// Another enum for compatibility with windows phone.
    /// </summary>
    public enum TraceEventType
    {
        Critical = 1,
        Error = 2,
        Warning = 4,
        Information = 8,
        Verbose = 16,
        Start = 256,
        Stop = 512,
        Suspend = 1024,
        Resume = 2048,
        Transfer = 4096
    }
#endif
}
