using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Data.Core.Processor.Filter.LongIndex
{
    interface ILongIndexNode
    {
        bool Contains(long number);

        void Add(long number);

        void Remove(long number);

        short Base
        {
            get;
        }
    }
}
