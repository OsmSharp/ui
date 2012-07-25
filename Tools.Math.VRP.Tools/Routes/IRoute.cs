using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.VRP.Core.Routes
{
    /// <summary>
    /// Represents a route.
    /// </summary>
    public interface IRoute : IEnumerable<int>
    {
        /// <summary>
        /// Returns true if the route is empty.
        /// </summary>
        bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// Returns true if the last customer is linked with the first one.
        /// </summary>
        bool IsRound
        {
            get;
        }

        /// <summary>
        /// Returns the amount of customers in the route.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Returns the first customer.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int First
        {
            get;
        }

        /// <summary>
        /// Returns the last customer.
        /// </summary>
        int Last
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
        /// Removes a customer from the route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        bool Remove(int customer);

        /// <summary>
        /// Inserts a customer between two others.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="customer"></param>
        /// <param name="to"></param>
        void Insert(int from, int customer, int to);

        /// <summary>
        /// Returns the neigbours of a customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        int[] GetNeigbours(int customer);

        /// <summary>
        /// Returns the index of the given customer the first being zero.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        int GetIndexOf(int customer);

        /// <summary>
        /// Returns true if the route is valid.
        /// </summary>
        /// <returns></returns>
        bool IsValid();
    }
}
