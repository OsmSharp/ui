// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using OsmSharp.Osm.API;
using System.Collections.Generic;

namespace OsmSharp.Test.Unittests.Osm.API
{
    /// <summary>
    /// Some generic API tests against the development API instance: http://api06.dev.openstreetmap.org/
    /// </summary>
    /// <remarks>
    /// Make sure the API test-instance is online.
    /// </remarks>
    [TestFixture]
    public class APITests
    {
        /// <summary>
        /// Tries a simple connection test.
        /// </summary>
        [Test]
        public void APITestConnectionAndCapabilities()
        {
            // intialize the connection.
            var apiInstance = new APIConnection("http://api06.dev.openstreetmap.org/", 
                "osmsharp", "osmsharp");

            // get capabilities.
            APICapabilities apiCapabilities = apiInstance.GetCapabilities();

            // check result.
            Assert.AreEqual(0.25, apiCapabilities.AreaMaximum);
            Assert.AreEqual(0.6, apiCapabilities.VersionMinimum);
            Assert.AreEqual(0.6, apiCapabilities.VersionMaximum);
            Assert.AreEqual(5000, apiCapabilities.TracePointsPerPage);
            Assert.AreEqual(2000, apiCapabilities.WayNodesMaximum);
            Assert.AreEqual(300, apiCapabilities.TimeoutSeconds);
            Assert.AreEqual(50000, apiCapabilities.ChangeSetsMaximumElement);
        }

        /// <summary>
        /// Tries to open/close/get a changeset.
        /// </summary>
        [Test]
        public void APITestChangeSetOpenCloseGet()
        {
            // intialize the connection.
            var apiInstance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // opens the changeset.
            long changesetId = apiInstance.ChangeSetOpen("A test changeset!");

            // get the changeset.
            ChangeSetInfo simpleChangesetInfo = 
                apiInstance.ChangeSetGet(changesetId);

            Assert.IsTrue(simpleChangesetInfo.Open);
            Assert.IsFalse(simpleChangesetInfo.ClosedAt.HasValue);

            // closes the current changeset.
            apiInstance.ChangeSetClose();

            // get the same changeset again!
            simpleChangesetInfo =
                apiInstance.ChangeSetGet(changesetId);
            Assert.IsFalse(simpleChangesetInfo.Open);
            Assert.IsTrue(simpleChangesetInfo.ClosedAt.HasValue);
        }

        /// <summary>
        /// Tries a simple node creation/get.
        /// </summary>
        [Test]
        public void APITestNodeCreateGet()
        {
            // intialize the connection.
            var apiInstance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            long changesetId = apiInstance.ChangeSetOpen("Simple Node Creation Test");

            // initialize the node.
            var node = new Node();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new TagsCollection();
            node.Tags.Add("type", "testnode");
            node.Visible = true;

            // save the node.
            node = apiInstance.NodeCreate(node);

            // close the changeset.
            apiInstance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(node.Id.HasValue);

            // get the new node id.
            long nodeId = node.Id.Value;

            // get the node from the api.
            Node nodeAPI = apiInstance.NodeGet(node.Id.Value);
            Assert.AreEqual(node.Latitude, nodeAPI.Latitude);
            Assert.AreEqual(node.Longitude, nodeAPI.Longitude);
            Assert.AreEqual(node.Tags.Count, nodeAPI.Tags.Count);
            Assert.AreEqual(node.Visible, nodeAPI.Visible);
            Assert.IsTrue(nodeAPI.ChangeSetId.HasValue);
            Assert.AreEqual(changesetId, nodeAPI.ChangeSetId.Value);
        }

        /// <summary>
        /// Tries a simple node creation/get/update.
        /// </summary>
        [Test]
        public void APITestNodeCreateGetUpdate()
        {
            // intialize the connection.
            var apiInstance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            apiInstance.ChangeSetOpen("Simple Node Creation Test");

            // initialize the node.
            var node = new Node();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new TagsCollection();
            node.Tags.Add("type", "testnode");
            node.Visible = true;

            // save the node.
            node = apiInstance.NodeCreate(node);

            // close the changeset.
            apiInstance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(node.Id.HasValue);

            // get the new node id.
            long nodeId = node.Id.Value;

            // open new changeset.
            apiInstance.ChangeSetOpen("Simple Node Update Test");

            // get the node.
            Node apiNode = apiInstance.NodeGet(node.Id.Value);
            apiNode.Tags.Add("another_tag", "test adding a tag!");
            apiInstance.NodeUpdate(apiNode);

            // close the current changeset.
            apiInstance.ChangeSetClose();

            // get the api node.
            apiNode = apiInstance.NodeGet(node.Id.Value);

            Assert.AreEqual(2, apiNode.Tags.Count);
            Assert.IsTrue(apiNode.Tags.ContainsKey("another_tag"));
            Assert.AreEqual("test adding a tag!", apiNode.Tags["another_tag"]);
        }

        /// <summary>
        /// Tries a simple node creation/get/update.
        /// </summary>
        [Test]
        public void APITestNodeCreateGetDelete()
        {
            // intialize the connection.
            var apiInstance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            apiInstance.ChangeSetOpen("Simple Node Creation Test");

            // initialize the node.
            var node = new Node();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new TagsCollection();
            node.Tags.Add("type", "testnode");
            node.Visible = true;

            // save the node.
            node = apiInstance.NodeCreate(node);

            // close the changeset.
            apiInstance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(node.Id.HasValue);

            // get the new node id.
            long nodeId = node.Id.Value;

            // get the node again: a node can only be deleted using the correct changesetid and version.
            node = apiInstance.NodeGet(node.Id.Value);

            // open new changeset.
            apiInstance.ChangeSetOpen("Simple Node Delete Test");

            // get the node.
            apiInstance.NodeDelete(node);

            // close the current changeset.
            apiInstance.ChangeSetClose();

            // get the node.
            Node apiNode = apiInstance.NodeGet(node.Id.Value);
            Assert.IsNull(apiNode);
        }

        /// <summary>
        /// Tries a simple way creation/get.
        /// </summary>
        [Test]
        public void APITestWayCreateGet()
        {
            // intialize the connection.
            var apiInstance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            long changesetId = apiInstance.ChangeSetOpen("Simple Way Creation Test");

            // initialize the way.
            var way = new Way();
            way.Tags = new TagsCollection();
            way.Tags.Add("type", "testway");
            way.Nodes = new List<long>();
            way.Visible = true;

            // initialize the nodes.
            var node = new Node();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new TagsCollection();
            node.Tags.Add("type", "testnode1");
            node.Visible = true;
            node = apiInstance.NodeCreate(node);
            way.Nodes.Add(node.Id.Value);
            node = new Node();
            node.Latitude = -0.494497 + 0.0001f;
            node.Longitude = -24.119325 + 0.0001f;
            node.Tags = new TagsCollection();
            node.Tags.Add("type", "testnode2");
            node.Visible = true;
            node = apiInstance.NodeCreate(node);
            way.Nodes.Add(node.Id.Value);

            // save the way.
            way = apiInstance.WayCreate(way);

            // close the changeset.
            apiInstance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(way.Id.HasValue);

            // get the new way id.
            long wayId = way.Id.Value;

            // get the way from the api.
            Way wayAPI = apiInstance.WayGet(way.Id.Value);
            Assert.AreEqual(wayId, wayAPI.Id.Value);
            Assert.AreEqual(way.Tags.Count, wayAPI.Tags.Count);
            Assert.AreEqual(way.Visible, wayAPI.Visible);
            Assert.IsTrue(wayAPI.ChangeSetId.HasValue);
            Assert.AreEqual(changesetId, wayAPI.ChangeSetId.Value);
            Assert.AreEqual(way.Nodes[0], wayAPI.Nodes[0]);
            Assert.AreEqual(way.Nodes[1], wayAPI.Nodes[1]);
        }

        /// <summary>
        /// Tries a simple way creation/get/update.
        /// </summary>
        [Test]
        public void APITestWayCreateGetUpdate()
        {
            // intialize the connection.
            var apiInstance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            apiInstance.ChangeSetOpen("Simple Way Creation Test");

            // initialize the way.
            var way = new Way();
            way.Tags = new TagsCollection();
            way.Tags.Add("type", "testway");
            way.Nodes = new List<long>();
            way.Visible = true;

            // initialize the nodes.
            var node = new Node();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new TagsCollection();
            node.Tags.Add("type", "testnode1");
            node.Visible = true;
            node = apiInstance.NodeCreate(node);
            way.Nodes.Add(node.Id.Value);
            node = new Node();
            node.Latitude = -0.494497 + 0.0001f;
            node.Longitude = -24.119325 + 0.0001f;
            node.Tags = new TagsCollection();
            node.Tags.Add("type", "testnode2");
            node.Visible = true;
            node = apiInstance.NodeCreate(node);
            way.Nodes.Add(node.Id.Value);

            // save the way.
            way = apiInstance.WayCreate(way);

            // close the changeset.
            apiInstance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(way.Id.HasValue);

            // get the new way id.
            long wayId = way.Id.Value;

            // open new changeset.
            apiInstance.ChangeSetOpen("Simple Way Update Test");

            // get the way.
            Way apiWay = apiInstance.WayGet(way.Id.Value);
            apiWay.Tags.Add("another_tag", "test adding a tag!");
            apiInstance.WayUpdate(apiWay);

            // close the current changeset.
            apiInstance.ChangeSetClose();

            // get the api way.
            apiWay = apiInstance.WayGet(way.Id.Value);

            Assert.AreEqual(2, apiWay.Tags.Count);
            Assert.IsTrue(apiWay.Tags.ContainsKey("another_tag"));
            Assert.AreEqual("test adding a tag!", apiWay.Tags["another_tag"]);
        }

        /// <summary>
        /// Tries a simple way creation/get/update.
        /// </summary>
        [Test]
        public void APITestWayCreateGetDelete()
        {
            // intialize the connection.
            var apiInstance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            apiInstance.ChangeSetOpen("Simple Way Creation Test");

            // initialize the way.
            var way = new Way();
            way.Tags = new TagsCollection();
            way.Tags.Add("type", "testway");
            way.Nodes = new List<long>();
            way.Visible = true;

            // initialize the nodes.
            var node = new Node();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new TagsCollection();
            node.Tags.Add("type", "testnode1");
            node.Visible = true;
            node = apiInstance.NodeCreate(node);
            way.Nodes.Add(node.Id.Value);
            node = new Node();
            node.Latitude = -0.494497 + 0.0001f;
            node.Longitude = -24.119325 + 0.0001f;
            node.Tags = new TagsCollection();
            node.Tags.Add("type", "testnode2");
            node.Visible = true;
            node = apiInstance.NodeCreate(node);
            way.Nodes.Add(node.Id.Value);

            // save the way.
            way = apiInstance.WayCreate(way);

            // close the changeset.
            apiInstance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(way.Id.HasValue);

            // get the new way id.
            long wayId = way.Id.Value;

            // get the way again: a way can only be deleted using the correct changesetid and version.
            way = apiInstance.WayGet(way.Id.Value);

            // open new changeset.
            apiInstance.ChangeSetOpen("Simple Way Delete Test");

            // get the way.
            apiInstance.WayDelete(way);

            // close the current changeset.
            apiInstance.ChangeSetClose();

            // get the way.
            Way apiWay = apiInstance.WayGet(way.Id.Value);
            Assert.IsNull(apiWay);
        }


        /// <summary>
        /// Tries a simple relation creation/get.
        /// </summary>
        [Test]
        public void APITestRelationCreateGet()
        {
            // intialize the connection.
            var apiInstance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            long changesetId = apiInstance.ChangeSetOpen("Simple Relation Creation Test");

            // initialize the relation.
            var relation = new Relation();
            relation.Tags = new TagsCollection();
            relation.Tags.Add("type", "testrelation");
            relation.Members = new List<RelationMember>();
            relation.Visible = true;

            // initialize the nodes.
            var node = new Node();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new TagsCollection();
            node.Tags.Add("type", "testnode1");
            node.Visible = true;
            node = apiInstance.NodeCreate(node);
            relation.Members.Add(new RelationMember()
            {
                MemberId = node.Id.Value,
                MemberRole = "some_nodes_role",
                MemberType = OsmGeoType.Node
            });
            node = new Node();
            node.Latitude = -0.494497 + 0.0001f;
            node.Longitude = -24.119325 + 0.0001f;
            node.Tags = new TagsCollection();
            node.Tags.Add("type", "testnode2");
            node.Visible = true;
            node = apiInstance.NodeCreate(node);
            relation.Members.Add(new RelationMember()
            {
                MemberId = node.Id.Value,
                MemberRole = "some_nodes_role",
                MemberType = OsmGeoType.Node
            });

            // save the relation.
            relation = apiInstance.RelationCreate(relation);

            // close the changeset.
            apiInstance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(relation.Id.HasValue);

            // get the new relation id.
            long relationId = relation.Id.Value;

            // get the relation from the api.
            Relation relationAPI = apiInstance.RelationGet(relation.Id.Value);
            Assert.AreEqual(relationId, relationAPI.Id.Value);
            Assert.AreEqual(relation.Tags.Count, relationAPI.Tags.Count);
            Assert.AreEqual(relation.Visible, relationAPI.Visible);
            Assert.IsTrue(relationAPI.ChangeSetId.HasValue);
            Assert.AreEqual(changesetId, relationAPI.ChangeSetId.Value);
            Assert.AreEqual(relation.Members[0].MemberId, relationAPI.Members[0].MemberId);
            Assert.AreEqual(relation.Members[0].MemberRole, relationAPI.Members[0].MemberRole);
            Assert.AreEqual(relation.Members[0].MemberType, relationAPI.Members[0].MemberType);
            Assert.AreEqual(relation.Members[1].MemberId, relationAPI.Members[1].MemberId);
            Assert.AreEqual(relation.Members[1].MemberRole, relationAPI.Members[1].MemberRole);
            Assert.AreEqual(relation.Members[1].MemberType, relationAPI.Members[1].MemberType);
        }

        /// <summary>
        /// Tries a simple relation creation/get/update.
        /// </summary>
        [Test]
        public void APITestRelationCreateGetUpdate()
        {
            // intialize the connection.
            var apiInstance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            apiInstance.ChangeSetOpen("Simple Relation Creation Test");

            // initialize the relation.
            var relation = new Relation();
            relation.Tags = new TagsCollection();
            relation.Tags.Add("type", "testrelation");
            relation.Members = new List<RelationMember>();
            relation.Visible = true;

            // initialize the nodes.
            var node = new Node();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new TagsCollection();
            node.Tags.Add("type", "testnode1");
            node.Visible = true;
            node = apiInstance.NodeCreate(node);
            relation.Members.Add(new RelationMember()
            {
                MemberId = node.Id.Value,
                MemberRole = "some_nodes_role",
                MemberType = OsmGeoType.Node
            });
            node = new Node();
            node.Latitude = -0.494497 + 0.0001f;
            node.Longitude = -24.119325 + 0.0001f;
            node.Tags = new TagsCollection();
            node.Tags.Add("type", "testnode2");
            node.Visible = true;
            node = apiInstance.NodeCreate(node);
            relation.Members.Add(new RelationMember()
            {
                MemberId = node.Id.Value,
                MemberRole = "some_nodes_role",
                MemberType = OsmGeoType.Node
            });

            // save the relation.
            relation = apiInstance.RelationCreate(relation);

            // close the changeset.
            apiInstance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(relation.Id.HasValue);

            // get the new relation id.
            long relationId = relation.Id.Value;

            // open new changeset.
            apiInstance.ChangeSetOpen("Simple Relation Update Test");

            // get the relation.
            Relation apiRelation = apiInstance.RelationGet(relation.Id.Value);
            apiRelation.Tags.Add("another_tag", "test adding a tag!");
            apiInstance.RelationUpdate(apiRelation);

            // close the current changeset.
            apiInstance.ChangeSetClose();

            // get the api relation.
            apiRelation = apiInstance.RelationGet(relation.Id.Value);

            Assert.AreEqual(2, apiRelation.Tags.Count);
            Assert.IsTrue(apiRelation.Tags.ContainsKey("another_tag"));
            Assert.AreEqual("test adding a tag!", apiRelation.Tags["another_tag"]);
        }

        /// <summary>
        /// Tries a simple relation creation/get/update.
        /// </summary>
        [Test]
        public void APITestRelationCreateGetDelete()
        {
            // intialize the connection.
            var apiInstance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            apiInstance.ChangeSetOpen("Simple Relation Creation Test");

            // initialize the relation.
            var relation = new Relation();
            relation.Tags = new TagsCollection();
            relation.Tags.Add("type", "testrelation");
            relation.Members = new List<RelationMember>();
            relation.Visible = true;

            // initialize the nodes.
            var node = new Node();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new TagsCollection();
            node.Tags.Add("type", "testnode1");
            node.Visible = true;
            node = apiInstance.NodeCreate(node);
            relation.Members.Add(new RelationMember()
            {
                MemberId = node.Id.Value,
                MemberRole = "some_nodes_role",
                MemberType = OsmGeoType.Node
            });
            node = new Node();
            node.Latitude = -0.494497 + 0.0001f;
            node.Longitude = -24.119325 + 0.0001f;
            node.Tags = new TagsCollection();
            node.Tags.Add("type", "testnode2");
            node.Visible = true;
            node = apiInstance.NodeCreate(node);
            relation.Members.Add(new RelationMember()
            {
                MemberId = node.Id.Value,
                MemberRole = "some_nodes_role",
                MemberType = OsmGeoType.Node
            });

            // save the relation.
            relation = apiInstance.RelationCreate(relation);

            // close the changeset.
            apiInstance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(relation.Id.HasValue);

            // get the new relation id.
            long relationId = relation.Id.Value;

            // get the relation again: a relation can only be deleted using the correct changesetid and version.
            relation = apiInstance.RelationGet(relation.Id.Value);

            // open new changeset.
            apiInstance.ChangeSetOpen("Simple Relation Delete Test");

            // get the relation.
            apiInstance.RelationDelete(relation);

            // close the current changeset.
            apiInstance.ChangeSetClose();

            // get the relation.
            Relation apiRelation = apiInstance.RelationGet(relation.Id.Value);
            Assert.IsNull(apiRelation);
        }

        /// <summary>
        /// Tries to delete everything inside a boundingbox.
        /// </summary>
        [Test]
        public void APITestDeleteBoundingBox()
        {
            // intialize the connection.
            var apiInstance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");
            
            // get all objects in the box.
            var box = new GeoCoordinateBox(new GeoCoordinate(-0.494497, -24.119325),
                new GeoCoordinate(-0.494497, -24.119325));
            box = box.Resize(0.001f); // create a box around the usual test coordinates.
            List<OsmGeo> boxObjects = apiInstance.BoundingBoxGet(box);

            // delete all objects.
            apiInstance.ChangeSetOpen("delete test objects again");

            foreach (OsmGeo geoObject in boxObjects)
            {
                switch(geoObject.Type)
                {
                    case OsmGeoType.Relation:
                        apiInstance.RelationDelete(geoObject as Relation);
                        break;
                }
            }

            foreach (OsmGeo geoObject in boxObjects)
            {
                switch (geoObject.Type)
                {
                    case OsmGeoType.Way:
                        apiInstance.WayDelete(geoObject as Way);
                        break;
                }
            }

            foreach (OsmGeo geoObject in boxObjects)
            {
                switch (geoObject.Type)
                {
                    case OsmGeoType.Node:
                        apiInstance.NodeDelete(geoObject as Node);
                        break;
                }
            }
        }
    }
}