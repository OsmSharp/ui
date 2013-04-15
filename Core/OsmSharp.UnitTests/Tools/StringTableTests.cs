using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OsmSharp.Tools.Collections;

namespace OsmSharp.UnitTests
{
    /// <summary>
    /// Summary description for StringTableTests
    /// </summary>
    [TestFixture]
    public class StringTableTests
    {
        /// <summary>
        /// Tests adding strings to a string table.
        /// </summary>
        [Test]
        public void TestStringTable_AddStrings()
        {
            ObjectTable<string> table = new ObjectTable<string>(false);
            uint zero = table.Add("zero");
            uint one = table.Add("one");
            uint two = table.Add("two");
            uint three = table.Add("three");
            uint four = table.Add("four");
            uint five = table.Add("five");
            uint six = table.Add("six");

            Assert.AreEqual("zero", table.Get(0));
            Assert.AreEqual("one", table.Get(1));
            Assert.AreEqual("two", table.Get(2));
            Assert.AreEqual("three", table.Get(3));
            Assert.AreEqual("four", table.Get(4));
            Assert.AreEqual("five", table.Get(5));
            Assert.AreEqual("six", table.Get(6));


            table = new ObjectTable<string>(true);
            zero = table.Add("zero");
            one = table.Add("one");
            two = table.Add("two");
            three = table.Add("three");
            four = table.Add("four");
            five = table.Add("five");
            six = table.Add("six");

            Assert.AreEqual("zero", table.Get(0));
            Assert.AreEqual("one", table.Get(1));
            Assert.AreEqual("two", table.Get(2));
            Assert.AreEqual("three", table.Get(3));
            Assert.AreEqual("four", table.Get(4));
            Assert.AreEqual("five", table.Get(5));
            Assert.AreEqual("six", table.Get(6));
        }

        /// <summary>
        /// Tests adding strings twice to a string table.
        /// </summary>
        [Test]
        public void TestStringTable_AddStringsTwice()
        {
            ObjectTable<string> table = new ObjectTable<string>(false);
            uint zero = table.Add("zero");
            uint one = table.Add("one");
            uint two = table.Add("two");
            uint three = table.Add("three");
            uint four = table.Add("four");
            uint five = table.Add("five");
            uint six = table.Add("six");

            Assert.AreEqual("zero", table.Get(0));
            Assert.AreEqual("one", table.Get(1));
            Assert.AreEqual("two", table.Get(2));
            Assert.AreEqual("three", table.Get(3));
            Assert.AreEqual("four", table.Get(4));
            Assert.AreEqual("five", table.Get(5));
            Assert.AreEqual("six", table.Get(6));

            Assert.AreEqual((uint)0, table.Add("zero"));
            Assert.AreEqual((uint)1, table.Add("one"));
            Assert.AreEqual((uint)2, table.Add("two"));
            Assert.AreEqual((uint)3, table.Add("three"));
            Assert.AreEqual((uint)4, table.Add("four"));
            Assert.AreEqual((uint)5, table.Add("five"));
            Assert.AreEqual((uint)6, table.Add("six"));

            Assert.AreEqual("zero", table.Get(0));
            Assert.AreEqual("one", table.Get(1));
            Assert.AreEqual("two", table.Get(2));
            Assert.AreEqual("three", table.Get(3));
            Assert.AreEqual("four", table.Get(4));
            Assert.AreEqual("five", table.Get(5));
            Assert.AreEqual("six", table.Get(6));

            table = new ObjectTable<string>(true);
            zero = table.Add("zero");
            one = table.Add("one");
            two = table.Add("two");
            three = table.Add("three");
            four = table.Add("four");
            five = table.Add("five");
            six = table.Add("six");

            Assert.AreEqual("zero", table.Get(0));
            Assert.AreEqual("one", table.Get(1));
            Assert.AreEqual("two", table.Get(2));
            Assert.AreEqual("three", table.Get(3));
            Assert.AreEqual("four", table.Get(4));
            Assert.AreEqual("five", table.Get(5));
            Assert.AreEqual("six", table.Get(6));

            Assert.AreEqual((uint)0, table.Add("zero"));
            Assert.AreEqual((uint)1, table.Add("one"));
            Assert.AreEqual((uint)2, table.Add("two"));
            Assert.AreEqual((uint)3, table.Add("three"));
            Assert.AreEqual((uint)4, table.Add("four"));
            Assert.AreEqual((uint)5, table.Add("five"));
            Assert.AreEqual((uint)6, table.Add("six"));

            Assert.AreEqual("zero", table.Get(0));
            Assert.AreEqual("one", table.Get(1));
            Assert.AreEqual("two", table.Get(2));
            Assert.AreEqual("three", table.Get(3));
            Assert.AreEqual("four", table.Get(4));
            Assert.AreEqual("five", table.Get(5));
            Assert.AreEqual("six", table.Get(6));
        }
    }
}