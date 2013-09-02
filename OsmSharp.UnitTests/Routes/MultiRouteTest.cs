// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using NUnit.Framework;
using OsmSharp.Math.VRP.Core.Routes;

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
        /// <param name="isRound"></param>
        /// <returns></returns>
        protected abstract IMultiRoute BuildRoute(bool isRound);

        /// <summary>
        /// Tests adding customers.
        /// </summary>
        public void DoTestAdd()
        {
            IMultiRoute multiRoute = this.BuildRoute(true);

            Assert.AreEqual(0, multiRoute.Count);
            int count = 10;
            int routes = 3;
            for (int routeIdx = 0; routeIdx < routes; routeIdx++)
            {
                int customerStart = (routeIdx * count);
                IRoute route = multiRoute.Add(customerStart);
                Assert.AreEqual(1, route.Count);
                Assert.AreEqual(false, route.IsEmpty);
                Assert.AreEqual(true, route.IsRound);
                for (int customer = customerStart + 1; customer < customerStart + count; customer++)
                {
                    route.InsertAfter(customer - 1, customer);

                    Assert.AreEqual(customer - customerStart + 1, route.Count);
                    Assert.AreEqual(false, route.IsEmpty);
                    Assert.AreEqual(true, route.IsRound);
                    Assert.AreEqual(customerStart, route.First);
                    Assert.AreEqual(customerStart, route.Last);
                }

                for (int customer = customerStart + 1; customer < customerStart + count - 1; customer++)
                {
                    Assert.IsTrue(route.Contains(customer));
                    Assert.IsTrue(route.Contains(customer, customer + 1));
                }
                Assert.IsTrue(route.Contains(customerStart + count - 1));
                Assert.IsTrue(route.Contains(customerStart + count - 1, customerStart));

                Assert.AreEqual(routeIdx + 1, multiRoute.Count);
                Assert.AreEqual(count * (routeIdx + 1), multiRoute.Size);
            }

            // test with initializing the routes empty.
            multiRoute = this.BuildRoute(true);

            Assert.AreEqual(0, multiRoute.Count);
            for (int routeIdx = 0; routeIdx < routes; routeIdx++)
            {
                int customerStart = (routeIdx * count);
                IRoute route = multiRoute.Add();
                Assert.AreEqual(0, route.Count);
                Assert.AreEqual(true, route.IsEmpty);
                Assert.AreEqual(true, route.IsRound);
                for (int customer = customerStart; customer < customerStart + count; customer++)
                {
                    route.InsertAfter(customer - 1, customer);

                    Assert.AreEqual(customer - customerStart + 1, route.Count);
                    Assert.AreEqual(false, route.IsEmpty);
                    Assert.AreEqual(true, route.IsRound);
                    Assert.AreEqual(customerStart, route.First);
                    Assert.AreEqual(customerStart, route.Last);
                }

                for (int customer = customerStart; customer < customerStart + count - 1; customer++)
                {
                    Assert.IsTrue(route.Contains(customer));
                    Assert.IsTrue(route.Contains(customer, customer + 1));
                }
                Assert.IsTrue(route.Contains(customerStart + count - 1));
                Assert.IsTrue(route.Contains(customerStart + count - 1, customerStart));

                Assert.AreEqual(routeIdx + 1, multiRoute.Count);
                Assert.AreEqual(count * (routeIdx + 1), multiRoute.Size);
            }
        }

        /// <summary>
        /// Tests adding customers.
        /// </summary>
        public void DoTestRemove()
        {
            IMultiRoute multiRoute = this.BuildRoute(true);

            Assert.AreEqual(0, multiRoute.Count);
            int count = 10;
            int routes = 3;

            // test with initializing the routes empty.
            var customersPerRoute = new List<List<int>>();
            Assert.AreEqual(0, multiRoute.Count);
            for (int routeIdx = 0; routeIdx < routes; routeIdx++)
            {
                customersPerRoute.Add(new List<int>());
                int customerStart = (routeIdx * count);
                IRoute route = multiRoute.Add();

                for (int customer = customerStart; customer < customerStart + count; customer++)
                {
                    customersPerRoute[routeIdx].Add(customer);
                    route.InsertAfter(customer - 1, customer);
                    //route.InsertAfterAndRemove(customer - 1, customer, -1);
                }
            }

            // remove all the customers.
            while (customersPerRoute.Count > 0)
            {
                int routeIdx = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(customersPerRoute.Count);
                int customerIdx = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(customersPerRoute[routeIdx].Count);

                int customer = customersPerRoute[routeIdx][customerIdx];
                customersPerRoute[routeIdx].RemoveAt(customerIdx);

                IRoute route = multiRoute.Route(routeIdx);
                route.Remove(customer);

                Assert.AreEqual(customersPerRoute[routeIdx].Count, route.Count);
                Assert.AreEqual(customersPerRoute[routeIdx].Count == 0, route.IsEmpty);
                Assert.AreEqual(true, route.IsRound);
                Assert.AreEqual(route.First, route.Last);

                if (customersPerRoute[routeIdx].Count == 0)
                {
                    customersPerRoute.RemoveAt(routeIdx);
                    multiRoute.Remove(routeIdx);
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

            for (int routeToTest = 0; routeToTest < routes; routeToTest++)
            {
                for (int customerToRemove = (routeToTest * count); customerToRemove < (routeToTest * count) + count; customerToRemove++)
                {
                    for (int customerToPlaceAfter = (routeToTest * count); customerToPlaceAfter < (routeToTest * count) + count; customerToPlaceAfter++)
                    {
                        if (customerToRemove != customerToPlaceAfter)
                        {
                            IMultiRoute multiRoute = this.BuildRoute(true);

                            Assert.AreEqual(0, multiRoute.Count);

                            // test with initializing the routes empty.
                            var customersPerRoute = new List<List<int>>();
                            Assert.AreEqual(0, multiRoute.Count);
                            IRoute route;
                            for (int routeIdx = 0; routeIdx < routes; routeIdx++)
                            {
                                customersPerRoute.Add(new List<int>());
                                int customerStart = (routeIdx * count);
                                route = multiRoute.Add();

                                for (int customer = customerStart; customer < customerStart + count; customer++)
                                {
                                    customersPerRoute[routeIdx].Add(customer);
                                    route.InsertAfter(customer - 1, customer);
                                }
                            }

                            route = multiRoute.Route(routeToTest);

                            route.Remove(customerToRemove);
                            route.InsertAfter(customerToPlaceAfter, customerToRemove);

                            Assert.IsTrue(route.Contains(customerToPlaceAfter, customerToRemove));
                            Assert.AreEqual(count, route.Count);
                            var customersInRoute = new HashSet<int>(route);
                            Assert.AreEqual(customersInRoute.Count, route.Count);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests all enumerations of a route.
        /// </summary>
        public void DoTestEnumerateBetween()
        {
            int count = 10;
            int routes = 3;

            IMultiRoute multiRoute = this.BuildRoute(true);

            Assert.AreEqual(0, multiRoute.Count);

            // test with initializing the routes empty.
            var customersPerRoute = new List<List<int>>();
            Assert.AreEqual(0, multiRoute.Count);
            IRoute route;
            for (int routeIdx = 0; routeIdx < routes; routeIdx++)
            {
                customersPerRoute.Add(new List<int>());
                int customerStart = (routeIdx * count);
                route = multiRoute.Add();

                for (int customer = customerStart; customer < customerStart + count; customer++)
                {
                    customersPerRoute[routeIdx].Add(customer);
                    route.InsertAfter(customer - 1, customer);
                    //route.InsertAfterAndRemove(customer - 1, customer, -1);
                }
            }

            for (int routeToTest = 0; routeToTest < routes; routeToTest++)
            {
                route = multiRoute.Route(routeToTest);

                for (int from = (routeToTest * count); from < (routeToTest * count) + count; from++)
                {
                    for (int to = (routeToTest * count); to < (routeToTest * count) + count; to++)
                    {
                        IEnumerator<int> enumerator = route.Between(from, to).GetEnumerator();
                        for (int customer = from; customer - 1 != to; customer++)
                        {
                            if (customer == (routeToTest * count) + count)
                            {
                                customer = (routeToTest * count);
                            }

                            Assert.IsTrue(enumerator.MoveNext());
                            Assert.AreEqual(customer, enumerator.Current);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Do get neighbour tests.
        /// </summary>
        protected void DoTestGetNeighbours()
        {
            int count = 10;
            int routes = 3;

            IMultiRoute multiRoute = this.BuildRoute(true);

            Assert.AreEqual(0, multiRoute.Count);

            // test with initializing the routes empty.
            var customersPerRoute = new List<List<int>>();
            Assert.AreEqual(0, multiRoute.Count);
            IRoute route;
            for (int routeIdx = 0; routeIdx < routes; routeIdx++)
            {
                customersPerRoute.Add(new List<int>());
                int customerStart = (routeIdx * count);
                route = multiRoute.Add();

                for (int customer = customerStart; customer < customerStart + count; customer++)
                {
                    customersPerRoute[routeIdx].Add(customer);
                    route.InsertAfter(customer - 1, customer);
                    //route.InsertAfterAndRemove(customer - 1, customer, -1);
                }
            }

            for (int routeToTest = 0; routeToTest < routes; routeToTest++)
            {
                route = multiRoute.Route(routeToTest);

                int[] neighbours;
                for (int customer = (routeToTest * count); customer < (routeToTest * count) + count - 1; customer++)
                {
                    neighbours = route.GetNeigbours(customer);
                    Assert.IsTrue(neighbours[0] == customer + 1);
                }
                neighbours = route.GetNeigbours((routeToTest * count) + count - 1);
                Assert.IsTrue(neighbours[0] == (routeToTest * count));
            }
        }
    }
}