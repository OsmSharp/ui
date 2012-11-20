using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Tools.Core.Collections.PriorityQueues
{
    /// <summary>
    /// Represents general functionality of a priority queue.
    /// </summary>
    public interface IPriorityQueue<T>
    {
        /// <summary>
        /// Returns the number of items in this queue.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Enqueues a given item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        void Enqueue(T item, float priority);

        /// <summary>
        /// Returns the smallest weight in the queue.
        /// </summary>
        /// <returns></returns>
        float PeekWeight();

        /// <summary>
        /// Returns the object with the smallest weight.
        /// </summary>
        /// <returns></returns>
        T Peek();

        /// <summary>
        /// Returns the object with the smallest weight and removes it.
        /// </summary>
        /// <returns></returns>
        T DeQueue();
    }
}
