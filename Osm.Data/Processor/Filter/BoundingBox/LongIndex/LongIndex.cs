using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Data.Core.Processor.Filter.LongIndex
{
    public class LongIndex
    {
        private LongIndexNode _root;

        public LongIndex()
        {
            _root = new LongIndexNode(1);
        }

        public void Add(long number)
        {
            while (number >= LongIndexNode.CalculateBaseNumber((short)(_root.Base + 1)))
            {
                LongIndexNode old_root = _root;
                _root = new LongIndexNode((short)(_root.Base + 1));
                _root.has_0 = old_root;
            }

            _root.Add(number);
        }

        public void Remove(long number)
        {
            if (number < LongIndexNode.CalculateBaseNumber((short)(_root.Base + 1)))
            {
                _root.Remove(number);
            }
        }

        public bool Contains(long number)
        {
            if (number < LongIndexNode.CalculateBaseNumber((short)(_root.Base + 1)))
            {
                return _root.Contains(number);
            }
            return false;
        }

        public void Clear()
        {
            _root = new LongIndexNode(1);
        }    
    }
}
