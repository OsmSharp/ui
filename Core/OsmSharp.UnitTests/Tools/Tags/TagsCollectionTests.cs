using NUnit.Framework;
using OsmSharp.Tools.Collections.Tags;

namespace OsmSharp.UnitTests.Tools.Tags
{
    /// <summary>
    /// Contains tags collection tests.
    /// </summary>
    public abstract class TagsCollectionTests
    {
        /// <summary>
        /// Creates a tags collection.
        /// </summary>
        /// <returns></returns>
        protected abstract TagsCollection CreateTagsCollection();

        /// <summary>
        /// Tests an empty tags collection.
        /// </summary>
        protected void TestTagsCollectionEmpty()
        {
            TagsCollection collection = new SimpleTagsCollection();

            Assert.AreEqual(0, collection.Count);
        }

        /// <summary>
        /// Tests an empty tags collection.
        /// </summary>
        protected void TestTagsCollectionSimple()
        {
            TagsCollection collection = new SimpleTagsCollection();

            collection["simple"] = "yes";

            Assert.IsTrue(collection.ContainsKey("simple"));
            Assert.IsTrue(collection.ContainsKeyValue("simple","yes"));
            Assert.AreEqual("yes", collection["simple"]);
            Assert.AreEqual(1, collection.Count);
        }
    }
}
