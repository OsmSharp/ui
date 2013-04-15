using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Tools.Math.VRP.Core.BestPlacement.InsertionCosts
{
    /// <summary>
    /// Abstract a data structure keeping pre-calculated insertion costs.
    /// </summary>
    public interface IInsertionCosts
    {
        /// <summary>
        /// Returns the cheapest insertion and removes it.
        /// </summary>
        /// <returns></returns>
        InsertionCost PopCheapest(int customer_from, int customer_to);

        /// <summary>
        /// Returns the cheapest insertion.
        /// </summary>
        /// <returns></returns>
        InsertionCost PeekCheapest(int customer_from, int customer_to);

        /// <summary>
        /// Adds a new cost.
        /// </summary>
        /// <param name="customer_from"></param>
        /// <param name="customer_to"></param>
        /// <param name="customer"></param>
        /// <param name="cost"></param>
        void Add(int customer_from, int customer_to, int customer, float cost);

        /// <summary>
        /// Adds new costs in bulk.
        /// </summary>
        /// <param name="customer_from"></param>
        /// <param name="customer_to"></param>
        /// <param name="costs"></param>
        void Add(int customer_from, int customer_to, IEnumerable<InsertionCost> costs);

        /// <summary>
        /// Returns the amount of costs in this data structures between the two given customers.
        /// </summary>
        /// <param name="customer_from"></param>
        /// <param name="customer_to"></param>
        /// <returns></returns>
        int Count(int customer_from, int customer_to);

        /// <summary>
        /// Removes all costs in this data structures between the two given customers.
        /// </summary>
        /// <param name="customer_from"></param>
        /// <param name="customer_to"></param>
        /// <returns></returns>
        bool Remove(int customer_from, int customer_to);

        /// <summary>
        /// Clears out all costs.
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Represents an insertion cost.
    /// </summary>
    public class InsertionCost
    {
        /// <summary>
        /// The customer to insert.
        /// </summary>
        public int Customer { get; set; }

        /// <summary>
        /// The actual insertion cost.
        /// </summary>
        public float Cost { get; set; }
    }
}
