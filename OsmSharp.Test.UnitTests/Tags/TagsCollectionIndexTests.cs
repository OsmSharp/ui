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

using System.Collections.Generic;
using NUnit.Framework;
using OsmSharp.Collections.Tags;

namespace OsmSharp.Test.Unittests.Tags
{
    /// <summary>
    /// Abstract class containing tests for the tags collection index.
    /// </summary>
    public abstract class TagsCollectionIndexTests
    {
        /// <summary>
        /// Tests the given tags collection index.
        /// </summary>
        /// <param name="tagsCollectionIndex"></param>
        protected void TestTagsCollectionIndex(ITagsCollectionIndex tagsCollectionIndex)
        {
            Dictionary<uint, TagsCollection> addedTags = new Dictionary<uint, TagsCollection>();
            for (int i = 0; i < 100; i++)
            {
                SimpleTagsCollection tagsCollection = new SimpleTagsCollection();
                int tagCollectionSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(3) + 1;
                for (int idx = 0; idx < tagCollectionSize; idx++)
                {
                    int tagValue = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(3);
                    tagsCollection.Add(
                        string.Format("key_{0}", tagValue),
                        string.Format("value_{0}", tagValue));
                }
                int addCount = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(2) + 1;
                for (int idx = 0; idx < addCount; idx++)
                {
                    uint tagsId = tagsCollectionIndex.Add(tagsCollection);
                    addedTags[tagsId] = tagsCollection;

                    TagsCollection indexTags = tagsCollectionIndex.Get(tagsId);
                    Assert.AreEqual(tagsCollection.Count, indexTags.Count);
                    foreach (Tag tag in tagsCollection)
                    {
                        Assert.IsTrue(indexTags.ContainsKeyValue(tag.Key, tag.Value));
                    }
                }
            }

            // check the index.
            foreach (KeyValuePair<uint, TagsCollection> pair in addedTags)
            {
                TagsCollection indexTags = tagsCollectionIndex.Get(pair.Key);
                Assert.AreEqual(pair.Value.Count, indexTags.Count);
                foreach (Tag tag in pair.Value)
                {
                    Assert.IsTrue(indexTags.ContainsKeyValue(tag.Key, tag.Value));
                }
            }
        }
    }
}