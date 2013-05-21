using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.IO.StreamCache
{
    /// <summary>
    /// An in-memory stream cache.
    /// </summary>
    public class MemoryCachedStream : IStreamCache
    {
        /// <summary>
        /// Holds the streams.
        /// </summary>
        private readonly HashSet<Stream> _streams = new HashSet<Stream>(); 

        /// <summary>
        /// Creates a new stream.
        /// </summary>
        /// <returns></returns>
        public System.IO.Stream CreateNew()
        {
            Stream stream = new MemoryStream();
            _streams.Add(stream);
            return stream;
        }

        /// <summary>
        /// Disposes all resource associated with this object.
        /// </summary>
        /// <param name="stream"></param>
        public void Dispose(Stream stream)
        {
            _streams.Remove(stream);
            stream.Dispose();
        }

        /// <summary>
        /// Disposes all resource associated with this object.
        /// </summary>
        public void Dispose()
        {
            foreach (var stream in _streams)
            {
                stream.Dispose();
            }
            _streams.Clear();
        }
    }
}
