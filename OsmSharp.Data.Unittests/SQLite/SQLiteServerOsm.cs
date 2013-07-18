using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Data.SQLite.Osm.Streams;
using OsmSharp.Osm.Data.PBF.Processor;
using System.Reflection;
using OsmSharp.Data.SQLite.Osm;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.Osm.Simple;
using OsmSharp.Collections.Tags;
using System.Data.SQLite;
using OsmSharp.Osm.Data;
using OsmSharp.Osm.Data.Streams;

namespace OsmSharp.Data.Unittests.SQLite
{
    /// <summary>
    /// Contains database tests for SQLite and osm-data.
    /// </summary>
    [TestFixture]
    public class SQLiteServerOsm : DataProviderOsmTests
    {
        /// <summary>
        /// Tests read/write node.
        /// </summary>
        [Test]
        public void SQLiteTestNodeReadWrite()
        {
            base.TestNodeReadWrite();
        }

        /// <summary>
        /// Tests read/write way.
        /// </summary>
        [Test]
        public void SQLiteTestWayReadWrite()
        {
            base.TestWayReadWrite();
        }

        /// <summary>
        /// Tests read/write relation.
        /// </summary>
        [Test]
        public void SQLiteTestRelationReadWrite()
        {
            base.TestRelationReadWrite();
        }

        /// <summary>
        /// Tests a write to a sqlite database and verifies all the data.
        /// </summary>
        [Test]
        public void SQLiteTestReadWriteData()
        {
            base.TestReadWriteData();
        }

        #region SQLite Implementations

        private SQLiteConnection _connection = null;

        public override void NotifyEmptyExpected()
        {
            _connection = null;
        }

        private SQLiteConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = new SQLiteConnection("Data Source=:memory:;Version=3;New=True;");
            }
            return _connection;
        }

        public override IDataSourceReadOnly CreateDataSource()
        {
            return new SQLiteDataSource(this.GetConnection());
        }

        //public override OsmStreamSource CreateDataStreamSource()
        //{
        //    return new SQLiteOsmStreamSource(this.GetConnection());
        //}

        public override OsmStreamTarget CreateDataStreamTarget()
        {
            return new SQLiteOsmStreamTarget(this.GetConnection());
        }

        #endregion
    }
}
