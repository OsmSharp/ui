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
