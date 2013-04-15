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
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using OsmSharp.Osm.Simple;
using OsmSharp.Osm.Data.Core.Processor.ChangeSets;

namespace OsmSharp.Osm.Data.XML.Processor.ChangeSets
{
    /// <summary>
    /// A changeset source.
    /// </summary>
    public class XmlDataProcessorChangeSetSource : DataProcessorChangeSetSource
    {
        private SimpleChangeSet _next;

        private XmlSerializer _ser_create;

        private XmlSerializer _ser_modify;

        private XmlSerializer _ser_delete;

        private XmlReader _reader;

        private string _file_name;

        private Stream _stream;
        
        /// <summary>
        /// Creates a new changeset source.
        /// </summary>
        /// <param name="file_name"></param>
        public XmlDataProcessorChangeSetSource(string file_name)
        {
            _file_name = file_name;
        }

        /// <summary>
        /// Creates a new changeset source.
        /// </summary>
        /// <param name="stream"></param>
        public XmlDataProcessorChangeSetSource(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Initializes this changeset source.
        /// </summary>
        public override void Initialize()
        {
            _next = null;
            _ser_create = new XmlSerializer(typeof(Osm.Xml.v0_6.create));
            _ser_modify = new XmlSerializer(typeof(Osm.Xml.v0_6.modify));
            _ser_delete = new XmlSerializer(typeof(Osm.Xml.v0_6.delete));

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.CheckCharacters = false;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;

            if (_stream != null)
            {
                _reader = XmlReader.Create(_stream, settings);     
            }
            else
            {
                TextReader text_reader = new StreamReader(new FileInfo(_file_name).OpenRead(), Encoding.UTF8);
                _reader = XmlReader.Create(text_reader, settings);     
            }
        }

        /// <summary>
        /// Moves to the next changeset.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            while (_reader.Read())
            {
                if (_reader.NodeType == XmlNodeType.Element && (_reader.Name == "modify" || _reader.Name == "create"||_reader.Name == "delete"))
                {
                    // create a stream for only this element.
                    string name = _reader.Name;
                    string next_element = _reader.ReadOuterXml();
                    XmlReader reader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(next_element)));
                    object osm_obj = null;

                    // select type of element.
                    switch (name)
                    {
                        case "delete":
                            osm_obj = _ser_delete.Deserialize(reader);
                            if (osm_obj is OsmSharp.Osm.Xml.v0_6.delete)
                            {
                                _next = XmlSimpleConverter.ConvertToSimple(osm_obj as OsmSharp.Osm.Xml.v0_6.delete);
                                return true;
                            }
                            break;
                        case "modify":
                            osm_obj = _ser_modify.Deserialize(reader);
                            if (osm_obj is OsmSharp.Osm.Xml.v0_6.modify)
                            {
                                _next = XmlSimpleConverter.ConvertToSimple(osm_obj as OsmSharp.Osm.Xml.v0_6.modify);
                                return true;
                            }
                            break;
                        case "create":
                            osm_obj = _ser_create.Deserialize(reader);
                            if (osm_obj is OsmSharp.Osm.Xml.v0_6.create)
                            {
                                _next = XmlSimpleConverter.ConvertToSimple(osm_obj as OsmSharp.Osm.Xml.v0_6.create);
                                return true;
                            }
                            break;
                    }
                }
            }
            _next = null;
            return false;
        }

        /// <summary>
        /// Returns the current changeset.
        /// </summary>
        /// <returns></returns>
        public override SimpleChangeSet Current()
        {
            return _next;
        }

        /// <summary>
        /// Resets this changeset source.
        /// </summary>
        public override void Reset()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.CheckCharacters = false;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;

            TextReader text_reader = new StreamReader(new FileInfo(_file_name).OpenRead(), Encoding.UTF8);
            _reader = XmlReader.Create(text_reader, settings);     
        }

        /// <summary>
        /// Closes this changeset source.
        /// </summary>
        public override void Close()
        {
            if (_stream != null)
            {
                _stream.Close();
            }
            _reader.Close();
        }
    }
}
