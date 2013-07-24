// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ServiceStack.Redis;
using OsmSharp.Osm.Data;
using OsmSharp.Data.Redis.Osm;
using OsmSharp.Osm.Data.Streams;
using OsmSharp.Data.Redis.Osm.Streams;

namespace OsmSharp.Data.Unittests.Redis
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

        public override void NotifyEmptyExpected()
        {
            _client = null;
        }

        private RedisClient GetConnection()
        {
            if (_client == null)
            {
                _client = new RedisClient("10.0.0.11", 6379);
                _client.FlushDb();
            }
            return _client;
        }

        public override IDataSourceReadOnly CreateDataSource()
        {
            return new RedisDataSource(this.GetConnection());
        }

        public override OsmStreamTarget CreateDataStreamTarget()
        {
            return new RedisOsmStreamTarget(this.GetConnection());
        }

        #endregion
    }
}
