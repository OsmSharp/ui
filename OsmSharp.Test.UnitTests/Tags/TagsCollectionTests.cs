// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using NUnit.Framework;
using OsmSharp.Collections.Tags;

namespace OsmSharp.Test.Unittests.Tags
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
