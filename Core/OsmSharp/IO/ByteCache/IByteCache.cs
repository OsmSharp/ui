using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.IO.ByteCache
{
    /// <summary>
    /// Abstract representation of a byte cache.
    /// </summary>
    public interface IByteCache : IDisposable
    {
        /// <summary>
        /// Gets the size of this cache.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Adds a new series of data.
        /// </summary>
        /// <returns></returns>
        uint Add(byte[] data);

        /// <summary>
        /// Gets the data associated with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        byte[] Get(uint id);

        /// <summary>
        /// Removes all data associated with the given id.
        /// </summary>
        /// <param name="id"></param>
        void Remove(uint id);
    }
}
