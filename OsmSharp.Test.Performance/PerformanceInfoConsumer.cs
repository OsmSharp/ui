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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OsmSharp.Test.Performance
{
    /// <summary>
    /// A class that consumes perfomance information.
    /// </summary>
    public class PerformanceInfoConsumer
    {
        /// <summary>
        /// Holds the name of this consumer.
        /// </summary>
        private string _name;

        /// <summary>
        /// Creates the a new performance info consumer.
        /// </summary>
        /// <param name="name"></param>
        public PerformanceInfoConsumer(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Creates a new performance consumer.
        /// </summary>
        /// <param name="key"></param>
        public static PerformanceInfoConsumer Create(string key)
        {
            return new PerformanceInfoConsumer(key);
        }

        /// <summary>
        /// Holds the ticks when started.
        /// </summary>
        private long? _ticks;

        /// <summary>
        /// Holds the amount of memory before start.
        /// </summary>
        private long? _memory;

        /// <summary>
        /// Reports the start of the process/time period to measure.
        /// </summary>
        public void Start()
        {
            GC.Collect();

            Process p = Process.GetCurrentProcess();
            _memory = p.PrivateMemorySize64;
            _ticks = DateTime.Now.Ticks;
            //OsmSharp.Logging.Log.TraceEvent("PF:" + _name, System.Diagnostics.TraceEventType.Information,
            //    string.Format("Started at {0}.", new DateTime(_ticks.Value).ToShortTimeString()));
        }

        /// <summary>
        /// Reports a message in the middle of progress.
        /// </summary>
        /// <param name="message"></param>
        public void Report(string message)
        {
            OsmSharp.Logging.Log.TraceEvent("PF:" + _name, System.Diagnostics.TraceEventType.Information,
                message);
        }

        /// <summary>
        /// Reports a message in the middle of progress.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Report(string message, params object[] args)
        {
            OsmSharp.Logging.Log.TraceEvent("PF:" + _name, System.Diagnostics.TraceEventType.Information,
                message, args);
        }

        /// <summary>
        /// Reports the end of the process/time period to measure.
        /// </summary>
        public void Stop()
        {
            if (_ticks.HasValue)
            {
                double seconds = new TimeSpan(DateTime.Now.Ticks - _ticks.Value).TotalMilliseconds / 1000.0;

                GC.Collect();
                Process p = Process.GetCurrentProcess();
                double memoryDiff = System.Math.Round((p.PrivateMemorySize64 - _memory.Value) / 1024.0 / 1024.0, 4);

                OsmSharp.Logging.Log.TraceEvent("PF:" + _name, System.Diagnostics.TraceEventType.Information,
                    string.Format("Ended at at {0}, spent {1}s and {2}MB of memory diff.",
                        new DateTime(_ticks.Value).ToShortTimeString(),
                        seconds, memoryDiff));
            }
        }
    }
}
