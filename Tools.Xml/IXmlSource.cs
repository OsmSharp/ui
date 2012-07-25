using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Tools.Xml
{
    /// <summary>
    /// Reprents an xml source.
    /// </summary>
    public interface IXmlSource
    {
        /// <summary>
        /// Returns the name of this xml source.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Returns true if the xml source is readonly.
        /// </summary>
        bool IsReadOnly
        {
            get;
        }

        /// <summary>
        /// Returns true if the source has data.
        /// </summary>
        bool HasData
        {
            get;
        }

        /// <summary>
        /// Returns the reader for this xml source.
        /// </summary>
        XmlReader GetReader();

        /// <summary>
        /// Returns a write for this xml source.
        /// </summary>
        XmlWriter GetWriter();

        /// <summary>
        /// Closes the xml source.
        /// </summary>
        void Close();
    }
}
