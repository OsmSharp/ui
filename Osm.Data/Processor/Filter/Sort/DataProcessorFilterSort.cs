using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core.Simple;

namespace Osm.Data.Core.Processor.Filter.Sort
{
    public class DataProcessorFilterSort : DataProcessorFilter
    {
        private SimpleOsmGeoType _current_type = SimpleOsmGeoType.Node;

        public DataProcessorFilterSort()
            :base()
        {

        }

        public override void Initialize()
        {
            // no intialisation this filter does the same thing every time.
            this.Source.Initialize();
        }

        public override bool MoveNext()
        {
            if (this.Source.MoveNext())
            {
                bool finished = false;
                while (this.Current().Type != _current_type)
                {
                    if (!this.Source.MoveNext())
                    {
                        finished = true;
                        break;
                    }
                }

                if (!finished && this.Current().Type == _current_type)
                {
                    return true;
                }
            }

            switch (_current_type)
            {
                case SimpleOsmGeoType.Node:
                    this.Source.Reset();
                    _current_type = SimpleOsmGeoType.Way;
                    return this.MoveNext();
                case SimpleOsmGeoType.Way:
                    this.Source.Reset();
                    _current_type = SimpleOsmGeoType.Relation;
                    return this.MoveNext();
                case SimpleOsmGeoType.Relation:
                    return false;
            }
            throw new InvalidOperationException("Unkown SimpleOsmGeoType");
            
        }

        public override SimpleOsmGeo Current()
        {
            return this.Source.Current();
        }

        public override void Reset()
        {
            _current_type = SimpleOsmGeoType.Node;
            this.Source.Reset();
        }
    }
}
