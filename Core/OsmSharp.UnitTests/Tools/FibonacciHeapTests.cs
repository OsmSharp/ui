//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Tools.Collections.PriorityQueues;
//using NUnit.Framework;

//namespace OsmSharp.UnitTests.Tools
//{
//    /// <summary>
//    /// Tests against the fibonaci heap.
//    /// </summary>
//    [TestFixture]
//    public class FibonacciHeapTests
//    {
//        /// <summary>
//        /// Tests adding one element to the heap.
//        /// </summary>
//        [Test]
//        public void TestFibonacciHeapQueueOneElement()
//        {
//            // creates a new binairy heap.
//            FibonacciQueue<string> heap = new FibonacciQueue<string>();

//            // enqueue one item.
//            heap.Enqueue("one", 1);

//            // test the result.
//            Assert.AreEqual(1, heap.Count);
//            Assert.AreEqual(1, heap.PeekWeight());
//            Assert.AreEqual("one", heap.Peek());
//        }

//        /// <summary>
//        /// Tests adding one element to the heap.
//        /// </summary>
//        [Test]
//        public void TestFibonacciHeapQueueMultipleElements()
//        {
//            // creates a new binairy heap.
//            FibonacciQueue<string> heap = new FibonacciQueue<string>();

//            // enqueue one item.
//            heap.Enqueue("one", 1);
//            heap.Enqueue("two", 2);
//            heap.Enqueue("three", 3);
//            heap.Enqueue("four", 4);
//            heap.Enqueue("five", 5);
//            heap.Enqueue("six", 6);

//            // test the result.
//            Assert.AreEqual(6, heap.Count);
//            Assert.AreEqual(1, heap.PeekWeight());
//            Assert.AreEqual("one", heap.Peek());
//        }

//        /// <summary>
//        /// Tests adding one element to the heap.
//        /// </summary>
//        [Test]
//        public void TestFibonacciHeapQueueDeQueueRandom()
//        {
//            // the elements.
//            List<KeyValuePair<string, float>> elements =
//                new List<KeyValuePair<string, float>>();
//            elements.Add(new KeyValuePair<string, float>("one", 1));
//            elements.Add(new KeyValuePair<string, float>("two", 2));
//            elements.Add(new KeyValuePair<string, float>("three", 3));
//            elements.Add(new KeyValuePair<string, float>("four", 4));
//            elements.Add(new KeyValuePair<string, float>("five", 5));
//            elements.Add(new KeyValuePair<string, float>("six", 6));
//            elements.Add(new KeyValuePair<string, float>("seven", 7));
//            elements.Add(new KeyValuePair<string, float>("eight", 8));
//            elements.Add(new KeyValuePair<string, float>("nine", 9));
//            elements.Add(new KeyValuePair<string, float>("ten", 10));

//            // creates a new binairy heap.
//            FibonacciQueue<string> heap = new FibonacciQueue<string>();

//            // enqueue one item.
//            while (elements.Count > 0)
//            { // keep selecting existing elements.
//                int selected = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(elements.Count);

//                KeyValuePair<string, float> selected_pair =
//                    elements[selected];
//                elements.RemoveAt(selected);

//                // add to the heap.
//                heap.Enqueue(selected_pair.Key, selected_pair.Value);
//            }

//            // test the result.
//            Assert.AreEqual(10, heap.Count);
//            Assert.AreEqual(1, heap.PeekWeight());
//            Assert.AreEqual("one", heap.Peek());

//            // remove the items one by one and test the results again.
//            while (heap.Count > 0)
//            { // keep removing.
//                Assert.AreEqual(10 - elements.Count, heap.Count);
//                Assert.AreEqual(elements.Count + 1, heap.PeekWeight());

//                // dequeue.
//                elements.Add(new KeyValuePair<string, float>(heap.Dequeue(), elements.Count + 1));
//            }

//            // try to dequeue again.
//            Assert.AreEqual(null, heap.Dequeue());

//            // clear the elements list and try again!
//            elements.Clear();
//            elements.Add(new KeyValuePair<string, float>("one", 1));
//            elements.Add(new KeyValuePair<string, float>("two", 2));
//            elements.Add(new KeyValuePair<string, float>("three", 3));
//            elements.Add(new KeyValuePair<string, float>("four", 4));
//            elements.Add(new KeyValuePair<string, float>("five", 5));
//            elements.Add(new KeyValuePair<string, float>("six", 6));
//            elements.Add(new KeyValuePair<string, float>("seven", 7));
//            elements.Add(new KeyValuePair<string, float>("eight", 8));
//            elements.Add(new KeyValuePair<string, float>("nine", 9));
//            elements.Add(new KeyValuePair<string, float>("ten", 10));

//            // enqueue one item.
//            while (elements.Count > 0)
//            { // keep selecting existing elements.
//                int selected = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(elements.Count);

//                KeyValuePair<string, float> selected_pair =
//                    elements[selected];
//                elements.RemoveAt(selected);

//                // add to the heap.
//                heap.Enqueue(selected_pair.Key, selected_pair.Value);
//            }

//            // test the result.
//            Assert.AreEqual(10, heap.Count);
//            Assert.AreEqual(1, heap.PeekWeight());
//            Assert.AreEqual("one", heap.Peek());

//            // remove the items one by one and test the results again.
//            while (heap.Count > 0)
//            { // keep removing.
//                Assert.AreEqual(10 - elements.Count, heap.Count);
//                Assert.AreEqual(elements.Count + 1, heap.PeekWeight());

//                // dequeue.
//                elements.Add(new KeyValuePair<string, float>(heap.Dequeue(), elements.Count + 1));
//            }

//            // try to dequeue again.
//            Assert.AreEqual(null, heap.Dequeue());
//        }
//    }
//}
