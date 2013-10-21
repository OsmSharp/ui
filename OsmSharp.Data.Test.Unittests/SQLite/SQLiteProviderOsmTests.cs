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

using System.Data.SQLite;
using NUnit.Framework;
using OsmSharp.Data.SQLite.Osm;
using OsmSharp.Data.SQLite.Osm.Streams;
using OsmSharp.Osm.Data;
using OsmSharp.Osm.Streams;

namespace OsmSharp.Data.Test.Unittests.SQLite
{
    /// <summary>
    /// Contains database tests for SQLite and osm-data.
    /// </summary>
    [TestFixture]
    public class SQLiteProviderOsmTests : DataProviderOsmTests
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

        /// <summary>
        /// Tests a write to a sqlite database and gets ways using it's nodes.
        /// </summary>
        [Test]
        public void SQLiteTestGetWaysForNode()
        {
            base.TestGetWaysForNode();
        }

        /// <summary>
        /// Tests a write to a sqlite database and gets relations using it's members.
        /// </summary>
        [Test]
        public void SQLiteTestGetRelationsForMember()
        {
            base.TestGetRelationsForMember();
        }

        /// <summary>
        /// Tests a bounding box query.
        /// </summary>
        [Test]
        public void SQLiteTestBoundingBoxQueries()
        {
            base.TestBoundingBoxQueries();
        }

        #region SQLite Implementations

        private SQLiteConnection _connection = null;

        /// <summary>
        /// Notifies that the current test expects an empty database.
        /// </summary>
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

        /// <summary>
        /// Creates a data source for this test.
        /// </summary>
        /// <returns></returns>
        public override IDataSourceReadOnly CreateDataSource()
        {
            return new SQLiteDataSource(this.GetConnection());
        }

        /// <summary>
        /// Creates an osm stream target for this test.
        /// </summary>
        /// <returns></returns>
        public override OsmStreamTarget CreateDataStreamTarget()
        {
            return new SQLiteOsmStreamTarget(this.GetConnection());
        }

        #endregion
    }
}
