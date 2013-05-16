using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using OsmSharp.Collections.SpatialIndexes.Serialization.v1;
using OsmSharp.Collections.Tags;
using OsmSharp.IO;
using OsmSharp.Math.Geo;

namespace OsmSharp.Collections.SpatialIndexes.Serialization
{
    /// <summary>
    /// Abstract representation of a spatial index serializer.
    /// </summary>
    /// <remarks>Versioning is implemented in the file format to guarantee backward compatibility.</remarks>
    public abstract class SpatialIndexSerializer<T>
    {
        #region Versioning

        /// <summary>
        /// Returns the version number.
        /// </summary>
        public abstract uint Version { get; }

        /// <summary>
        /// Builds a uniform version header.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private byte[] BuildVersionHeader(uint version)
        {
            if (version <= 0 || version >= 100000)
            { // checks the version numbering.
                throw new ArgumentOutOfRangeException("version",
                    "Version number has to be larger than zero and smaller than 100000");
            }
            return System.Text.Encoding.ASCII.GetBytes(
                string.Format("OsmSharp.Collections.SpatialIndexes.v{0}", version.ToString("00000")));
        }

        /// <summary>
        /// Reads the version header.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string GetVersionHeader(Stream stream)
        {
            // seek to beginning of stream.
            stream.Seek(0, SeekOrigin.Begin);

            // get the first 23 bytes and read header.
            var buffer = new byte[23];
            stream.Read(buffer, 0, 23);
            return System.Text.Encoding.ASCII.GetString(buffer);
        }

        /// <summary>
        /// Writes the version header.
        /// </summary>
        private void WriteVersionHeader(Stream stream)
        {
            // seek to the beginning of the stream.
            stream.Seek(0, SeekOrigin.Begin);

            // write the header bytes.
            byte[] header = this.BuildVersionHeader(this.Version);
            stream.Write(header, 0, header.Length);
        }

        /// <summary>
        /// Reads and validates the header.
        /// </summary>
        /// <param name="stream"></param>
        private uint? ReadAndValidateHeader(Stream stream)
        {
            // get the version string.
            string versionString = this.GetVersionHeader(stream);

            // validate.
            if (versionString != null && versionString.Length == 23)
            { // the length is correct, now validate content.
                uint version;
                if (versionString.Substring(0, 18) == "OsmSharp.Collections.SpatialIndexes.v" &&
                    uint.TryParse(versionString.Substring(18, 5), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out version))
                { // the version got parsed too!
                    return version;
                }
            }
            return null;
        }

        #endregion

        /// <summary>
        /// Returns true if this serializer can deserialize the data in the given stream.
        /// </summary>
        /// <param name="stream"></param>
        public virtual bool CanDeSerialize(Stream stream)
        {
            uint? version = this.ReadAndValidateHeader(stream);
            return (version.HasValue && version.Value == this.Version);
        }

        /// <summary>
        /// Serializes the given graph and tags index to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="index"></param>
        public void Serialize(Stream stream, RTreeStreamIndex<T> index)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (index == null)
                throw new ArgumentNullException("index");

            // write the header.
            this.WriteVersionHeader(stream);

            // wrap the stream.
            var spatialIndexSerializerStream = new SpatialIndexSerializerStream(stream);

            // do the version-specific serialization.
            this.DoSerialize(spatialIndexSerializerStream, index);
        }

        /// <summary>
        /// Serializes the given index to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="index"></param>
        protected abstract void DoSerialize(SpatialIndexSerializerStream stream, RTreeStreamIndex<T> index);

        /// <summary>
        /// Deserializes the given stream into an index.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lazy"></param>
        /// <returns></returns>
        public RTreeStreamIndex<T> Deserialize(Stream stream, bool lazy = true)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (this.CanDeSerialize(stream))
            {
                // read/verify the current version header.
                this.ReadAndValidateHeader(stream);

                // wrap the stream.
                var spatialIndexSerializerStream = new SpatialIndexSerializerStream(stream);

                // do the actual version-specific deserialization.
                return this.DoDeserialize(spatialIndexSerializerStream, lazy);
            }
            throw new ArgumentOutOfRangeException("stream", "Cannot deserialize the given stream, version unsupported or content unrecognized!");
        }


        /// <summary>
        /// Deserializes the given stream into an index.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lazy"></param>
        /// <returns></returns>
        protected abstract RTreeStreamIndex<T> DoDeserialize(SpatialIndexSerializerStream stream, bool lazy);
    }
}