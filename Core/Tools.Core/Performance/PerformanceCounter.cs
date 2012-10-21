// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Core.Output;
using System.Diagnostics;

namespace Tools.Core.Performance
{
    public static class PerformanceCounter
    {
        public static void RegisterCounter(int id, string name)
        {
            if (!_counters.ContainsKey(id))
            {
                _counters.Add(id, name);
            }
        }

        private static Dictionary<int, long> _current_counters = new Dictionary<int, long>();

        private static Dictionary<int, long> _counts = new Dictionary<int, long>();

        private static Dictionary<int, string> _counters = new Dictionary<int, string>();

        public static void Run(int id)
        {
            if (!_current_counters.ContainsKey(id))
            {
                 _current_counters.Add(id, System.Diagnostics.Stopwatch.GetTimestamp());
            }
        }

        public static void Pause(int id)
        {
            if (_current_counters.ContainsKey(id))
            {
                long ticks =
                    System.Diagnostics.Stopwatch.GetTimestamp() - _current_counters[id];
                long value;
                if (!_counts.TryGetValue(id, out value))
                {
                    _counts.Add(id, ticks);
                }
                else
                {
                    _counts[id] = value + ticks;
                }
                _current_counters.Remove(id);
            }
        }

        public static void ReportCounters()
        {
            foreach (KeyValuePair<int, long> count in _counts)
            {
                TimeSpan span = new TimeSpan(count.Value);
                string name;
                if (_counters.TryGetValue(count.Key, out name))
                {
                    OutputTextStreamHost.WriteLine("{0}[{1}]:{2}ms", name, count.Key, span.TotalMilliseconds);
                }
                else
                {
                    OutputTextStreamHost.WriteLine("(no name)[{0}]:{1}ms", count.Key, span.TotalMilliseconds);
                }
            }
        }
    }
}
