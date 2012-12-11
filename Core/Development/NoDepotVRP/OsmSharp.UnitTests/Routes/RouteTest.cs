using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.UnitTests.Routes
{
    /// <summary>
    /// General tests for routes.
    /// </summary>
    public abstract class RouteTest
    {
        /// <summary>
        /// Creates the IRoute implementation to perform tests on with an initial customer.
        /// </summary>
        /// <returns></returns>
        protected abstract IRoute BuildRoute(int customer, bool is_round);

        /// <summary>
        /// Creates the IRoute implementation to perform tests one but one that is empty.
        /// </summary>
        /// <param name="is_round"></param>
        /// <returns></returns>
        protected abstract IRoute BuildRoute(bool is_round);

        /// <summary>
        /// Some tests on an IRoute.
        /// </summary>
        public void DoTestAdd()
        {
            // create a new empty route.
            IRoute route = this.BuildRoute(0, true);
            if (route != null)
            { // this part needs testing!
                Assert.AreEqual(1, route.Count);
                Assert.AreEqual(false, route.IsEmpty);
                Assert.AreEqual(true, route.IsRound);

                for (int customer = 1; customer < 100; customer++)
                {
                    route.Insert(customer - 1, customer, -1);

                    Assert.AreEqual(customer + 1, route.Count);
                    Assert.AreEqual(false, route.IsEmpty);
                    Assert.AreEqual(true, route.IsRound);
                    Assert.AreEqual(0, route.First);
                    Assert.AreEqual(0, route.Last);
                }
            }

            // create a new empty route.
            route = this.BuildRoute(true);
            if (route != null)
            { // this part needs testing.
                Assert.AreEqual(0, route.Count);
                Assert.AreEqual(true, route.IsEmpty);
                Assert.AreEqual(true, route.IsRound);

                for (int customer = 0; customer < 100; customer++)
                {
                    route.Insert(customer - 1, customer, -1);

                    Assert.AreEqual(customer + 1, route.Count);
                    Assert.AreEqual(false, route.IsEmpty);
                    Assert.AreEqual(true, route.IsRound);
                    Assert.AreEqual(0, route.First);
                    Assert.AreEqual(0, route.Last);
                }
            }
        }

        /// <summary>
        /// Test removing customers.
        /// </summary>
        public void DoTestRemove()
        {
            // create a new empty route.
            int count = 100;
            IRoute route = this.BuildRoute(0, true);
            List<int> customers = new List<int>();
            if (route != null)
            { // this part needs testing!
                customers.Add(0);
                for (int customer = 1; customer < count; customer++)
                {
                    route.Insert(customer - 1, customer, -1);
                    customers.Add(customer);
                }

                // remove customers.
                while (customers.Count > 0)
                {
                    int customer_idx = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(
                        customers.Count);
                    int customer = customers[customer_idx];
                    customers.RemoveAt(customer_idx);

                    route.Remove(customer);

                    Assert.AreEqual(customers.Count, route.Count);
                    Assert.AreEqual(customers.Count == 0, route.IsEmpty);
                    Assert.AreEqual(true, route.IsRound);
                    Assert.AreEqual(route.Last, route.First);
                }
            }

            route = this.BuildRoute(true);
            customers = new List<int>();
            if (route != null)
            { // this part needs testing!
                for (int customer = 0; customer < count; customer++)
                {
                    route.Insert(customer - 1, customer, -1);
                    customers.Add(customer);
                }

                // remove customers.
                while (customers.Count > 0)
                {
                    int customer_idx = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(
                        customers.Count);
                    int customer = customers[customer_idx];
                    customers.RemoveAt(customer_idx);

                    route.Remove(customer);

                    Assert.AreEqual(customers.Count, route.Count);
                    Assert.AreEqual(customers.Count == 0, route.IsEmpty);
                    Assert.AreEqual(true, route.IsRound);
                    Assert.AreEqual(route.Last, route.First);
                }
            }
        }

        /// <summary>
        /// Tests if the route containts functions work correctly.
        /// </summary>
        public void DoTestContains()
        {
            // create a new empty route.
            int count = 100;
            IRoute route = this.BuildRoute(true);
            if (route != null)
            { // this part needs testing.
                Assert.AreEqual(0, route.Count);
                Assert.AreEqual(true, route.IsEmpty);
                Assert.AreEqual(true, route.IsRound);

                for (int customer = 0; customer < count; customer++)
                {
                    route.Insert(customer - 1, customer, -1);

                    Assert.AreEqual(customer + 1, route.Count);
                    Assert.AreEqual(false, route.IsEmpty);
                    Assert.AreEqual(true, route.IsRound);
                    Assert.AreEqual(0, route.First);
                    Assert.AreEqual(0, route.Last);
                }

                for (int customer = 0; customer < count - 1; customer++)
                {
                    Assert.IsTrue(route.Contains(customer));
                    Assert.IsTrue(route.Contains(customer, customer + 1));
                }
                Assert.IsTrue(route.Contains(count - 1));
                Assert.IsTrue(route.Contains(count - 1, 0));
            }
        }

        /// <summary>
        /// Test removing adding every customer at every position.
        /// </summary>
        public void DoTestAddRemoveComplete()
        {            
            // create a new empty route.
            int count = 10;
            for (int customer_to_remove = 0; customer_to_remove < count; customer_to_remove++)
            {
                for (int customer_to_place_after = 0; customer_to_place_after < count; customer_to_place_after++)
                {
                    if (customer_to_remove != customer_to_place_after)
                    {
                        IRoute route = this.BuildRoute(true);
                        if (route != null)
                        { // this part needs testing.
                            for (int customer = 0; customer < count; customer++)
                            {
                                route.Insert(customer - 1, customer, -1);
                            }
                        }

                        route.Remove(customer_to_remove);
                        route.Insert(customer_to_place_after, customer_to_remove, -1);

                        Assert.IsTrue(route.Contains(customer_to_place_after, customer_to_remove));
                        Assert.AreEqual(count, route.Count);
                        HashSet<int> customers_in_route = new HashSet<int>(route);
                        Assert.AreEqual(customers_in_route.Count, route.Count);
                    }
                }
            }
        }
    }
}