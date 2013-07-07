// OsmSharp - OpenStreetMap tools & library.
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
using OsmSharp.Osm.Simple;
using System.Diagnostics;

namespace OsmSharp.Osm.Data.Streams.Filters
{
    /// <summary>
    /// A data source reporting progress.
    /// </summary>
    public class OsmStreamFilterProgress : OsmStreamSource
    {
        private readonly OsmStreamSource _reader;

        private long _start;
        private long _node;
        private long _way;
        private long _relation;

        /// <summary>
        /// Creates a new progress reporting source.
        /// </summary>
        /// <param name="reader"></param>
        public OsmStreamFilterProgress(OsmStreamSource reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public override void Initialize()
        {
            _reader.Initialize();

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
            return _reader.MoveNext();
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo Current()
        {
            OsmGeo current = _reader.Current();

            switch (current.Type)
            {
                case OsmGeoType.Node:
                    _node++;

                    if ((_node % 10000) == 0)
                    {
#if !WINDOWS_PHONE
                        Process p = Process.GetCurrentProcess();
                        long stop = DateTime.Now.Ticks;
                        float seconds = ((float)(stop - _start)) / (float)TimeSpan.TicksPerSecond;
                        if ((_node % 1000000) == 0) { GC.Collect(); }
                        OsmSharp.IO.Output.OutputStreamHost.WriteLine("Node[{0}]: {1}nodes/s @ {2}MB",
                            _node, (int)((double)_node / seconds), p.PrivateMemorySize64 / 1024 / 1024);
#endif
#if WINDOWS_PHONE
                        long stop = DateTime.Now.Ticks;
                        float seconds = ((float)(stop - _start)) / (float)TimeSpan.TicksPerSecond;
                        if ((_node % 1000000) == 0) { GC.Collect(); }
                        OsmSharp.IO.Output.OutputStreamHost.WriteLine("Node[{0}]: {1}nodes/s",
                            _node, (int)((double)_node / seconds));
#endif
                    }
                    break;
                case OsmGeoType.Relation:
                    _relation++;

                    if ((_relation % 1000) == 0)
                    {
#if !WINDOWS_PHONE
                        Process p = Process.GetCurrentProcess();
                        long stop = DateTime.Now.Ticks;
                        float seconds = ((float)(stop - _start)) / (float)TimeSpan.TicksPerSecond;
                        if ((_relation % 10000) == 0) { GC.Collect(); }
                        OsmSharp.IO.Output.OutputStreamHost.WriteLine("Relation[{0}]: {1}relations/s @ {2}MB",
                            _relation, (int)((double)_relation / seconds), p.PrivateMemorySize64 / 1024 / 1024);
#endif
#if WINDOWS_PHONE
                        long stop = DateTime.Now.Ticks;
                        float seconds = ((float)(stop - _start)) / (float)TimeSpan.TicksPerSecond;
                        if ((_relation % 10000) == 0) { GC.Collect(); }
                        OsmSharp.IO.Output.OutputStreamHost.WriteLine("Relation[{0}]: {1}relations/s",
                            _relation, (int)((double)_relation / seconds));
#endif
                    }
                    break;
                case OsmGeoType.Way:
                    _way++;

                    if ((_way % 10000) == 0)
                    {
#if !WINDOWS_PHONE
                        Process p = Process.GetCurrentProcess();
                        long stop = DateTime.Now.Ticks;
                        float seconds = ((float)(stop - _start)) / (float)TimeSpan.TicksPerSecond;
                        if ((_way % 100000) == 0) { GC.Collect(); }
                        OsmSharp.IO.Output.OutputStreamHost.WriteLine("Way[{0}]: {1}ways/s @ {2}MB",
                            _way, (int)((double)_way / seconds), p.PrivateMemorySize64 / 1024 / 1024);
#endif
#if WINDOWS_PHONE
                        long stop = DateTime.Now.Ticks;
                        float seconds = ((float)(stop - _start)) / (float)TimeSpan.TicksPerSecond;
                        if ((_way % 100000) == 0) { GC.Collect(); }
                        OsmSharp.IO.Output.OutputStreamHost.WriteLine("Way[{0}]: {1}ways/s",
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

            _reader.Reset();
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return _reader.CanReset; }
        }
    }
}