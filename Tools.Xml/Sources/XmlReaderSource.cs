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
