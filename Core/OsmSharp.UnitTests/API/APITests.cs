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
using OsmSharp.Osm.Data.Core.API;
using OsmSharp.Osm.Simple;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.UnitTests.API
{
    /// <summary>
    /// Some generic API tests against the development API instance: http://api06.dev.openstreetmap.org/
    /// </summary>
    /// <remarks>
    /// Make sure the API test-instance is online and 
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
            APIConnection api_instance = new APIConnection("http://api06.dev.openstreetmap.org/", 
                "osmsharp", "osmsharp");

            // get capabilities.
            APICapabilities api_capabilities = api_instance.GetCapabilities();

            // check result.
            Assert.AreEqual(0.25, api_capabilities.AreaMaximum);
            Assert.AreEqual(0.6, api_capabilities.VersionMinimum);
            Assert.AreEqual(0.6, api_capabilities.VersionMaximum);
            Assert.AreEqual(5000, api_capabilities.TracePointsPerPage);
            Assert.AreEqual(2000, api_capabilities.WayNodesMaximum);
            Assert.AreEqual(300, api_capabilities.TimeoutSeconds);
            Assert.AreEqual(50000, api_capabilities.ChangeSetsMaximumElement);
        }

        /// <summary>
        /// Tries to open/close/get a changeset.
        /// </summary>
        [Test]
        public void APITestChangeSetOpenCloseGet()
        {
            // intialize the connection.
            APIConnection api_instance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // opens the changeset.
            long changeset_id = api_instance.ChangeSetOpen("A test changeset!");

            // get the changeset.
            SimpleChangeSetInfo simple_changeset_info = 
                api_instance.ChangeSetGet(changeset_id);

            Assert.IsTrue(simple_changeset_info.Open);
            Assert.IsFalse(simple_changeset_info.ClosedAt.HasValue);

            // closes the current changeset.
            api_instance.ChangeSetClose();

            // get the same changeset again!
            simple_changeset_info =
                api_instance.ChangeSetGet(changeset_id);
            Assert.IsFalse(simple_changeset_info.Open);
            Assert.IsTrue(simple_changeset_info.ClosedAt.HasValue);
        }

        /// <summary>
        /// Tries a simple node creation/get.
        /// </summary>
        [Test]
        public void APITestNodeCreateGet()
        {
            // intialize the connection.
            APIConnection api_instance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            long changeset_id = api_instance.ChangeSetOpen("Simple Node Creation Test");

            // initialize the node.
            SimpleNode node = new SimpleNode();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new Dictionary<string, string>();
            node.Tags.Add("type", "testnode");
            node.Visible = true;

            // save the node.
            node = api_instance.NodeCreate(node);

            // close the changeset.
            api_instance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(node.Id.HasValue);

            // get the new node id.
            long node_id = node.Id.Value;

            // get the node from the api.
            SimpleNode node_api = api_instance.NodeGet(node.Id.Value);
            Assert.AreEqual(node.Latitude, node_api.Latitude);
            Assert.AreEqual(node.Longitude, node_api.Longitude);
            Assert.AreEqual(node.Tags.Count, node_api.Tags.Count);
            Assert.AreEqual(node.Visible, node_api.Visible);
            Assert.IsTrue(node_api.ChangeSetId.HasValue);
            Assert.AreEqual(changeset_id, node_api.ChangeSetId.Value);
        }

        /// <summary>
        /// Tries a simple node creation/get/update.
        /// </summary>
        [Test]
        public void APITestNodeCreateGetUpdate()
        {
            // intialize the connection.
            APIConnection api_instance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            long changeset_id = api_instance.ChangeSetOpen("Simple Node Creation Test");

            // initialize the node.
            SimpleNode node = new SimpleNode();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new Dictionary<string, string>();
            node.Tags.Add("type", "testnode");
            node.Visible = true;

            // save the node.
            node = api_instance.NodeCreate(node);

            // close the changeset.
            api_instance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(node.Id.HasValue);

            // get the new node id.
            long node_id = node.Id.Value;

            // open new changeset.
            changeset_id = api_instance.ChangeSetOpen("Simple Node Update Test");

            // get the node.
            SimpleNode api_node = api_instance.NodeGet(node.Id.Value);
            api_node.Tags.Add("another_tag", "test adding a tag!");
            api_instance.NodeUpdate(api_node);

            // close the current changeset.
            api_instance.ChangeSetClose();

            // get the api node.
            api_node = api_instance.NodeGet(node.Id.Value);

            Assert.AreEqual(2, api_node.Tags.Count);
            Assert.IsTrue(api_node.Tags.ContainsKey("another_tag"));
            Assert.AreEqual("test adding a tag!", api_node.Tags["another_tag"]);
        }

        /// <summary>
        /// Tries a simple node creation/get/update.
        /// </summary>
        [Test]
        public void APITestNodeCreateGetDelete()
        {
            // intialize the connection.
            APIConnection api_instance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            long changeset_id = api_instance.ChangeSetOpen("Simple Node Creation Test");

            // initialize the node.
            SimpleNode node = new SimpleNode();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new Dictionary<string, string>();
            node.Tags.Add("type", "testnode");
            node.Visible = true;

            // save the node.
            node = api_instance.NodeCreate(node);

            // close the changeset.
            api_instance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(node.Id.HasValue);

            // get the new node id.
            long node_id = node.Id.Value;

            // get the node again: a node can only be deleted using the correct changesetid and version.
            node = api_instance.NodeGet(node.Id.Value);

            // open new changeset.
            changeset_id = api_instance.ChangeSetOpen("Simple Node Delete Test");

            // get the node.
            api_instance.NodeDelete(node);

            // close the current changeset.
            api_instance.ChangeSetClose();

            // get the node.
            SimpleNode api_node = api_instance.NodeGet(node.Id.Value);
            Assert.IsNull(api_node);
        }

        /// <summary>
        /// Tries a simple way creation/get.
        /// </summary>
        [Test]
        public void APITestWayCreateGet()
        {
            // intialize the connection.
            APIConnection api_instance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            long changeset_id = api_instance.ChangeSetOpen("Simple Way Creation Test");

            // initialize the way.
            SimpleWay way = new SimpleWay();
            way.Tags = new Dictionary<string, string>();
            way.Tags.Add("type", "testway");
            way.Nodes = new List<long>();
            way.Visible = true;

            // initialize the nodes.
            SimpleNode node = new SimpleNode();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new Dictionary<string, string>();
            node.Tags.Add("type", "testnode1");
            node.Visible = true;
            node = api_instance.NodeCreate(node);
            way.Nodes.Add(node.Id.Value);
            node = new SimpleNode();
            node.Latitude = -0.494497 + 0.0001f;
            node.Longitude = -24.119325 + 0.0001f;
            node.Tags = new Dictionary<string, string>();
            node.Tags.Add("type", "testnode2");
            node.Visible = true;
            node = api_instance.NodeCreate(node);
            way.Nodes.Add(node.Id.Value);

            // save the way.
            way = api_instance.WayCreate(way);

            // close the changeset.
            api_instance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(way.Id.HasValue);

            // get the new way id.
            long way_id = way.Id.Value;

            // get the way from the api.
            SimpleWay way_api = api_instance.WayGet(way.Id.Value);
            Assert.AreEqual(way_id, way_api.Id.Value);
            Assert.AreEqual(way.Tags.Count, way_api.Tags.Count);
            Assert.AreEqual(way.Visible, way_api.Visible);
            Assert.IsTrue(way_api.ChangeSetId.HasValue);
            Assert.AreEqual(changeset_id, way_api.ChangeSetId.Value);
            Assert.AreEqual(way.Nodes[0], way_api.Nodes[0]);
            Assert.AreEqual(way.Nodes[1], way_api.Nodes[1]);
        }

        /// <summary>
        /// Tries a simple way creation/get/update.
        /// </summary>
        [Test]
        public void APITestWayCreateGetUpdate()
        {
            // intialize the connection.
            APIConnection api_instance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            long changeset_id = api_instance.ChangeSetOpen("Simple Way Creation Test");

            // initialize the way.
            SimpleWay way = new SimpleWay();
            way.Tags = new Dictionary<string, string>();
            way.Tags.Add("type", "testway");
            way.Nodes = new List<long>();
            way.Visible = true;

            // initialize the nodes.
            SimpleNode node = new SimpleNode();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new Dictionary<string, string>();
            node.Tags.Add("type", "testnode1");
            node.Visible = true;
            node = api_instance.NodeCreate(node);
            way.Nodes.Add(node.Id.Value);
            node = new SimpleNode();
            node.Latitude = -0.494497 + 0.0001f;
            node.Longitude = -24.119325 + 0.0001f;
            node.Tags = new Dictionary<string, string>();
            node.Tags.Add("type", "testnode2");
            node.Visible = true;
            node = api_instance.NodeCreate(node);
            way.Nodes.Add(node.Id.Value);

            // save the way.
            way = api_instance.WayCreate(way);

            // close the changeset.
            api_instance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(way.Id.HasValue);

            // get the new way id.
            long way_id = way.Id.Value;

            // open new changeset.
            changeset_id = api_instance.ChangeSetOpen("Simple Way Update Test");

            // get the way.
            SimpleWay api_way = api_instance.WayGet(way.Id.Value);
            api_way.Tags.Add("another_tag", "test adding a tag!");
            api_instance.WayUpdate(api_way);

            // close the current changeset.
            api_instance.ChangeSetClose();

            // get the api way.
            api_way = api_instance.WayGet(way.Id.Value);

            Assert.AreEqual(2, api_way.Tags.Count);
            Assert.IsTrue(api_way.Tags.ContainsKey("another_tag"));
            Assert.AreEqual("test adding a tag!", api_way.Tags["another_tag"]);
        }

        /// <summary>
        /// Tries a simple way creation/get/update.
        /// </summary>
        [Test]
        public void APITestWayCreateGetDelete()
        {
            // intialize the connection.
            APIConnection api_instance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            long changeset_id = api_instance.ChangeSetOpen("Simple Way Creation Test");

            // initialize the way.
            SimpleWay way = new SimpleWay();
            way.Tags = new Dictionary<string, string>();
            way.Tags.Add("type", "testway");
            way.Nodes = new List<long>();
            way.Visible = true;

            // initialize the nodes.
            SimpleNode node = new SimpleNode();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new Dictionary<string, string>();
            node.Tags.Add("type", "testnode1");
            node.Visible = true;
            node = api_instance.NodeCreate(node);
            way.Nodes.Add(node.Id.Value);
            node = new SimpleNode();
            node.Latitude = -0.494497 + 0.0001f;
            node.Longitude = -24.119325 + 0.0001f;
            node.Tags = new Dictionary<string, string>();
            node.Tags.Add("type", "testnode2");
            node.Visible = true;
            node = api_instance.NodeCreate(node);
            way.Nodes.Add(node.Id.Value);

            // save the way.
            way = api_instance.WayCreate(way);

            // close the changeset.
            api_instance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(way.Id.HasValue);

            // get the new way id.
            long way_id = way.Id.Value;

            // get the way again: a way can only be deleted using the correct changesetid and version.
            way = api_instance.WayGet(way.Id.Value);

            // open new changeset.
            changeset_id = api_instance.ChangeSetOpen("Simple Way Delete Test");

            // get the way.
            api_instance.WayDelete(way);

            // close the current changeset.
            api_instance.ChangeSetClose();

            // get the way.
            SimpleWay api_way = api_instance.WayGet(way.Id.Value);
            Assert.IsNull(api_way);
        }


        /// <summary>
        /// Tries a simple relation creation/get.
        /// </summary>
        [Test]
        public void APITestRelationCreateGet()
        {
            // intialize the connection.
            APIConnection api_instance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            long changeset_id = api_instance.ChangeSetOpen("Simple Relation Creation Test");

            // initialize the relation.
            SimpleRelation relation = new SimpleRelation();
            relation.Tags = new Dictionary<string, string>();
            relation.Tags.Add("type", "testrelation");
            relation.Members = new List<SimpleRelationMember>();
            relation.Visible = true;

            // initialize the nodes.
            SimpleNode node = new SimpleNode();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new Dictionary<string, string>();
            node.Tags.Add("type", "testnode1");
            node.Visible = true;
            node = api_instance.NodeCreate(node);
            relation.Members.Add(new SimpleRelationMember()
            {
                MemberId = node.Id.Value,
                MemberRole = "some_nodes_role",
                MemberType = SimpleRelationMemberType.Node
            });
            node = new SimpleNode();
            node.Latitude = -0.494497 + 0.0001f;
            node.Longitude = -24.119325 + 0.0001f;
            node.Tags = new Dictionary<string, string>();
            node.Tags.Add("type", "testnode2");
            node.Visible = true;
            node = api_instance.NodeCreate(node);
            relation.Members.Add(new SimpleRelationMember()
            {
                MemberId = node.Id.Value,
                MemberRole = "some_nodes_role",
                MemberType = SimpleRelationMemberType.Node
            });

            // save the relation.
            relation = api_instance.RelationCreate(relation);

            // close the changeset.
            api_instance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(relation.Id.HasValue);

            // get the new relation id.
            long relation_id = relation.Id.Value;

            // get the relation from the api.
            SimpleRelation relation_api = api_instance.RelationGet(relation.Id.Value);
            Assert.AreEqual(relation_id, relation_api.Id.Value);
            Assert.AreEqual(relation.Tags.Count, relation_api.Tags.Count);
            Assert.AreEqual(relation.Visible, relation_api.Visible);
            Assert.IsTrue(relation_api.ChangeSetId.HasValue);
            Assert.AreEqual(changeset_id, relation_api.ChangeSetId.Value);
            Assert.AreEqual(relation.Members[0].MemberId, relation_api.Members[0].MemberId);
            Assert.AreEqual(relation.Members[0].MemberRole, relation_api.Members[0].MemberRole);
            Assert.AreEqual(relation.Members[0].MemberType, relation_api.Members[0].MemberType);
            Assert.AreEqual(relation.Members[1].MemberId, relation_api.Members[1].MemberId);
            Assert.AreEqual(relation.Members[1].MemberRole, relation_api.Members[1].MemberRole);
            Assert.AreEqual(relation.Members[1].MemberType, relation_api.Members[1].MemberType);
        }

        /// <summary>
        /// Tries a simple relation creation/get/update.
        /// </summary>
        [Test]
        public void APITestRelationCreateGetUpdate()
        {
            // intialize the connection.
            APIConnection api_instance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            long changeset_id = api_instance.ChangeSetOpen("Simple Relation Creation Test");

            // initialize the relation.
            SimpleRelation relation = new SimpleRelation();
            relation.Tags = new Dictionary<string, string>();
            relation.Tags.Add("type", "testrelation");
            relation.Members = new List<SimpleRelationMember>();
            relation.Visible = true;

            // initialize the nodes.
            SimpleNode node = new SimpleNode();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new Dictionary<string, string>();
            node.Tags.Add("type", "testnode1");
            node.Visible = true;
            node = api_instance.NodeCreate(node);
            relation.Members.Add(new SimpleRelationMember()
            {
                MemberId = node.Id.Value,
                MemberRole = "some_nodes_role",
                MemberType = SimpleRelationMemberType.Node
            });
            node = new SimpleNode();
            node.Latitude = -0.494497 + 0.0001f;
            node.Longitude = -24.119325 + 0.0001f;
            node.Tags = new Dictionary<string, string>();
            node.Tags.Add("type", "testnode2");
            node.Visible = true;
            node = api_instance.NodeCreate(node);
            relation.Members.Add(new SimpleRelationMember()
            {
                MemberId = node.Id.Value,
                MemberRole = "some_nodes_role",
                MemberType = SimpleRelationMemberType.Node
            });

            // save the relation.
            relation = api_instance.RelationCreate(relation);

            // close the changeset.
            api_instance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(relation.Id.HasValue);

            // get the new relation id.
            long relation_id = relation.Id.Value;

            // open new changeset.
            changeset_id = api_instance.ChangeSetOpen("Simple Relation Update Test");

            // get the relation.
            SimpleRelation api_relation = api_instance.RelationGet(relation.Id.Value);
            api_relation.Tags.Add("another_tag", "test adding a tag!");
            api_instance.RelationUpdate(api_relation);

            // close the current changeset.
            api_instance.ChangeSetClose();

            // get the api relation.
            api_relation = api_instance.RelationGet(relation.Id.Value);

            Assert.AreEqual(2, api_relation.Tags.Count);
            Assert.IsTrue(api_relation.Tags.ContainsKey("another_tag"));
            Assert.AreEqual("test adding a tag!", api_relation.Tags["another_tag"]);
        }

        /// <summary>
        /// Tries a simple relation creation/get/update.
        /// </summary>
        [Test]
        public void APITestRelationCreateGetDelete()
        {
            // intialize the connection.
            APIConnection api_instance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");

            // open a changeset.
            long changeset_id = api_instance.ChangeSetOpen("Simple Relation Creation Test");

            // initialize the relation.
            SimpleRelation relation = new SimpleRelation();
            relation.Tags = new Dictionary<string, string>();
            relation.Tags.Add("type", "testrelation");
            relation.Members = new List<SimpleRelationMember>();
            relation.Visible = true;

            // initialize the nodes.
            SimpleNode node = new SimpleNode();
            node.Latitude = -0.494497;
            node.Longitude = -24.119325;
            node.Tags = new Dictionary<string, string>();
            node.Tags.Add("type", "testnode1");
            node.Visible = true;
            node = api_instance.NodeCreate(node);
            relation.Members.Add(new SimpleRelationMember()
            {
                MemberId = node.Id.Value,
                MemberRole = "some_nodes_role",
                MemberType = SimpleRelationMemberType.Node
            });
            node = new SimpleNode();
            node.Latitude = -0.494497 + 0.0001f;
            node.Longitude = -24.119325 + 0.0001f;
            node.Tags = new Dictionary<string, string>();
            node.Tags.Add("type", "testnode2");
            node.Visible = true;
            node = api_instance.NodeCreate(node);
            relation.Members.Add(new SimpleRelationMember()
            {
                MemberId = node.Id.Value,
                MemberRole = "some_nodes_role",
                MemberType = SimpleRelationMemberType.Node
            });

            // save the relation.
            relation = api_instance.RelationCreate(relation);

            // close the changeset.
            api_instance.ChangeSetClose();

            // check if the id now has a value.
            Assert.IsTrue(relation.Id.HasValue);

            // get the new relation id.
            long relation_id = relation.Id.Value;

            // get the relation again: a relation can only be deleted using the correct changesetid and version.
            relation = api_instance.RelationGet(relation.Id.Value);

            // open new changeset.
            changeset_id = api_instance.ChangeSetOpen("Simple Relation Delete Test");

            // get the relation.
            api_instance.RelationDelete(relation);

            // close the current changeset.
            api_instance.ChangeSetClose();

            // get the relation.
            SimpleRelation api_relation = api_instance.RelationGet(relation.Id.Value);
            Assert.IsNull(api_relation);
        }

        /// <summary>
        /// Tries to delete everything inside a boundingbox.
        /// </summary>
        [Test]
        public void APITestDeleteBoundingBox()
        {
            // intialize the connection.
            APIConnection api_instance = new APIConnection("http://api06.dev.openstreetmap.org/",
                "osmsharp", "osmsharp");
            
            // get all objects in the box.
            GeoCoordinateBox box = new GeoCoordinateBox(new GeoCoordinate(-0.494497, -24.119325),
                new GeoCoordinate(-0.494497, -24.119325));
            box = box.Resize(0.001f); // create a box around the usual test coordinates.
            List<SimpleOsmGeo> box_objects = api_instance.BoundingBoxGet(box);

            // delete all objects.
            long changeset_id = api_instance.ChangeSetOpen("delete test objects again");

            foreach (SimpleOsmGeo geo_object in box_objects)
            {
                switch(geo_object.Type)
                {
                    case SimpleOsmGeoType.Relation:
                        api_instance.RelationDelete(geo_object as SimpleRelation);
                        break;
                }
            }

            foreach (SimpleOsmGeo geo_object in box_objects)
            {
                switch (geo_object.Type)
                {
                    case SimpleOsmGeoType.Way:
                        api_instance.WayDelete(geo_object as SimpleWay);
                        break;
                }
            }

            foreach (SimpleOsmGeo geo_object in box_objects)
            {
                switch (geo_object.Type)
                {
                    case SimpleOsmGeoType.Node:
                        api_instance.NodeDelete(geo_object as SimpleNode);
                        break;
                }
            }
        }
    }
}
