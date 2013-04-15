using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Collections.PriorityQueues;

namespace OsmSharp.Tools.Math.VRP.Core.BestPlacement.InsertionCosts
{
    /// <summary>
    /// Keeps insertion costs using binary heap priority queues.
    /// </summary>
    public class BinaryHeapInsertionCosts : IInsertionCosts
    {
        /// <summary>
        /// Holds all the insertion cost heaps.
        /// </summary>
        private Dictionary<int, Dictionary<int, BinairyHeap<InsertionCost>>> _costs;

        /// <summary>
        /// Creates a new insertion cost data structure using binary heap priority queues.
        /// </summary>
        public BinaryHeapInsertionCosts()
        {
            _costs = new Dictionary<int, Dictionary<int, BinairyHeap<InsertionCost>>>();
        }

        /// <summary>
        /// Returns the cheapest cost and removes.
        /// </summary>
        /// <param name="customer_from"></param>
        /// <param name="customer_to"></param>
        /// <returns></returns>
        public InsertionCost PopCheapest(int customer_from, int customer_to)
        {
            BinairyHeap<InsertionCost> customer_heap;
            Dictionary<int, BinairyHeap<InsertionCost>> heaps;
            if (_costs.TryGetValue(customer_from, out heaps) && heaps.TryGetValue(customer_to, out customer_heap))
            { // there is a heap for this customer pair.
                return customer_heap.Pop();
            }
            return null;
        }

        /// <summary>
        /// Returns the cheapest cost.
        /// </summary>
        /// <param name="customer_from"></param>
        /// <param name="customer_to"></param>
        /// <returns></returns>
        public InsertionCost PeekCheapest(int customer_from, int customer_to)
        {
            BinairyHeap<InsertionCost> customer_heap;
            Dictionary<int, BinairyHeap<InsertionCost>> heaps;
            if (_costs.TryGetValue(customer_from, out heaps) && heaps.TryGetValue(customer_to, out customer_heap))
            { // there is a heap for this customer pair.
                return customer_heap.Peek();
            }
            return null;
        }

        /// <summary>
        /// Adds a new cost.
        /// </summary>
        /// <param name="customer_from"></param>
        /// <param name="customer_to"></param>
        /// <param name="customer"></param>
        /// <param name="cost"></param>
        /// <returns></returns>
        public void Add(int customer_from, int customer_to, int customer, float cost)
        {
            BinairyHeap<InsertionCost> customer_heap;
            Dictionary<int, BinairyHeap<InsertionCost>> heaps;
            if (!_costs.TryGetValue(customer_from, out heaps))
            { // there is no heap for this customer pair.
                customer_heap = new BinairyHeap<InsertionCost>();
                heaps = new Dictionary<int, BinairyHeap<InsertionCost>>();
                heaps.Add(customer_to, customer_heap);
                _costs.Add(customer_from, heaps);
            }
            else if (!heaps.TryGetValue(customer_to, out customer_heap))
            { // there is no heap for this customer pair.
                customer_heap = new BinairyHeap<InsertionCost>();
                heaps.Add(customer_to, customer_heap);
            }
            customer_heap.Push(new InsertionCost()
            {
                Cost = cost,
                Customer = customer
            }, cost);
        }

        /// <summary>
        /// Adds a new cost.
        /// </summary>
        /// <param name="customer_from"></param>
        /// <param name="customer_to"></param>
        /// <param name="costs"></param>
        /// <returns></returns>
        public void Add(int customer_from, int customer_to, IEnumerable<InsertionCost> costs)
        {
            BinairyHeap<InsertionCost> customer_heap;
            Dictionary<int, BinairyHeap<InsertionCost>> heaps;
            if (!_costs.TryGetValue(customer_from, out heaps))
            { // there is no heap for this customer pair.
                customer_heap = new BinairyHeap<InsertionCost>();
                heaps = new Dictionary<int, BinairyHeap<InsertionCost>>();
                heaps.Add(customer_to, customer_heap);
                _costs.Add(customer_from, heaps);
            }
            else if (!heaps.TryGetValue(customer_to, out customer_heap))
            { // there is no heap for this customer pair.
                customer_heap = new BinairyHeap<InsertionCost>();
                heaps.Add(customer_to, customer_heap);
            }
            foreach (InsertionCost cost in costs)
            {
                customer_heap.Push(new InsertionCost()
                {
                    Cost = cost.Cost,
                    Customer = cost.Customer
                }, cost.Cost);
            }
        }

        /// <summary>
        /// Returns the amount of costs stored.
        /// </summary>
        /// <param name="customer_from"></param>
        /// <param name="customer_to"></param>
        /// <returns></returns>
        public int Count(int customer_from, int customer_to)
        {
            BinairyHeap<InsertionCost> customer_heap;
            Dictionary<int, BinairyHeap<InsertionCost>> heaps;
            if (_costs.TryGetValue(customer_from, out heaps) && heaps.TryGetValue(customer_to, out customer_heap))
            { // there is a heap for this customer pair.
                return customer_heap.Count;
            }
            return 0;
        }

        /// <summary>
        /// Removes all the costs.
        /// </summary>
        /// <param name="customer_from"></param>
        /// <param name="customer_to"></param>
        /// <returns></returns>
        public bool Remove(int customer_from, int customer_to)
        {
            Dictionary<int, BinairyHeap<InsertionCost>> heaps;
            if (_costs.TryGetValue(customer_from, out heaps) && heaps.Remove(customer_to))
            { // there is a heap for this customer pair.
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears all costs.
        /// </summary>
        public void Clear()
        {
            _costs.Clear();
        }
    }
}
