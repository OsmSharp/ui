using System;
using System.Net;
using System.Collections.Generic;

namespace System.Diagnostics
{
#if WINDOWS_PHONE
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
#if __ANDROID__ || IOS
	/// <summary>
    /// Compatibility class with .NET to use the tracing facilities. 
    /// </summary>
    public class TraceSource
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="System.Diagnostics.TraceSource"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
        public TraceSource(string name)
		{
			//_tag = name;
            this.Listeners = new List<TraceListener>();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="System.Diagnostics.TraceSource"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="level">Level.</param>
        public TraceSource(string name, SourceLevels level)
		{
            //_tag = name;
            this.Listeners = new List<TraceListener>();
        }

        /// <summary>
        /// Traces an event.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="message"></param>
        internal void TraceEvent(TraceEventType type, int id, string message)
        {
			switch (type) {
			case TraceEventType.Critical:
			case TraceEventType.Error:
                //Android.Util.Log.Error (_tag, string.Format ("[{0}:{2}@{3}]:{1}", id, message, type.ToString(),
                //                                             DateTime.Now.Ticks));
                this.WriteLine(message);
                //this.WriteLine(string.Format("[{0}:{2}@{3}]:{1}", id, message, type.ToString(),
                //                                             DateTime.Now.Ticks));
				break;
			case TraceEventType.Warning:
                //Android.Util.Log.Warn (_tag, string.Format ("[{0}:{2}@{3}]:{1}", id, message, type.ToString(),
                //                                            DateTime.Now.Ticks));
                this.WriteLine(message);
                //this.WriteLine(string.Format("[{0}:{2}@{3}]:{1}", id, message, type.ToString(),
                //                                            DateTime.Now.Ticks));
				break;
			default:
                //Android.Util.Log.Info (_tag, string.Format ("[{0}:{2}@{3}]:{1}", id, message, type.ToString(),
                //                                            DateTime.Now.Ticks));
                this.WriteLine(message);
                //this.WriteLine(string.Format("[{0}:{2}@{3}]:{1}", id, message, type.ToString(),
                //                                            DateTime.Now.Ticks));
				break;
			}
        }

        /// <summary>
        /// Writes the message to all listeners.
        /// </summary>
        /// <param name="message"></param>
        private void WriteLine(string message)
        {
            foreach (TraceListener listener in this.Listeners)
            {
                listener.WriteLine(message);
            }
        }

        /// <summary>
        /// Traces an event.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
		/// <param name="messageWithParams"></param>
        /// <param name="args"></param>
        internal void TraceEvent(TraceEventType type, int id, string messageWithParams, object[] args)
        {
			string message = string.Format (messageWithParams, args);
			this.TraceEvent (type, id, message);
        }

        /// <summary>
        /// The list of listeners.
        /// </summary>
        public List<TraceListener> Listeners { get; private set; }
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
