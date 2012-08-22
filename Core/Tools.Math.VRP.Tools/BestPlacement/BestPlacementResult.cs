using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.VRP.Core.BestPlacement
{
    /// <summary>
    /// The result returned after calculating a best-placement position.
    /// </summary>
    public struct CheapestInsertionResult
    {
        /// <summary>
        /// The increase in weight.
        /// </summary>
        public float Increase;

        /// <summary>
        /// The customer where the customer needs to be placed after.
        /// </summary>
        public int CustomerBefore;

        /// <summary>
        /// The customer where the customer needs to be placed before.
        /// </summary>
        public int CustomerAfter;

        /// <summary>
        /// The customer being placed.
        /// </summary>
        public int Customer;
    }
}
