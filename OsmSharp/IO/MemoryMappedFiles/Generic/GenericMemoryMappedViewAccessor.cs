// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2014 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;

namespace OsmSharp.IO.MemoryMappedFiles.Generic
{
    public class GenericMemoryMappedViewAccessor<T> : IMemoryMappedViewAccessor<T>
        where T : struct
    {
        /// <summary>
        /// Holds the stream.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Creates a new generic memory mapped view accessor.
        /// </summary>
        /// <param name="stream"></param>
        public GenericMemoryMappedViewAccessor(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Returns true if this stream can be read from.
        /// </summary>
        public bool CanRead
        {
            get { return _stream.CanRead; }
        }

        /// <summary>
        /// Returns true if this stream can be written to.
        /// </summary>
        public bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        /// <summary>
        /// Returns the capacity of this accessor.
        /// </summary>
        public long Capacity
        {
            get { throw new NotImplementedException(); }
        }

        public void Read(long position, out T structure)
        {
            throw new NotImplementedException();
        }

        public int ReadArray(long position, T[] array, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public void Write(long position, ref T structure)
        {
            throw new NotImplementedException();
        }

        public void WriteArray(long position, T[] array, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
