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
using System.IO;
using System.Xml;

namespace OsmSharp.Osm.Data.XML.Raw.Processor
{
    class XmlFragmentWriter : XmlTextWriter
    {
        public XmlFragmentWriter(TextWriter w) : base(w) { }

        public XmlFragmentWriter(Stream w, Encoding encoding) : base(w, encoding) { }

        public XmlFragmentWriter(string filename, Encoding encoding) :
            base(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None), encoding) { }
        
        bool _skip = false;

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            // STEP 1 - Omits XSD and XSI declarations.
            // From Kzu - http://weblogs.asp.net/cazzu/archive/2004/01/23/62141.aspx
            if (prefix == "xmlns" && (localName == "xsd" || localName == "xsi"))
            {
                _skip = true;
                return;
            }
            base.WriteStartAttribute(prefix, localName, ns);
        }

        public override void WriteString(string text)
        {
            if (_skip) return;
            base.WriteString(text);
        }

        public override void WriteEndAttribute()
        {
            if (_skip)
            {
                // Reset the flag, so we keep writing.
                _skip = false;
                return;
            }
            base.WriteEndAttribute();
        }
        
        public override void WriteStartDocument()
        {
            // STEP 2: Do nothing so we omit the xml declaration.
        }
    }
}
