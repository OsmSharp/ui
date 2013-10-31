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

using System.IO;
using System.Linq;
using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Serializer;

namespace OsmSharp.Test.Unittests.Collections.Tags.Serializer
{
    /// <summary>
    /// Tests tag index serializer.
    /// </summary>
    [TestFixture]
    public class TagIndexSerializerTests
    {
        /// <summary>
        /// Tests a simple tag serialization.
        /// </summary>
        [Test]
        public void TestSimpleTagSerializaton()
        {
            SimpleTagsIndex tagsIndex = new SimpleTagsIndex();

            SimpleTagsCollection tagsCollection = new SimpleTagsCollection();
            tagsCollection.Add("key1", "value1");

            uint tagsId = tagsIndex.Add(tagsCollection);

            ITagsIndexReadonly tagsIndexReadonly = this.SerializeDeserialize(tagsIndex);
            Assert.AreEqual(tagsIndex.Max, tagsIndexReadonly.Max);
            for (uint idx = 0; idx < tagsIndex.Max; idx++)
            {
                this.CompareTagsCollections(tagsIndex.Get(idx),
                    tagsIndexReadonly.Get(idx));
            }
        }

        /// <summary>
        /// Tests a simple tag serialization but starting at a non-null position in a stream.
        /// </summary>
        [Test]
        public void TestSimpleTagSerializatonNonBeginPosition()
        {
            SimpleTagsIndex tagsIndex = new SimpleTagsIndex();

            SimpleTagsCollection tagsCollection = new SimpleTagsCollection();
            tagsCollection.Add("key1", "value1");

            uint tagsId = tagsIndex.Add(tagsCollection);

            ITagsIndexReadonly tagsIndexReadonly = this.SerializeDeserialize(tagsIndex, 1201);
            Assert.AreEqual(tagsIndex.Max, tagsIndexReadonly.Max);
            for (uint idx = 0; idx < tagsIndex.Max; idx++)
            {
                this.CompareTagsCollections(tagsIndex.Get(idx),
                    tagsIndexReadonly.Get(idx));
            }
        }

        /// <summary>
        /// Tests a simple tag serialization.
        /// </summary>
        [Test]
        public void TestRandomTagSerializaton()
        {
            SimpleTagsIndex tagsIndex = new SimpleTagsIndex();

            SimpleTagsCollection tagsCollection = new SimpleTagsCollection();
            for (int i = 0; i < 100; i++)
            {
                int tagCollectionSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(10) + 1;
                for (int idx = 0; idx < tagCollectionSize; idx++)
                {
                    int tagValue = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(10);
                    tagsCollection.Add(
                        string.Format("key_{0}", tagValue),
                        string.Format("value_{0}", tagValue));
                }
                uint tagsId = tagsIndex.Add(tagsCollection);
            }

            ITagsIndexReadonly tagsIndexReadonly = this.SerializeDeserialize(tagsIndex);
            Assert.AreEqual(tagsIndex.Max, tagsIndexReadonly.Max);
            for (uint idx = 0; idx < tagsIndex.Max; idx++)
            {
                this.CompareTagsCollections(tagsIndex.Get(idx),
                    tagsIndexReadonly.Get(idx));
            }
        }

        /// <summary>
        /// Tests a simple tag serialization.
        /// </summary>
        [Test]
        public void TestRandomTagSerializatonNonBeginPosition()
        {
            SimpleTagsIndex tagsIndex = new SimpleTagsIndex();

            SimpleTagsCollection tagsCollection = new SimpleTagsCollection();
            for (int i = 0; i < 100; i++)
            {
                int tagCollectionSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(10) + 1;
                for (int idx = 0; idx < tagCollectionSize; idx++)
                {
                    int tagValue = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(10);
                    tagsCollection.Add(
                        string.Format("key_{0}", tagValue),
                        string.Format("value_{0}", tagValue));
                }
                uint tagsId = tagsIndex.Add(tagsCollection);
            }

            ITagsIndexReadonly tagsIndexReadonly = this.SerializeDeserialize(tagsIndex, 1201);
            Assert.AreEqual(tagsIndex.Max, tagsIndexReadonly.Max);
            for (uint idx = 0; idx < tagsIndex.Max; idx++)
            {
                this.CompareTagsCollections(tagsIndex.Get(idx),
                    tagsIndexReadonly.Get(idx));
            }
        }

        /// <summary>
        /// Tests a partial tag serialization.
        /// </summary>
        [Test]
        public void TestRandomPartialTagSerializaton()
        {
            SimpleTagsIndex tagsIndex = new SimpleTagsIndex();

            SimpleTagsCollection tagsCollection = new SimpleTagsCollection();
            for (int i = 0; i < 100; i++)
            {
                int tagCollectionSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(10) + 1;
                for (int idx = 0; idx < tagCollectionSize; idx++)
                {
                    int tagValue = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(10);
                    tagsCollection.Add(
                        string.Format("key_{0}", tagValue),
                        string.Format("value_{0}", tagValue));
                }
                uint tagsId = tagsIndex.Add(tagsCollection);
            }

            uint from = 40;
            uint to = 50;

            ITagsIndexReadonly tagsIndexReadonly = this.SerializeDeserialize(tagsIndex, from, to);
            Assert.AreEqual(System.Math.Min(to, tagsIndex.Max), tagsIndexReadonly.Max);
            for (uint idx = 0; idx < tagsIndex.Max; idx++)
            {
                if (idx >= from && idx < to)
                {
                    this.CompareTagsCollections(tagsIndex.Get(idx),
                        tagsIndexReadonly.Get(idx));
                }
                else
                {
                    Assert.IsNull(tagsIndexReadonly.Get(idx));
                }
            }

            from = 0;
            to = 100;

            tagsIndexReadonly = this.SerializeDeserialize(tagsIndex, from, to);
            Assert.AreEqual(System.Math.Min(to, tagsIndex.Max), tagsIndexReadonly.Max);
            for (uint idx = 0; idx < tagsIndex.Max; idx++)
            {
                if (idx >= from && idx < to)
                {
                    this.CompareTagsCollections(tagsIndex.Get(idx),
                        tagsIndexReadonly.Get(idx));
                }
                else
                {
                    Assert.IsNull(tagsIndexReadonly.Get(idx));
                }
            }

            from = 10;
            to = 1000;

            tagsIndexReadonly = this.SerializeDeserialize(tagsIndex, from, to);
            Assert.AreEqual(System.Math.Min(to, tagsIndex.Max), tagsIndexReadonly.Max);
            for (uint idx = 0; idx < tagsIndex.Max; idx++)
            {
                if (idx >= from && idx < to)
                {
                    this.CompareTagsCollections(tagsIndex.Get(idx),
                        tagsIndexReadonly.Get(idx));
                }
                else
                {
                    Assert.IsNull(tagsIndexReadonly.Get(idx));
                }
            }
        }

        /// <summary>
        /// Tests a blocked tag serialization.
        /// </summary>
        [Test]
        public void TestRandomBlockTagSerializaton()
        {
            SimpleTagsIndex tagsIndex = new SimpleTagsIndex();

            SimpleTagsCollection tagsCollection = new SimpleTagsCollection();
            for (int i = 0; i < 100; i++)
            {
                int tagCollectionSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(10) + 1;
                for (int idx = 0; idx < tagCollectionSize; idx++)
                {
                    int tagValue = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(10);
                    tagsCollection.Add(
                        string.Format("key_{0}", tagValue),
                        string.Format("value_{0}", tagValue));
                }
                uint tagsId = tagsIndex.Add(tagsCollection);
            }

            ITagsIndexReadonly tagsIndexReadonly = this.SerializeDeserializeBlock(tagsIndex, 10, 0);
            Assert.AreEqual(tagsIndex.Max, tagsIndexReadonly.Max);
            for (uint idx = 0; idx < tagsIndex.Max; idx++)
            {
                this.CompareTagsCollections(tagsIndex.Get(idx),
                    tagsIndexReadonly.Get(idx));
            }
        }

        /// <summary>
        /// Tests a blocked tag serialization but not when the stream is at position zero.
        /// </summary>
        [Test]
        public void TestRandomBlockTagSerializatonNonBeginPosition()
        {
            SimpleTagsIndex tagsIndex = new SimpleTagsIndex();

            SimpleTagsCollection tagsCollection = new SimpleTagsCollection();
            for (int i = 0; i < 100; i++)
            {
                int tagCollectionSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(10) + 1;
                for (int idx = 0; idx < tagCollectionSize; idx++)
                {
                    int tagValue = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(10);
                    tagsCollection.Add(
                        string.Format("key_{0}", tagValue),
                        string.Format("value_{0}", tagValue));
                }
                uint tagsId = tagsIndex.Add(tagsCollection);
            }

            ITagsIndexReadonly tagsIndexReadonly = this.SerializeDeserializeBlock(tagsIndex, 10, 123);
            Assert.AreEqual(tagsIndex.Max, tagsIndexReadonly.Max);
            for (uint idx = 0; idx < tagsIndex.Max; idx++)
            {
                this.CompareTagsCollections(tagsIndex.Get(idx),
                    tagsIndexReadonly.Get(idx));
            }
        }

        /// <summary>
        /// Compares two tags collections.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="collection"></param>
        private void CompareTagsCollections(TagsCollection reference, TagsCollection collection)
        {
            Assert.AreEqual(reference.Count, collection.Count);
            foreach (Tag tag in reference)
            {
                Assert.IsTrue(collection.Contains(tag));
            }
        }

        /// <summary>
        /// Serialize/deserialize index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private ITagsIndexReadonly SerializeDeserialize(ITagsIndex index)
        {
            MemoryStream stream = new MemoryStream();
            TagIndexSerializer.Serialize(stream, index);
            stream.Seek(0, SeekOrigin.Begin);

            return TagIndexSerializer.Deserialize(stream);
        }

        /// <summary>
        /// Serialize/deserialize index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private ITagsIndexReadonly SerializeDeserialize(ITagsIndex index, int position)
        {
            MemoryStream stream = new MemoryStream();

            stream.Seek(position, SeekOrigin.Begin);
            TagIndexSerializer.Serialize(stream, index);

            stream.Seek(position, SeekOrigin.Begin);
            return TagIndexSerializer.Deserialize(stream);
        }

        /// <summary>
        /// Serialize/deserialize index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private ITagsIndexReadonly SerializeDeserialize(ITagsIndex index, uint from, uint to)
        {
            MemoryStream stream = new MemoryStream();
            TagIndexSerializer.Serialize(stream, index, from, to);
            stream.Seek(0, SeekOrigin.Begin);

            return TagIndexSerializer.Deserialize(stream);
        }

        /// <summary>
        /// Serialize/deserialize index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="blockSize"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private ITagsIndexReadonly SerializeDeserializeBlock(ITagsIndex index, uint blockSize, int position)
        {
            MemoryStream stream = new MemoryStream();

            stream.Seek(position, SeekOrigin.Begin);
            TagIndexSerializer.SerializeBlocks(stream, index, blockSize);

            stream.Seek(position, SeekOrigin.Begin);
            return TagIndexSerializer.DeserializeBlocks(stream);
        }
    }
}
