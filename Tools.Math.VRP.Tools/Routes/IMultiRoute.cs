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

        /// <summary>
        /// Returns true if there is an edge in this route from from to to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        bool Contains(int from, int to);

        /// <summary>
        /// Returns true if the given customer is in this route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        bool Contains(int customer);
    }
}
