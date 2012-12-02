using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Core.Simple;

namespace OsmSharp.Osm.Data.Core.Processor.Progress
{
    public class ProgressDataProcessorSource : DataProcessorSource
    {
        private DataProcessorSource _source;

        private long _start;
        private long _node;
        private long _way;
        private long _relation;

        public ProgressDataProcessorSource(DataProcessorSource source)
        {
            _source = source;
        }

        public override void Initialize()
        {
            _source.Initialize();

            _start = DateTime.Now.Ticks;
            _node = 0;
            _way = 0;
            _relation = 0;
        }

        public override bool MoveNext()
        {
            return _source.MoveNext();
        }

        public override SimpleOsmGeo Current()
        {
            SimpleOsmGeo current = _source.Current();

            switch (current.Type)
            {
                case SimpleOsmGeoType.Node:
                    _node++;

                    if ((_node % 10000) == 0)
                    {
                        long stop = DateTime.Now.Ticks;
                        long seconds = (stop - _start) / TimeSpan.TicksPerSecond;
                        OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Node[{0}]: {1}nodes/s", _node, (int)((double)_node / (double)seconds));
                    }
                    break;
                case SimpleOsmGeoType.Relation:
                    _relation++;

                    if ((_relation % 1000) == 0)
                    {
                        long stop = DateTime.Now.Ticks;
                        long seconds = (stop - _start) / TimeSpan.TicksPerSecond;
                        OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Relation[{0}]: {1}relations/s", _relation, (int)((double)_relation / (double)seconds));
                    }
                    break;
                case SimpleOsmGeoType.Way:
                    _way++;

                    if ((_way % 1000) == 0)
                    {
                        long stop = DateTime.Now.Ticks;
                        long seconds = (stop - _start) / TimeSpan.TicksPerSecond;
                        OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Way[{0}]: {1}ways/s", _way, (int)((double)_way / (double)seconds));
                    }
                    break;
            }

            return current;
        }

        public override void Reset()
        {
            _source.Reset();
        }

        public override bool CanReset
        {
            get { return _source.CanReset; }
        }
    }
}
