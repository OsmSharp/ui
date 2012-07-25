using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core
{
    internal static class OsmBaseIdGenerator
    {
        private static int _id = 0;

        public static int NewId()
        {
            _id--;
            return _id;
        }
    }
}
