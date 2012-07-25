using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core.Simple;

namespace Osm.Data.Core.Processor
{
    public abstract class DataProcessorFilter : DataProcessorSource
    {
        private DataProcessorSource _source;

        public DataProcessorFilter()
        {

        }

        public void RegisterSource(DataProcessorSource source)
        {
            _source = source;
        }

        protected DataProcessorSource Source
        {
            get
            {
                return _source;
            }
        }

        public abstract override void Initialize();

        public abstract override bool MoveNext();

        public abstract override SimpleOsmGeo Current();
    }
}
