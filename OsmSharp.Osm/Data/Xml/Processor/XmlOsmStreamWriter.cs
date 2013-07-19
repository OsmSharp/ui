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
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using OsmSharp.Osm.Data.Streams;
using OsmSharp.Osm;
using OsmSharp.Osm.Data.Core.Processor;
using OsmSharp.Collections.Tags;

namespace OsmSharp.Osm.Data.Xml.Processor
{
    /// <summary>
    /// A data processor target that write OSM XML.
    /// </summary>
    public class XmlOsmStreamWriter : OsmStreamTarget
    {
        private XmlFragmentWriter _writer;

        private TextWriter _textWriter;

        private XmlSerializer _serNode;

        private XmlSerializer _serWay;

        private XmlSerializer _serRelation;

        private readonly string _fileName;

        /// <summary>
        /// Creates a new Xml data processor target.
        /// </summary>
        /// <param name="fileName"></param>
        public XmlOsmStreamWriter(string fileName)
            :base()
        {
            _fileName = fileName;
        }

        /// <summary>
        /// Creates a new Xml data processor target.
        /// </summary>
        /// <param name="textWriter"></param>
        public XmlOsmStreamWriter(TextWriter textWriter)
            : base()
        {
            _textWriter = textWriter;
        }

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {
            _serNode = new XmlSerializer(typeof(Osm.Xml.v0_6.node));
            _serWay = new XmlSerializer(typeof(Osm.Xml.v0_6.way));
            _serRelation = new XmlSerializer(typeof(Osm.Xml.v0_6.relation));

            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.Encoding = Encoding.ASCII;

            if (_textWriter == null)
            {
                _textWriter = File.CreateText(_fileName);
            }

            _textWriter.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            _textWriter.WriteLine("<osm version=\"0.6\" generator=\"OsmSharp\">");

            _writer = new XmlFragmentWriter(_textWriter);

        }

        /// <summary>
        /// Adds a node to the xml output stream.
        /// </summary>
        /// <param name="simpleNode"></param>
        public override void AddNode(Node simpleNode)
        {
            var nd = new OsmSharp.Osm.Xml.v0_6.node();

            // set id
            nd.idSpecified = false;
            if (simpleNode.Id.HasValue)
            {
                nd.id = simpleNode.Id.Value;
                nd.idSpecified = true;
            }

            // set changeset.
            nd.changesetSpecified = false;
            if(simpleNode.ChangeSetId.HasValue)
            {
                nd.changeset = simpleNode.ChangeSetId.Value;
                nd.changesetSpecified = true;
            }

            // set visible.
            nd.visibleSpecified = false;
            if (simpleNode.Visible.HasValue)
            {
                nd.visible = simpleNode.Visible.Value;
                nd.visibleSpecified = true;
            }

            // set timestamp.
            nd.timestampSpecified = false;
            if (simpleNode.TimeStamp.HasValue)
            {
                nd.timestamp = simpleNode.TimeStamp.Value;
                nd.timestampSpecified = true;
            }

            // set latitude.
            nd.latSpecified = false;
            if (simpleNode.Latitude.HasValue)
            {
                nd.lat = simpleNode.Latitude.Value;
                nd.latSpecified = true;
            }

            // set longitude.
            nd.lonSpecified = false;
            if (simpleNode.Longitude.HasValue)
            {
                nd.lon = simpleNode.Longitude.Value;
                nd.lonSpecified = true;
            }

            // set uid
            nd.uidSpecified = false;
            if (simpleNode.UserId.HasValue)
            {
                nd.uid  = simpleNode.UserId.Value;
                nd.uidSpecified = true;
            }

            // set user
            nd.user = simpleNode.UserName;

            // set tags.
            nd.tag = this.ConvertToXmlTags(simpleNode.Tags);

            // set version.
            if (simpleNode.Version.HasValue)
            {
                nd.version = simpleNode.Version.Value;
                nd.versionSpecified = true;
            }

            // serialize node.
            _serNode.Serialize(_writer, nd);
            _writer.Flush();
            _textWriter.Write(_textWriter.NewLine);
        }

        /// <summary>
        /// Adds a way to this target.
        /// </summary>
        /// <param name="simpleWay"></param>
        public override void AddWay(Way simpleWay)
        {
            var wa = new OsmSharp.Osm.Xml.v0_6.way();

            wa.idSpecified = false;
            if (simpleWay.Id.HasValue)
            {
                wa.idSpecified = true;
                wa.id = simpleWay.Id.Value;
            }

            // set changeset.
            wa.changesetSpecified = false;
            if (simpleWay.ChangeSetId.HasValue)
            {
                wa.changeset = simpleWay.ChangeSetId.Value;
                wa.changesetSpecified = true;
            }

            // set visible.
            wa.visibleSpecified = false;
            if (simpleWay.Visible.HasValue)
            {
                wa.visible = simpleWay.Visible.Value;
                wa.visibleSpecified = true;
            }

            // set timestamp.
            wa.timestampSpecified = false;
            if (simpleWay.TimeStamp.HasValue)
            {
                wa.timestamp = simpleWay.TimeStamp.Value;
                wa.timestampSpecified = true;
            }

            // set uid
            wa.uidSpecified = false;
            if (simpleWay.UserId.HasValue)
            {
                wa.uid = simpleWay.UserId.Value;
                wa.uidSpecified = true;
            }

            // set user
            wa.user = simpleWay.UserName;

            // set tags.
            wa.tag = this.ConvertToXmlTags(simpleWay.Tags);

            // set nodes.
            if (simpleWay.Nodes != null)
            {
                wa.nd = new OsmSharp.Osm.Xml.v0_6.nd[simpleWay.Nodes.Count];
                for (int idx = 0; idx < simpleWay.Nodes.Count; idx++)
                {
                    var nd = new OsmSharp.Osm.Xml.v0_6.nd();
                    nd.refSpecified = true;
                    nd.@ref = simpleWay.Nodes[idx];
                    wa.nd[idx] = nd;
                }
            }

            // set version.
            if (simpleWay.Version.HasValue)
            {
                wa.version = simpleWay.Version.Value;
                wa.versionSpecified = true;
            }

            // serialize way.
            _serWay.Serialize(_writer, wa);
            _writer.Flush();
            _textWriter.Write(_textWriter.NewLine);
        }

        /// <summary>
        /// Adds a relation to this target.
        /// </summary>
        /// <param name="simpleRelation"></param>
        public override void AddRelation(Relation simpleRelation)
        {
            var re = new OsmSharp.Osm.Xml.v0_6.relation();

            re.idSpecified = false;
            if (simpleRelation.Id.HasValue)
            {
                re.idSpecified = true;
                re.id = simpleRelation.Id.Value;
            }

            // set changeset.
            re.changesetSpecified = false;
            if (simpleRelation.ChangeSetId.HasValue)
            {
                re.changeset = simpleRelation.ChangeSetId.Value;
                re.changesetSpecified = true;
            }

            // set visible.
            re.visibleSpecified = false;
            if (simpleRelation.Visible.HasValue)
            {
                re.visible = simpleRelation.Visible.Value;
                re.visibleSpecified = true;
            }

            // set timestamp.
            re.timestampSpecified = false;
            if (simpleRelation.TimeStamp.HasValue)
            {
                re.timestamp = simpleRelation.TimeStamp.Value;
                re.timestampSpecified = true;
            }

            // set uid
            re.uidSpecified = false;
            if (simpleRelation.UserId.HasValue)
            {
                re.uid = simpleRelation.UserId.Value;
                re.uidSpecified = true;
            }

            // set user
            re.user = simpleRelation.UserName;

            // set tags.
            re.tag = this.ConvertToXmlTags(simpleRelation.Tags);

            // set members.
            if (simpleRelation.Members != null)
            {
                re.member = new OsmSharp.Osm.Xml.v0_6.member[simpleRelation.Members.Count];
                for (int idx = 0; idx < simpleRelation.Members.Count; idx++)
                {
                    var mem = new OsmSharp.Osm.Xml.v0_6.member();
                    RelationMember memberToAdd = simpleRelation.Members[idx];

                    // set memberid
                    mem.refSpecified = false;
                    if (memberToAdd.MemberId.HasValue)
                    {
                        mem.@ref = memberToAdd.MemberId.Value;
                        mem.refSpecified = true;
                    }

                    // set type
                    mem.typeSpecified = false;
                    if (memberToAdd.MemberType.HasValue)
                    {
                        switch (memberToAdd.MemberType.Value)
                        {
                            case RelationMemberType.Node:
                                mem.type = OsmSharp.Osm.Xml.v0_6.memberType.node;
                                break;
                            case RelationMemberType.Way:
                                mem.type = OsmSharp.Osm.Xml.v0_6.memberType.way;
                                break;
                            case RelationMemberType.Relation:
                                mem.type = OsmSharp.Osm.Xml.v0_6.memberType.relation;
                                break;
                        }
                        mem.typeSpecified = true;
                    }

                    mem.role = memberToAdd.MemberRole;

                    re.member[idx] = mem;
                }
            }

            // set version.
            if (simpleRelation.Version.HasValue)
            {
                re.version = simpleRelation.Version.Value;
                re.versionSpecified = true;
            }

            // serialize relation.
            _serRelation.Serialize(_writer, re);
            _writer.Flush();
            _textWriter.Write(_textWriter.NewLine);
        }

        private OsmSharp.Osm.Xml.v0_6.tag[] ConvertToXmlTags(TagsCollection tags)
        {
            if (tags != null)
            {
                var xml_tags = new OsmSharp.Osm.Xml.v0_6.tag[tags.Count];

                int idx = 0;
                foreach (Tag pair in tags)
                {
                    xml_tags[idx] = new OsmSharp.Osm.Xml.v0_6.tag();
                    xml_tags[idx].k = pair.Key;
                    xml_tags[idx].v = pair.Value;
                    idx++;
                }

                return xml_tags;
            }
            return null;
        }

        /// <summary>
        /// Closes this target.
        /// </summary>
        public override void Close()
        {
            base.Close();

            _writer.Flush();
            _textWriter.WriteLine("</osm>");
            _textWriter.Flush();
            _writer.Close();
            _textWriter.Close();
        }
    }
}