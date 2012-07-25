using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Core.Collections
{
    public class DictionaryWithEvents<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public event AddEventHandler AddEvent;

        public void Add(TKey pKey, TValue pValue)
        {
            if (AddEvent != null)
                AddEvent(new AddEventArgs(pKey, pValue));
            base.Add(pKey, pValue);
        }

        public delegate void AddEventHandler(AddEventArgs pAddEventArgs);

        public class AddEventArgs : EventArgs
        {
            private TKey _key;
            private TValue _value;

            public AddEventArgs(TKey key, TValue value)
            {

            }

            public TKey Key
            {
                get
                {
                    return _key;
                }
            }

            public TValue Value
            {
                get
                {
                    return _value;
                }
            }
        }
    }
}
