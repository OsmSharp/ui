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
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.IO.LZ4.Seekable
{
    /// <summary>
    /// A random access compressed stream reader.
    /// </summary>
    public class LZ4SeekableStreamReader : Stream
    {
        // RANDOM ACCESS STRUCTURE: very simple basic block-based definition.
        //
        // conditions:  - good compression ratio is important but not critical.
        //              - decompression performance is the most important part.
        // structure:   [BUFFER_SIZE][NEXT_LOCATION1|DATA][NEXT_LOCATION2|DATA][NEXT_LOCATION3|DATA][NEXT_LOCATION4|DATA][NEXT_LOCATION5|DATA]...
        // when reading: 
        //               Build index    [0] = NEXT_LOCATION1
        //                              [1] = NEXT_LOCATION2
        //                              [2] = NEXT_LOCATION3
        //                              [3] = NEXT_LOCATION4
        //                              [4] = NEXT_LOCATION5
        //

        /// <summary>
        /// Holds the random-access stream.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Holds the buffer.
        /// </summary>
        private byte[] _buffer;

        /// <summary>
        /// Holds the block sizes.
        /// </summary>
        private List<ushort> _blockSizes = null;

        /// <summary>
        /// Creates a compressed seekable stream using the existing index in the stream.
        /// </summary>
        /// <param name="stream">The random-access stream.</param>
        public LZ4SeekableStreamReader(Stream stream)
        {
            _stream = stream;

            // read the buffer size.
            _stream.Seek(0, SeekOrigin.Begin);
            var shortBytes = new byte[2];
            _stream.Read(shortBytes, 0, 2);
            _buffer = new byte[BitConverter.ToInt16(shortBytes, 0)];
        }

        /// <summary>
        /// Creates a compressed seekable stream overwriting the existing data starting at position 0.
        /// </summary>
        /// <param name="stream">The random-access stream.</param>
        /// <param name="bufferSize">The buffer size.</param>
        public LZ4SeekableStreamReader(Stream stream, ushort bufferSize)
        {
            _stream = stream;

            // write the buffer size.
            _stream.Seek(0, SeekOrigin.Begin);
            _stream.Write(BitConverter.GetBytes(bufferSize), 0, 2);
            _blockSizes = new List<ushort>();
            _blockSizes.Add(0);
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the length in bytes of this stream.
        /// </summary>
        public override long Length
        {
            get { return _stream.Length; }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        #region Read/Write blocks

        /// <summary>
        /// Reads the indices.
        /// </summary>
        private void ReadIndex()
        {
            var shortBytes = new byte[2];
            _stream.Seek(2, SeekOrigin.Begin);
            _stream.Read(shortBytes, 0, 2);
            var blockSize = BitConverter.ToUInt16(shortBytes, 0);
            _blockSizes = new List<ushort>();
            _blockSizes.Add(blockSize);
            while(blockSize > 0)
            {
                _stream.Seek(blockSize, SeekOrigin.Current);
                _stream.Read(shortBytes, 0, 2);
                blockSize = BitConverter.ToUInt16(shortBytes, 0);
                _blockSizes.Add(blockSize);
            }
        }

        #endregion
    }
}