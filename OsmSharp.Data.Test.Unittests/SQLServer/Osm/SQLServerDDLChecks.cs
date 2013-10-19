// OsmSharp - OpenStreetMap (OSM) SDK
//
// Copyright (C) 2013 Abelshausen Ben
//                    Alexander Sinitsyn
//                    Simon Hughes
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
using System.Data.SqlClient;
using System.Reflection;
using NUnit.Framework;
using OsmSharp.Data.SQLServer.Osm.SchemaTools;
using OsmSharp.Osm;
using OsmSharp.Osm.Xml.Streams;

namespace OsmSharp.Data.Test.Unittests.SQLServer.Osm
{
    /// <summary>
    /// Test class for SQL Server DDL
    /// </summary>
    [TestFixture]
    public class SQLServerDDLChecks
    {
        private SQLServerDDLChecksStreamTarget _testTarget;

        /// <summary>
        /// Setup the test fixture.
        /// </summary>
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            // Arrange
            const string connectionString = @"Server=TestDataWindows\SQLEXPRESS;Database=osmsharp;User Id=osmsharp;Password=osmsharp;";

            var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            var source = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Data.Test.Unittests.SQLServer.Data.ukraine1.osm"));
            //var source = new PBFDataProcessorSource(new FileInfo("C:\\great-britain-latest.osm.pbf").OpenRead());

            // Act
            _testTarget = new SQLServerDDLChecksStreamTarget(connectionString);
            _testTarget.RegisterSource(source);
            _testTarget.Pull();
        }

        /// <summary>
        /// Test the nodeusr.
        /// </summary>
        [Test]
        public void NodeUsr()
        {
            Console.WriteLine(_testTarget.NodeUsr);
            Assert.LessOrEqual(_testTarget.NodeUsr, SQLServerSchemaConstants.NodeUsr);
        }

        /// <summary>
        /// Test the node tags key.
        /// </summary>
        [Test]
        public void NodeTagsKey()
        {
            Console.WriteLine(_testTarget.NodeTagsKey);
            Assert.LessOrEqual(_testTarget.NodeTagsKey, SQLServerSchemaConstants.NodeTagsKey);
        }

        /// <summary>
        /// Tests the node tags value.
        /// </summary>
        [Test]
        public void NodeTagsValue()
        {
            Console.WriteLine(_testTarget.NodeTagsValue);
            Assert.LessOrEqual(_testTarget.NodeTagsValue, SQLServerSchemaConstants.NodeTagsValue);
        }

        /// <summary>
        /// Tests the way usr.
        /// </summary>
        [Test]
        public void WayUsr()
        {
            Console.WriteLine(_testTarget.WayUsr);
            Assert.LessOrEqual(_testTarget.WayUsr, SQLServerSchemaConstants.WayUsr);
        }

        /// <summary>
        /// Tests the way tags key.
        /// </summary>
        [Test]
        public void WayTagsKey()
        {
            Console.WriteLine(_testTarget.WayTagsKey);
            Assert.LessOrEqual(_testTarget.WayTagsKey, SQLServerSchemaConstants.WayTagsKey);
        }

        /// <summary>
        /// Tests the way tags value.
        /// </summary>
        [Test]
        public void WayTagsValue()
        {
            Console.WriteLine(_testTarget.WayTagsValue);
            Assert.LessOrEqual(_testTarget.WayTagsValue, SQLServerSchemaConstants.WayTagsValue);
        }

        /// <summary>
        /// Tests the relation usr.
        /// </summary>
        [Test]
        public void RelationUsr()
        {
            Console.WriteLine(_testTarget.RelationUsr);
            Assert.LessOrEqual(_testTarget.RelationUsr, SQLServerSchemaConstants.RelationUsr);
        }

        /// <summary>
        /// Tests the relation tags key.
        /// </summary>
        [Test]
        public void RelationTagsKey()
        {
            Console.WriteLine(_testTarget.RelationTagsKey);
            Assert.LessOrEqual(_testTarget.RelationTagsKey, SQLServerSchemaConstants.RelationTagsKey);
        }

        /// <summary>
        /// Tests the relaiton tags value.
        /// </summary>
        [Test]
        public void RelationTagsValue()
        {
            Console.WriteLine(_testTarget.RelationTagsValue);
            Assert.LessOrEqual(_testTarget.RelationTagsValue, SQLServerSchemaConstants.RelationTagsValue);
        }

        /// <summary>
        /// Tests the relation member role.
        /// </summary>
        [Test]
        public void RelationMemberRole()
        {
            Console.WriteLine(_testTarget.RelationMemberRole);
            Assert.LessOrEqual(_testTarget.RelationMemberRole, SQLServerSchemaConstants.RelationMemberRole);
        }

        /// <summary>
        /// Tests the relation member type string lenghts.
        /// </summary>
        [Test]
        public void RelationMemberTypeStringLengths()
        {
            Console.WriteLine(OsmGeoType.Node.ToString().Length);
            Assert.LessOrEqual(OsmGeoType.Node.ToString().Length, SQLServerSchemaConstants.RelationMemberType);

            Console.WriteLine(OsmGeoType.Relation.ToString().Length);
            Assert.LessOrEqual(OsmGeoType.Relation.ToString().Length, SQLServerSchemaConstants.RelationMemberType);

            Console.WriteLine(OsmGeoType.Way.ToString().Length);
            Assert.LessOrEqual(OsmGeoType.Way.ToString().Length, SQLServerSchemaConstants.RelationMemberType);
        }
    }
}
