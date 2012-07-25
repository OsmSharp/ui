using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.Raw
{
    public interface IResolveMatcher
    {
        /// <summary>
        /// Returns true if the way is a suitable candidate for the point being resolved.
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        bool MatchName(string name);
    }
}
