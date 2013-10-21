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
using Oracle.ManagedDataAccess.Client;
using OsmSharp.Data.Oracle.Osm;
using OsmSharp.Data.Oracle.Osm.Streams;
using OsmSharp.Osm.Data;
using OsmSharp.Osm.Streams;

namespace OsmSharp.Data.Test.Unittests.Oracle
{
    /// <summary>
    /// Contains database tests for Oracle and osm-data.
    /// </summary>
    [TestFixture]
    public class OracleProviderOsmTests : DataProviderOsmTests
    {
        /// <summary>
        /// Tests read/write node.
        /// </summary>
        [Test]
        public void OracleTestNodeReadWrite()
        {
            base.TestNodeReadWrite();
        }

        /// <summary>
        /// Tests read/write way.
        /// </summary>
        [Test]
        public void OracleTestWayReadWrite()
        {
            base.TestWayReadWrite();
        }

        /// <summary>
        /// Tests read/write relation.
        /// </summary>
        [Test]
        public void OracleTestRelationReadWrite()
        {
            base.TestRelationReadWrite();
        }

        /// <summary>
        /// Tests a write to a Oracle database and verifies all the data.
        /// </summary>
        [Test]
        public void OracleTestReadWriteData()
        {
            base.TestReadWriteData();
        }

        /// <summary>
        /// Tests a write to a Oracle database and gets ways using it's nodes.
        /// </summary>
        [Test]
        public void OracleTestGetWaysForNode()
        {
            base.TestGetWaysForNode();
        }

        /// <summary>
        /// Tests a write to a Oracle database and gets relations using it's members.
        /// </summary>
        [Test]
        public void OracleTestGetRelationsForMember()
        {
            base.TestGetRelationsForMember();
        }

        /// <summary>
        /// Tests a bounding box query.
        /// </summary>
        [Test]
        public void OracleTestBoundingBoxQueries()
        {
            base.TestBoundingBoxQueries();
        }

        #region Oracle Implementations

        private OracleConnection _connection = null;

        /// <summary>
        /// Notifies that the current test expects an empty database.
        /// </summary>
        public override void NotifyEmptyExpected()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
            _connection = null;
        }

        private OracleConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=TestDataWindows)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=xe)));" + 
                                                    "User Id=osmsharp;Password=osmsharp;");
                _connection.Open();

                OracleSchemaTools.Drop(_connection);
            }
            return _connection;
        }

        /// <summary>
        /// Creates a data source for this test.
        /// </summary>
        /// <returns></returns>
        public override IDataSourceReadOnly CreateDataSource()
        {
            return new OracleDataSource(this.GetConnection());
        }

        /// <summary>
        /// Creates a stream target for this test.
        /// </summary>
        /// <returns></returns>
        public override OsmStreamTarget CreateDataStreamTarget()
        {
            return new OracleOsmStreamTarget(this.GetConnection(), true);
        }

        #endregion
    }
}