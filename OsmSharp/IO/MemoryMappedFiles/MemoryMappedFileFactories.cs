using OsmSharp.Math.Geo.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.IO.MemoryMappedFiles
{
    /// <summary>
    /// A memory mapped file graph factory for some general primitives.
    /// </summary>
    public static class MemoryMappedFileFactories
    {
        /// <summary>
        /// Returns a memory mapped file factory.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static MemoryMappedFileFactory<ulong> UInt64File(MemoryMappedFileParameters parameters)
        {
            return null;
        }

        /// <summary>
        /// Returns a memory mapped file factory.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static MemoryMappedFileFactory<uint> UInt32File(MemoryMappedFileParameters parameters)
        {
            return null;
        }

        /// <summary>
        /// Returns a memory mapped file factory.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static MemoryMappedFileFactory<int> Int32File(MemoryMappedFileParameters parameters)
        {
            return null;
        }

        /// <summary>
        /// Returns a memory mapped file factory.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static MemoryMappedFileFactory<float> SingleFile(MemoryMappedFileParameters parameters)
        {
            return null;
        }

        /// <summary>
        /// Returns a memory mapped file factory.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static MemoryMappedFileFactory<GeoCoordinateSimple> GeoCoordinateSimpleFile(MemoryMappedFileParameters parameters)
        {
            return null;
        }
    }
}
