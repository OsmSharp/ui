using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.IO.StreamCache
{
    /// <summary>
    /// Abstract representation of a stream cache.
    /// </summary>
    public interface IStreamCache : IDisposable
    {
        /// <summary>
        /// Creates a new cached stream.
        /// </summary>
        /// <returns></returns>
        Stream CreateNew();

        /// <summary>
        /// The given stream can be disposed.
        /// </summary>
        /// <param name="stream"></param>
        void Dispose(Stream stream);
    }
}
