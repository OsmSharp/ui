using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core.Sources
{
    /// <summary>
    /// Represents any source of ways.
    /// </summary>
    public interface IWaySource
    {
        /// <summary>
        /// Returns a way with the given id from this source.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Way GetWay(long id);
    }
}
