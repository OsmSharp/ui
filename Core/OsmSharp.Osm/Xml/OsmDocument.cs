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
using OsmSharp.Tools.Xml;
using System.Xml.Serialization;
using System.Xml;

namespace OsmSharp.Osm.Xml
{

    /// <summary>
    /// Represents a osm document.
    /// </summary>
    public class OsmDocument
    {
        /// <summary>
        /// The actual osm object.
        /// </summary>
        private object _osm_object;

        /// <summary>
        /// The xml source this documents comes from.
        /// </summary>
        private IXmlSource _source;

        /// <summary>
        /// Creates a new osm document based on an xml source.
        /// </summary>
        /// <param name="source"></param>
        public OsmDocument(IXmlSource source)
        {
            _source = source;
        }

        /// <summary>
        /// Returns the readonly flag.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return _source.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets/Sets the osm object.
        /// </summary>
        public object Osm
        {
            get
            {
                this.DoReadOsm();

                return _osm_object;
            }
            set
            {
                _osm_object = value;
            }
        }

        /// <summary>
        /// Saves this osm back to it's source.
        /// </summary>
        public void Save()
        {
            this.DoWriteOsm();
        }

        #region Private Serialize/Desirialize functions

        private void DoReadOsm()
        {
            if (_osm_object == null && _source.HasData)
            {
                XmlSerializer xmlSerializer = null;
                xmlSerializer = new XmlSerializer(typeof(v0_6.osm));

                XmlReader reader = _source.GetReader();
                _osm_object = xmlSerializer.Deserialize(reader);

                reader.Close();
            }
        }

        private void DoWriteOsm()
        {
            if (_osm_object != null)
            {
                XmlSerializer xmlSerializer = null;
                xmlSerializer = new XmlSerializer(typeof(v0_6.osm));

                XmlWriter writer = _source.GetWriter();
                xmlSerializer.Serialize(writer,_osm_object);
                writer.Flush();
                writer.Close();

                xmlSerializer = null;
                writer = null;
            }
        }

        #endregion

        /// <summary>
        /// Closes the osm document.
        /// </summary>
        public void Close()
        {
            _source = null;
            _osm_object = null;
        }
    }

    /// <summary>
    /// The supported osm document versions.
    /// </summary>
    public enum OsmVersion
    {
        /// <summary>
        /// v0.6
        /// </summary>
        Osmv0_6
    }
}
