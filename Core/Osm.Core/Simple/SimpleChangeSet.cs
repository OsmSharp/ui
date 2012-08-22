using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core.Simple
{
    public class SimpleChangeSet
    {
        public SimpleChangeSet()
        {

        }

        public List<SimpleChange> Changes { get; set; }
        
    }
}
