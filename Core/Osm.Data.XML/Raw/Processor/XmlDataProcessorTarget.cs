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
using OsmSharp.Osm.Core.Simple;
using OsmSharp.Osm.Data.Core.Processor;

namespace OsmSharp.Osm.Data.XML.Raw.Processor
{
    public class XmlDataProcessorTarget : DataProcessorTarget
    {
        private XmlFragmentWriter _writer;

        private TextWriter _text_writer;

        private XmlSerializer _ser_node;

        private XmlSerializer _ser_way;

        private XmlSerializer _ser_relation;

        private string _file_name;

        public XmlDataProcessorTarget(string file_name)
            :base()
        {
            _file_name = file_name;
        }

        public override void Initialize()
        {
            _ser_node = new XmlSerializer(typeof(Osm.Core.Xml.v0_6.node));
            _ser_way = new XmlSerializer(typeof(Osm.Core.Xml.v0_6.way));
            _ser_relation = new XmlSerializer(typeof(Osm.Core.Xml.v0_6.relation));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.Encoding = Encoding.ASCII;

            _text_writer = File.CreateText(_file_name);
            _text_writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            _text_writer.WriteLine("<osm version=\"0.6\" generator=\"OsmSharp\">");

            _writer = new XmlFragmentWriter(_text_writer);
        }

        public override void ApplyChange(SimpleChangeSet change)
        {

        }

        /// <summary>
        /// Adds a node to the xml output stream.
        /// </summary>
        /// <param name="node_to_add"></param>
        public override void AddNode(SimpleNode node_to_add)
        {
            OsmSharp.Osm.Core.Xml.v0_6.node nd = new OsmSharp.Osm.Core.Xml.v0_6.node();

            // set id
            nd.idSpecified = false;
            if (node_to_add.Id.HasValue)
            {
                nd.id = node_to_add.Id.Value;
                nd.idSpecified = true;
            }

            // set changeset.
            nd.changesetSpecified = false;
            if(node_to_add.ChangeSetId.HasValue)
            {
                nd.changeset = node_to_add.ChangeSetId.Value;
                nd.changesetSpecified = true;
            }

            // set visible.
            nd.visibleSpecified = false;
            if (node_to_add.Visible.HasValue)
            {
                nd.visible = node_to_add.Visible.Value;
                nd.visibleSpecified = true;
            }

            // set timestamp.
            nd.timestampSpecified = false;
            if (node_to_add.TimeStamp.HasValue)
            {
                nd.timestamp = node_to_add.TimeStamp.Value;
                nd.timestampSpecified = true;
            }

            // set latitude.
            nd.latSpecified = false;
            if (node_to_add.Latitude.HasValue)
            {
                nd.lat = node_to_add.Latitude.Value;
                nd.latSpecified = true;
            }

            // set longitude.
            nd.lonSpecified = false;
            if (node_to_add.Longitude.HasValue)
            {
                nd.lon = node_to_add.Longitude.Value;
                nd.lonSpecified = true;
            }

            // set uid
            nd.uidSpecified = false;
            if (node_to_add.UserId.HasValue)
            {
                nd.uid  = node_to_add.UserId.Value;
                nd.uidSpecified = true;
            }

            // set user
            nd.user = node_to_add.UserName;

            // set tags.
            nd.tag = this.ConvertToXmlTags(node_to_add.Tags);
            
            // write to output.
            string node_string = string.Empty;

            // serialize node.
            _ser_node.Serialize(_writer, nd);
            _writer.Flush();
            _text_writer.Write(_text_writer.NewLine);
        }

        public override void AddWay(SimpleWay way_to_add)
        {
            OsmSharp.Osm.Core.Xml.v0_6.way wa = new OsmSharp.Osm.Core.Xml.v0_6.way();

            wa.idSpecified = false;
            if (way_to_add.Id.HasValue)
            {
                wa.idSpecified = true;
                wa.id = way_to_add.Id.Value;
            }

            // set changeset.
            wa.changesetSpecified = false;
            if (way_to_add.ChangeSetId.HasValue)
            {
                wa.changeset = way_to_add.ChangeSetId.Value;
                wa.changesetSpecified = true;
            }

            // set visible.
            wa.visibleSpecified = false;
            if (way_to_add.Visible.HasValue)
            {
                wa.visible = way_to_add.Visible.Value;
                wa.visibleSpecified = true;
            }

            // set timestamp.
            wa.timestampSpecified = false;
            if (way_to_add.TimeStamp.HasValue)
            {
                wa.timestamp = way_to_add.TimeStamp.Value;
                wa.timestampSpecified = true;
            }

            // set uid
            wa.uidSpecified = false;
            if (way_to_add.UserId.HasValue)
            {
                wa.uid = way_to_add.UserId.Value;
                wa.uidSpecified = true;
            }

            // set user
            wa.user = way_to_add.UserName;

            // set tags.
            wa.tag = this.ConvertToXmlTags(way_to_add.Tags);

            // set nodes.
            if (way_to_add.Nodes != null)
            {
                wa.nd = new OsmSharp.Osm.Core.Xml.v0_6.nd[way_to_add.Nodes.Count];
                for (int idx = 0; idx < way_to_add.Nodes.Count; idx++)
                {
                    OsmSharp.Osm.Core.Xml.v0_6.nd nd = new OsmSharp.Osm.Core.Xml.v0_6.nd();
                    nd.refSpecified = true;
                    nd.@ref = way_to_add.Nodes[idx];
                    wa.nd[idx] = nd;
                }
            }

            // serialize way.
            _ser_way.Serialize(_writer, wa);
            _writer.Flush();
            _text_writer.Write(_text_writer.NewLine);
        }

        public override void AddRelation(SimpleRelation relation_to_add)
        {
            OsmSharp.Osm.Core.Xml.v0_6.relation re = new OsmSharp.Osm.Core.Xml.v0_6.relation();

            re.idSpecified = false;
            if (relation_to_add.Id.HasValue)
            {
                re.idSpecified = true;
                re.id = relation_to_add.Id.Value;
            }

            // set changeset.
            re.changesetSpecified = false;
            if (relation_to_add.ChangeSetId.HasValue)
            {
                re.changeset = relation_to_add.ChangeSetId.Value;
                re.changesetSpecified = true;
            }

            // set visible.
            re.visibleSpecified = false;
            if (relation_to_add.Visible.HasValue)
            {
                re.visible = relation_to_add.Visible.Value;
                re.visibleSpecified = true;
            }

            // set timestamp.
            re.timestampSpecified = false;
            if (relation_to_add.TimeStamp.HasValue)
            {
                re.timestamp = relation_to_add.TimeStamp.Value;
                re.timestampSpecified = true;
            }

            // set uid
            re.uidSpecified = false;
            if (relation_to_add.UserId.HasValue)
            {
                re.uid = relation_to_add.UserId.Value;
                re.uidSpecified = true;
            }

            // set user
            re.user = relation_to_add.UserName;

            // set tags.
            re.tag = this.ConvertToXmlTags(relation_to_add.Tags);

            // set members.
            if (relation_to_add.Members != null)
            {
                re.member = new OsmSharp.Osm.Core.Xml.v0_6.member[relation_to_add.Members.Count];
                for (int idx = 0; idx < relation_to_add.Members.Count; idx++)
                {
                    OsmSharp.Osm.Core.Xml.v0_6.member mem = new OsmSharp.Osm.Core.Xml.v0_6.member();
                    SimpleRelationMember member_to_add = relation_to_add.Members[idx];

                    // set memberid
                    mem.refSpecified = false;
                    if (member_to_add.MemberId.HasValue)
                    {
                        mem.@ref = member_to_add.MemberId.Value;
                        mem.refSpecified = true;
                    }

                    // set type
                    mem.typeSpecified = false;
                    if (member_to_add.MemberType.HasValue)
                    {
                        switch (member_to_add.MemberType.Value)
                        {
                            case SimpleRelationMemberType.Node:
                                mem.type = OsmSharp.Osm.Core.Xml.v0_6.memberType.node;
                                break;
                            case SimpleRelationMemberType.Way:
                                mem.type = OsmSharp.Osm.Core.Xml.v0_6.memberType.way;
                                break;
                            case SimpleRelationMemberType.Relation:
                                mem.type = OsmSharp.Osm.Core.Xml.v0_6.memberType.relation;
                                break;
                        }
                        mem.typeSpecified = true;
                    }

                    mem.role = member_to_add.MemberRole;

                    re.member[idx] = mem;
                }
            }

            // serialize relation.
            _ser_relation.Serialize(_writer, re);
            _writer.Flush();
            _text_writer.Write(_text_writer.NewLine);
        }

        private OsmSharp.Osm.Core.Xml.v0_6.tag[] ConvertToXmlTags(IDictionary<string, string> tags)
        {
            if (tags != null)
            {
                OsmSharp.Osm.Core.Xml.v0_6.tag[] xml_tags = new OsmSharp.Osm.Core.Xml.v0_6.tag[tags.Count];

                int idx = 0;
                foreach (KeyValuePair<string, string> pair in tags)
                {
                    xml_tags[idx] = new OsmSharp.Osm.Core.Xml.v0_6.tag();
                    xml_tags[idx].k = pair.Key;
                    xml_tags[idx].v = pair.Value;
                    idx++;
                }

                return xml_tags;
            }
            return null;
        }

        public override void Close()
        {
            base.Close();

            _writer.Flush();
            _text_writer.WriteLine("</osm>");
            _text_writer.Flush();
            _writer.Close();
            _text_writer.Close();
        }
    }
}
