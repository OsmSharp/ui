using System.Diagnostics;

namespace OsmSharp.Wpf.UI.Log
{
    /// <summary>
    /// A log trace listener that writes message to the debug.
    /// </summary>
    public class DebugTraceListener : Logging.TraceListener
    {
        /// <summary>
        /// Writes the given message to the debug.
        /// </summary>
        public override void Write(string message)
        {
            Debug.Write(message);
        }

        /// <summary>
        /// Writes the given message to the debug.
        /// </summary>
        public override void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }
    }
}