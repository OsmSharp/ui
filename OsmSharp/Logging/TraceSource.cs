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
#if __ANDROID__
	/// <summary>
    /// Another class for compatibility with windows phone.
    /// </summary>
    public class TraceSource
    {
		/// <summary>
		/// Holds the tag.
		/// </summary>
		private string _tag;

		/// <summary>
		/// Initializes a new instance of the <see cref="System.Diagnostics.TraceSource"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
        public TraceSource(string name)
		{
			_tag = name;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="System.Diagnostics.TraceSource"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="level">Level.</param>
        public TraceSource(string name, SourceLevels level)
		{
			_tag = name;
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
				Android.Util.Log.Error (_tag, string.Format ("[{0}:{2}@{3}]:{1}", id, message, type.ToString(),
				                                             DateTime.Now.Ticks));
				break;
			case TraceEventType.Warning:
				Android.Util.Log.Warn (_tag, string.Format ("[{0}:{2}@{3}]:{1}", id, message, type.ToString(),
				                                            DateTime.Now.Ticks));
				break;
			default:
				Android.Util.Log.Info (_tag, string.Format ("[{0}:{2}@{3}]:{1}", id, message, type.ToString(),
				                                            DateTime.Now.Ticks));
				break;
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

        public List<TraceListener> Listeners { get; set; }
    }

#endif
#if IOS
	/// <summary>
	/// Another class for compatibility with windows phone.
	/// </summary>
	public class TraceSource
	{
		/// <summary>
		/// Holds the tag.
		/// </summary>
		private string _tag;

		/// <summary>
		/// Initializes a new instance of the <see cref="System.Diagnostics.TraceSource"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		public TraceSource(string name)
		{
			_tag = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="System.Diagnostics.TraceSource"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="level">Level.</param>
		public TraceSource(string name, SourceLevels level)
		{
			_tag = name;
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
				Console.WriteLine (string.Format ("{4}[{0}:{2}@{3}]:{1}", id, message, type.ToString (),
				                                 DateTime.Now.Ticks, _tag));
				break;
			case TraceEventType.Warning:
				Console.WriteLine (string.Format ("{4}[{0}:{2}@{3}]:{1}", id, message, type.ToString (),
				                                 DateTime.Now.Ticks, _tag));
				break;
			default:
				Console.WriteLine (string.Format ("{4}[{0}:{2}@{3}]:{1}", id, message, type.ToString (),
				                                  DateTime.Now.Ticks, _tag));
				break;
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
		/// Gets or sets the listeners.
		/// </summary>
		/// <value>The listeners.</value>
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
