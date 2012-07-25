using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core.Simple;

namespace Osm.Data.Core.Processor.ChangeSets
{
    public abstract class DataProcessorChangeSetFilter : DataProcessorChangeSetSource
    {
        private DataProcessorChangeSetSource _source;

        public DataProcessorChangeSetFilter()
        {

        }

        protected DataProcessorChangeSetSource Source
        {
            get
            {
                return _source;
            }
        }

        public abstract override void Initialize();

        public abstract override bool MoveNext();

        public abstract override SimpleChangeSet Current();

        public void RegisterSource(DataProcessorChangeSetSource source)
        {
            _source = source;
        }
    }
}
