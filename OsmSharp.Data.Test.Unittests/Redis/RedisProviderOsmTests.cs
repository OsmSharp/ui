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
using OsmSharp.Data.Redis.Osm;
using OsmSharp.Data.Redis.Osm.Streams;
using OsmSharp.Osm.Data;
using OsmSharp.Osm.Streams;
using ServiceStack.Redis;

namespace OsmSharp.Data.Test.Unittests.Redis
{
    /// <summary>
    /// Contains database tests for Redis and osm-data.
    /// </summary>
    [TestFixture]
    public class RedisProviderOsmTests : DataProviderOsmTests
    {
        /// <summary>
        /// Tests read/write node.
        /// </summary>
        [Test]
        public void RedisTestNodeReadWrite()
        {
            base.TestNodeReadWrite();
        }

        /// <summary>
        /// Tests read/write way.
        /// </summary>
        [Test]
        public void RedisTestWayReadWrite()
        {
            base.TestWayReadWrite();
        }

        /// <summary>
        /// Tests read/write relation.
        /// </summary>
        [Test]
        public void RedisTestRelationReadWrite()
        {
            base.TestRelationReadWrite();
        }

        /// <summary>
        /// Tests a write to a Redis database and verifies all the data.
        /// </summary>
        [Test]
        public void RedisTestReadWriteData()
        {
            base.TestReadWriteData();
        }

        /// <summary>
        /// Tests a write to a Redis database and gets ways using it's nodes.
        /// </summary>
        [Test]
        public void RedisTestGetWaysForNode()
        {
            base.TestGetWaysForNode();
        }

        /// <summary>
        /// Tests a write to a Redis database and gets relations using it's members.
        /// </summary>
        [Test]
        public void RedisTestGetRelationsForMember()
        {
            base.TestGetRelationsForMember();
        }

        /// <summary>
        /// Tests a bounding box query.
        /// </summary>
        [Test]
        public void RedisTestBoundingBoxQueries()
        {
            base.TestBoundingBoxQueries();
        }

        #region Redis Implementations

        private RedisClient _client = null;

        /// <summary>
        /// Notifies that the current test expects an empty database.
        /// </summary>
        public override void NotifyEmptyExpected()
        {
            _client = null;
        }

        private RedisClient GetConnection()
        {
            if (_client == null)
            {
                _client = new RedisClient("TestDataLinux", 6379);
                _client.FlushDb();
            }
            return _client;
        }

        /// <summary>
        /// Creates a data source for this test.
        /// </summary>
        /// <returns></returns>
        public override IDataSourceReadOnly CreateDataSource()
        {
            return new RedisDataSource(this.GetConnection());
        }

        /// <summary>
        /// Creates a stream target for this test.
        /// </summary>
        /// <returns></returns>
        public override OsmStreamTarget CreateDataStreamTarget()
        {
            return new RedisOsmStreamTarget(this.GetConnection());
        }

        #endregion
    }
}
