using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric
{
    /// <summary>
    /// Represents a dynamic multi route where all routes start/end at one single depot represented by customer 0.
    /// </summary>
    public class DepotDynamicAsymmetricMultiRoute : DynamicAsymmetricMultiRoute
    {
        /// <summary>
        /// Creates a new solution in the form of a DynamicAsymmetricMultiRoute.
        /// </summary>
        /// <param name="size"></param>
        public DepotDynamicAsymmetricMultiRoute(int size)
            : base(size, false)
        {

        }

        /// <summary>
        /// Creates a new solution based on an existing one.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="next_array"></param>
        protected DepotDynamicAsymmetricMultiRoute(int[] first, int[] next_array)
            : base(first, next_array, false)
        {

        }

        /// <summary>
        /// Clones this solution.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            int[] first = new int[this.Count];
            for (int idx = 0; idx < this.Count; idx++)
            {
                IRoute route = this.Route(idx);
                if (route.Count > 0)
                {
                    first[idx] = route.ElementAt<int>(1);
                }
                else
                {
                    first[idx] = -1;
                }
            }
            return new DepotDynamicAsymmetricMultiRoute(first, _next_array.Clone() as int[]);
        }

        /// <summary>
        /// Adds a new empty route.
        /// </summary>
        /// <returns></returns>
        public override IRoute Add()
        {
            return base.Add();
        }

        /// <summary>
        /// Adds a new route initialized with the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public override IRoute Add(int customer)
        {
            return base.Add(customer);
        }

        /// <summary>
        /// Adds a new route by copying the given one.
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public override IRoute Add(IRoute route)
        {
            return base.Add(route);
        }

        /// <summary>
        /// Returns the route at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override IRoute Route(int idx)
        {
            return new DepotRoute(base.Route(idx));
        }
    }
}
