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
using System.IO.Compression;
using OsmSharp.Osm.Core.Simple;
using OsmSharp.Osm.Data.Core.Processor;

namespace OsmSharp.Osm.Data.XML.Raw.Processor
{
    public class XmlDataProcessorSource : DataProcessorSource
    {
        private XmlReader _reader;

        private XmlSerializer _ser_node;

        private XmlSerializer _ser_way;

        private XmlSerializer _ser_relation;

        private SimpleOsmGeo _next;

        private string _file_name;

        private Stream _stream;

        private bool _gzip;


        public XmlDataProcessorSource(string file_name) :
            this(file_name,false)
        {

        }

        public XmlDataProcessorSource(Stream stream) :
            this(stream, false)
        {

        }

        public XmlDataProcessorSource(Stream stream, bool gzip)
        {
            _stream = stream;
            _gzip = gzip;
        }

        public XmlDataProcessorSource(string file_name, bool gzip)
        {
            _file_name = file_name;
            _gzip = gzip;
        }

        public override void Initialize()
        {
            _next = null;
            _ser_node = new XmlSerializer(typeof(Osm.Core.Xml.v0_6.node));
            _ser_way = new XmlSerializer(typeof(Osm.Core.Xml.v0_6.way));
            _ser_relation = new XmlSerializer(typeof(Osm.Core.Xml.v0_6.relation));

            this.Reset();
        }

        public override void Reset()
        {            
            // create the xml reader settings.
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.CheckCharacters = false;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            //settings.IgnoreWhitespace = true;

            // create the stream.
            Stream file_stream = null;
            if (_stream != null)
            { // take the preset stream.
                file_stream = _stream;

                // seek to the beginning of the stream.
                if (file_stream.CanSeek)
                { // if a non-seekable stream is given resetting is disabled.
                    file_stream.Seek(0, SeekOrigin.Begin);
                }
            }
            else
            { // create a file stream.
                file_stream = new FileInfo(_file_name).OpenRead();
            }

            // decompress if needed.
            if (_gzip)
            {
                file_stream = new GZipStream(file_stream, CompressionMode.Decompress);
            }

            TextReader text_reader = new StreamReader(file_stream, Encoding.UTF8);
            _reader = XmlReader.Create(text_reader, settings);     
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get
            {
                if (_file_name == null)
                {
                    return _stream.CanSeek;
                }
                return true;
            }
        }

        public override bool MoveNext()
        {
            while (_reader.Read())
            {
                if (_reader.NodeType == XmlNodeType.Element && (_reader.Name == "node" || _reader.Name == "way" || _reader.Name == "relation"))
                {
                    // create a stream for only this element.
                    string name = _reader.Name;
                    string next_element = _reader.ReadOuterXml();
                    XmlReader reader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(next_element)));
                    object osm_obj = null;

                    // select type of element.
                    switch (name)
                    {
                         case "node":
                             osm_obj = _ser_node.Deserialize(reader);
                             if (osm_obj is OsmSharp.Osm.Core.Xml.v0_6.node)
                             {
                                 _next = XmlSimpleConverter.ConvertToSimple(osm_obj as OsmSharp.Osm.Core.Xml.v0_6.node);
                                 return true;
                             }
                             break;
                         case "way":
                             osm_obj = _ser_way.Deserialize(reader);
                             if (osm_obj is OsmSharp.Osm.Core.Xml.v0_6.way)
                             {
                                 _next = XmlSimpleConverter.ConvertToSimple(osm_obj as OsmSharp.Osm.Core.Xml.v0_6.way);
                                 return true;
                             }
                             break;
                         case "relation":
                             osm_obj = _ser_relation.Deserialize(reader);
                             if (osm_obj is OsmSharp.Osm.Core.Xml.v0_6.relation)
                             {
                                 _next = XmlSimpleConverter.ConvertToSimple(osm_obj as OsmSharp.Osm.Core.Xml.v0_6.relation);
                                 return true;
                             }
                             break;
                    }
                }
            }
            _next = null;
            return false;
        }

        public override SimpleOsmGeo Current()
        {
            return _next;
        }
    }
}
