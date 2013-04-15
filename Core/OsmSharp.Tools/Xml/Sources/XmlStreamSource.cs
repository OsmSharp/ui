using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace OsmSharp.Tools.Xml.Sources
{
    /// <summary>
    /// XML stream source.
    /// </summary>
    public class XmlStreamSource : IXmlSource
    {
        /// <summary>
        /// The reference to the file.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Creates a new xml file source.
        /// </summary>
        /// <param name="stream"></param>
        public XmlStreamSource(Stream stream)
        {
            _stream = stream;
        }

        #region IXmlSource Members

        /// <summary>
        /// Returns an xml reader.
        /// </summary>
        public XmlReader GetReader()
        {
            _stream.Seek(0, SeekOrigin.Begin);
            return XmlReader.Create(_stream);
        }

        /// <summary>
        /// Returns an xml writer.
        /// </summary>
        /// <returns></returns>
        public XmlWriter GetWriter()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CheckCharacters = true;
            settings.CloseOutput = true;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.NewLineChars  = Environment.NewLine;
            settings.NewLineHandling = NewLineHandling.Entitize;
            settings.OmitXmlDeclaration = true;

            _stream.SetLength(0);
            return XmlWriter.Create(_stream);
        }

        /// <summary>
        /// Returns true if this file source is readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                //if (!_stream.CanWrite)
                //{
                //    return true;
                //}
                return false;
            }
        }

        /// <summary>
        /// Returns true if the file source has data.
        /// </summary>
        public bool HasData
        {
            get
            {
                return _stream.Length > 1;
            }
        }

        /// <summary>
        /// Closes this file source.
        /// </summary>
        public void Close()
        {
            _stream = null;
        }

        #endregion
    }
}
