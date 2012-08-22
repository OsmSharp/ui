using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Xml;
using System.Xml.Serialization;
using System.Xml;

namespace Osm.Core.Xml
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
        /// Returns the name of this osm document.
        /// </summary>
        public string Name
        {
            get
            {
                return _source.Name;
            }
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
        Osmv0_6
    }
}
