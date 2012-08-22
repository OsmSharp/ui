using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Tools.Xml.Sources
{
    /// <summary>
    /// Represents an xml source for a file.
    /// </summary>
    public class XmlFileSource : IXmlSource
    {
        /// <summary>
        /// The reference to the file.
        /// </summary>
        private FileInfo _file;

        /// <summary>
        /// Creates a new xml file source.
        /// </summary>
        /// <param name="filename"></param>
        public XmlFileSource(string filename)
        {
            _file = new FileInfo(filename);
        }

        /// <summary>
        /// Creates a new xml file source.
        /// </summary>
        /// <param name="file"></param>
        public XmlFileSource(FileInfo file)
        {
            _file = file;
        }

        #region IXmlSource Members

        /// <summary>
        /// Returns an xml reader.
        /// </summary>
        public XmlReader GetReader()
        {
            return XmlReader.Create(_file.OpenRead());
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
            if (!_file.Exists)
            {
                return XmlWriter.Create(_file.FullName,settings);
            }
            return XmlWriter.Create(_file.FullName, settings);
        }

        /// <summary>
        /// Returns true if this file source is readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                if (_file.Exists && _file.IsReadOnly)
                {
                    return true;
                }
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
                _file.Refresh();
                if (_file.Exists)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Returns the name of the file source.
        /// </summary>
        public string Name
        {
            get
            {
                return _file.Name;
            }
        }

        /// <summary>
        /// Closes this file source.
        /// </summary>
        public void Close()
        {
            _file = null;
        }

        #endregion
    }
}
