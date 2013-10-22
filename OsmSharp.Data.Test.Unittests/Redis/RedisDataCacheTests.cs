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
using OsmSharp.Data.Redis.Osm.Cache;
using OsmSharp.Test.Unittests.Osm.Cache;
using ServiceStack.Redis;

namespace OsmSharp.Data.Test.Unittests.Redis
{
    /// <summary>
    /// Contains tests for an osm data cache with a redis backend.
    /// </summary>
    [TestFixture]
    public class RedisDataCacheTests : OsmDataCacheTestsBase
    {      /// <summary>
        /// Tests simple node read/write.
        /// </summary>
        [Test]
        public void OsmDataCacheRedisNodeTest()
        {
            OsmDataCacheRedis cache = new OsmDataCacheRedis(this.GetConnection());
            base.DoOsmDataCacheTestNode(cache);
            cache.Dispose();
        }

        /// <summary>
        /// Tests simple way read/write.
        /// </summary>
        [Test]
        public void OsmDataCacheRedisWayTest()
        {
            OsmDataCacheRedis cache = new OsmDataCacheRedis(this.GetConnection());
            base.DoOsmDataCacheTestWay(cache);
            cache.Dispose();
        }

        /// <summary>
        /// Tests simple relation read/write.
        /// </summary>
        [Test]
        public void OsmDataCacheRedisRelationTest()
        {
            OsmDataCacheRedis cache = new OsmDataCacheRedis(this.GetConnection());
            base.DoOsmDataCacheTestRelation(cache);
            cache.Dispose();
        }

        /// <summary>
        /// Tests clear.
        /// </summary>
        [Test]
        public void OsmDataCacheRedisClearTest()
        {
            base.DoOsmDataCacheTestClear(new OsmDataCacheRedis(this.GetConnection()));
        }


        private RedisClient _client = null;

        /// <summary>
        /// Notifies that the current test expects an empty database.
        /// </summary>
        public void NotifyEmptyExpected()
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
    }
}
