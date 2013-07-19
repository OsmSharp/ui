using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Data;
using OsmSharp.Data.PostgreSQL.Osm;
using OsmSharp.Osm.Data.Streams;
using OsmSharp.Data.PostgreSQL.Osm.Streams;
using NUnit.Framework;
using Npgsql;

namespace OsmSharp.Data.Unittests.PostgreSQL
{
    /// <summary>
    /// Contains database tests for PostgreSQL and osm-data.
    /// </summary>
    [TestFixture]
    public class PostgreSQLProviderOsmTests : DataProviderOsmTests
    {
        /// <summary>
        /// Tests read/write node.
        /// </summary>
        [Test]
        public void PostgreSQLTestNodeReadWrite()
        {
            base.TestNodeReadWrite();
        }

        /// <summary>
        /// Tests read/write way.
        /// </summary>
        [Test]
        public void PostgreSQLTestWayReadWrite()
        {
            base.TestWayReadWrite();
        }

        /// <summary>
        /// Tests read/write relation.
        /// </summary>
        [Test]
        public void PostgreSQLTestRelationReadWrite()
        {
            base.TestRelationReadWrite();
        }

        /// <summary>
        /// Tests a write to a PostgreSQL database and verifies all the data.
        /// </summary>
        [Test]
        public void PostgreSQLTestReadWriteData()
        {
            base.TestReadWriteData();
        }

        #region PostgreSQL Implementations

        private NpgsqlConnection _connection = null;

        public override void NotifyEmptyExpected()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
            _connection = null;
        }

        private NpgsqlConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = new NpgsqlConnection("Server=10.0.0.11;Port=5432;Database=osm;User Id=osmsharp;Password=osmsharp;");
                _connection.Open();

                PostgreSQLSchemaTools.Drop(_connection);
            }
            return _connection;
        }

        public override IDataSourceReadOnly CreateDataSource()
        {
            return new PostgreSQLDataSource(this.GetConnection());
        }

        public override OsmStreamTarget CreateDataStreamTarget()
        {
            return new PostgreSQLOsmStreamTarget(this.GetConnection());
        }

        #endregion
    }
}
