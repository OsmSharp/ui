// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

namespace OsmSharp.IO.MemoryMappedFiles
{
    /// <summary>
    /// Abstract representation of a memory-mapped accessor: Provides random access to unmanaged blocks of memory from managed code.
    /// </summary>
    public abstract class MemoryMappedAccessor<T> : IDisposable
        where T : struct
    {
        /// <summary>
        /// Holds the file that created this accessor.
        /// Need to keep track of this to make sure everything is disposed correctly no matter what!
        /// </summary>
        private MemoryMappedFile _file;

        /// <summary>
        /// Holds the stream.
        /// </summary>
        protected Stream _stream;

        /// <summary>
        /// The buffer to use while read/writing.
        /// </summary>
        protected byte[] _buffer;

        /// <summary>
        /// The size of a single element.
        /// </summary>
        protected int _elementSize;

        /// <summary>
        /// Creates a new memory mapped accessor.
        /// </summary>
        /// <param name="file">The file that created this memory mapped accessor.</param>
        /// <param name="stream">The stream containing the data.</param>
        /// <param name="elementSize">The element size.</param>
        public MemoryMappedAccessor(MemoryMappedFile file, Stream stream, int elementSize)
        {
            if (file == null) { throw new ArgumentNullException("file"); }
            if (stream == null) { throw new ArgumentNullException("stream"); }
            if (!stream.CanSeek) { throw new ArgumentException("Stream to create a memory mapped file needs to be seekable."); }

            _file = file;
            _stream = stream;
            _elementSize = elementSize;
            _buffer = new byte[64 * elementSize];
        }

        /// <summary>
        /// Determines whether the accessory is writable.
        /// </summary>
        public virtual bool CanWrite
        {
            get
            {
                return _stream.CanWrite;
            }
        }

        /// <summary>
        /// Gets the capacity of this memory mapped file in bytes.
        /// </summary>
        public long Capacity
        {
            get
            {
                return _stream.Length;
            }
        }

        /// <summary>
        /// Gets the capacity of this memory mapped file in the number of structs it can store.
        /// </summary>
        public long CapacityElements
        {
            get
            {
                return _stream.Length / this.ElementSize;
            }
        }

        /// <summary>
        /// Gets the size in bytes of one element.
        /// </summary>
        public int ElementSize
        {
            get { return _elementSize; }
        }

        /// <summary>
        /// Reads from the buffer at the given position.
        /// </summary>
        /// <param name="position">The position to read from.</param>
        /// <returns></returns>
        protected abstract T ReadFrom(int position);

        /// <summary>
        /// Writes to the stream.
        /// </summary>
        /// <param name="structure">The structure to write to.</param>
        protected abstract void WriteTo(T structure);

        /// <summary>
        /// Reads one element at the given position.
        /// </summary>
        /// <param name="position">The position to read from.</param>
        /// <param name="structure">The resulting structure.</param>
        public void Read(long position, out T structure)
        {
            _stream.Seek(position, SeekOrigin.Begin);
            _stream.Read(_buffer, 0, _elementSize);
            structure = this.ReadFrom(0);
        }

        /// <summary>
        /// Reads elements starting at the given position.
        /// </summary>
        /// <param name="position">The position to read.</param>
        /// <param name="array">The array to fill with the resulting data.</param>
        /// <param name="offset">The offset to start filling the array.</param>
        /// <param name="count">The number of elements to read.</param>
        /// <returns></returns>
        public int ReadArray(long position, T[] array, int offset, int count)
        {
            if (_buffer.Length < count * _elementSize)
            { // increase buffer if needed.
                Array.Resize(ref _buffer, count * _elementSize);
            }
            _stream.Seek(position, SeekOrigin.Begin);
            _stream.Read(_buffer, 0, count * _elementSize);
            for (int i = 0; i < count; i++)
            {
                array[i + offset] = this.ReadFrom(i * _elementSize);
            }
            return count;
        }

        /// <summary>
        /// Writes an element at the given position.
        /// </summary>
        /// <param name="position">The position to write to.</param>
        /// <param name="structure">The structure.</param>
        public void Write(long position, ref T structure)
        {
            _stream.Seek(position, SeekOrigin.Begin);
            this.WriteTo(structure);
        }

        /// <summary>
        /// Writes an array of elements at the given position.
        /// </summary>
        /// <param name="position">The position to write to.</param>
        /// <param name="array">The array to with the data.</param>
        /// <param name="offset">The offset to start using the array at.</param>
        /// <param name="count">The number of elements to write.</param>
        public void WriteArray(long position, T[] array, int offset, int count)
        {
            _stream.Seek(position, SeekOrigin.Begin);
            for (int i = 0; i < count; i++)
            {
                this.WriteTo(array[i + offset]);
            }
        }

        /// <summary>
        /// Diposes of all native resources associated with this object.
        /// </summary>
        public virtual void Dispose()
        {
            _file.Disposed<T>(this);
            _file = null;
        }
    }
}