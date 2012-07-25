using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.VRP.Core.Routes
{
    /// <summary>
    /// Represents an object containing multiple routes.
    /// </summary>
    public interface IMultiRoute : ICloneable
    {
        /// <summary>
        /// Returns one of the routes.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        IRoute Route(int idx);

        /// <summary>
        /// Returns the number of routes.
        /// </summary>
        int Count
        {
            get;
        }

    }
}
