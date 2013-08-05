using System;
using System.Net;
using System.Collections.Generic;

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

        public TraceSource(string name, SourceLevels level)
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

        public List<TraceListener> Listeners { get; set; }
    }

#endif
#if WINDOWS_PHONE

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

    /// <summary>
    /// A trace listener.
    /// </summary>
    public class TraceListener
    {

    }

    /// <summary>
    /// A source levels enum.
    /// </summary>
    public enum SourceLevels
    {
        All
    }
#endif
}
