using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Collections.PriorityQueues;
using NUnit.Framework;

namespace OsmSharp.UnitTests.Tools
{
    /// <summary>
    /// Tests against the binairy heap.
    /// </summary>
    [TestFixture]
    public class BinairyHeapTests
    {
        /// <summary>
        /// Tests adding one element to the heap.
        /// </summary>
        [Test]
        public void TestBinairyHeapQueueOneElement()
        {
            // creates a new binairy heap.
            BinairyHeap<string> heap = new BinairyHeap<string>();

            // enqueue one item.
            heap.Push("one", 1);

            // test the result.
            Assert.AreEqual(1, heap.Count);
            Assert.AreEqual(1, heap.PeekWeight());
            Assert.AreEqual("one", heap.Peek());
        }

        /// <summary>
        /// Tests adding one element to the heap.
        /// </summary>
        [Test]
        public void TestBinairyHeapQueueMultipleElements()
        {
            // creates a new binairy heap.
            BinairyHeap<string> heap = new BinairyHeap<string>();

            // enqueue one item.
            heap.Push("one", 1);
            heap.Push("two", 2);
            heap.Push("three", 3);
            heap.Push("four", 4);
            heap.Push("five", 5);
            heap.Push("six", 6);

            // test the result.
            Assert.AreEqual(6, heap.Count);
            Assert.AreEqual(1, heap.PeekWeight());
            Assert.AreEqual("one", heap.Peek());
        }

        /// <summary>
        /// Tests adding one element to the heap.
        /// </summary>
        [Test]
        public void TestBinairyHeapQueueDeQueueRandom()
        {
            // the elements.
            List<KeyValuePair<string, float>> elements =
                new List<KeyValuePair<string, float>>();
            elements.Add(new KeyValuePair<string, float>("one", 1));
            elements.Add(new KeyValuePair<string, float>("two", 2));
            elements.Add(new KeyValuePair<string, float>("three", 3));
            elements.Add(new KeyValuePair<string, float>("four", 4));
            elements.Add(new KeyValuePair<string, float>("five", 5));
            elements.Add(new KeyValuePair<string, float>("six", 6));
            elements.Add(new KeyValuePair<string, float>("seven", 7));
            elements.Add(new KeyValuePair<string, float>("eight", 8));
            elements.Add(new KeyValuePair<string, float>("nine", 9));
            elements.Add(new KeyValuePair<string, float>("ten", 10));

            // creates a new binairy heap.
            BinairyHeap<string> heap = new BinairyHeap<string>();

            // enqueue one item.
            while (elements.Count > 0)
            { // keep selecting existing elements.
                int selected = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(elements.Count);

                KeyValuePair<string, float> selected_pair =
                    elements[selected];
                elements.RemoveAt(selected);

                // add to the heap.
                heap.Push(selected_pair.Key, selected_pair.Value);
            }

            // test the result.
            Assert.AreEqual(10, heap.Count);
            Assert.AreEqual(1, heap.PeekWeight());
            Assert.AreEqual("one", heap.Peek());

            // remove the items one by one and test the results again.
            while (heap.Count > 0)
            { // keep removing.
                Assert.AreEqual(10 - elements.Count, heap.Count);
                Assert.AreEqual(elements.Count + 1, heap.PeekWeight());

                // dequeue.
                elements.Add(new KeyValuePair<string, float>(heap.Pop(), elements.Count + 1));
            }

            // try to dequeue again.
            Assert.AreEqual(null, heap.Pop());

            // clear the elements list and try again!
            elements.Clear();
            elements.Add(new KeyValuePair<string, float>("one", 1));
            elements.Add(new KeyValuePair<string, float>("two", 2));
            elements.Add(new KeyValuePair<string, float>("three", 3));
            elements.Add(new KeyValuePair<string, float>("four", 4));
            elements.Add(new KeyValuePair<string, float>("five", 5));
            elements.Add(new KeyValuePair<string, float>("six", 6));
            elements.Add(new KeyValuePair<string, float>("seven", 7));
            elements.Add(new KeyValuePair<string, float>("eight", 8));
            elements.Add(new KeyValuePair<string, float>("nine", 9));
            elements.Add(new KeyValuePair<string, float>("ten", 10));

            // enqueue one item.
            while (elements.Count > 0)
            { // keep selecting existing elements.
                int selected = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(elements.Count);

                KeyValuePair<string, float> selected_pair =
                    elements[selected];
                elements.RemoveAt(selected);

                // add to the heap.
                heap.Push(selected_pair.Key, selected_pair.Value);
            }

            // test the result.
            Assert.AreEqual(10, heap.Count);
            Assert.AreEqual(1, heap.PeekWeight());
            Assert.AreEqual("one", heap.Peek());

            // remove the items one by one and test the results again.
            while (heap.Count > 0)
            { // keep removing.
                Assert.AreEqual(10 - elements.Count, heap.Count);
                Assert.AreEqual(elements.Count + 1, heap.PeekWeight());

                // dequeue.
                elements.Add(new KeyValuePair<string, float>(heap.Pop(), elements.Count + 1));
            }

            // try to dequeue again.
            Assert.AreEqual(null, heap.Pop());
        }
    }
}
