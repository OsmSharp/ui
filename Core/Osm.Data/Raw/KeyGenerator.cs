using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Data
{
    /// <summary>
    /// Generator to generate keys for objects that are new and have an id < 0.
    /// </summary>
    public static class KeyGenerator
    {
        private static int _current_id = 0;

        /// <summary>
        /// Returns the next unique id.
        /// </summary>
        /// <returns></returns>
        public static int GenerateNew()
        {
            _current_id--;
            return _current_id;
        }
    }
}
