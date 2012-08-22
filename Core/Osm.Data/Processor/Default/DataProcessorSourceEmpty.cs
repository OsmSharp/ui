using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core.Simple;

namespace Osm.Data.Core.Processor.Default
{
    public class DataProcessorSourceEmpty : DataProcessorSource
    {
        public override void Initialize()
        {

        }

        public override bool MoveNext()
        {
            return false;
        }

        public override SimpleOsmGeo Current()
        {
            return null;
        }

        public override void Reset()
        {

        }
    }
}
