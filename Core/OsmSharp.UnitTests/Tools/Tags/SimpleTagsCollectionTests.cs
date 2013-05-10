using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Collections.Tags;

namespace OsmSharp.UnitTests.Tools.Tags
{
    /// <summary>
    /// Contains tests for the SimpleTagsCollection.
    /// </summary>
    [TestFixture]
    public class SimpleTagsCollectionTests : TagsCollectionTests
    {
        /// <summary>
        /// Creates a test tags collection.
        /// </summary>
        /// <returns></returns>
        protected override TagsCollection CreateTagsCollection()
        {
            return new SimpleTagsCollection();
        }

        /// <summary>
        /// Tests a simple tags collection.
        /// </summary>
        [Test]
        public void TestSimpleTagsCollectionEmpty()
        {
            this.TestTagsCollectionEmpty();
        }

        /// <summary>
        /// Tests a simple tags collection.
        /// </summary>
        [Test]
        public void TestSimpleTagsCollectionSimple()
        {
            this.TestTagsCollectionSimple();
        }
    }
}