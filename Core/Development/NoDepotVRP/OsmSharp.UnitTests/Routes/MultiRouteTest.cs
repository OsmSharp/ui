using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.UnitTests.Routes
{
    /// <summary>
    /// Containts tests for a IMultiRoute implementation.
    /// </summary>
    public abstract class MultiRouteTest
    {
        /// <summary>
        /// Builds a multi route instances to test on.
        /// </summary>
        /// <param name="is_round"></param>
        /// <returns></returns>
        protected abstract IMultiRoute BuildRoute(bool is_round);

        /// <summary>
        /// Tests adding customers.
        /// </summary>
        public void DoTestAdd()
        {
            IMultiRoute multi_route = this.BuildRoute(true);

            Assert.AreEqual(0, multi_route.Count);
            int count = 10;
            int routes = 3;
            for (int route_idx = 0; route_idx < routes; route_idx++)
            {
                int customer_start = (route_idx * count);
                IRoute route = multi_route.Add(customer_start);
                Assert.AreEqual(1, route.Count);
                Assert.AreEqual(false, route.IsEmpty);
                Assert.AreEqual(true, route.IsRound);
                for (int customer = customer_start + 1; customer < customer_start + count; customer++)
                {
                    route.Insert(customer - 1, customer, -1);

                    Assert.AreEqual(customer - customer_start + 1, route.Count);
                    Assert.AreEqual(false, route.IsEmpty);
                    Assert.AreEqual(true, route.IsRound);
                    Assert.AreEqual(customer_start, route.First);
                    Assert.AreEqual(customer_start, route.Last);
                }

                for (int customer = customer_start + 1; customer < customer_start + count - 1; customer++)
                {
                    Assert.IsTrue(route.Contains(customer));
                    Assert.IsTrue(route.Contains(customer, customer + 1));
                }
                Assert.IsTrue(route.Contains(customer_start + count - 1));
                Assert.IsTrue(route.Contains(customer_start + count - 1, customer_start));

                Assert.AreEqual(route_idx + 1, multi_route.Count);
                Assert.AreEqual(count * (route_idx + 1), multi_route.Size);
            }

            // test with initializing the routes empty.
            multi_route = this.BuildRoute(true);

            Assert.AreEqual(0, multi_route.Count);
            for (int route_idx = 0; route_idx < routes; route_idx++)
            {
                int customer_start = (route_idx * count);
                IRoute route = multi_route.Add();
                Assert.AreEqual(0, route.Count);
                Assert.AreEqual(true, route.IsEmpty);
                Assert.AreEqual(true, route.IsRound);
                for (int customer = customer_start; customer < customer_start + count; customer++)
                {
                    route.Insert(customer - 1, customer, -1);

                    Assert.AreEqual(customer - customer_start + 1, route.Count);
                    Assert.AreEqual(false, route.IsEmpty);
                    Assert.AreEqual(true, route.IsRound);
                    Assert.AreEqual(customer_start, route.First);
                    Assert.AreEqual(customer_start, route.Last);
                }

                for (int customer = customer_start; customer < customer_start + count - 1; customer++)
                {
                    Assert.IsTrue(route.Contains(customer));
                    Assert.IsTrue(route.Contains(customer, customer + 1));
                }
                Assert.IsTrue(route.Contains(customer_start + count - 1));
                Assert.IsTrue(route.Contains(customer_start + count - 1, customer_start));

                Assert.AreEqual(route_idx + 1, multi_route.Count);
                Assert.AreEqual(count * (route_idx + 1), multi_route.Size);
            }
        }

        /// <summary>
        /// Tests adding customers.
        /// </summary>
        public void DoTestRemove()
        {
            IMultiRoute multi_route = this.BuildRoute(true);

            Assert.AreEqual(0, multi_route.Count);
            int count = 10;
            int routes = 3;

            // test with initializing the routes empty.
            List<List<int>> customers_per_route = new List<List<int>>();
            Assert.AreEqual(0, multi_route.Count);
            for (int route_idx = 0; route_idx < routes; route_idx++)
            {
                customers_per_route.Add(new List<int>());
                int customer_start = (route_idx * count);
                IRoute route = multi_route.Add();

                for (int customer = customer_start; customer < customer_start + count; customer++)
                {
                    customers_per_route[route_idx].Add(customer);
                    route.Insert(customer - 1, customer, -1);
                }
            }

            // remove all the customers.
            while (customers_per_route.Count > 0)
            {
                int route_idx = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(customers_per_route.Count);
                int customer_idx = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(customers_per_route[route_idx].Count);

                int customer = customers_per_route[route_idx][customer_idx];
                customers_per_route[route_idx].RemoveAt(customer_idx);

                IRoute route = multi_route.Route(route_idx);
                route.Remove(customer);

                Assert.AreEqual(customers_per_route[route_idx].Count, route.Count);
                Assert.AreEqual(customers_per_route[route_idx].Count == 0, route.IsEmpty);
                Assert.AreEqual(true, route.IsRound);
                Assert.AreEqual(route.First, route.Last);

                if (customers_per_route[route_idx].Count == 0)
                {
                    customers_per_route.RemoveAt(route_idx);
                    multi_route.Remove(route_idx);
                }
            }
        }

        /// <summary>
        /// Tests removing adding every customer at every position.
        /// </summary>
        public void DoTestAddRemoveComplete()
        {
            int count = 10;
            int routes = 3;

            for (int route_to_test = 0; route_to_test < routes; route_to_test++)
            {
                for (int customer_to_remove = (route_to_test * count); customer_to_remove < (route_to_test * count) + count; customer_to_remove++)
                {
                    for (int customer_to_place_after = (route_to_test * count); customer_to_place_after < (route_to_test * count) + count; customer_to_place_after++)
                    {
                        if (customer_to_remove != customer_to_place_after)
                        {
                            IMultiRoute multi_route = this.BuildRoute(true);

                            Assert.AreEqual(0, multi_route.Count);

                            // test with initializing the routes empty.
                            List<List<int>> customers_per_route = new List<List<int>>();
                            Assert.AreEqual(0, multi_route.Count);
                            IRoute route;
                            for (int route_idx = 0; route_idx < routes; route_idx++)
                            {
                                customers_per_route.Add(new List<int>());
                                int customer_start = (route_idx * count);
                                route = multi_route.Add();

                                for (int customer = customer_start; customer < customer_start + count; customer++)
                                {
                                    customers_per_route[route_idx].Add(customer);
                                    route.Insert(customer - 1, customer, -1);
                                }
                            }

                            route = multi_route.Route(route_to_test);

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
}
