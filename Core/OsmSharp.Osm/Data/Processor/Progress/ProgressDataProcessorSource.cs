using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Simple;
using System.Diagnostics;

namespace OsmSharp.Osm.Data.Core.Processor.Progress
{
    /// <summary>
    /// A data source reporting progress.
    /// </summary>
    public class ProgressDataProcessorSource : DataProcessorSource
    {
        private DataProcessorSource _source;

        private long _start;
        private long _node;
        private long _way;
        private long _relation;

        /// <summary>
        /// Creates a new progress reporting source.
        /// </summary>
        /// <param name="source"></param>
        public ProgressDataProcessorSource(DataProcessorSource source)
        {
            _source = source;
        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public override void Initialize()
        {
            _source.Initialize();

            _start = DateTime.Now.Ticks;
            _node = 0;
            _way = 0;
            _relation = 0;
        }

        /// <summary>
        /// Move to the next object.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            return _source.MoveNext();
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override SimpleOsmGeo Current()
        {
            SimpleOsmGeo current = _source.Current();

            switch (current.Type)
            {
                case SimpleOsmGeoType.Node:
                    _node++;

                    if ((_node % 10000) == 0)
                    {
#if !WINDOWS_PHONE
                        Process p = Process.GetCurrentProcess();
                        long stop = DateTime.Now.Ticks;
                        float seconds = ((float)(stop - _start)) / (float)TimeSpan.TicksPerSecond;
                        if ((_node % 1000000) == 0) { GC.Collect(); }
                        OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Node[{0}]: {1}nodes/s @ {2}MB",
                            _node, (int)((double)_node / seconds), p.PrivateMemorySize64 / 1024 / 1024);
#endif
#if WINDOWS_PHONE
                        long stop = DateTime.Now.Ticks;
                        float seconds = ((float)(stop - _start)) / (float)TimeSpan.TicksPerSecond;
                        if ((_node % 1000000) == 0) { GC.Collect(); }
                        OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Node[{0}]: {1}nodes/s",
                            _node, (int)((double)_node / seconds));
#endif
                    }
                    break;
                case SimpleOsmGeoType.Relation:
                    _relation++;

                    if ((_relation % 1000) == 0)
                    {
#if !WINDOWS_PHONE
                        Process p = Process.GetCurrentProcess();
                        long stop = DateTime.Now.Ticks;
                        float seconds = ((float)(stop - _start)) / (float)TimeSpan.TicksPerSecond;
                        if ((_relation % 10000) == 0) { GC.Collect(); }
                        OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Relation[{0}]: {1}relations/s @ {2}MB",
                            _relation, (int)((double)_relation / seconds), p.PrivateMemorySize64 / 1024 / 1024);
#endif
#if WINDOWS_PHONE
                        long stop = DateTime.Now.Ticks;
                        float seconds = ((float)(stop - _start)) / (float)TimeSpan.TicksPerSecond;
                        if ((_relation % 10000) == 0) { GC.Collect(); }
                        OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Relation[{0}]: {1}relations/s",
                            _relation, (int)((double)_relation / seconds));
#endif
                    }
                    break;
                case SimpleOsmGeoType.Way:
                    _way++;

                    if ((_way % 10000) == 0)
                    {
#if !WINDOWS_PHONE
                        Process p = Process.GetCurrentProcess();
                        long stop = DateTime.Now.Ticks;
                        float seconds = ((float)(stop - _start)) / (float)TimeSpan.TicksPerSecond;
                        if ((_way % 100000) == 0) { GC.Collect(); }
                        OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Way[{0}]: {1}ways/s @ {2}MB",
                            _way, (int)((double)_way / seconds), p.PrivateMemorySize64 / 1024 / 1024);
#endif
#if WINDOWS_PHONE
                        long stop = DateTime.Now.Ticks;
                        float seconds = ((float)(stop - _start)) / (float)TimeSpan.TicksPerSecond;
                        if ((_way % 100000) == 0) { GC.Collect(); }
                        OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Way[{0}]: {1}ways/s",
                            _way, (int)((double)_way / seconds));
#endif
                    }
                    break;
            }

            return current;
        }

        /// <summary>
        /// Resets this source.
        /// </summary>
        public override void Reset()
        {
            _start = DateTime.Now.Ticks;
            _node = 0;
            _way = 0;
            _relation = 0;

            _source.Reset();
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return _source.CanReset; }
        }
    }
}
