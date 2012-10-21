// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Tools.Xml.Sources
{
    /// <summary>
    /// Class implementing an xml reader source.
    /// </summary>
    public class XmlReaderSource : IXmlSource
    {
        /// <summary>
        /// The string source.
        /// </summary>
        private XmlReader _reader;

        /// <summary>
        /// Creates a new xml reader source.
        /// </summary>
        /// <param name="source"></param>
        public XmlReaderSource(XmlReader reader)
        {
            _reader = reader;
        }

        #region IXmlSource Members

        /// <summary>
        /// Returns an xml reader.
        /// </summary>
        public XmlReader GetReader()
        {
            return _reader;
        }

        /// <summary>
        /// Returns an xml writer.
        /// </summary>
        public XmlWriter GetWriter()
        {
            return null;
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool HasData
        {
            get
            {
                return _reader != null;
            }
        }

        public string Name
        {
            get
            {
                return "Generic Reader Source";
            }
        }

        public void Close()
        {
            _reader.Close();
            _reader = null;
        }

        #endregion
    }
}
