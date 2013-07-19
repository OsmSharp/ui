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
