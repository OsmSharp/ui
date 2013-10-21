// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System.Diagnostics;

namespace OsmSharp.Logging
{
    /// <summary>
    /// Logging class.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Holds the tracesource.
        /// </summary>
        private static TraceSource _source = new TraceSource("OsmSharp", SourceLevels.All);

        /// <summary>
        /// Holds the logging enabled flag.
        /// </summary>
        private static bool _loggingEnabled = false;

        /// <summary>
        /// Disables all logging.
        /// </summary>
        public static void Disable()
        {
            _loggingEnabled = false;
        }

        /// <summary>
        /// Enables all logging.
        /// </summary>
        public static void Enable()
        {
            _loggingEnabled = true;
        }

        /// <summary>
        /// Writes a trace event message.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public static void TraceEvent(string name, TraceEventType type, string message)
        {
            if (_loggingEnabled)
            {
                _source.TraceEvent(type, 0, message);
            }
        }

        /// <summary>
        /// Writes a trace event message.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void TraceEvent(string name, TraceEventType type, string message, params object[] args)
        {
            if (_loggingEnabled)
            {
                _source.TraceEvent(type, 0, message, args);
            }
        }

        /// <summary>
        /// Registers a trace listener.
        /// </summary>
        /// <param name="listener"></param>
        public static void RegisterListener(TraceListener listener)
        {
            if (_loggingEnabled)
            {
                _source.Listeners.Add(listener);
            }
        }

#if !(WINDOWS_PHONE || __ANDROID__ || IOS)
        /// <summary>
        /// Registers a console trace listener.
        /// </summary>
        public static void RegisterConsoleListener()
        {
            ConsoleTraceListener console =
                new ConsoleTraceListener(false);
            console.Filter =
                new EventTypeFilter(SourceLevels.All);
            console.Name = "console";
            _source.Listeners.Add(console);
        }
#endif
    }
}
