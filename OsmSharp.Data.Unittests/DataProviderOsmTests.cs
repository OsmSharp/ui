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
using OsmSharp.Osm.Data;
using OsmSharp.Osm.Data.Streams;
using OsmSharp.Osm.Simple;
using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Data.PBF.Processor;
using System.Reflection;
using OsmSharp.Osm.Data.Memory;

namespace OsmSharp.Data.Unittests
{
    /// <summary>
    /// Contains common test-code for data providers storing/loading osm-data.
    /// </summary>
    public abstract class DataProviderOsmTests
    {
        /// <summary>
        /// Notifies the current provider that an empty database is expected.
        /// </summary>
        public abstract void NotifyEmptyExpected();

        /// <summary>
        /// Creates a datasource for the current provider.
        /// </summary>
        /// <returns></returns>
        public abstract IDataSourceReadOnly CreateDataSource();

        /// <summary>
        /// Creates a data stream target for the current provider.
        /// </summary>
        /// <returns></returns>
        public abstract OsmStreamTarget CreateDataStreamTarget();

        /// <summary>
        /// Creates a data stream source for the current provider.
        /// </summary>
        /// <returns></returns>
        //public abstract OsmStreamSource CreateDataStreamSource();

        /// <summary>
        /// Tests read/writing a node.
        /// </summary>
        protected void TestNodeReadWrite()
        {
            this.NotifyEmptyExpected(); // empty test database.

            // create a test-node.
            Node node = new Node();
            node.Id = 1;
            node.Latitude = 51.0;
            node.Longitude = 4.0;

            // create a target, add the node, create a source and verify node in db.
            var target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddNode(node);
            target.Close();
            var dataSource = this.CreateDataSource();
            Node foundNode = dataSource.GetNode(1);
            this.CompareNodes(node, foundNode);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-node.
            node = new Node();
            node.Id = 1;
            node.Latitude = 51.0;
            node.Longitude = 4.0;
            node.UserName = "ben";

            // create a target, add the node, create a source and verify node in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddNode(node);
            target.Close();
            dataSource = this.CreateDataSource();
            foundNode = dataSource.GetNode(1);
            this.CompareNodes(node, foundNode);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-node.
            node = new Node();
            node.Id = 1;
            node.Latitude = 51.0;
            node.Longitude = 4.0;
            node.UserName = "ben";
            node.UserId = 10;

            // create a target, add the node, create a source and verify node in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddNode(node);
            target.Close();
            dataSource = this.CreateDataSource();
            foundNode = dataSource.GetNode(1);
            this.CompareNodes(node, foundNode);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-node.
            node = new Node();
            node.Id = 1;
            node.Latitude = 51.0;
            node.Longitude = 4.0;
            node.UserName = "ben";
            node.UserId = 10;
            node.Version = 1;

            // create a target, add the node, create a source and verify node in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddNode(node);
            target.Close();
            dataSource = this.CreateDataSource();
            foundNode = dataSource.GetNode(1);
            this.CompareNodes(node, foundNode);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-node.
            node = new Node();
            node.Id = 1;
            node.Latitude = 51.0;
            node.Longitude = 4.0;
            node.UserName = "ben";
            node.UserId = 10;
            node.Version = 1;
            node.Visible = true;

            // create a target, add the node, create a source and verify node in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddNode(node);
            target.Close();
            dataSource = this.CreateDataSource();
            foundNode = dataSource.GetNode(1);
            this.CompareNodes(node, foundNode);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-node.
            node = new Node();
            node.Id = 1;
            node.Latitude = 51.0;
            node.Longitude = 4.0;
            node.UserName = "ben";
            node.UserId = 10;
            node.Version = 1;
            node.Visible = true;
            node.TimeStamp = DateTime.Now.ToUnixTime().FromUnixTime(); // make sure the time is rounded.

            // create a target, add the node, create a source and verify node in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddNode(node);
            target.Close();
            dataSource = this.CreateDataSource();
            foundNode = dataSource.GetNode(1);
            this.CompareNodes(node, foundNode);


            this.NotifyEmptyExpected(); // empty test database.

            // create a test-node.
            node = new Node();
            node.Id = 1;
            node.Latitude = 51.0;
            node.Longitude = 4.0;
            node.UserName = "ben";
            node.UserId = 10;
            node.Version = 1;
            node.Visible = true;
            node.TimeStamp = DateTime.Now.ToUnixTime().FromUnixTime(); // make sure the time is rounded.
            node.Tags = new SimpleTagsCollection();
            node.Tags.Add("tag", "value");

            // create a target, add the node, create a source and verify node in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddNode(node);
            target.Close();
            dataSource = this.CreateDataSource();
            foundNode = dataSource.GetNode(1);
            this.CompareNodes(node, foundNode);
        }

        /// <summary>
        /// Tests read/writing a way.
        /// </summary>
        protected void TestWayReadWrite()
        {
            this.NotifyEmptyExpected(); // empty test database.

            // create a test-way.
            Way way = new Way();
            way.Id = 1;

            // create a target, add the way, create a source and verify way in db.
            var target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddWay(way);
            target.Close();
            var dataSource = this.CreateDataSource();
            Way foundWay = dataSource.GetWay(1);
            this.CompareWays(way, foundWay);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-way.
            way = new Way();
            way.Id = 1;
            way.UserName = "ben";

            // create a target, add the way, create a source and verify way in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddWay(way);
            target.Close();
            dataSource = this.CreateDataSource();
            foundWay = dataSource.GetWay(1);
            this.CompareWays(way, foundWay);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-way.
            way = new Way();
            way.Id = 1;
            way.UserName = "ben";
            way.UserId = 10;

            // create a target, add the way, create a source and verify way in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddWay(way);
            target.Close();
            dataSource = this.CreateDataSource();
            foundWay = dataSource.GetWay(1);
            this.CompareWays(way, foundWay);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-way.
            way = new Way();
            way.Id = 1;
            way.UserName = "ben";
            way.UserId = 10;
            way.Version = 1;

            // create a target, add the way, create a source and verify way in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddWay(way);
            target.Close();
            dataSource = this.CreateDataSource();
            foundWay = dataSource.GetWay(1);
            this.CompareWays(way, foundWay);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-way.
            way = new Way();
            way.Id = 1;
            way.UserName = "ben";
            way.UserId = 10;
            way.Version = 1;
            way.Visible = true;

            // create a target, add the way, create a source and verify way in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddWay(way);
            target.Close();
            dataSource = this.CreateDataSource();
            foundWay = dataSource.GetWay(1);
            this.CompareWays(way, foundWay);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-way.
            way = new Way();
            way.Id = 1;
            way.UserName = "ben";
            way.UserId = 10;
            way.Version = 1;
            way.Visible = true;
            way.TimeStamp = DateTime.Now.ToUnixTime().FromUnixTime(); // make sure the time is rounded.

            // create a target, add the way, create a source and verify way in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddWay(way);
            target.Close();
            dataSource = this.CreateDataSource();
            foundWay = dataSource.GetWay(1);
            this.CompareWays(way, foundWay);


            this.NotifyEmptyExpected(); // empty test database.

            // create a test-way.
            way = new Way();
            way.Id = 1;
            way.UserName = "ben";
            way.UserId = 10;
            way.Version = 1;
            way.Visible = true;
            way.TimeStamp = DateTime.Now.ToUnixTime().FromUnixTime(); // make sure the time is rounded.
            way.Tags = new SimpleTagsCollection();
            way.Tags.Add("tag", "value");

            // create a target, add the way, create a source and verify way in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddWay(way);
            target.Close();
            dataSource = this.CreateDataSource();
            foundWay = dataSource.GetWay(1);
            this.CompareWays(way, foundWay);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-way.
            way = new Way();
            way.Id = 1;
            way.UserName = "ben";
            way.UserId = 10;
            way.Version = 1;
            way.Visible = true;
            way.TimeStamp = DateTime.Now.ToUnixTime().FromUnixTime(); // make sure the time is rounded.
            way.Nodes = new List<long>();
            way.Nodes.Add(1);
            way.Nodes.Add(2);
            way.Nodes.Add(3);
            way.Nodes.Add(1);

            // create a target, add the way, create a source and verify way in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddWay(way);
            target.Close();
            dataSource = this.CreateDataSource();
            foundWay = dataSource.GetWay(1);
            this.CompareWays(way, foundWay);
        }

        /// <summary>
        /// Tests read/writing a relation.
        /// </summary>
        protected void TestRelationReadWrite()
        {
            this.NotifyEmptyExpected(); // empty test database.

            // create a test-relation.
            Relation relation = new Relation();
            relation.Id = 1;

            // create a target, add the relation, create a source and verify relation in db.
            var target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddRelation(relation);
            target.Close();
            var dataSource = this.CreateDataSource();
            Relation foundRelation = dataSource.GetRelation(1);
            this.CompareRelations(relation, foundRelation);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-relation.
            relation = new Relation();
            relation.Id = 1;
            relation.UserName = "ben";

            // create a target, add the relation, create a source and verify relation in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddRelation(relation);
            target.Close();
            dataSource = this.CreateDataSource();
            foundRelation = dataSource.GetRelation(1);
            this.CompareRelations(relation, foundRelation);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-relation.
            relation = new Relation();
            relation.Id = 1;
            relation.UserName = "ben";
            relation.UserId = 10;

            // create a target, add the relation, create a source and verify relation in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddRelation(relation);
            target.Close();
            dataSource = this.CreateDataSource();
            foundRelation = dataSource.GetRelation(1);
            this.CompareRelations(relation, foundRelation);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-relation.
            relation = new Relation();
            relation.Id = 1;
            relation.UserName = "ben";
            relation.UserId = 10;
            relation.Version = 1;

            // create a target, add the relation, create a source and verify relation in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddRelation(relation);
            target.Close();
            dataSource = this.CreateDataSource();
            foundRelation = dataSource.GetRelation(1);
            this.CompareRelations(relation, foundRelation);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-relation.
            relation = new Relation();
            relation.Id = 1;
            relation.UserName = "ben";
            relation.UserId = 10;
            relation.Version = 1;
            relation.Visible = true;

            // create a target, add the relation, create a source and verify relation in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddRelation(relation);
            target.Close();
            dataSource = this.CreateDataSource();
            foundRelation = dataSource.GetRelation(1);
            this.CompareRelations(relation, foundRelation);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-relation.
            relation = new Relation();
            relation.Id = 1;
            relation.UserName = "ben";
            relation.UserId = 10;
            relation.Version = 1;
            relation.Visible = true;
            relation.TimeStamp = DateTime.Now.ToUnixTime().FromUnixTime(); // make sure the time is rounded.

            // create a target, add the relation, create a source and verify relation in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddRelation(relation);
            target.Close();
            dataSource = this.CreateDataSource();
            foundRelation = dataSource.GetRelation(1);
            this.CompareRelations(relation, foundRelation);


            this.NotifyEmptyExpected(); // empty test database.

            // create a test-relation.
            relation = new Relation();
            relation.Id = 1;
            relation.UserName = "ben";
            relation.UserId = 10;
            relation.Version = 1;
            relation.Visible = true;
            relation.TimeStamp = DateTime.Now.ToUnixTime().FromUnixTime(); // make sure the time is rounded.
            relation.Tags = new SimpleTagsCollection();
            relation.Tags.Add("tag", "value");

            // create a target, add the relation, create a source and verify relation in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddRelation(relation);
            target.Close();
            dataSource = this.CreateDataSource();
            foundRelation = dataSource.GetRelation(1);
            this.CompareRelations(relation, foundRelation);

            this.NotifyEmptyExpected(); // empty test database.

            // create a test-relation.
            relation = new Relation();
            relation.Id = 1;
            relation.UserName = "ben";
            relation.UserId = 10;
            relation.Version = 1;
            relation.Visible = true;
            relation.TimeStamp = DateTime.Now.ToUnixTime().FromUnixTime(); // make sure the time is rounded.
            relation.Members = new List<RelationMember>();
            relation.Members.Add(new RelationMember() { MemberId = 1, MemberRole = "node1", MemberType = RelationMemberType.Node });
            relation.Members.Add(new RelationMember() { MemberId = 2, MemberRole = "node2", MemberType = RelationMemberType.Node });
            relation.Members.Add(new RelationMember() { MemberId = 1, MemberRole = "node1", MemberType = RelationMemberType.Node });
            relation.Members.Add(new RelationMember() { MemberId = 1, MemberRole = "way", MemberType = RelationMemberType.Way });

            // create a target, add the relation, create a source and verify relation in db.
            target = this.CreateDataStreamTarget();
            target.Initialize();
            target.AddRelation(relation);
            target.Close();
            dataSource = this.CreateDataSource();
            foundRelation = dataSource.GetRelation(1);
            this.CompareRelations(relation, foundRelation);
        }

        /// <summary>
        /// Tests read/write and actual data file.
        /// </summary>
        protected void TestReadWriteData()
        {
            this.NotifyEmptyExpected(); // empty test database.

            // create the target and pull the data from the test-file into the sqlite database.
            OsmStreamTarget target = this.CreateDataStreamTarget();
            PBFOsmStreamSource source = new PBFOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Data.Unittests.Data.Osm.test.osm.pbf"));
            target.RegisterSource(source);
            target.Pull();

            IDataSourceReadOnly dataSource = this.CreateDataSource();
            MemoryDataSource memorySource = MemoryDataSource.CreateFrom(source);
            foreach (Node node in memorySource.GetNodes())
            {
                Node dbNode = dataSource.GetNode(node.Id.Value);
                this.CompareNodes(node, dbNode);
            }
            foreach (Way way in memorySource.GetWays())
            {
                Way dbWay = dataSource.GetWay(way.Id.Value);
                this.CompareWays(way, dbWay);
            }
            foreach (Relation relation in memorySource.GetRelations())
            {
                Relation dbRelation = dataSource.GetRelation(relation.Id.Value);
                this.CompareRelations(relation, dbRelation);
            }
        }

        #region Comparison Methods

        /// <summary>
        /// Compares a found node to an expected node.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="found"></param>
        private void CompareNodes(Node expected, Node found)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(found);
            Assert.AreEqual(expected.Id, found.Id);
            Assert.AreEqual(expected.ChangeSetId, found.ChangeSetId);
            Assert.AreEqual((float)expected.Latitude, (float)found.Latitude);
            Assert.AreEqual((float)expected.Longitude, (float)found.Longitude);
            Assert.AreEqual(expected.TimeStamp, found.TimeStamp);
            Assert.AreEqual(expected.Type, found.Type);
            Assert.AreEqual(expected.UserId, found.UserId);
            Assert.AreEqual(expected.UserName, found.UserName);
            Assert.AreEqual(expected.Version, found.Version);
            Assert.AreEqual(expected.Visible, found.Visible);

            this.CompareTags(expected.Tags, found.Tags);
        }

        /// <summary>
        /// Compares a found way to an expected way.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="found"></param>
        private void CompareWays(Way expected, Way found)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(found);
            Assert.AreEqual(expected.Id, found.Id);
            Assert.AreEqual(expected.ChangeSetId, found.ChangeSetId);
            Assert.AreEqual(expected.TimeStamp, found.TimeStamp);
            Assert.AreEqual(expected.Type, found.Type);
            Assert.AreEqual(expected.UserId, found.UserId);
            Assert.AreEqual(expected.UserName, found.UserName);
            Assert.AreEqual(expected.Version, found.Version);
            Assert.AreEqual(expected.Visible, found.Visible);

            if (expected.Nodes == null)
            {
                Assert.IsNull(found.Nodes);
            }
            else
            {
                Assert.IsNotNull(found.Nodes);
                Assert.AreEqual(expected.Nodes.Count, found.Nodes.Count);
                for (int idx = 0; idx < expected.Nodes.Count; idx++)
                {
                    Assert.AreEqual(expected.Nodes[idx], found.Nodes[idx]);
                }
            }

            this.CompareTags(expected.Tags, found.Tags);
        }

        /// <summary>
        /// Compares a found relation to an expected relation.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="found"></param>
        private void CompareRelations(Relation expected, Relation found)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(found);
            Assert.AreEqual(expected.Id, found.Id);
            Assert.AreEqual(expected.ChangeSetId, found.ChangeSetId);
            Assert.AreEqual(expected.TimeStamp, found.TimeStamp);
            Assert.AreEqual(expected.Type, found.Type);
            Assert.AreEqual(expected.UserId, found.UserId);
            Assert.AreEqual(expected.UserName, found.UserName);
            Assert.AreEqual(expected.Version, found.Version);
            Assert.AreEqual(expected.Visible, found.Visible);

            if (expected.Members == null)
            {
                Assert.IsNull(found.Members);
            }
            else
            {
                Assert.IsNotNull(found.Members);
                Assert.AreEqual(expected.Members.Count, found.Members.Count);
                for (int idx = 0; idx < expected.Members.Count; idx++)
                {
                    Assert.AreEqual(expected.Members[idx].MemberId, found.Members[idx].MemberId);
                    Assert.AreEqual(expected.Members[idx].MemberRole, found.Members[idx].MemberRole);
                    Assert.AreEqual(expected.Members[idx].MemberType, found.Members[idx].MemberType);
                }
            }

            this.CompareTags(expected.Tags, found.Tags);
        }

        /// <summary>
        /// Compares two tag collection for indentical content.
        /// </summary>
        /// <param name="expectedTags"></param>
        /// <param name="foundTags"></param>
        private void CompareTags(TagsCollection expectedTags, TagsCollection foundTags)
        {
            if (expectedTags == null)
            {
                Assert.IsNull(foundTags);
            }
            else
            {
                Assert.AreEqual(expectedTags.Count, foundTags.Count);
                foreach (Tag expectedTag in expectedTags)
                {
                    Assert.IsTrue(foundTags.ContainsKeyValue(expectedTag.Key, expectedTag.Value));
                }
            }
        }

        #endregion
    }
}
