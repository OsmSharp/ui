using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core;

namespace Tools.Math.VRP.Core.Routes.ASymmetric
{
    /// <summary>
    /// Interface representing a route along serveral customers and the operations possible on it.
    /// </summary>
    public interface IASymmetricRoute : IRoute, IEnumerable<int>
    {
        /// <summary>
        /// Returns the customer right after this one.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        int Next(int customer);
    }
}
